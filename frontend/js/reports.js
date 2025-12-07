// Lógica de reportes y exportaciones

checkAuth();

let categories = [];
let paymentMethods = [];

document.addEventListener('DOMContentLoaded', async () => {
    setDefaultMonthYear('reportMonth', 'reportYear');
    await loadInitialData();
    setupEventListeners();
});

async function loadInitialData() {
    try {
        categories = await CategoryService.getAll();
        paymentMethods = await PaymentMethodService.getAll();
        
        loadFilterOptions();
    } catch (error) {
        console.error('Error al cargar datos:', error);
        showToast('Error al cargar los datos', 'error');
    }
}

function setupEventListeners() {
    document.getElementById('applyFiltersBtn').addEventListener('click', applyFilters);
    document.getElementById('clearFiltersBtn').addEventListener('click', clearFilters);
    document.getElementById('generateReportBtn').addEventListener('click', generateMonthlyReport);
    document.getElementById('exportExcelBtn').addEventListener('click', exportExcel);
    document.getElementById('exportJsonBtn').addEventListener('click', exportJson);
    document.getElementById('exportTxtBtn').addEventListener('click', exportTxt);
}

function loadFilterOptions() {
    const categorySelect = document.getElementById('filterCategory');
    categorySelect.innerHTML = '<option value="">Todas</option>' +
        categories.map(cat => `<option value="${cat.id}">${cat.name}</option>`).join('');
    
    const methodSelect = document.getElementById('filterPaymentMethod');
    methodSelect.innerHTML = '<option value="">Todos</option>' +
        paymentMethods.map(method => `<option value="${method.id}">${method.name}</option>`).join('');
}

