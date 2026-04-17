export function createSidebar() {
  const sidebar = document.createElement('aside');
  sidebar.className = 'sidebar';

  sidebar.innerHTML = `
    <div class="sidebar__brand">
      <p class="sidebar__title">Grand Palacio</p>
      <p class="sidebar__subtitle">SISTEMA DE ESTADÍAS</p>
    </div>
    <nav>
      <p class="sidebar__section">Principal</p>
      <ul class="nav">
        <li class="nav__item"><a href="#" class="nav__link">Dashboard</a></li>
      </ul>
      <p class="sidebar__section">Gestión</p>
      <ul class="nav">
        <li class="nav__item"><a href="#" class="nav__link">Habitaciones</a></li>
        <li class="nav__item"><a href="#" class="nav__link">Huéspedes</a></li>
        <li class="nav__item"><a href="#" class="nav__link nav__link--active">Estadías</a></li>
      </ul>
    </nav>
  `;

  return sidebar;
}
