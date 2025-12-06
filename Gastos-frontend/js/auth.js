async function login() {
  const correo = document.getElementById("email").value.trim();
  const password = document.getElementById("password").value.trim();

  if (!correo || !password) {
    alert("Completa todos los campos.");
    return;
  }

  try {
    const res = await fetch(`${API_URL}/Auth/login`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ correo, password })
    });

    const text = await res.text();

    if (!res.ok) {
      alert("Error al iniciar sesión: " + text);
      return;
    }

    const data = JSON.parse(text);

    localStorage.setItem("token", data.token);

    alert("Inicio de sesión exitoso.");
    window.location.href = "dashboard.html";

  } catch (e) {
    console.error(e);
    alert("Error de conexión con el servidor.");
  }
}
