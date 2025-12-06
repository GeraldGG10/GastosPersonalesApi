// js/auth.js
async function login() {
  const correo = document.getElementById("email").value.trim();
  const password = document.getElementById("password").value.trim();

  if (!correo || !password) {
    alert("Completa todos los campos.");
    return;
  }

  try {
    console.log("=== INICIO LOGIN ===");
    console.log("1. Email:", correo);
    console.log("2. Password length:", password.length);
    
    // Backend espera "Email" y "Password" (con mayúsculas)
    const payload = {
      Email: correo,
      Password: password
    };
    
    console.log("3. Payload preparado:", { Email: correo, Password: "****" });
    console.log("4. Haciendo petición a /Auth/login...");
    
    const res = await apiFetch("/Auth/login", {
      method: "POST",
      body: JSON.stringify(payload)
    });

    console.log("5. Respuesta recibida:");
    console.log("   - Tipo:", typeof res);
    console.log("   - Contenido:", res);
    console.log("   - Keys:", res ? Object.keys(res) : "null");

    // Verificar si hay token en la respuesta
    const token = res?.Token || res?.token;
    
    if (!token) {
      console.error("❌ NO SE RECIBIÓ TOKEN");
      console.error("   Respuesta completa:", JSON.stringify(res, null, 2));
      
      // Verificar si hay mensaje de error
      const errorMsg = res?.Message || res?.message || res?.error || "No se recibió token del servidor";
      alert("Error al iniciar sesión: " + errorMsg);
      console.log("=== FIN LOGIN CON ERROR (SIN TOKEN) ===");
      return;
    }

    console.log("6. ✓ Token recibido:");
    console.log("   - Longitud:", token.length);
    console.log("   - Primeros 30 chars:", token.substring(0, 30) + "...");
    
    // Intentar guardar en localStorage
    console.log("7. Guardando token en localStorage...");
    try {
      localStorage.setItem("token", token);
      console.log("   ✓ localStorage.setItem ejecutado");
    } catch (storageError) {
      console.error("   ❌ ERROR al guardar en localStorage:", storageError);
      alert("Error: No se pudo guardar la sesión. Verifica los permisos del navegador.");
      return;
    }
    
    // Verificar inmediatamente si se guardó
    const tokenVerificacion = localStorage.getItem("token");
    console.log("8. Verificación inmediata:");
    console.log("   - Token en localStorage:", tokenVerificacion ? "✓ EXISTE" : "❌ NO EXISTE");
    console.log("   - Son iguales:", tokenVerificacion === token ? "✓ SÍ" : "❌ NO");
    
    if (!tokenVerificacion) {
      console.error("❌ FALLO CRÍTICO: Token no se guardó en localStorage");
      alert("Error crítico: No se pudo guardar la sesión.");
      return;
    }
    
    // Guardar datos del usuario
    console.log("9. Guardando datos del usuario...");
    const userData = {
      nombre: res.FullName || res.fullName || "",
      email: res.Email || res.email || correo
    };
    localStorage.setItem("usuario", JSON.stringify(userData));
    console.log("   ✓ Usuario guardado:", userData);
    
    console.log("10. ✓ TODO GUARDADO CORRECTAMENTE");
    console.log("11. Redirigiendo a dashboard en 500ms...");
    console.log("=== FIN LOGIN EXITOSO ===");
    
    // Pequeño delay para asegurar que localStorage se guardó
    setTimeout(() => {
      console.log("12. Ejecutando redirección...");
      window.location.replace("dashboard.html");
    }, 500);
    
  } catch (e) {
    console.error("❌ EXCEPCIÓN EN LOGIN:", e);
    console.error("   Stack:", e.stack);
    alert("Error de conexión con el servidor: " + e.message);
    console.log("=== FIN LOGIN CON EXCEPCIÓN ===");
  }
}

// Hacer la función accesible globalmente
window.login = login;

// Al cargar el script
console.log("✓ auth.js cargado correctamente");
console.log("  Token actual en localStorage:", localStorage.getItem("token") ? "Existe" : "No existe");

// Test de localStorage
try {
  localStorage.setItem("test", "123");
  const testVal = localStorage.getItem("test");
  localStorage.removeItem("test");
  console.log("  Test de localStorage:", testVal === "123" ? "✓ FUNCIONA" : "❌ FALLA");
} catch (e) {
  console.error("  ❌ localStorage NO DISPONIBLE:", e.message);
}