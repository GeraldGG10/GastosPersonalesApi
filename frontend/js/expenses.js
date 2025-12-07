// Lógica de gestión de gastos

checkAuth();

let expenses = [];
let categories = [];
let paymentMethods = [];
let editingExpenseId = null;

document.addEventListener('DOMContentLoaded', async () => {
    await loadInitialData();
    setupEventListeners();
});

async function loadInitialData() {
    try {
        expenses = await ExpenseService.getAll();
        categories = await CategoryService.getAll();
        paymentMethods = await PaymentMethodService.getAll();
        
        loadExpensesTable();
        loadCategoryOptions();
        loadPaymentMethodOptions();
    } catch (error) {
        console.error('Error al cargar datos:', error);
        showToast('Error al cargar los datos', 'error');
    }
}

function setupEventListeners() {
    // Botón agregar gasto
    document.getElementById('addExpenseBtn').addEventListener('click', () => {
        editingExpenseId = null;
        document.getElementById('modalTitle').textContent = 'Nuevo Gasto';
        document.getElementById('expenseForm').reset();
        document.getElementById('expenseDate').valueAsDate = new Date();
        openModal('expenseModal');
    });
    
    // Botón importar
    document.getElementById('importBtn').addEventListener('click', () => {
        document.getElementById('importForm').reset();
        document.getElementById('importResult').innerHTML = '';
        openModal('importModal');
    });
    
    // Formulario de gasto
    document.getElementById('expenseForm').addEventListener('submit', handleExpenseSubmit);
    
    // Formulario de importación
    document.getElementById('importForm').addEventListener('submit', handleImportSubmit);
    
    // Cerrar modales
    document.querySelectorAll('.close').forEach(btn => {
        btn.addEventListener('click', (e) => {
            closeModal(e.target.closest('.modal').id);
        });
    });
    
    document.getElementById('cancelExpenseBtn').addEventListener('click', () => {
        closeModal('expenseModal');
    });
    
    document.getElementById('cancelImportBtn').addEventListener('click', () => {
        closeModal('importModal');
    });
}

function loadExpensesTable() {
    const tbody = document.getElementById('expensesTableBody');
    
    if (expenses.length === 0) {
        tbody.innerHTML = '<tr><td colspan="6" class="empty-state">No hay gastos registrados</td></tr>';
        return;
    }
    
    tbody.innerHTML = expenses.map(expense => {
        const category = categories.find(c => c.id === expense.categoryId);
        const method = paymentMethods.find(m => m.id === expense.paymentMethodId);
        
        return `
            <tr>
                <td>${formatDate(expense.date)}</td>
                <td>${expense.description || 'Sin descripción'}</td>
                <td>${formatCurrency(expense.amount)}</td>
                <td>${category ? category.name : 'N/A'}</td>
                <td>${method ? method.name : 'N/A'}</td>
                <td>
                    <div class="action-buttons">
                        <button class="btn-icon" onclick="editExpense(${expense.id})" title="Editar">✏️</button>
                        <button class="btn-icon" onclick="deleteExpense(${expense.id})" title="Eliminar">🗑️</button>
                    </div>
                </td>
            </tr>
        `;
    }).join('');
}

function loadCategoryOptions() {
    const select = document.getElementById('expenseCategory');
    select.innerHTML = '<option value="">Seleccione una categoría</option>' +
        categories.map(cat => `<option value="${cat.id}">${cat.name}</option>`).join('');
}

function loadPaymentMethodOptions() {
    const select = document.getElementById('expensePaymentMethod');
    select.innerHTML = '<option value="">Seleccione un método</option>' +
        paymentMethods.map(method => `<option value="${method.id}">${method.name}</option>`).join('');
}

async function handleExpenseSubmit(e) {
    e.preventDefault();
    
    const expenseData = {
        amount: parseFloat(document.getElementById('expenseAmount').value),
        date: document.getElementById('expenseDate').value,
        categoryId: parseInt(document.getElementById('expenseCategory').value),
        paymentMethodId: parseInt(document.getElementById('expensePaymentMethod').value),
        description: document.getElementById('expenseDescription').value
    };
    
    try {
        if (editingExpenseId) {
            await ExpenseService.update(editingExpenseId, expenseData);
            showToast('Gasto actualizado correctamente', 'success');
        } else {
            await ExpenseService.create(expenseData);
            showToast('Gasto creado correctamente', 'success');
        }
        
        closeModal('expenseModal');
        await loadInitialData();
    } catch (error) {
        console.error('Error al guardar gasto:', error);
        showToast(error.message || 'Error al guardar el gasto', 'error');
    }
}

async function editExpense(id) {
    editingExpenseId = id;
    const expense = expenses.find(e => e.id === id);
    
    if (!expense) return;
    
    document.getElementById('modalTitle').textContent = 'Editar Gasto';
    document.getElementById('expenseAmount').value = expense.amount;
    document.getElementById('expenseDate').value = formatDateForInput(expense.date);
    document.getElementById('expenseCategory').value = expense.categoryId;
    document.getElementById('expensePaymentMethod').value = expense.paymentMethodId;
    document.getElementById('expenseDescription').value = expense.description || '';
    
    openModal('expenseModal');
}

async function deleteExpense(id) {
    if (!confirmAction('¿Está seguro de eliminar este gasto?')) return;
    
    try {
        await ExpenseService.delete(id);
        showToast('Gasto eliminado correctamente', 'success');
        await loadInitialData();
    } catch (error) {
        console.error('Error al eliminar gasto:', error);
        showToast('Error al eliminar el gasto', 'error');
    }
}

async function handleImportSubmit(e) {
    e.preventDefault();
    
    const fileInput = document.getElementById('excelFile');
    const file = fileInput.files[0];
    
    if (!file) {
        showToast('Seleccione un archivo', 'warning');
        return;
    }
    
    const formData = new FormData();
    formData.append('file', file);
    
    try {
        const result = await ExpenseService.import(formData);
        
        const resultContainer = document.getElementById('importResult');
        resultContainer.className = result.errorCount > 0 ? 'import-result error' : 'import-result success';
        
        let html = `
            <h4>Resultado de Importación</h4>
            <p>Total de filas: ${result.totalRows}</p>
            <p>Insertados: ${result.successCount}</p>
            <p>Errores: ${result.errorCount}</p>
        `;
        
        if (result.errors && result.errors.length > 0) {
            html += '<h5>Errores:</h5><ul>';
            result.errors.forEach(error => {
                html += `<li>${error}</li>`;
            });
            html += '</ul>';
        }
        
        resultContainer.innerHTML = html;
        
        if (result.successCount > 0) {
            showToast(`${result.successCount} gastos importados correctamente`, 'success');
            await loadInitialData();
        }
    } catch (error) {
        console.error('Error al importar:', error);
        showToast('Error al importar el archivo', 'error');
    }
}

function openModal(modalId) {
    document.getElementById(modalId).classList.add('active');
}

function closeModal(modalId) {
    document.getElementById(modalId).classList.remove('active');
}