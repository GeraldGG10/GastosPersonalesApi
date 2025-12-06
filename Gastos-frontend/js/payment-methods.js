async function obtenerMetodos() {
  const token = localStorage.getItem("token");

  const res = await fetch(`${API_URL}/PaymentMethods`, {
    method: "GET",
    headers: { "Authorization": `Bearer ${token}` }
  });

  const data = await res.json();

  let html = "";

  data.forEach(m => {
    html += `
      <div class="item-row">
        <span>${m.name}</span>
        <button class="btn" onclick="eliminarMetodo(${m.id})">Eliminar</button>
      </div>
    `;
  });

  document.getElementById("listaMetodos").innerHTML = html;
}

async function crearMetodo() {
  const nombre = document.getElementById("nombreMetodo").value.trim();
  const token = localStorage.getItem("token");

  if (!nombre) return alert("Escribe un nombre");

  const res = await fetch(`${API_URL}/PaymentMethods`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      "Authorization": `Bearer ${token}`
    },
    body: JSON.stringify({ name: nombre, icon: "", isActive: true })
  });

  if (res.ok) {
    cerrarModal();
    obtenerMetodos();
  } else {
    alert("Error al crear método");
  }
}

async function eliminarMetodo(id) {
  const token = localStorage.getItem("token");

  if (!confirm("¿Seguro que quieres eliminarlo?")) return;

  const res = await fetch(`${API_URL}/PaymentMethods/${id}`, {
    method: "DELETE",
    headers: { "Authorization": `Bearer ${token}` }
  });

  if (res.ok) obtenerMetodos();
  else alert("Error al eliminar método");
}

function abrirModal() {
  document.getElementById("modalMetodo").classList.remove("hidden");
}

function cerrarModal() {
  document.getElementById("modalMetodo").classList.add("hidden");
}

document.addEventListener("DOMContentLoaded", obtenerMetodos);
