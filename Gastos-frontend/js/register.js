// js/register.js
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
    const res = await apiFetch("/Auth/register", {
      method: "POST",
      body: JSON.stringify({ nombre, correo, password })
    });

    // si backend devuelve objeto vacío en success, res puede ser {}
    if (!res || (typeof res === "object" && Object.keys(res).length === 0)) {
      // asumir success si status OK (apiFetch no devuelve status aquí),
      // pero mejor verificar en backend. Informar al usuario:
      alert("Cuenta creada correctamente.");
      window.location.href = "index.html";
      return;
    }

    // si hay mensaje de error
    if (res && res.message) {
      alert("Error al registrar: " + res.message);
      return;
    }

    // fallback
    alert("Cuenta creada correctamente.");
    window.location.href = "index.html";
  } catch (e) {
    console.error(e);
    alert("Error de conexión con el servidor.");
  }
}
