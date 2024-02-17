//Al cargar un archivo, habilitar botones para consultar o insertar registros
function alCargarArchivo() {
    var fileInput = $('#fileInput');
    var showButton = $('#showButton');
    var insertButton = $('#insertButton');

    showButton.prop('disabled', !fileInput.val());
    insertButton.prop('disabled', !fileInput.val());
}

//Por cada registro sin DNI, colorearlo
$(document).ready(function () {
    var miTabla = $('.tabla-registros');

    //Iterar sobre las fila de la tabla
    miTabla.find('tbody tr').each(function () {
        var fila = $(this);
        var dni = fila.find('td:eq(2)').text();

        //Aplicar estilo
        if (dni.trim() === '') {
            fila.addClass('table-danger');
        }
    });
});