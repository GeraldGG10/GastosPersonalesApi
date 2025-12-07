// Script para manejar el menú móvil
// Agregar este script a todos los archivos HTML después de utils.js

document.addEventListener('DOMContentLoaded', () => {
    // Crear botón hamburguesa si no existe
    if (!document.querySelector('.menu-toggle')) {
        const menuToggle = document.createElement('button');
        menuToggle.className = 'menu-toggle';
        menuToggle.innerHTML = '☰';
        menuToggle.setAttribute('aria-label', 'Toggle menu');
        document.body.appendChild(menuToggle);

        // Toggle sidebar
        menuToggle.addEventListener('click', () => {
            const sidebar = document.querySelector('.sidebar');
            sidebar.classList.toggle('active');
        });

        // Cerrar sidebar al hacer click en un enlace (mobile)
        const sidebarLinks = document.querySelectorAll('.sidebar-menu a');
        sidebarLinks.forEach(link => {
            link.addEventListener('click', () => {
                if (window.innerWidth <= 768) {
                    document.querySelector('.sidebar').classList.remove('active');
                }
            });
        });

        // Cerrar sidebar al hacer click fuera (mobile)
        document.addEventListener('click', (e) => {
            const sidebar = document.querySelector('.sidebar');
            const isClickInsideSidebar = sidebar.contains(e.target);
            const isClickOnToggle = menuToggle.contains(e.target);

            if (!isClickInsideSidebar && !isClickOnToggle && sidebar.classList.contains('active')) {
                if (window.innerWidth <= 768) {
                    sidebar.classList.remove('active');
                }
            }
        });
    }
});