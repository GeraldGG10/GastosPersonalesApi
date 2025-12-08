// Lógica del Dashboard

checkAuth();

document.addEventListener('DOMContentLoaded', async () => {
    await loadDashboardData();
});

async function loadDashboardData() {
    try {
        // Cargar estadísticas generales
        await loadStats();
        
        // Cargar alertas de presupuesto
        await loadBudgetAlerts();
        
        // Cargar gastos recientes
        await loadRecentExpenses();
    } catch (error) {
        console.error('Error al cargar dashboard:', error);
        showToast('Error al cargar los datos del dashboard', 'error');
    }
}

async function loadStats() {
    try {
        const expenses = await ExpenseService.getAll();
        const categories = await CategoryService.getAll();
        const methods = await PaymentMethodService.getAll();
        
        // Total gastado este mes
        const now = new Date();
        const currentMonth = now.getMonth() + 1;
        const currentYear = now.getFullYear();
        
        const monthExpenses = expenses.filter(e => {
            const expenseDate = new Date(e.date);
            return expenseDate.getMonth() + 1 === currentMonth && 
                expenseDate.getFullYear() === currentYear;
        });
        
        const totalMonth = monthExpenses.reduce((sum, e) => sum + e.amount, 0);
        
        // Actualizar valores
        document.getElementById('totalMonth').textContent = formatCurrency(totalMonth);
        document.getElementById('totalExpenses').textContent = expenses.length;
        document.getElementById('totalCategories').textContent = categories.length;
        document.getElementById('totalMethods').textContent = methods.length;
    } catch (error) {
        console.error('Error al cargar estadísticas:', error);
    }
}

async function loadBudgetAlerts() {
    try {
        const { month, year } = getCurrentMonthYear();
        const exceeded = await BudgetService.getExceeded(month, year);
        const categories = await CategoryService.getAll();
        
        const alertsContainer = document.getElementById('budgetAlerts');
        
        if (!exceeded || exceeded.length === 0) {
            alertsContainer.innerHTML = '<p class="empty-state">No hay alertas de presupuesto</p>';
            return;
        }
        
        alertsContainer.innerHTML = exceeded.map(budget => {
            const category = categories.find(c => c.id === budget.categoryId);
            const categoryName = category ? category.name : `Categoría ${budget.categoryId}`;
            
            let statusClass = 'ok';
            let statusText = 'OK';
            
            if (budget.percentage >= 100) {
                statusClass = 'exceeded';
                statusText = 'EXCEDIDO';
            } else if (budget.percentage >= 80) {
                statusClass = 'critical';
                statusText = 'CRÍTICO';
            } else if (budget.percentage >= 50) {
                statusClass = 'warning';
                statusText = 'ADVERTENCIA';
            }
            
            return `
                <div class="budget-alert ${statusClass}">
                    <div class="budget-alert-info">
                        <h4>${categoryName}</h4>
                        <p>${formatCurrency(budget.spentAmount)} de ${formatCurrency(budget.budgetAmount)}</p>
                    </div>
                    <div class="budget-percentage">${budget.percentage.toFixed(1)}%</div>
                </div>
            `;
        }).join('');
    } catch (error) {
        console.error('Error al cargar alertas:', error);
        document.getElementById('budgetAlerts').innerHTML = 
            '<p class="empty-state">Error al cargar alertas</p>';
    }
}

async function loadRecentExpenses() {
    try {
        const expenses = await ExpenseService.getAll();
        const categories = await CategoryService.getAll();
        const methods = await PaymentMethodService.getAll();
        
        const recentExpenses = expenses.slice(0, 5);
        const container = document.getElementById('recentExpenses');
        
        if (recentExpenses.length === 0) {
            container.innerHTML = '<p class="empty-state">No hay gastos recientes</p>';
            return;
        }
        
        container.innerHTML = recentExpenses.map(expense => {
            const category = categories.find(c => c.id === expense.categoryId);
            const method = methods.find(m => m.id === expense.paymentMethodId);
            
            return `
                <div class="recent-expense">
                    <div class="expense-info">
                        <p><strong>${expense.description || 'Sin descripción'}</strong></p>
                        <p>${formatDate(expense.date)} - ${category ? category.name : 'Sin categoría'}</p>
                        <p><small>${method ? method.name : 'Sin método'}</small></p>
                    </div>
                    <div class="expense-amount">${formatCurrency(expense.amount)}</div>
                </div>
            `;
        }).join('');
    } catch (error) {
        console.error('Error al cargar gastos recientes:', error);
        document.getElementById('recentExpenses').innerHTML = 
            '<p class="empty-state">Error al cargar gastos</p>';
    }
}