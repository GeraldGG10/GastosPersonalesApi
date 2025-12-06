// API helpers
const API_URL = "http://localhost:5241/api"; // <--- Cambia a tu URL de backend si aplica

async function apiFetch(path, options = {}) {
  const token = localStorage.getItem("token");
  const headers = options.headers || {};
  headers["Content-Type"] = headers["Content-Type"] || "application/json";
  if (token) headers["Authorization"] = "Bearer " + token;
  const res = await fetch(`${API_URL}${path}`, { ...options, headers });
  const text = await res.text();
  try { return JSON.parse(text || "{}"); } catch (err) { return text; }
}

function formatCurrency(num) {
  if (num == null) return "0.00";
  return Number(num).toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 });
}
