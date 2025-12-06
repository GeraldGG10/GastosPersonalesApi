// js/auth.js
async function login() {
  const correo = document.getElementById("email").value.trim();
  const password = document.getElementById("password").value.trim();

  if (!correo || !password) {
    alert("Completa todos los campos.");
    return;
  }

  try {
    // Usamos apiFetch para normalizar headers y token
    const res = await apiFetch("/Auth/login", {
      method: "POST",
      body: JSON.stringify({ correo, password })
    });

    // Si la respuesta tiene token, guardamos
    if (res && res.token) {
      localStorage.setItem("token", res.token);
      // opcional: guardar usuario
      localStorage.setItem("usuario", JSON.stringify({ nombre: res.nombre, email: res.email }));
      window.location.href = "dashboard.html";
      return;
    }

    // si viene texto o mensaje de error
    const message = (res && res.message) ? res.message : JSON.stringify(res);
    alert("Error al iniciar sesión: " + message);
  } catch (e) {
    console.error(e);
    alert("Error de conexión con el servidor.");
  }
}
