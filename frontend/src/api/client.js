// ── Slide 32: Axios best practices ───────────────────────────────
// ONE configured axios instance for the whole app. Components never
// call axios directly – they call feature api.js functions that use
// this client. Benefits: single base URL, shared headers, one place
// for auth tokens and error logging.
import axios from "axios";

// ── Lab 11, Task 01: the API base URL is CONFIGURATION, not code ──
// Local development leaves VITE_API_URL unset, so baseURL stays "/api"
// and the Vite dev-server proxy forwards to the local API. In
// production VITE_API_URL is set on the hosting platform (Vercel) to
// the deployed ASP.NET Core API, so the SAME source builds for both
// environments and no URL is ever hard-coded.
const API_ROOT = import.meta.env.VITE_API_URL ?? "";

export const apiBaseUrl = `${API_ROOT}/api`;

export const apiClient = axios.create({
  baseURL: apiBaseUrl,
  timeout: 15000, // free-tier hosts can cold-start on the first request
});

// REQUEST interceptor – in Lecture 04 this attaches the real JWT.
apiClient.interceptors.request.use((config) => {
  const raw = localStorage.getItem("campuseats-user");
  if (raw) {
    const user = JSON.parse(raw);
    // Demo token; the backend lectures replace this with a real JWT.
    config.headers.Authorization = `Bearer demo-token-for-${user.role}`;
  }
  return config;
});

// RESPONSE interceptor – central place to log / map errors.
apiClient.interceptors.response.use(
  (response) => response,
  (error) => {
    console.error("[API error]", error.config?.url, error.message);
    return Promise.reject(error);
  }
);
