document.addEventListener("DOMContentLoaded", () => {
  cargarCategorias();
  cargarMetodos();
});

async function cargarCategorias() {
  const res = await apiFetch("/Categories"); // usa apiFetch para token automático
  const select = document.getElementById("categoriaId");
  select.innerHTML = ""; // limpiar antes de cargar

  res.forEach(cat => {
    select.innerHTML += `<option value="${cat.id}">${cat.name}</option>`;
  });
}

async function cargarMetodos() {
  const res = await apiFetch("/PaymentMethods"); // usa apiFetch
  const select = document.getElementById("paymentMethodId");
  select.innerHTML = ""; // limpiar antes de cargar

  res.forEach(m => {
    select.innerHTML += `<option value="${m.id}">${m.name}</option>`;
  });
}

document.getElementById("gastoForm").addEventListener("submit", async (e) => {
  e.preventDefault();

  const gasto = {
    amount: parseFloat(document.getElementById("amount").value),
    categoryId: parseInt(document.getElementById("categoriaId").value),
    paymentMethodId: parseInt(document.getElementById("paymentMethodId").value),
    date: document.getElementById("fecha").value,
    description: document.getElementById("descripcion").value
  };

  const res = await apiFetch("/Expenses", {
    method: "POST",
    body: JSON.stringify(gasto)
  });

  if (res && res.id) {
    alert("Gasto registrado correctamente");
    document.getElementById("gastoForm").reset();
  } else {
    alert("Error al registrar gasto");
  }
});
