import { getBookings, checkIn, checkOut, createBooking } from '../entities/booking/api.js';
import { getAvailableRooms } from '../entities/room/api.js';
import { createButton } from '../shared/ui/button.js';
import { createInputField } from '../shared/ui/input.js';

const statusLabels = {
  reserved: 'Pendiente',
  active: 'Activa',
  finalized: 'Finalizada',
};

const statusClasses = {
  reserved: 'status-reserved',
  active: 'status-active',
  finalized: 'status-finalized',
};

const statusOptions = [
  { value: 'all', label: 'Todos los estados' },
  { value: 'reserved', label: 'Pendiente' },
  { value: 'active', label: 'Activa' },
  { value: 'finalized', label: 'Finalizada' },
];

export function createBookingPanel() {
  const state = {
    bookings: [],
    availableRooms: [],
    selectedRoomType: null,
    selectedRoomNumber: '',
    capacity: 1,
    guestInputs: [],
  };

  const container = document.createElement('main');
  container.className = 'content-area';

  const header = document.createElement('div');
  header.className = 'page-header';
  header.innerHTML = `
    <div>
      <h1>Estadías</h1>
      <p class="page-subtitle">Administra las reservas, check-in y check-out de tu hotel.</p>
    </div>
  `;

  const toolbar = document.createElement('div');
  toolbar.className = 'topbar';

  const selectWrapper = document.createElement('div');
  selectWrapper.className = 'filter';
  const statusSelect = document.createElement('select');
  statusSelect.className = 'select';
  statusOptions.forEach((option) => {
    const opt = document.createElement('option');
    opt.value = option.value;
    opt.textContent = option.label;
    statusSelect.append(opt);
  });
  selectWrapper.append(statusSelect);

  const newButton = createButton({ text: '+ Nueva estadía', variant: 'primary' });
  newButton.addEventListener('click', openModal);

  toolbar.append(selectWrapper, newButton);

  const card = document.createElement('section');
  card.className = 'panel-card';

  const tableWrapper = document.createElement('div');
  tableWrapper.className = 'panel-card__body';

  const bookingsTableContainer = document.createElement('div');
  bookingsTableContainer.className = 'bookings-table';

  tableWrapper.append(bookingsTableContainer);
  card.append(tableWrapper);

  const toast = document.createElement('div');
  toast.className = 'toast';
  container.append(header, toolbar, card, toast);

  const modalOverlay = createModalOverlay();
  container.append(modalOverlay);

  statusSelect.addEventListener('change', renderBookings);

  loadBookings();

  function showToast(message) {
    toast.textContent = message;
    toast.classList.add('toast--visible');
    setTimeout(() => toast.classList.remove('toast--visible'), 3200);
  }

  async function loadBookings() {
    bookingsTableContainer.innerHTML = '<div class="empty-state">Cargando estadías…</div>';
    try {
      state.bookings = await getBookings();
      renderBookings();
    } catch (error) {
      bookingsTableContainer.innerHTML = `<div class="empty-state">No se pudieron cargar las estadías.</div>`;
      console.error(error);
    }
  }

  function getFilteredBookings() {
    const filter = statusSelect.value;
    if (filter === 'all') return state.bookings;
    return state.bookings.filter((booking) => booking.status === filter);
  }

  function renderBookings() {
    const bookingsToRender = getFilteredBookings();
    bookingsTableContainer.innerHTML = '';

    if (!bookingsToRender.length) {
      bookingsTableContainer.innerHTML = `
        <div class="empty-state">
          <strong>Sin estadías</strong>
          <p>No hay reservas registradas en este momento.</p>
        </div>
      `;
      return;
    }

    const table = document.createElement('table');
    table.className = 'table';

    table.innerHTML = `
      <thead>
        <tr>
          <th>ID</th>
          <th>HuéspeD</th>
          <th>Habitación</th>
          <th>Fecha inicio</th>
          <th>Fecha fin</th>
          <th>Estado</th>
          <th>Total</th>
          <th>Acciones</th>
        </tr>
      </thead>
    `;

    const tbody = document.createElement('tbody');

    bookingsToRender.forEach((booking) => {
      const row = document.createElement('tr');
      const guestName = booking.guests?.[0]?.name || '—';
      const roomName = booking.room?.number ? `N° ${booking.room.number} (${booking.room.type})` : '—';
      const totalValue = booking.payment?.total ?? '—';
      const totalText = typeof totalValue === 'number' ? formatCurrency(totalValue) : '—';
      const status = booking.status ? String(booking.status).toLowerCase() : '';
      const statusText = statusLabels[status] || status || '—';

      row.innerHTML = `
        <td>#${booking.id}</td>
        <td>${guestName}</td>
        <td>${roomName}</td>
        <td>${formatDate(booking.startDate)}</td>
        <td>${formatDate(booking.endDate)}</td>
        <td><span class="status-badge ${statusClasses[status] || ''}">${statusText}</span></td>
        <td>${totalText}</td>
        <td></td>
      `;

      const actionsCell = row.querySelector('td:last-child');
      if (status === 'reserved') {
        const button = createButton({ text: 'Check-In', variant: 'success' });
        button.addEventListener('click', async () => handleCheckIn(booking.id));
        actionsCell.append(button);
      }
      if (status === 'active') {
        const button = createButton({ text: 'Check-Out', variant: 'danger' });
        button.addEventListener('click', async () => handleCheckOut(booking.id));
        actionsCell.append(button);
      }
      if (status === 'finalized') {
        actionsCell.textContent = '—';
      }

      tbody.append(row);
    });

    table.append(tbody);
    bookingsTableContainer.append(table);
  }

  async function handleCheckIn(id) {
    try {
      await checkIn(id);
      showToast('Check-in realizado con éxito.');
      await loadBookings();
    } catch (error) {
      showToast('No se pudo completar el check-in.');
      console.error(error);
    }
  }

  async function handleCheckOut(id) {
    try {
      await checkOut(id);
      showToast('Check-out completado correctamente.');
      await loadBookings();
    } catch (error) {
      showToast('No se pudo completar el check-out.');
      console.error(error);
    }
  }

  function createModalOverlay() {
    const overlay = document.createElement('div');
    overlay.className = 'modal-overlay hidden';

    const modal = document.createElement('div');
    modal.className = 'modal';

    const header = document.createElement('div');
    header.className = 'modal__header';
    header.innerHTML = `
      <div>
        <h2 class="modal__title">Nueva estadía</h2>
      </div>
    `;

    const closeButton = document.createElement('button');
    closeButton.type = 'button';
    closeButton.className = 'modal__close';
    closeButton.textContent = '✕';
    closeButton.addEventListener('click', closeModal);
    header.append(closeButton);

    const form = document.createElement('div');
    form.className = 'modal__body';

    const dateGrid = document.createElement('div');
    dateGrid.className = 'form-grid';

    const startField = createInputField({
      label: 'Fecha inicio',
      name: 'startDate',
      type: 'date',
      placeholder: 'dd/mm/aaaa',
      required: true,
      onInput: updateSubmitState,
    });
    const endField = createInputField({
      label: 'Fecha fin',
      name: 'endDate',
      type: 'date',
      placeholder: 'dd/mm/aaaa',
      required: true,
      onInput: updateSubmitState,
    });

    dateGrid.append(startField.field, endField.field);

    const searchArea = document.createElement('div');
    searchArea.className = 'modal__section';

    const searchButton = createButton({ text: 'Buscar habitaciones disponibles', variant: 'secondary' });
    searchButton.addEventListener('click', async () => {
      await handleSearchAvailability(startField.input.value, endField.input.value);
    });

    const searchMessage = document.createElement('p');
    searchMessage.className = 'empty-state';
    searchMessage.textContent = 'Selecciona un rango de fechas para buscar habitaciones disponibles.';

    searchArea.append(searchButton, searchMessage);

    const availableRoomsSection = document.createElement('div');
    availableRoomsSection.className = 'modal__section';

    const availableRoomsTitle = document.createElement('h3');
    availableRoomsTitle.textContent = 'Habitaciones disponibles';
    availableRoomsSection.append(availableRoomsTitle);

    const availableRoomsContainer = document.createElement('div');
    availableRoomsSection.append(availableRoomsContainer);

    const guestSection = document.createElement('div');
    guestSection.className = 'modal__section';

    const guestTitle = document.createElement('h3');
    guestTitle.textContent = 'Datos de los huéspedes';
    guestSection.append(guestTitle);

    const guestCardsContainer = document.createElement('div');
    guestSection.append(guestCardsContainer);

    const actions = document.createElement('div');
    actions.className = 'form-actions';

    const cancelButton = createButton({ text: 'Cancelar', variant: 'secondary' });
    cancelButton.addEventListener('click', closeModal);

    const submitButton = createButton({ text: 'Reservar estadía', variant: 'primary' });
    submitButton.disabled = true;
    submitButton.addEventListener('click', async () => {
      await handleCreateBooking({
        start: startField.input.value,
        end: endField.input.value,
      });
    });

    actions.append(cancelButton, submitButton);
    form.append(dateGrid, searchArea, availableRoomsSection, guestSection, actions);
    modal.append(header, form);
    overlay.append(modal);

    return overlay;

    async function handleSearchAvailability(startDate, endDate) {
      if (!startDate || !endDate) {
        searchMessage.textContent = 'Debes ingresar fecha de inicio y fecha de fin.';
        return;
      }

      if (new Date(startDate) >= new Date(endDate)) {
        searchMessage.textContent = 'La fecha de inicio debe ser anterior a la fecha de fin.';
        return;
      }

      searchMessage.textContent = 'Buscando habitaciones…';
      availableRoomsContainer.innerHTML = '';
      guestCardsContainer.innerHTML = '';
      state.selectedRoomType = null;
      state.selectedRoomNumber = '';
      state.capacity = 1;
      state.guestInputs = [];
      submitButton.disabled = true;

      try {
        const availableRooms = await getAvailableRooms(startDate, endDate);
        state.availableRooms = availableRooms;

        if (!availableRooms.length) {
          searchMessage.textContent = 'No hay habitaciones disponibles en ese período.';
          return;
        }

        searchMessage.textContent = 'Selecciona una habitación y completa los datos del huésped.';
        renderAvailableRooms(availableRooms);
      } catch (error) {
        searchMessage.textContent = 'Error al consultar disponibilidad.';
        console.error(error);
      }
    }

    function renderAvailableRooms(availableRooms) {
      availableRoomsContainer.innerHTML = '';
      availableRooms.forEach((type) => {
        const card = document.createElement('div');
        card.className = 'card';
        const count = type.availableRooms.length;
        card.innerHTML = `
          <div class="card__header">
            <div>
              <strong>${type.roomType}</strong>
              <p>${count} habitación${count === 1 ? '' : 'es'} disponibles</p>
            </div>
          </div>
        `;

        const selectField = document.createElement('div');
        selectField.className = 'field';
        const selectLabel = document.createElement('label');
        selectLabel.textContent = 'Selecciona número de habitación';
        const select = document.createElement('select');
        select.className = 'select';
        select.innerHTML = '<option value="">Elige una habitación</option>';
        type.availableRooms.forEach((room) => {
          const item = document.createElement('option');
          item.value = room.id;
          item.textContent = `Habitación N° ${room.number}`;
          select.append(item);
        });
        select.addEventListener('change', () => {
          onSelectRoom(type, select.value);
        });

        selectField.append(selectLabel, select);
        card.append(selectField);
        availableRoomsContainer.append(card);
      });
    }

    function onSelectRoom(type, roomNumber) {
      state.selectedRoomType = type;
      state.selectedRoomNumber = roomNumber;
      state.capacity = inferCapacity(type.roomType || type.roomType);
      state.guestInputs = [];
      guestCardsContainer.innerHTML = '';
      if (!roomNumber) {
        submitButton.disabled = true;
        return;
      }

      renderGuestCards();
      updateSubmitState();
    }

    function renderGuestCards() {
      const firstGuest = createGuestCard('Primer invitado', 1, true);
      guestCardsContainer.append(firstGuest.card);
      state.guestInputs.push(firstGuest.inputs);

      if (state.capacity > 1) {
        const secondGuest = createGuestCard('Segundo invitado', 2, false);
        guestCardsContainer.append(secondGuest.card);
        state.guestInputs.push(secondGuest.inputs);
      }
    }

    function createGuestCard(title, index, requiredPhone) {
      const card = document.createElement('div');
      card.className = 'card';
      const header = document.createElement('div');
      header.className = 'card__header';
      header.innerHTML = `<p class="card__title">${title}</p>`;
      card.append(header);

      const guestGrid = document.createElement('div');
      guestGrid.className = 'form-grid';

      const nameField = createInputField({
        label: 'Nombre completo',
        name: `guest${index}Name`,
        type: 'text',
        placeholder: index === 1 ? 'María Gutiérrez' : 'Nombre opcional',
        required: true,
        onInput: updateSubmitState,
      });
      const ciField = createInputField({
        label: 'Cédula',
        name: `guest${index}Ci`,
        type: 'text',
        placeholder: index === 1 ? '7845621' : 'Cédula opcional',
        required: true,
        onInput: updateSubmitState,
      });
      const phoneField = createInputField({
        label: 'Teléfono',
        name: `guest${index}Phone`,
        type: 'tel',
        placeholder: '04141234567',
        required: requiredPhone,
        onInput: updateSubmitState,
      });

      if (!requiredPhone) {
        phoneField.input.required = false;
      }

      guestGrid.append(nameField.field, ciField.field, phoneField.field);
      card.append(guestGrid);

      return {
        card,
        inputs: {
          name: nameField.input,
          ci: ciField.input,
          phone: phoneField.input,
        },
      };
    }

    async function handleCreateBooking({ start, end }) {
      if (!state.selectedRoomNumber) {
        showToast('Selecciona una habitación antes de reservar.');
        return;
      }

      const guests = state.guestInputs.map((group) => ({
        name: group.name.value.trim(),
        ci: group.ci.value.trim(),
        phone: group.phone.value.trim(),
      }));

      const invalid = guests.some((guest, index) => {
        if (!guest.name || !guest.ci) return true;
        if (index === 0 && !guest.phone) return true;
        return false;
      });

      if (invalid) {
        showToast('Completa los campos obligatorios de los huéspedes.');
        return;
      }

      const payload = {
        startDate: start,
        endDate: end,
        roomId: Number(state.selectedRoomNumber),
        guests,
      };

      try {
        await createBooking(payload);
        closeModal();
        showToast('Estadía registrada correctamente.');
        await loadBookings();
      } catch (error) {
        showToast('No se pudo registrar la estadía.');
        console.error(error);
      }
    }

    function inferCapacity(roomType) {
      if (!roomType) return 1;
      const normalized = roomType.toLowerCase();
      if (normalized.includes('doble') || normalized.includes('suite') || normalized.includes('matrimonial') || normalized.includes('familiar') || normalized.includes('master')) {
        return 2;
      }
      return 1;
    }

    function updateSubmitState() {
      const hasDates = startField.input.value && endField.input.value;
      const hasSelectedRoom = !!state.selectedRoomNumber;
      const guestValid = state.guestInputs.length > 0 && state.guestInputs.every((group, index) => {
        const hasName = group.name.value.trim();
        const hasCi = group.ci.value.trim();
        const hasPhone = group.phone.value.trim();
        if (!hasName || !hasCi) return false;
        if (index === 0 && !hasPhone) return false;
        return true;
      });
      submitButton.disabled = !(hasDates && hasSelectedRoom && guestValid);
    }

    function closeModal() {
      overlay.classList.add('hidden');
      state.availableRooms = [];
      state.selectedRoomType = null;
      state.selectedRoomNumber = '';
      state.guestInputs = [];
      searchMessage.textContent = 'Selecciona un rango de fechas para buscar habitaciones disponibles.';
      availableRoomsContainer.innerHTML = '';
      guestCardsContainer.innerHTML = '';
      startField.input.value = '';
      endField.input.value = '';
      submitButton.disabled = true;
    }
  }

  function openModal() {
    modalOverlay.classList.remove('hidden');
  }

  return container;
}

function formatDate(value) {
  if (!value) return '—';
  const date = new Date(value);
  return date.toLocaleDateString('es-VE', { day: '2-digit', month: 'short', year: 'numeric' });
}

function formatCurrency(value) {
  return `${new Intl.NumberFormat('es-VE', {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2,
  }).format(value)} Bs.`;
}
