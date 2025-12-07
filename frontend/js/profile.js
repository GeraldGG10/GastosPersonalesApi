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

        const email = profile.correo || localStorage.getItem('userEmail') || '-';
        const name  = profile.correo || localStorage.getItem('userName')  || 'U';

        document.getElementById('userEmail').textContent = email;
        document.getElementById('userName').textContent  = name;

        // Iniciales
        const initials = name
            .split(' ')
            .map(n => n[0])
            .join('')
            .toUpperCase()
            .substring(0, 2);

        document.getElementById('userInitials').textContent = initials;

        // Pre-llenar form
        document.getElementById('newName').value = name;

    } catch (error) {
        console.error('Error al cargar perfil:', error);

        // Backup de localStorage
        const email = localStorage.getItem('userEmail');
        const name  = localStorage.getItem('userName');

        if (email) document.getElementById('userEmail').textContent = email;

        if (name) {
            document.getElementById('userName').textContent = name;
            document.getElementById('newName').value = name;

            const initials = name.split(' ')
                .map(n => n[0])
                .join('')
                .toUpperCase()
                .substring(0, 2);

            document.getElementById('userInitials').textContent = initials;
        }
    }
}


// =====================================
// CAMBIAR NOMBRE
// =====================================
async function handleUpdateName(e) {
    e.preventDefault();

    const newName = document.getElementById('newName').value;

    try {
        await AuthService.updateProfile({ nombre: newName });
        localStorage.setItem('userName', newName);

        showToast('Nombre actualizado correctamente', 'success');
        await loadProfile();

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

    const currentPassword  = document.getElementById('currentPassword').value;
    const newPassword      = document.getElementById('newPassword').value;
    const confirmPassword  = document.getElementById('confirmPassword').value;

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
        await AuthService.changePassword({
            currentPassword,
            newPassword
        });

        showToast('Contraseña cambiada correctamente', 'success');
        document.getElementById('changePasswordForm').reset();

    } catch (error) {
        console.error('Error al cambiar contraseña:', error);
        showToast(error.message || 'Error al cambiar la contraseña', 'error');
    }
}
