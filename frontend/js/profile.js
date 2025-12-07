// =====================================
// LÓGICA DE PERFIL DE USUARIO
// =====================================

checkAuth();


// =====================================
// INICIALIZACIÓN
// =====================================
document.addEventListener('DOMContentLoaded', async () => {
    await loadProfile();
    setupEventListeners();
});


// =====================================
// EVENTOS
// =====================================
function setupEventListeners() {
    document
        .getElementById('updateNameForm')
        .addEventListener('submit', handleUpdateName);

    document
        .getElementById('changePasswordForm')
        .addEventListener('submit', handleChangePassword);
}


// =====================================
// CARGAR PERFIL
// =====================================
async function loadProfile() {

    try {
        const profile = await AuthService.getProfile();

        // Los nombres correctos que devuelve el backend
        const email = profile.correo || profile.email || localStorage.getItem('userEmail') || '-';
        const name = profile.correo || profile.fullName || localStorage.getItem('userName') || 'Usuario';

        document.getElementById('userEmail').textContent = email;
        document.getElementById('userName').textContent = name;

        // Iniciales
        const initials = name
            .split(' ')
            .map(n => n[0])
            .join('')
            .toUpperCase()
            .substring(0, 2);

        document.getElementById('userInitials').textContent = initials || 'U';

        // Pre-llenar form
        document.getElementById('newName').value = name;

    } catch (error) {
        console.error('Error al cargar perfil:', error);

        // Backup de localStorage
        const email = localStorage.getItem('userEmail') || '-';
        const name = localStorage.getItem('userName') || 'Usuario';

        document.getElementById('userEmail').textContent = email;
        document.getElementById('userName').textContent = name;
        document.getElementById('newName').value = name;

        const initials = name.split(' ')
            .map(n => n[0])
            .join('')
            .toUpperCase()
            .substring(0, 2);

        document.getElementById('userInitials').textContent = initials || 'U';
    }
}


// =====================================
// CAMBIAR NOMBRE
// =====================================
async function handleUpdateName(e) {
    e.preventDefault();

    const newName = document.getElementById('newName').value.trim();

    if (!newName) {
        showToast('El nombre no puede estar vacío', 'error');
        return;
    }

    try {
        const result = await AuthService.updateProfile({ nombre: newName });
        
        if (result) {
            localStorage.setItem('userName', newName);
            showToast('Nombre actualizado correctamente', 'success');
            await loadProfile();
        } else {
            showToast('No se pudo actualizar el nombre', 'error');
        }

    } catch (error) {
        console.error('Error al actualizar nombre:', error);
        showToast(error.message || 'Error al actualizar el nombre', 'error');
    }
}


// =====================================
// CAMBIAR CONTRASEÑA
// =====================================
async function handleChangePassword(e) {
    e.preventDefault();

    const currentPassword = document.getElementById('currentPassword').value;
    const newPassword = document.getElementById('newPassword').value;
    const confirmPassword = document.getElementById('confirmPassword').value;

    // Validaciones
    if (newPassword !== confirmPassword) {
        showToast('Las contraseñas no coinciden', 'error');
        return;
    }

    if (newPassword.length < 6) {
        showToast('La nueva contraseña debe tener al menos 6 caracteres', 'error');
        return;
    }

    try {
        const result = await AuthService.changePassword({
            currentPassword,
            newPassword
        });

        if (result) {
            showToast('Contraseña cambiada correctamente', 'success');
            document.getElementById('changePasswordForm').reset();
        } else {
            showToast('Contraseña actual incorrecta', 'error');
        }

    } catch (error) {
        console.error('Error al cambiar contraseña:', error);
        showToast(error.message || 'Error al cambiar la contraseña', 'error');
    }
}