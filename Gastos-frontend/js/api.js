// js/api.js
const API_ROOT = "http://localhost:5241"; // sin cambiar
const API_URL = API_ROOT + "/api"; // deja así, incluye /api


async function apiFetch(path, options = {}) {
  const token = localStorage.getItem("token");
  const headers = options.headers ? { ...options.headers } : {};
  // No forzar Content-Type si body es FormData
  if (!(options.body instanceof FormData)) {
    headers["Content-Type"] = headers["Content-Type"] || "application/json";
  }
  if (token) headers["Authorization"] = "Bearer " + token;

  const res = await fetch(`${API_URL}${path}`, { ...options, headers });
  const text = await res.text();

  // intenta parsear JSON, si no devuelve texto bruto
  try {
    return JSON.parse(text || "{}");
  } catch (err) {
    return text;
  }
}

function formatCurrency(num) {
  if (num == null || isNaN(num)) return "0.00";
  return Number(num).toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 });
}

function ensureAuth() {
  if (!localStorage.getItem("token")) {
    window.location.href = "index.html";
    return false;
  }
  return true;
}
