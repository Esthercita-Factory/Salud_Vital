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

    var catalogoGrid = document.getElementById('sv-catalogo-grid');
    if (catalogoGrid) {
        var busqueda = document.getElementById('busqueda');
        var filtros = Array.from(document.querySelectorAll('.sv-filter-chip'));
        var sinResultados = document.getElementById('sv-catalogo-sin-resultados');
        var elementos = Array.from(catalogoGrid.children).map(function (elemento) {
            return {
                elemento: elemento,
                tarjeta: elemento.querySelector('.sv-tarjeta-mascota')
            };
        });
        var especieActiva = '';
        var temporizador;

        function normalizar(texto) {
            return (texto || '').toString().normalize('NFD').replace(/[\u0300-\u036f]/g, '').toLowerCase().trim();
        }

        function filtrarCatalogo() {
            var termino = normalizar(busqueda ? busqueda.value : '');
            var visibles = 0;

            elementos.forEach(function (item) {
                var tarjeta = item.tarjeta;
                var especie = tarjeta ? normalizar(tarjeta.querySelector('.sv-tarjeta-sub')?.textContent) : '';
                var contenido = tarjeta ? normalizar(tarjeta.textContent) : '';
                var coincideBusqueda = !termino || contenido.includes(termino);
                var coincideEspecie = !especieActiva || especie === normalizar(especieActiva);
                var visible = coincideBusqueda && coincideEspecie;

                item.elemento.hidden = !visible;
                if (visible) visibles++;
            });

            if (sinResultados) sinResultados.hidden = visibles > 0;
        }

        if (busqueda) {
            busqueda.addEventListener('input', function () {
                window.clearTimeout(temporizador);
                temporizador = window.setTimeout(filtrarCatalogo, 300);
            });
        }

        filtros.forEach(function (filtro) {
            filtro.addEventListener('click', function () {
                especieActiva = filtro.dataset.especie || '';
                filtros.forEach(function (otroFiltro) {
                    otroFiltro.classList.toggle('active', otroFiltro === filtro);
                });
                filtrarCatalogo();
            });
        });

        filtrarCatalogo();
    }

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
