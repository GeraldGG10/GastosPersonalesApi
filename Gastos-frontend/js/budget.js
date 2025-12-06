document.addEventListener("DOMContentLoaded", () => {
  cambiarModo();
});

function cambiarModo() {
  const modo = document.getElementById("modo").value;

  if (modo === "categorias") cargarPresupuestoCategorias();
  if (modo === "mensual") cargarPresupuestoMensual();
  if (modo === "anual") cargarPresupuestoAnual();
}

/* ======================================================
   A) PRESUPUESTO POR CATEGORÍA - MENSUAL
====================================================== */

async function cargarPresupuestoCategorias() {
  const token = localStorage.getItem("token");
  const resCategorias = await fetch(`${API_URL}/Categories`, {
    headers: { "Authorization": `Bearer ${token}` }
  });

  const categorias = await resCategorias.json();

  // Obtener presupuestos actuales
  const resBudgets = await fetch(`${API_URL}/Budgets`, {
    headers: { "Authorization": `Bearer ${token}` }
  });

  const budgets = await resBudgets.json();

  let html = `<h2>Presupuesto por categoría (mensual)</h2>`;

  categorias.forEach(cat => {
    const actual = budgets.find(b => b.categoryId === cat.id);
    const monto = actual?.amount ?? 0;

    html += crearTarjetaPresupuesto({
      titulo: cat.name,
      monto,
      usado: randomGastoSimulado(),
      categoriaId: cat.id,
      tipo: "categoria"
    });
  });

  document.getElementById("contenidoPresupuesto").innerHTML = html;
}

/* ======================================================
   B) PRESUPUESTO MENSUAL GENERAL
====================================================== */

async function cargarPresupuestoMensual() {
  const monto = 15000; // Simulado
  const usado = randomGastoSimulado();

  const html = `
    <h2>Presupuesto mensual general</h2>
    ${crearTarjetaPresupuesto({
      titulo: "Total Mensual",
      monto,
      usado,
      categoriaId: null,
      tipo: "mensual"
    })}
  `;

  document.getElementById("contenidoPresupuesto").innerHTML = html;
}

/* ======================================================
   C) PRESUPUESTO ANUAL GENERAL
====================================================== */

async function cargarPresupuestoAnual() {
  const monto = 15000 * 12; // Simulado
  const usado = randomGastoSimulado() * 10;

  const html = `
    <h2>Presupuesto anual</h2>
    ${crearTarjetaPresupuesto({
      titulo: "Total Año",
      monto,
      usado,
      categoriaId: null,
      tipo: "anual"
    })}
  `;

  document.getElementById("contenidoPresupuesto").innerHTML = html;
}

/* ======================================================
   CREAR TARJETA VISUAL (estilo Apple)
====================================================== */

function crearTarjetaPresupuesto({ titulo, monto, usado, categoriaId, tipo }) {
  const porcentaje = monto > 0 ? (usado / monto * 100) : 0;

  let color = "normal";
  if (porcentaje >= 100) color = "danger";
  else if (porcentaje >= 80) color = "danger";
  else if (porcentaje >= 50) color = "warning";

  return `
    <div class="card-apple">
      <h3>${titulo}</h3>
      <p>Presupuesto: <strong>${monto.toLocaleString()} RD$</strong></p>
      <p>Gastado: <strong>${usado.toLocaleString()} RD$</strong></p>

      <div class="progress-container">
        <div class="progress-bar ${color}" style="width:${porcentaje}%"></div>
      </div>

      <p style="margin-top:10px;">
        <strong>${porcentaje.toFixed(1)}%</strong> usado
      </p>

      <button class="btn btn-accent" onclick="editarPresupuesto('${categoriaId}', '${tipo}', ${monto})">
        Editar
      </button>
    </div>
  `;
}

/* ======================================================
   SIMULADOR DE GASTOS (mientras creamos Expenses)
====================================================== */
function randomGastoSimulado() {
  return Math.floor(Math.random() * 12000) + 2000;
}

/* ======================================================
   EDITAR PRESUPUESTO (interfaz simple)
====================================================== */
function editarPresupuesto(id, tipo, actual) {
  const nuevo = prompt(`Nuevo monto para ${tipo}:`, actual);

  if (!nuevo || isNaN(nuevo)) return alert("Valor inválido");

  alert(`(PROXIMO PASO) Aquí enviaremos el PUT a la API con: ${nuevo} `);

  // FUTURO:
  // PUT /api/Budgets/{id}
}
