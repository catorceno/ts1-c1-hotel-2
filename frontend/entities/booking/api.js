import { apiFetch } from '../../shared/api/api.js';

export function getBookings() {
  return apiFetch('/api/Booking');
}

export function checkIn(id) {
  return apiFetch(`/api/Booking/${id}/checkin`, { method: 'POST' });
}

export function checkOut(id) {
  return apiFetch(`/api/Booking/${id}/checkout`, { method: 'POST' });
}

export function createBooking(bookingData) {
  return apiFetch('/api/Booking', {
    method: 'POST',
    body: JSON.stringify(bookingData)
  });
}
