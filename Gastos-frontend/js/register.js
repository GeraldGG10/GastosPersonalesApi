async function registrar() {
  const nombre = document.getElementById("name").value.trim();
  const correo = document.getElementById("email").value.trim();
  const password = document.getElementById("password").value.trim();
  const password2 = document.getElementById("password2").value.trim();

  if (!nombre || !correo || !password || !password2) {
    alert("Completa todos los campos.");
    return;
  }

  if (password !== password2) {
    alert("Las contraseñas no coinciden.");
    return;
  }

  try {
    const res = await fetch(`${API_URL}/Auth/register`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ nombre, correo, password })
    });

    const text = await res.text();

    if (!res.ok) {
      alert("Error al registrar: " + text);
      return;
    }

    alert("Cuenta creada exitosamente.");
    window.location.href = "index.html";

  } catch (e) {
    console.error(e);
    alert("Error de conexión con el servidor.");
  }
}
