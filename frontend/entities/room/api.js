import { apiFetch } from '../../shared/api/api.js';

export async function getAvailableRooms(startDate, endDate){
  const params = new URLSearchParams({ startDate, endDate });
  return apiFetch(`/api/Room/available?${params.toString()}`);
}