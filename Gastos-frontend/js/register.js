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
    console.log("=== INICIO REGISTRO ===");
    console.log("Intentando registrar usuario:", correo);
    
    // ✅ CORREGIDO: Backend espera "Email", "Password" y "FullName" (con mayúsculas)
    const payload = {
      Email: correo,      // Backend espera "Email", no "correo"
      Password: password, // Backend espera "Password"
      FullName: nombre    // Backend espera "FullName", no "nombre"
    };
    
    console.log("Datos enviados:", { Email: correo, Password: "****", FullName: nombre });
    
    const res = await apiFetch("/Auth/register", {
      method: "POST",
      body: JSON.stringify(payload)
    });

    console.log("Respuesta del servidor:", res);
    console.log("Tipo de respuesta:", typeof res);

    // Si hay un mensaje de error específico, mostrarlo
    if (res && (res.Message || res.message)) {
      const errorMsg = res.Message || res.message;
      console.error("Error del servidor:", errorMsg);
      alert("Error al registrar: " + errorMsg);
      return;
    }

    // Si hay errores de validación (común en .NET)
    if (res && res.errors) {
      console.error("Errores de validación:", res.errors);
      const erroresTexto = Object.values(res.errors).flat().join('\n');
      alert("Errores de validación:\n" + erroresTexto);
      return;
    }

    // Si hay un título de error (común en .NET)
    if (res && res.title) {
      console.error("Error:", res.title, res.detail || '');
      alert("Error: " + res.title + (res.detail ? '\n' + res.detail : ''));
      return;
    }

    // ✅ Si el registro retorna un Token, guardarlo y redirigir al dashboard
    if (res && (res.Token || res.token)) {
      const token = res.Token || res.token;
      console.log("✓ Registro exitoso con token, iniciando sesión automáticamente...");
      localStorage.setItem("token", token);
      
      if (res.Email || res.FullName) {
        localStorage.setItem("usuario", JSON.stringify({ 
          nombre: res.FullName || nombre, 
          email: res.Email || correo 
        }));
      }
      
      alert("Cuenta creada correctamente. Redirigiendo...");
      window.location.href = "dashboard.html";
      return;
    }

    // Si llegamos aquí y hay un ID, asumir éxito pero sin auto-login
    if (res && res.Id) {
      console.log("✓ Registro exitoso");
      alert("Cuenta creada correctamente.");
      window.location.href = "index.html";
      return;
    }

    // Si backend devuelve objeto vacío en success
    if (!res || (typeof res === "object" && Object.keys(res).length === 0)) {
      console.log("✓ Registro exitoso (respuesta vacía)");
      alert("Cuenta creada correctamente.");
      window.location.href = "index.html";
      return;
    }

    // fallback - mostrar toda la respuesta
    console.log("Respuesta no manejada:", res);
    alert("Cuenta creada correctamente.");
    window.location.href = "index.html";
    console.log("=== FIN REGISTRO ===");
    
  } catch (e) {
    console.error("❌ Error en registro:", e);
    alert("Error de conexión con el servidor.");
    console.log("=== FIN REGISTRO CON EXCEPCIÓN ===");
  }
}

// Hacer la función accesible globalmente
window.registrar = registrar;