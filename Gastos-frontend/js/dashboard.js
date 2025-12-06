if (!localStorage.getItem("token")) location.href = "index.html";

async function cargarDashboard() {
  try {
    const expenses = await apiFetch("/Expenses");
    const expenseList = Array.isArray(expenses) ? expenses : (expenses?.items || []);

    const total = expenseList.reduce((sum, e) => sum + (e.amount || 0), 0);

    const hoy = new Date();
    const mesActual = hoy.getMonth();
    const yearActual = hoy.getFullYear();
    
    const gastosMes = expenseList
      .filter(e => {
        const d = new Date(e.date);
        return d.getMonth() === mesActual && d.getFullYear() === yearActual;
      })
      .reduce((sum, e) => sum + (e.amount || 0), 0);

    const porCategoria = {};
    expenseList.forEach(e => {
      const catId = e.categoryId || 0;
      if (!porCategoria[catId]) {
        porCategoria[catId] = { id: catId, total: 0, count: 0 };
      }
      porCategoria[catId].total += e.amount || 0;
      porCategoria[catId].count++;
    });

    const categorias = Object.values(porCategoria).sort((a, b) => b.total - a.total);
    const categoriaTop = categorias.length > 0 ? `Categoría ${categorias[0].id}` : "—";

    document.getElementById("totalGastado").innerText = "$ " + formatCurrency(total);
    document.getElementById("gastosMes").innerText = "$ " + formatCurrency(gastosMes);
    document.getElementById("categoriaTop").innerText = categoriaTop;

    const ctx = document.getElementById("chartCategorias").getContext("2d");
    const top5 = categorias.slice(0, 5);
    
    new Chart(ctx, {
      type: "doughnut",
      data: {
        labels: top5.map(c => `Categoría ${c.id}`),
        datasets: [{
          data: top5.map(c => c.total),
          backgroundColor: [
            "rgba(255,106,0,0.9)",
            "rgba(10,102,255,0.9)",
            "rgba(76,217,100,0.9)",
            "rgba(255,204,0,0.9)",
            "rgba(255,69,58,0.9)"
          ]
        }]
      },
      options: { 
        plugins: { 
          legend: { labels: { color: "#dcdcdc" } } 
        } 
      }
    });
  } catch (err) {
    console.error(err);
    alert("Error al cargar el dashboard.");
  }
}

cargarDashboard();
