const BASE_URL = 'http://localhost:5162';

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