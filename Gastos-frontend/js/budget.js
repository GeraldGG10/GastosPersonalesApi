// js/budget.js
// Requiere: js/api.js (apiFetch, formatCurrency, ensureAuth)

if (!ensureAuth()) throw new Error("No autorizado");

document.addEventListener("DOMContentLoaded", () => {
  // Inicializa modo y carga
  const modoSel = document.getElementById("modo");
  if (modoSel) modoSel.value = modoSel.value || "categorias";
  cambiarModo();
});

/* ---------------------------
   Funciones utilitarias
----------------------------*/

// Obtener todos los gastos reales
async function obtenerGastos() {
  try {
    // usa ruta en espaÃ±ol: /api/Gastos
    const res = await apiFetch("/Expenses");
    // apiFetch devuelve array o objeto; aseguramos array
    return Array.isArray(res) ? res : (res?.items || []);
  } catch (err) {
    console.error("Error obtenerGastos:", err);
    return [];
  }
}

// Obtener categorÃ­as reales
async function obtenerCategorias() {
  try {
    return await apiFetch("/Categories"); // <- corregido
  } catch (err) {
    console.error("Error obtenerCategorias:", err);
    return [];
  }
}

// Obtener presupuestos guardados
async function obtenerPresupuestos() {
  try {
    return await apiFetch("/Budgets"); // <- corregido
  } catch (err) {
    console.error("Error obtenerPresupuestos:", err);
    return [];
  }
}

/* ---------------------------
   Cambio de modo
----------------------------*/
function cambiarModo() {
  const modo = document.getElementById("modo").value;

  if (modo === "categorias") cargarPresupuestoCategorias();
  else if (modo === "mensual") cargarPresupuestoMensual();
  else if (modo === "anual") cargarPresupuestoAnual();
}

/* ======================================================
   A) PRESUPUESTO POR CATEGORÃA - MENSUAL
====================================================== */

async function cargarPresupuestoCategorias() {
  const categorias = await obtenerCategorias();
  const presupuestos = await obtenerPresupuestos();
  const gastos = await obtenerGastos();

  let html = `<h2>Presupuesto por categorÃ­a (mensual)</h2>`;

  // Si no hay categorÃ­as, avisar
  if (!Array.isArray(categorias) || categorias.length === 0) {
    html += `<div class="card-apple">No hay categorÃ­as registradas.</div>`;
    document.getElementById("contenidoPresupuesto").innerHTML = html;
    return;
  }

  // Para cada categorÃ­a, buscar presupuesto y sumar gastos de este mes
  const hoy = new Date();
  const mesActual = hoy.getMonth() + 1; // 1..12
  const yearActual = hoy.getFullYear();

  for (const cat of categorias) {
    // presupuesto para esta categorÃ­a (si existe)
    const actual = (presupuestos || []).find(p => Number(p.categoryId) === Number(cat.id));
    const monto = actual?.amount ?? 0;

    // sumar gastos del mes actual y de la categorÃ­a
    const usado = (gastos || [])
      .filter(g => {
        try {
          const d = new Date(g.date || g.fecha || g.fechaHora || g.fechaCreacion);
          return (
            Number(g.categoryId ?? g.categoriaId ?? g.categoria) === Number(cat.id) &&
            d.getMonth() + 1 === mesActual &&
            d.getFullYear() === yearActual
          );
        } catch {
          return false;
        }
      })
      .reduce((sum, g) => sum + Number(g.amount ?? g.monto ?? 0), 0);

    html += crearTarjetaPresupuesto({
      titulo: cat.name ?? cat.nombre ?? `CategorÃ­a ${cat.id}`,
      monto,
      usado,
      referencia: actual?.id ?? null,
      categoriaId: cat.id,
      tipo: "categoria"
    });
  }

  document.getElementById("contenidoPresupuesto").innerHTML = html;

  // DespuÃ©s de inyectar HTML, ajustar barras (para animaciÃ³n si hace falta)
  ajustarBarrasAnimadas();
}

