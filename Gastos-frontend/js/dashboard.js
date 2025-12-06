if (!localStorage.getItem("token")) location.href = "index.html";

async function cargarDashboard() {
  try {
    const data = await apiFetch("/reportes/dashboard"); // tu endpoint
    // Seguridad: manejar distintas formas de respuesta
    const total = data?.totalGastado ?? data?.total ?? 0;
    const mes = data?.gastosMes ?? 0;
    const top = data?.categoriaTop ?? (data?.categorias && data.categorias[0]?.nombre) ?? "—";
    document.getElementById("totalGastado").innerText = "$ " + formatCurrency(total);
    document.getElementById("gastosMes").innerText = "$ " + formatCurrency(mes);
    document.getElementById("categoriaTop").innerText = top;

    const categorias = data?.categorias ?? [];
    const ctx = document.getElementById("chartCategorias").getContext("2d");
    new Chart(ctx, {
      type: "doughnut",
      data: {
        labels: categorias.map(c => c.nombre),
        datasets: [{
          data: categorias.map(c => c.total),
          backgroundColor: categorias.map((c,i) => i%2==0 ? "rgba(255,106,0,0.9)" : "rgba(10,102,255,0.9)")
        }]
      },
      options: { plugins: { legend: { labels: { color: "#dcdcdc" } } } }
    });
  } catch (err) {
    console.error(err);
    alert("Error al cargar el dashboard. Revisa la API o el token.");
  }
}

cargarDashboard();
