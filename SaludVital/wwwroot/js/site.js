// Cambio de tema claro/oscuro, con preferencia guardada en el navegador.
document.addEventListener('DOMContentLoaded', function () {
    function toggleTema() {
        var actual = document.documentElement.getAttribute('data-bs-theme') === 'dark' ? 'light' : 'dark';
        document.documentElement.setAttribute('data-bs-theme', actual);
        localStorage.setItem('sv-tema', actual === 'dark' ? 'oscuro' : 'claro');
    }

    var boton = document.getElementById('boton-tema');
    if (boton) boton.addEventListener('click', toggleTema);

    var botonSidebar = document.getElementById('boton-tema-sidebar');
    if (botonSidebar) botonSidebar.addEventListener('click', toggleTema);

    // Modal de confirmación de eliminación: cualquier botón con [data-eliminar]
    // rellena el mensaje y la URL de envío antes de mostrarlo.
    // El id del registro viaja en la URL (data-url), no como campo de formulario.
    var modal = document.getElementById('modalEliminar');
    if (!modal) {
        return;
    }

    var modalBs = bootstrap.Modal.getOrCreateInstance(modal);
    var texto = document.getElementById('modalEliminarTexto');
    var formulario = document.getElementById('formEliminar');

    document.querySelectorAll('[data-eliminar]').forEach(function (boton) {
        boton.addEventListener('click', function () {
            if (texto) {
                texto.textContent = boton.dataset.mensaje || 'Esta acción no se puede deshacer.';
            }
            if (formulario) {
                formulario.action = boton.dataset.url || '';
            }
            modalBs.show();
        });
    });
});