/* ======================================================
   B) PRESUPUESTO MENSUAL GENERAL
====================================================== */

async function cargarPresupuestoMensual() {
  const presupuestos = await obtenerPresupuestos();
  const gastos = await obtenerGastos();

  const hoy = new Date();
  const mesActual = hoy.getMonth() + 1;
  const yearActual = hoy.getFullYear();

  // Si hay un presupuesto general (sin categoryId) tomarlo; sino, sumar presupuestos por categorÃ­a
  const general = (presupuestos || []).find(p => !p.categoryId && !p.categoryId === 0);
  let monto = general?.amount ?? 0;

  if (!monto || monto === 0) {
    // sumar presupuestos por categorÃ­a como fallback
    monto = (presupuestos || []).reduce((s, p) => s + Number(p.amount ?? 0), 0);
  }

  // calcular usado en mes actual
  const usado = (gastos || [])
    .filter(g => {
      try {
        const d = new Date(g.date || g.fecha);
        return d.getMonth() + 1 === mesActual && d.getFullYear() === yearActual;
      } catch {
        return false;
      }
    })
    .reduce((sum, g) => sum + Number(g.amount ?? g.monto ?? 0), 0);

  const html = `
    <h2>Presupuesto mensual general</h2>
    ${crearTarjetaPresupuesto({
      titulo: "Total Mensual",
      monto,
      usado,
      referencia: general?.id ?? null,
      categoriaId: null,
      tipo: "mensual"
    })}
  `;

  document.getElementById("contenidoPresupuesto").innerHTML = html;
  ajustarBarrasAnimadas();
}

/* ======================================================
   C) PRESUPUESTO ANUAL GENERAL
====================================================== */

async function cargarPresupuestoAnual() {
  const presupuestos = await obtenerPresupuestos();
  const gastos = await obtenerGastos();

  const year = new Date().getFullYear();

  // Intentar leer un presupuesto anual (si el backend tiene uno por tipo)
  let monto = (presupuestos || []).find(p => p.period === "annual")?.amount ?? 0;

  // Si no existe, tomar suma de presupuestos mensuales * 12 como heurÃ­stica
  if (!monto || monto === 0) {
    const sumaMensual = (presupuestos || []).reduce((s, p) => s + Number(p.amount ?? 0), 0);
    monto = sumaMensual * 12;
  }

  // Gastos del aÃ±o
  const usado = (gastos || [])
    .filter(g => {
      try {
        const d = new Date(g.date || g.fecha);
        return d.getFullYear() === year;
      } catch {
        return false;
      }
    })
    .reduce((sum, g) => sum + Number(g.amount ?? g.monto ?? 0), 0);

  const html = `
    <h2>Presupuesto anual</h2>
    ${crearTarjetaPresupuesto({
      titulo: "Total AÃ±o",
      monto,
      usado,
      referencia: null,
      categoriaId: null,
      tipo: "anual"
    })}
  `;

  document.getElementById("contenidoPresupuesto").innerHTML = html;
  ajustarBarrasAnimadas();
}

/* ======================================================
   CREAR TARJETA VISUAL (estilo Apple)
====================================================== */

