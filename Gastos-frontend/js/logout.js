function logout() {
  localStorage.removeItem("token");
  window.location.href = "index.html"; // Te manda al inicio de sesión
}
