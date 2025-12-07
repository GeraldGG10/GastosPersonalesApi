// Configuración global de la API
const API_CONFIG = {
    BASE_URL: 'https://localhost:7262/api', // Cambia esto por tu URL del backend
    ENDPOINTS: {
        AUTH: {
            LOGIN: '/auth/login',
            REGISTER: '/auth/register',
            PROFILE: '/auth/profile',
            UPDATE_PROFILE: '/auth/profile',
            CHANGE_PASSWORD: '/auth/change-password'
        },
        EXPENSES: '/expenses',
        CATEGORIES: '/categories',
        PAYMENT_METHODS: '/paymentmethods',
        BUDGETS: '/budgets',
        REPORTS: '/reports'
    }
};

// Obtener token del localStorage
function getToken() {
    return localStorage.getItem('token');
}

// Guardar token en localStorage
function saveToken(token) {
    localStorage.setItem('token', token);
}

// Eliminar token del localStorage
function removeToken() {
    localStorage.removeItem('token');
}

// Verificar si el usuario está autenticado
function isAuthenticated() {
    return !!getToken();
}

// Verificar autenticación en páginas protegidas
function checkAuth() {
    if (!isAuthenticated() && !window.location.pathname.includes('index.html')) {
        window.location.href = 'index.html';
    }
}