function crearTarjetaPresupuesto({ titulo, monto, usado, referencia = null, categoriaId = null, tipo = "categoria" }) {
  const porcentaje = monto > 0 ? Math.min(100, (usado / monto * 100)) : 0;

  let colorClass = "normal";
  if (porcentaje >= 100) colorClass = "danger";
  else if (porcentaje >= 80) colorClass = "danger";
  else if (porcentaje >= 50) colorClass = "warning";

  // Alertas: mostramos texto visible si pasa umbrales
  let alertaTexto = "";
  if (porcentaje >= 100) alertaTexto = "Â¡Has superado el presupuesto!";
  else if (porcentaje >= 80) alertaTexto = "Alerta: has alcanzado el 80%";
  else if (porcentaje >= 50) alertaTexto = "Aviso: has alcanzado el 50%";

  // Mostrar botÃ³n editar/crear presupuesto
  const editarBtn = `<button class="btn btn-accent" onclick="editarPresupuestoHandler('${referencia}', ${categoriaId}, '${tipo}', ${monto})">
                        ${referencia ? "Editar" : "Crear presupuesto"}
                      </button>`;

  return `
    <div class="card-apple" data-categoria="${categoriaId ?? ""}">
      <h3>${titulo}</h3>
      <p>Presupuesto: <strong>${formatCurrency(monto)} RD$</strong></p>
      <p>Gastado: <strong>${formatCurrency(usado)} RD$</strong></p>

      <div class="progress-container" aria-hidden="true">
        <div class="progress-bar ${colorClass}" style="width:${porcentaje}%"></div>
      </div>

      <p style="margin-top:10px;">
        <strong>${porcentaje.toFixed(1)}%</strong> usado
      </p>

      ${alertaTexto ? `<p style="color:${colorClass === 'danger' ? '#ff4e4e' : (colorClass === 'warning' ? '#ffc045' : '#4cd964')}; font-weight:700">${alertaTexto}</p>` : ""}

      <div style="display:flex; gap:10px; justify-content:flex-end; margin-top:10px;">
        ${editarBtn}
      </div>
    </div>
  `;
}

/* AnimaciÃ³n: asegurar que las barras tomen su width real tras render */
function ajustarBarrasAnimadas() {
  const bars = document.querySelectorAll(".progress-bar");
  bars.forEach(b => {
    const w = b.style.width || "0%";
    b.style.width = "0%";
    // forzar reflow
    // eslint-disable-next-line no-unused-expressions
    b.offsetHeight;
    b.style.width = w;
  });
}

/* ======================================================
   HANDLER: editar / crear presupuesto (UI simple + llamada a API)
   - Si referencia (id) existe -> PUT /Presupuestos/{id}
   - Si no existe -> POST /Presupuestos
====================================================== */

async function editarPresupuestoHandler(referenciaId, categoriaId, tipo, montoActual) {
  const texto = tipo === "categoria" ? `Presupuesto para categorÃ­a ${categoriaId}` : (tipo === "mensual" ? "Presupuesto mensual general" : "Presupuesto anual");
  const input = prompt(`Nuevo monto para ${texto}:`, montoActual ?? 0);
  if (input === null) return; // cancelado
  const nuevo = Number(input);
  if (isNaN(nuevo) || nuevo < 0) return alert("Valor invÃ¡lido");

  try {
    let res;
    // Si tenemos referenciaId (id del presupuesto) actualizamos
    if (referenciaId) {
      res = await apiFetch(`/Budgets/${referenciaId}`, {
        method: "PUT",
        body: JSON.stringify({ amount: nuevo })
      });
    } else {
      // crear nuevo: si es por categoria, mandamos categoryId
      const payload = tipo === "categoria" ? { categoryId: categoriaId, amount: nuevo, month: (new Date().getMonth()+1), year: new Date().getFullYear() } : { amount: nuevo };
      res = await apiFetch(`/Budgets`, {
        method: "POST",
        body: JSON.stringify(payload)
      });
    }

    alert("Presupuesto actualizado.");
    cambiarModo(); // recargar vista
  } catch (err) {
    console.error("Error editarPresupuestoHandler:", err);
    alert("Error al guardar presupuesto.");
  }
}

/* ======================================================
   EXPORTS / helpers finales
====================================================== */

// Si quieres forzar recarga desde consola:
window.cambiarModo = cambiarModo;
window.cargarPresupuestoCategorias = cargarPresupuestoCategorias;
window.cargarPresupuestoMensual = cargarPresupuestoMensual;
window.cargarPresupuestoAnual = cargarPresupuestoAnual;
window.editarPresupuestoHandler = editarPresupuestoHandler;

