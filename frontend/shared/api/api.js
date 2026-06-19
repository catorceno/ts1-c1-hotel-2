const BASE_URL = 'ts1-c1-hotel-2-production.up.railway.app';

// wrapper genérico
export async function apiFetch(path, options = {}){
  const response = await fetch(`${BASE_URL}${path}`, {
    headers: { 'Content-Type': 'application/json' },
    ...options,
  });

  if(!response.ok){
    const error = await response.json().catch(() => ({}));
    throw new Error(error.message || `Error ${response.status}`);
  }

  return response.json();
}