async function applyFilters() {
    try {
        const filters = {
            startDate: document.getElementById('startDate').value || null,
            endDate: document.getElementById('endDate').value || null,
            categoryId: document.getElementById('filterCategory').value || null,
            paymentMethodId: document.getElementById('filterPaymentMethod').value || null,
            search: document.getElementById('searchText').value || null
        };
        
        const expenses = await ReportService.filter(filters);
        
        const tbody = document.getElementById('filteredExpensesBody');
        document.getElementById('resultsCount').textContent = `${expenses.length} gastos encontrados`;
        
        if (expenses.length === 0) {
            tbody.innerHTML = '<tr><td colspan="5" class="empty-state">No se encontraron gastos con los filtros aplicados</td></tr>';
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
                </tr>
            `;
        }).join('');
        
        showToast('Filtros aplicados correctamente', 'success');
    } catch (error) {
        console.error('Error al aplicar filtros:', error);
        showToast('Error al aplicar los filtros', 'error');
    }
}

function clearFilters() {
    document.getElementById('startDate').value = '';
    document.getElementById('endDate').value = '';
    document.getElementById('filterCategory').value = '';
    document.getElementById('filterPaymentMethod').value = '';
    document.getElementById('searchText').value = '';
    
    document.getElementById('filteredExpensesBody').innerHTML = 
        '<tr><td colspan="5" class="empty-state">Aplica filtros para ver resultados</td></tr>';
    document.getElementById('resultsCount').textContent = '0 gastos encontrados';
    
    showToast('Filtros limpiados', 'info');
}

async function generateMonthlyReport() {
    try {
        const month = parseInt(document.getElementById('reportMonth').value);
        const year = parseInt(document.getElementById('reportYear').value);
        
        const report = await ReportService.monthly(month, year);
        
        const container = document.getElementById('monthlyReportContainer');
        
        container.innerHTML = `
            <div class="report-section">
                <h3>Resumen - ${report.monthName} ${report.year}</h3>
                <div class="report-grid">
                    <div class="report-item">
                        <div class="report-label">Total Gastado</div>
                        <div class="report-value">${formatCurrency(report.currentMonth.totalAmount)}</div>
                    </div>
                    <div class="report-item">
                        <div class="report-label">Cantidad de Gastos</div>
                        <div class="report-value">${report.currentMonth.expenseCount}</div>
                    </div>
                    <div class="report-item">
                        <div class="report-label">Promedio por Gasto</div>
                        <div class="report-value">${formatCurrency(report.currentMonth.averageExpense)}</div>
                    </div>
                </div>
            </div>
            
            <div class="report-section">
                <h3>Comparación con ${report.previousMonth.monthName} ${report.previousMonth.year}</h3>
                <div class="report-grid">
                    <div class="report-item">
                        <div class="report-label">Mes Anterior</div>
                        <div class="report-value">${formatCurrency(report.previousMonth.totalAmount)}</div>
                    </div>
                    <div class="report-item">
                        <div class="report-label">Diferencia</div>
                        <div class="report-value" style="color: ${report.comparison.difference >= 0 ? 'var(--danger)' : 'var(--success)'}">
                            ${report.comparison.difference >= 0 ? '+' : ''}${formatCurrency(report.comparison.difference)}
                        </div>
                    </div>
                    <div class="report-item">
                        <div class="report-label">Cambio Porcentual</div>
                        <div class="report-value" style="color: ${report.comparison.percentageChange >= 0 ? 'var(--danger)' : 'var(--success)'}">
                            ${report.comparison.percentageChange >= 0 ? '+' : ''}${report.comparison.percentageChange.toFixed(2)}%
                        </div>
                    </div>
                    <div class="report-item">
                        <div class="report-label">Estado</div>
                        <div class="report-value">
                            <span class="badge ${report.comparison.status === 'AUMENTO' ? 'danger' : report.comparison.status === 'REDUCCIÓN' ? 'success' : 'info'}">
                                ${report.comparison.status}
                            </span>
                        </div>
                    </div>
                </div>
            </div>
            
            <div class="report-section">
                <h3>Top 5 Categorías</h3>
                <div class="table-container">
                    <table class="data-table">
                        <thead>
                            <tr>
                                <th>#</th>
                                <th>Categoría</th>
                                <th>Total</th>
                                <th>Cantidad</th>
                                <th>% del Total</th>
                            </tr>
                        </thead>
                        <tbody>
                            ${report.topCategories.map(cat => {
                                const category = categories.find(c => c.id === cat.categoryId);
                                return `
                                    <tr>
                                        <td>${cat.rank}</td>
                                        <td>${category ? category.name : 'Categoría ' + cat.categoryId}</td>
                                        <td>${formatCurrency(cat.total)}</td>
                                        <td>${cat.count}</td>
                                        <td>${cat.percentage.toFixed(1)}%</td>
                                    </tr>
                                `;
                            }).join('')}
                        </tbody>
                    </table>
                </div>
            </div>
            
            <div class="report-section">
                <h3>Comparación por Categoría</h3>
                <div class="table-container">
                    <table class="data-table">
                        <thead>
                            <tr>
                                <th>Categoría</th>
                                <th>Actual</th>
                                <th>Anterior</th>
                                <th>Diferencia</th>
                                <th>Cambio %</th>
                            </tr>
                        </thead>
                        <tbody>
                            ${report.categoryComparison.map(comp => {
                                const category = categories.find(c => c.id === comp.categoryId);
                                return `
                                    <tr>
                                        <td>${category ? category.name : 'Categoría ' + comp.categoryId}</td>
                                        <td>${formatCurrency(comp.currentTotal)}</td>
                                        <td>${formatCurrency(comp.previousTotal)}</td>
                                        <td style="color: ${comp.difference >= 0 ? 'var(--danger)' : 'var(--success)'}">
                                            ${comp.difference >= 0 ? '+' : ''}${formatCurrency(comp.difference)}
                                        </td>
                                        <td style="color: ${comp.percentageChange >= 0 ? 'var(--danger)' : 'var(--success)'}">
                                            ${comp.percentageChange >= 0 ? '+' : ''}${comp.percentageChange.toFixed(2)}%
                                        </td>
                                    </tr>
                                `;
                            }).join('')}
                        </tbody>
                    </table>
                </div>
            </div>
        `;
        
        showToast('Reporte generado correctamente', 'success');
    } catch (error) {
        console.error('Error al generar reporte:', error);
        showToast('Error al generar el reporte', 'error');
    }
}

async function exportExcel() {
    try {
        const blob = await ReportService.exportExcel();
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `gastos_${new Date().toISOString().split('T')[0]}.xlsx`;
        document.body.appendChild(a);
        a.click();
        window.URL.revokeObjectURL(url);
        document.body.removeChild(a);
        
        showToast('Excel descargado correctamente', 'success');
    } catch (error) {
        console.error('Error al exportar Excel:', error);
        showToast('Error al exportar a Excel', 'error');
    }
}

async function exportJson() {
    try {
        const blob = await ReportService.exportJson();
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `gastos_${new Date().toISOString().split('T')[0]}.json`;
        document.body.appendChild(a);
        a.click();
        window.URL.revokeObjectURL(url);
        document.body.removeChild(a);
        
        showToast('JSON descargado correctamente', 'success');
    } catch (error) {
        console.error('Error al exportar JSON:', error);
        showToast('Error al exportar a JSON', 'error');
    }
}

async function exportTxt() {
    try {
        const blob = await ReportService.exportTxt();
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `gastos_${new Date().toISOString().split('T')[0]}.txt`;
        document.body.appendChild(a);
        a.click();
        window.URL.revokeObjectURL(url);
        document.body.removeChild(a);
        
        showToast('TXT descargado correctamente', 'success');
    } catch (error) {
        console.error('Error al exportar TXT:', error);
        showToast('Error al exportar a TXT', 'error');
    }
}