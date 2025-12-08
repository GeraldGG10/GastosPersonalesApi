// Lógica de gestión de presupuestos

checkAuth();

let budgets = [];
let categories = [];
let editingBudgetId = null;

document.addEventListener('DOMContentLoaded', async () => {
    setDefaultMonthYear('filterMonth', 'filterYear');
    setDefaultMonthYear('budgetMonth', 'budgetYear');

    await loadInitialData();
    setupEventListeners();
});

function setupEventListeners() {
    document.getElementById('addBudgetBtn').addEventListener('click', () => {
        editingBudgetId = null;
        document.getElementById('modalTitle').textContent = 'Nuevo Presupuesto';
        document.getElementById('budgetForm').reset();
        setDefaultMonthYear('budgetMonth', 'budgetYear');
        openModal('budgetModal');
    });

    document.getElementById('budgetForm').addEventListener('submit', handleBudgetSubmit);
    document.getElementById('filterBudgetsBtn').addEventListener('click', loadBudgets);
    document.getElementById('checkExceededBtn').addEventListener('click', showExceededBudgets);

    document.querySelector('#budgetModal .close').addEventListener('click', () => {
        closeModal('budgetModal');
    });

    document.getElementById('cancelBudgetBtn').addEventListener('click', () => {
        closeModal('budgetModal');
    });
}

async function loadInitialData() {
    try {
        categories = await CategoryService.getAll();
        loadCategoryOptions();
        await loadBudgets();
    } catch (error) {
        console.error('Error al cargar datos:', error);
        showToast('Error al cargar los datos', 'error');
    }
}

function loadCategoryOptions() {
    const select = document.getElementById('budgetCategory');
    select.innerHTML = '<option value="">Seleccione una categoría</option>' +
        categories.map(cat => `<option value="${cat.id}">${cat.name}</option>`).join('');
}

async function loadBudgets() {
    try {
        const month = parseInt(document.getElementById('filterMonth').value);
        const year = parseInt(document.getElementById('filterYear').value);

        budgets = await BudgetService.getAll();
        const expenses = await ExpenseService.getAll();

        // Filtrar presupuestos por mes y año
        const filteredBudgets = budgets.filter(b => b.month === month && b.year === year);

        const container = document.getElementById('budgetsContainer');

        if (filteredBudgets.length === 0) {
            container.innerHTML = '<p class="empty-state">No hay presupuestos para este período</p>';
            return;
        }

        container.innerHTML = filteredBudgets.map(budget => {
            const category = categories.find(c => c.id === budget.categoryId);

            // Calcular gasto acumulado
            const categoryExpenses = expenses.filter(e => {
                const expenseDate = new Date(e.date);
                return e.categoryId === budget.categoryId &&
                    expenseDate.getMonth() + 1 === month &&
                    expenseDate.getFullYear() === year;
            });

            const spent = categoryExpenses.reduce((sum, e) => sum + e.amount, 0);
            const percentage = budget.amount > 0 ? (spent / budget.amount) * 100 : 0;
            const remaining = budget.amount - spent;

            let progressClass = '';
            let statusText = 'OK';

            if (percentage >= 100) {
                progressClass = 'danger';
                statusText = 'EXCEDIDO';
            } else if (percentage >= 80) {
                progressClass = 'warning';
                statusText = 'CRÍTICO';
            } else if (percentage >= 50) {
                progressClass = 'warning';
                statusText = 'ADVERTENCIA';
            }

            return `
                <div class="budget-card">
                    <div class="budget-header">
                        <div>
                            <div class="budget-title">${category ? category.name : 'Categoría ' + budget.categoryId}</div>
                            <div class="budget-amount">${getMonthName(budget.month)} ${budget.year}</div>
                        </div>
                        <div class="action-buttons">
                            <button class="btn-icon" onclick="editBudget(${budget.id})" title="Editar">✏️</button>
                            <button class="btn-icon" onclick="deleteBudget(${budget.id})" title="Eliminar">🗑️</button>
                        </div>
                    </div>
                    <div class="budget-progress">
                        <div class="progress-bar">
                            <div class="progress-fill ${progressClass}" style="width: ${Math.min(percentage, 100)}%"></div>
                        </div>
                    </div>
                    <div class="budget-stats">
                        <span>Gastado: ${formatCurrency(spent)}</span>
                        <span><strong>${percentage.toFixed(1)}%</strong></span>
                        <span>Presupuesto: ${formatCurrency(budget.amount)}</span>
                    </div>
                    <div class="budget-stats">
                        <span>Restante: ${formatCurrency(remaining)}</span>
                        <span class="badge ${progressClass === 'danger' ? 'danger' : progressClass === 'warning' ? 'warning' : 'success'}">${statusText}</span>
                    </div>
                </div>
            `;
        }).join('');
    } catch (error) {
        console.error('Error al cargar presupuestos:', error);
        showToast('Error al cargar los presupuestos', 'error');
    }
}

