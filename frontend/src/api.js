// The API base URL is NEVER hard-coded. It comes from the environment,
// so the same source builds against localhost in development and
// against the deployed Railway API in production (Lab 11, Task 01).
const API = import.meta.env.VITE_API_URL;

export const apiBaseUrl = API;

async function request(path, options = {}) {
  if (!API) {
    throw new Error(
      'VITE_API_URL is not set. Configure it in .env locally and in the ' +
        'hosting platform\u2019s environment variables for production.'
    );
  }

  const res = await fetch(`${API}${path}`, {
    headers: { 'Content-Type': 'application/json', ...(options.headers ?? {}) },
    ...options,
  });

  if (!res.ok) {
    throw new Error(`${res.status} ${res.statusText}`);
  }

  return res.status === 204 ? null : res.json();
}

export const getMenu = () => request('/api/menu');
export const getHealth = () => request('/health');
