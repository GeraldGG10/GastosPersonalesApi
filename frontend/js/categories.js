// ===============================
// Gestión de Categorías
// ===============================

checkAuth();

// Estado global
let categories = [];
let editingCategoryId = null;


// ===============================
// Inicialización
// ===============================
document.addEventListener('DOMContentLoaded', async () => {
    await loadCategories();
    initEventListeners();
});


// ===============================
// Eventos
// ===============================
function initEventListeners() {
    
    // Abrir modal crear
    document.getElementById('addCategoryBtn')
        .addEventListener('click', () => {
            editingCategoryId = null;
            resetModal();
            openModal('categoryModal');
        });
    
    // Guardar
    document.getElementById('categoryForm')
        .addEventListener('submit', handleCategorySubmit);
    
    // Cerrar modal
    document.querySelector('#categoryModal .close')
        .addEventListener('click', () => closeModal('categoryModal'));
    
    document.getElementById('cancelCategoryBtn')
        .addEventListener('click', () => closeModal('categoryModal'));
}


// ===============================
// Cargar Categorías
// ===============================
async function loadCategories() {
    try {
        categories = await CategoryService.getAll();
        renderCategoriesTable();
    } catch (error) {
        console.error('Error al cargar categorías:', error);
        showToast('Error al cargar las categorías', 'error');
    }
}


// ===============================
// Render tabla
// ===============================
function renderCategoriesTable() {
    const tbody = document.getElementById('categoriesTableBody');

    if (!categories.length) {
        tbody.innerHTML = `
            <tr>
                <td colspan="4" class="empty-state">
                    No hay categorías registradas
                </td>
            </tr>`;
        return;
    }

    tbody.innerHTML = categories.map(c => `
        <tr>
            <td>${c.id}</td>
            <td>${c.name}</td>
            
            <td>
                <span class="badge ${c.isActive ? 'success' : 'danger'}">
                    ${c.isActive ? 'Activo' : 'Inactivo'}
                </span>
            </td>

            <td>
                <div class="action-buttons">
                    <button class="btn-icon"
                        onclick="editCategory(${c.id})"
                        title="Editar">✏️</button>
                    <button class="btn-icon"
                        onclick="deleteCategory(${c.id})"
                        title="Eliminar">🗑️</button>
                </div>
            </td>
        </tr>
    `).join('');
}


// ===============================
// Guardar
// ===============================
async function handleCategorySubmit(event) {

    event.preventDefault();

    const categoryData = {
        name: document.getElementById('categoryName').value,
        isActive: document.getElementById('categoryActive').checked
    };

    try {
        await CategoryService.create(categoryData);
        showToast('Categoría guardada correctamente', 'success');
        closeModal('categoryModal');
        await loadCategories();

    } catch (error) {
        console.error('Error:', error);
        showToast(error.message || 'Error al guardar la categoría', 'error');
    }
}


// ===============================
// Editar
// ===============================
function editCategory(id) {
    editingCategoryId = id;
    const category = categories.find(c => c.id === id);
    if (!category) return;

    document.getElementById('modalTitle').textContent = 'Editar Categoría';
    document.getElementById('categoryName').value = category.name;
    document.getElementById('categoryActive').checked = category.isActive;

    openModal('categoryModal');
}


// ===============================
// ELIMINAR CATEGORÍA
// ===============================
async function deleteCategory(id) {
    if (!confirmAction('¿Está seguro de eliminar esta categoría?')) return;
    
    try {
        await CategoryService.delete(id);
        showToast('Categoría eliminada correctamente', 'success');
        await loadCategories();
    } catch (error) {
        console.error('Error al eliminar categoría:', error);
        showToast('Error al eliminar la categoría', 'error');
    }
}


// ===============================
// Utilidades de Modal
// ===============================
function resetModal() {
    document.getElementById('modalTitle').textContent = 'Nueva Categoría';
    document.getElementById('categoryForm').reset();
    document.getElementById('categoryActive').checked = true;
}

function openModal(id) {
    document.getElementById(id).classList.add('active');
}

function closeModal(id) {
    document.getElementById(id).classList.remove('active');
}