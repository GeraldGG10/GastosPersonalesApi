async function obtenerCategorias() {
  const token = localStorage.getItem("token");

  const res = await fetch(`${API_URL}/Categories`, {
    method: "GET",
    headers: { "Authorization": `Bearer ${token}` }
  });

  const data = await res.json();

  let html = "";

  data.forEach(cat => {
    html += `
      <div class="item-row">
        <span>${cat.name}</span>
        <button class="btn" onclick="eliminarCategoria(${cat.id})">Eliminar</button>
      </div>
    `;
  });

  document.getElementById("listaCategorias").innerHTML = html;
}

async function crearCategoria() {
  const nombre = document.getElementById("nombreCategoria").value.trim();
  const token = localStorage.getItem("token");

  if (!nombre) return alert("Escribe un nombre");

  const res = await fetch(`${API_URL}/Categories`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      "Authorization": `Bearer ${token}`
    },
    body: JSON.stringify({ name: nombre, isActive: true })
  });

  if (res.ok) {
    cerrarModal();
    obtenerCategorias();
  } else {
    alert("Error al crear categoría");
  }
}

async function eliminarCategoria(id) {
  const token = localStorage.getItem("token");

  if (!confirm("¿Seguro que quieres eliminarla?")) return;

  const res = await fetch(`${API_URL}/Categories/${id}`, {
    method: "DELETE",
    headers: { "Authorization": `Bearer ${token}` }
  });

  if (res.ok) obtenerCategorias();
  else alert("Error al eliminar categoría");
}

function abrirModal() {
  document.getElementById("modalCategoria").classList.remove("hidden");
}

function cerrarModal() {
  document.getElementById("modalCategoria").classList.add("hidden");
}

document.addEventListener("DOMContentLoaded", obtenerCategorias);
