// Lógica de autenticación

document.addEventListener('DOMContentLoaded', () => {
    // Si ya está autenticado, redirigir al dashboard
    if (isAuthenticated()) {
        window.location.href = 'dashboard.html';
        return;
    }

    const loginForm = document.getElementById('loginForm');
    const registerForm = document.getElementById('registerForm');
    const showRegisterBtn = document.getElementById('showRegister');
    const showLoginBtn = document.getElementById('showLogin');
    const loginFormElement = document.getElementById('loginFormElement');
    const registerFormElement = document.getElementById('registerFormElement');

    // Alternar entre login y registro
    showRegisterBtn.addEventListener('click', (e) => {
        e.preventDefault();
        loginForm.classList.remove('active');
        registerForm.classList.add('active');
        hideError('loginError');
    });

    showLoginBtn.addEventListener('click', (e) => {
        e.preventDefault();
        registerForm.classList.remove('active');
        loginForm.classList.add('active');
        hideError('registerError');
    });

    // Manejar login
    loginFormElement.addEventListener('submit', async (e) => {
        e.preventDefault();
        hideError('loginError');

        const email = document.getElementById('loginEmail').value;
        const password = document.getElementById('loginPassword').value;

        try {
            const response = await AuthService.login({
                email: email,
                password: password
            });

            if (response && response.token) {
                saveToken(response.token);
                localStorage.setItem('userEmail', response.email);
                localStorage.setItem('userName', response.fullName);
                window.location.href = 'dashboard.html';
            } else {
                showError('loginError', 'Credenciales inválidas');
            }
        } catch (error) {
            showError('loginError', error.message || 'Error al iniciar sesión');
        }
    });

    // Manejar registro
    registerFormElement.addEventListener('submit', async (e) => {
        e.preventDefault();
        hideError('registerError');

        const name = document.getElementById('registerName').value;
        const email = document.getElementById('registerEmail').value;
        const password = document.getElementById('registerPassword').value;

        // Validaciones
        if (!isValidEmail(email)) {
            showError('registerError', 'Email inválido');
            return;
        }

        if (password.length < 6) {
            showError('registerError', 'La contraseña debe tener al menos 6 caracteres');
            return;
        }

        try {
            const response = await AuthService.register({
                fullName: name,
                email: email,
                password: password
            });

            if (response && response.token) {
                saveToken(response.token);
                localStorage.setItem('userEmail', response.email);
                localStorage.setItem('userName', response.fullName);
                window.location.href = 'dashboard.html';
            } else {
                showError('registerError', 'El usuario ya existe o datos inválidos');
            }
        } catch (error) {
            showError('registerError', error.message || 'Error al registrarse');
        }
    });
});