async function handleBudgetSubmit(e) {
    e.preventDefault();

    const budgetData = {
        categoryId: parseInt(document.getElementById('budgetCategory').value),
        amount: parseFloat(document.getElementById('budgetAmount').value),
        month: parseInt(document.getElementById('budgetMonth').value),
        year: parseInt(document.getElementById('budgetYear').value)
    };

    try {
        if (editingBudgetId) {
            await BudgetService.update(editingBudgetId, budgetData);
            showToast('Presupuesto actualizado correctamente', 'success');
        } else {
            await BudgetService.create(budgetData);
            showToast('Presupuesto creado correctamente', 'success');
        }

        closeModal('budgetModal');
        await loadBudgets();
    } catch (error) {
        console.error('Error al guardar presupuesto:', error);
        showToast(error.message || 'Error al guardar el presupuesto', 'error');
    }
}

function editBudget(id) {
    editingBudgetId = id;
    const budget = budgets.find(b => b.id === id);

    if (!budget) return;

    document.getElementById('modalTitle').textContent = 'Editar Presupuesto';
    document.getElementById('budgetCategory').value = budget.categoryId;
    document.getElementById('budgetAmount').value = budget.amount;
    document.getElementById('budgetMonth').value = budget.month;
    document.getElementById('budgetYear').value = budget.year;

    openModal('budgetModal');
}

async function deleteBudget(id) {
    if (!confirmAction('¿Está seguro de eliminar este presupuesto?')) return;

    try {
        await BudgetService.delete(id);
        showToast('Presupuesto eliminado correctamente', 'success');
        await loadBudgets();
    } catch (error) {
        console.error('Error al eliminar presupuesto:', error);
        showToast('Error al eliminar el presupuesto', 'error');
    }
}

async function showExceededBudgets() {
    try {
        const month = parseInt(document.getElementById('filterMonth').value);
        const year = parseInt(document.getElementById('filterYear').value);

        const exceeded = await BudgetService.getExceeded(month, year);

        const container = document.getElementById('budgetsContainer');

        if (!exceeded || exceeded.length === 0) {
            container.innerHTML = '<p class="empty-state">No hay presupuestos excedidos o en advertencia</p>';
            return;
        }

        container.innerHTML = `
            <div class="card">
                <div class="card-header">
                    <h3>Presupuestos Excedidos / En Advertencia - ${getMonthName(month)} ${year}</h3>
                </div>
                <div class="card-body">
                    ${exceeded.map(budget => {
            const category = categories.find(c => c.id === budget.categoryId);

            let badgeClass = 'success';
            if (budget.status === 'EXCEDIDO') badgeClass = 'danger';
            else if (budget.status === 'CRÍTICO') badgeClass = 'warning';
            else if (budget.status === 'ADVERTENCIA') badgeClass = 'warning';

            return `
                            <div class="budget-card">
                                <div class="budget-header">
                                    <div>
                                        <div class="budget-title">${category ? category.name : 'Categoría ' + budget.categoryId}</div>
                                        <span class="badge ${badgeClass}">${budget.status}</span>
                                    </div>
                                </div>
                                <div class="budget-progress">
                                    <div class="progress-bar">
                                        <div class="progress-fill ${badgeClass === 'danger' ? 'danger' : 'warning'}" style="width: ${Math.min(budget.percentage, 100)}%"></div>
                                    </div>
                                </div>
                                <div class="budget-stats">
                                    <span>Gastado: ${formatCurrency(budget.spentAmount)}</span>
                                    <span><strong>${budget.percentage.toFixed(1)}%</strong></span>
                                    <span>Presupuesto: ${formatCurrency(budget.budgetAmount)}</span>
                                </div>
                                <div class="budget-stats">
                                    <span>Restante: ${formatCurrency(budget.remaining)}</span>
                                </div>
                            </div>
                        `;
        }).join('')}
                </div>
            </div>
        `;

        showToast('Mostrando presupuestos excedidos', 'info');
    } catch (error) {
        console.error('Error al cargar presupuestos excedidos:', error);
        showToast('Error al cargar los presupuestos excedidos', 'error');
    }
}

function openModal(modalId) {
    document.getElementById(modalId).classList.add('active');
}

function closeModal(modalId) {
    document.getElementById(modalId).classList.remove('active');
}