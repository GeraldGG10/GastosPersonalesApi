// js/api.js
const API_ROOT = "http://localhost:5241";
const API_URL = API_ROOT + "/api";

async function apiFetch(path, options = {}) {
  const token = localStorage.getItem("token");
  const headers = options.headers ? { ...options.headers } : {};

  // No forzar Content-Type si body es FormData
  if (!(options.body instanceof FormData)) {
    headers["Content-Type"] = headers["Content-Type"] || "application/json";
  }

  if (token) {
    headers["Authorization"] = "Bearer " + token;
    console.log("✓ Token añadido a la petición");
  } else {
    console.log("⚠ No hay token para esta petición");
  }

  console.log(`→ Petición: ${options.method || 'GET'} ${API_URL}${path}`);
  
  const res = await fetch(`${API_URL}${path}`, { ...options, headers });

  console.log(`← Respuesta: ${res.status} ${res.statusText}`);

  // Manejo de 401: token inválido o expirado
  if (res.status === 401) {
    console.error("❌ Error 401 - Token inválido o expirado");
    console.log("Limpiando localStorage y redirigiendo...");
    localStorage.removeItem("token");
    localStorage.removeItem("usuario");
    
    // Solo redirigir si no estamos ya en index.html
    if (!window.location.pathname.includes("index.html")) {
      console.log("Redirigiendo a index.html...");
      window.location.href = "index.html";
    }
    return null;
  }

  // Manejo de 400: Bad Request
  if (res.status === 400) {
    const text = await res.text();
    console.error("❌ Error 400 - Bad Request");
    console.error("Respuesta del servidor:", text);
    try {
      const errorData = JSON.parse(text);
      console.error("Error parseado:", errorData);
      return errorData;
    } catch (err) {
      console.error("No se pudo parsear el error:", text);
      return { message: text || "Error 400 - Bad Request" };
    }
  }

  // Manejo de otros errores HTTP
  if (!res.ok) {
    console.error(`❌ Error HTTP ${res.status}:`, res.statusText);
    const text = await res.text();
    console.error("Respuesta:", text);
    try {
      return JSON.parse(text || "{}");
    } catch (err) {
      return { message: text || `Error ${res.status}` };
    }
  }

  // Respuesta exitosa
  const text = await res.text();
  
  try {
    const parsed = JSON.parse(text || "{}");
    console.log("✓ Respuesta parseada correctamente");
    return parsed;
  } catch (err) {
    console.log("⚠ Respuesta no es JSON, devolviendo como texto");
    return text;
  }
}

function formatCurrency(num) {
  if (num == null || isNaN(num)) return "0.00";
  return Number(num).toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 });
}

function ensureAuth() {
  const token = localStorage.getItem("token");
  console.log("ensureAuth() - Token:", token ? "existe" : "no existe");
  
  if (!token) {
    console.log("❌ No autenticado, redirigiendo...");
    window.location.href = "index.html";
    return false;
  }
  
  console.log("✓ Usuario autenticado");
  return true;
}