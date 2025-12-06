document.addEventListener("DOMContentLoaded", () => {
  cargarCategorias();
  cargarMetodos();
});

async function cargarCategorias() {
  const token = localStorage.getItem("token");

  const res = await fetch(`${API_URL}/Categories`, {
    headers: { "Authorization": `Bearer ${token}` }
  });

  const data = await res.json();
  const select = document.getElementById("categoriaId");

  data.forEach(cat => {
    select.innerHTML += `<option value="${cat.id}">${cat.name}</option>`;
  });
}

async function cargarMetodos() {
  const token = localStorage.getItem("token");

  const res = await fetch(`${API_URL}/PaymentMethods`, {
    headers: { "Authorization": `Bearer ${token}` }
  });

  const data = await res.json();
  const select = document.getElementById("paymentMethodId");

  data.forEach(m => {
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

  const token = localStorage.getItem("token");

  const res = await fetch(`${API_URL}/Expenses`, {
    method: "POST",
    headers: {
      "Authorization": `Bearer ${token}`,
      "Content-Type": "application/json"
    },
    body: JSON.stringify(gasto)
  });

  if (res.ok) {
    alert("Gasto registrado correctamente");
    document.getElementById("gastoForm").reset();
  } else {
    alert("Error al registrar gasto");
  }
});
