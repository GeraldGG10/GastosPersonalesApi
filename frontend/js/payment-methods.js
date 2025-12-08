// Lógica de gestión de métodos de pago

checkAuth();

let paymentMethods = [];
let editingMethodId = null;

document.addEventListener('DOMContentLoaded', async () => {
    await loadPaymentMethods();
    setupEventListeners();
});

function setupEventListeners() {
    document.getElementById('addMethodBtn').addEventListener('click', () => {
        editingMethodId = null;
        document.getElementById('modalTitle').textContent = 'Nuevo Método de Pago';
        document.getElementById('methodForm').reset();
        document.getElementById('methodActive').checked = true;
        openModal('methodModal');
    });

    document.getElementById('methodForm').addEventListener('submit', handleMethodSubmit);

    document.querySelector('#methodModal .close').addEventListener('click', () => {
        closeModal('methodModal');
    });

    document.getElementById('cancelMethodBtn').addEventListener('click', () => {
        closeModal('methodModal');
    });
}

async function loadPaymentMethods() {
    try {
        paymentMethods = await PaymentMethodService.getAll();
        loadMethodsTable();
    } catch (error) {
        console.error('Error al cargar métodos de pago:', error);
        showToast('Error al cargar los métodos de pago', 'error');
    }
}

function loadMethodsTable() {
    const tbody = document.getElementById('methodsTableBody');

    if (paymentMethods.length === 0) {
        tbody.innerHTML = '<tr><td colspan="5" class="empty-state">No hay métodos de pago registrados</td></tr>';
        return;
    }

    tbody.innerHTML = paymentMethods.map(method => `
        <tr>
            <td>${method.id}</td>
            <td>${method.name}</td>
            <td>${method.icon || '-'}</td>
            <td>
                <span class="badge ${method.isActive ? 'success' : 'danger'}">
                    ${method.isActive ? 'Activo' : 'Inactivo'}
                </span>
            </td>
            <td>
                <div class="action-buttons">
                    <button class="btn-icon" onclick="editMethod(${method.id})" title="Editar">✏️</button>
                    <button class="btn-icon" onclick="deleteMethod(${method.id})" title="Eliminar">🗑️</button>
                </div>
            </td>
        </tr>
    `).join('');
}

async function handleMethodSubmit(e) {
    e.preventDefault();

    const methodData = {
        name: document.getElementById('methodName').value,
        icon: document.getElementById('methodIcon').value,
        isActive: document.getElementById('methodActive').checked
    };

    try {
        if (editingMethodId) {
            await PaymentMethodService.update(editingMethodId, methodData);
            showToast('Método de pago actualizado correctamente', 'success');
        } else {
            await PaymentMethodService.create(methodData);
            showToast('Método de pago guardado correctamente', 'success');
        }

        closeModal('methodModal');
        await loadPaymentMethods();
    } catch (error) {
        console.error('Error al guardar método:', error);
        showToast(error.message || 'Error al guardar el método de pago', 'error');
    }
}

function editMethod(id) {
    editingMethodId = id;
    const method = paymentMethods.find(m => m.id === id);

    if (!method) return;

    document.getElementById('modalTitle').textContent = 'Editar Método de Pago';
    document.getElementById('methodName').value = method.name;
    document.getElementById('methodIcon').value = method.icon || '';
    document.getElementById('methodActive').checked = method.isActive;

    openModal('methodModal');
}

// ===============================
// ELIMINAR MÉTODO DE PAGO
// ===============================
async function deleteMethod(id) {
    if (!confirmAction('¿Está seguro de eliminar este método de pago?')) return;

    try {
        await PaymentMethodService.delete(id);
        showToast('Método de pago eliminado correctamente', 'success');
        await loadPaymentMethods();
    } catch (error) {
        console.error('Error al eliminar método:', error);
        showToast('Error al eliminar el método de pago', 'error');
    }
}

function openModal(modalId) {
    document.getElementById(modalId).classList.add('active');
}

function closeModal(modalId) {
    document.getElementById(modalId).classList.remove('active');
}