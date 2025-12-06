if (!localStorage.getItem("token")) location.href = "index.html";

async function fetchGastos() {
  const list = await apiFetch("/Expenses") || [];
  const tbody = document.getElementById("tbodyGastos");
  tbody.innerHTML = "";

  list.forEach(g => {
    const tr = document.createElement("tr");
    tr.innerHTML = `<td>${new Date(g.date || g.fecha).toLocaleDateString()}</td>
                    <td>${g.description || g.descripcion || ""}</td>
                    <td>${g.categoryName || g.categoriaNombre || "-"}</td>
                    <td>$ ${formatCurrency(g.amount || g.monto)}</td>
                    <td><button class="btn" onclick='eliminar("${g.id}")'>Eliminar</button></td>`;
    tbody.appendChild(tr);
  });
}

function mostrarCrear() {
  document.getElementById("modalCrear").style.display = "block";
}

function cerrarModal(ev) {
  if (!ev || ev.target) document.getElementById("modalCrear").style.display = "none";
}

async function crearGasto() {
  const fecha = document.getElementById("g_fecha").value;
  const descripcion = document.getElementById("g_des").value;
  const monto = parseFloat(document.getElementById("g_monto").value);
  const categoria = document.getElementById("g_cat").value;

  if (!fecha || !monto || !categoria) { 
    alert("Complete fecha, monto y categoría."); 
    return; 
  }

  const res = await apiFetch("/Expenses", {
    method: "POST",
    body: JSON.stringify({ date: fecha, description: descripcion, amount: monto, categoryId: categoria })
  });

  if (res && res.id) {
    cerrarModal();
    fetchGastos();
  } else {
    alert("Error creando gasto.");
  }
}

async function eliminar(id) {
  if (!confirm("Eliminar gasto?")) return;
  await apiFetch(`/Expenses/${id}`, { method: "DELETE" });
  fetchGastos();
}

fetchGastos();
