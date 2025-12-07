// Servicio centralizado de API

// Función genérica para hacer peticiones HTTP
async function fetchAPI(endpoint, options = {}) {
    const token = getToken();
    
    const defaultHeaders = {
        'Content-Type': 'application/json'
    };
    
    if (token) {
        defaultHeaders['Authorization'] = `Bearer ${token}`;
    }
    
    const config = {
        ...options,
        headers: {
            ...defaultHeaders,
            ...options.headers
        }
    };
    
    try {
        const response = await fetch(`${API_CONFIG.BASE_URL}${endpoint}`, config);
        
        // Si es 401, redirigir al login
        if (response.status === 401) {
            removeToken();
            window.location.href = 'index.html';
            return null;
        }
        
        // Si es 204 No Content, retornar true
        if (response.status === 204) {
            return { success: true };
        }
        
        const data = await response.json();
        
        if (!response.ok) {
            throw new Error(data.Message || data.message || 'Error en la petición');
        }
        
        return data;
    } catch (error) {
        console.error('API Error:', error);
        throw error;
    }
}

// Métodos HTTP
const API = {
    get: (endpoint) => fetchAPI(endpoint, { method: 'GET' }),
    
    post: (endpoint, body) => fetchAPI(endpoint, {
        method: 'POST',
        body: JSON.stringify(body)
    }),
    
    put: (endpoint, body) => fetchAPI(endpoint, {
        method: 'PUT',
        body: JSON.stringify(body)
    }),
    
    delete: (endpoint) => fetchAPI(endpoint, { method: 'DELETE' }),
    
    // Para subir archivos
    uploadFile: async (endpoint, formData) => {
        const token = getToken();
        const headers = {};
        
        if (token) {
            headers['Authorization'] = `Bearer ${token}`;
        }
        
        try {
            const response = await fetch(`${API_CONFIG.BASE_URL}${endpoint}`, {
                method: 'POST',
                headers: headers,
                body: formData
            });
            
            if (response.status === 401) {
                removeToken();
                window.location.href = 'index.html';
                return null;
            }
            
            const data = await response.json();
            
            if (!response.ok) {
                throw new Error(data.Message || data.message || 'Error al subir archivo');
            }
            
            return data;
        } catch (error) {
            console.error('Upload Error:', error);
            throw error;
        }
    },
    
    // Para descargar archivos
    downloadFile: async (endpoint) => {
        const token = getToken();
        const headers = {};
        
        if (token) {
            headers['Authorization'] = `Bearer ${token}`;
        }
        
        try {
            const response = await fetch(`${API_CONFIG.BASE_URL}${endpoint}`, {
                method: 'GET',
                headers: headers
            });
            
            if (!response.ok) {
                throw new Error('Error al descargar archivo');
            }
            
            return response.blob();
        } catch (error) {
            console.error('Download Error:', error);
            throw error;
        }
    }
};

// Servicios específicos
const ExpenseService = {
    getAll: () => API.get(API_CONFIG.ENDPOINTS.EXPENSES),
    getById: (id) => API.get(`${API_CONFIG.ENDPOINTS.EXPENSES}/${id}`),
    create: (expense) => API.post(API_CONFIG.ENDPOINTS.EXPENSES, expense),
    update: (id, expense) => API.put(`${API_CONFIG.ENDPOINTS.EXPENSES}/${id}`, expense),
    delete: (id) => API.delete(`${API_CONFIG.ENDPOINTS.EXPENSES}/${id}`),
    import: (formData) => API.uploadFile(`${API_CONFIG.ENDPOINTS.EXPENSES}/import`, formData)
};

const CategoryService = {
    getAll: () => API.get(API_CONFIG.ENDPOINTS.CATEGORIES),
    create: (category) => API.post(API_CONFIG.ENDPOINTS.CATEGORIES, category)
};

const PaymentMethodService = {
    getAll: () => API.get(API_CONFIG.ENDPOINTS.PAYMENT_METHODS),
    create: (method) => API.post(API_CONFIG.ENDPOINTS.PAYMENT_METHODS, method)
};

const BudgetService = {
    getAll: () => API.get(API_CONFIG.ENDPOINTS.BUDGETS),
    getByCategoryMonth: (categoryId, month, year) => 
        API.get(`${API_CONFIG.ENDPOINTS.BUDGETS}/${categoryId}/${month}/${year}`),
    create: (budget) => API.post(API_CONFIG.ENDPOINTS.BUDGETS, budget),
    update: (id, budget) => API.put(`${API_CONFIG.ENDPOINTS.BUDGETS}/${id}`, budget),
    delete: (id) => API.delete(`${API_CONFIG.ENDPOINTS.BUDGETS}/${id}`),
    getPercentage: (categoryId, month, year) =>
        API.get(`${API_CONFIG.ENDPOINTS.BUDGETS}/percentage/${categoryId}/${month}/${year}`),
    getExceeded: (month, year) =>
        API.get(`${API_CONFIG.ENDPOINTS.BUDGETS}/exceeded/${month}/${year}`)
};

const ReportService = {
    filter: (params) => {
        const queryParams = new URLSearchParams();
        if (params.startDate) queryParams.append('startDate', params.startDate);
        if (params.endDate) queryParams.append('endDate', params.endDate);
        if (params.categoryId) queryParams.append('categoryId', params.categoryId);
        if (params.paymentMethodId) queryParams.append('paymentMethodId', params.paymentMethodId);
        if (params.search) queryParams.append('search', params.search);
        
        return API.get(`${API_CONFIG.ENDPOINTS.REPORTS}/filter?${queryParams.toString()}`);
    },
    monthly: (month, year) =>
        API.get(`${API_CONFIG.ENDPOINTS.REPORTS}/monthly/${month}/${year}`),
    exportExcel: () => API.downloadFile(`${API_CONFIG.ENDPOINTS.REPORTS}/export/excel`),
    exportJson: () => API.downloadFile(`${API_CONFIG.ENDPOINTS.REPORTS}/export/json`),
    exportTxt: () => API.downloadFile(`${API_CONFIG.ENDPOINTS.REPORTS}/export/txt`)
};

const AuthService = {
    login: (credentials) => API.post(API_CONFIG.ENDPOINTS.AUTH.LOGIN, credentials),
    register: (userData) => API.post(API_CONFIG.ENDPOINTS.AUTH.REGISTER, userData),
    getProfile: () => API.get(API_CONFIG.ENDPOINTS.AUTH.PROFILE),
    updateProfile: (data) => API.put(API_CONFIG.ENDPOINTS.AUTH.UPDATE_PROFILE, data),
    changePassword: (data) => API.post(API_CONFIG.ENDPOINTS.AUTH.CHANGE_PASSWORD, data)
};