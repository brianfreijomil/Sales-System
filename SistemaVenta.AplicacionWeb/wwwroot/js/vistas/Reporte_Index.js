
let dataTable;
$(document).ready(function () {

    $.datepicker.setDefaults($.datepicker.regional["es"])

    $("#txtFechaInicio").datepicker({ dateFormat: "dd/mm/yy" })
    $("#txtFechaFin").datepicker({ dateFormat: "dd/mm/yy" })

    dataTable = $('#tbdata').DataTable({
        responsive: true,
        "ajax": {
            "url": '/Reportes/ReporteVenta?fechaInicio=01/01/2000&fechaFin=01/01/2000',
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "fechaRegistro" },
            { "data": "numeroVenta" },
            { "data": "tipoDocumento" },
            { "data": "documentoCliente" },
            { "data": "nombreCliente" },
            { "data": "subTotalVenta" },
            { "data": "impuestoTotalVenta" },
            { "data": "totalVenta" },
            { "data": "producto" },
            { "data": "cantidad" },
            { "data": "precio" },
            { "data": "total" },
        ],
        order: [[0, "desc"]],
        dom: "Bfrtip",
        buttons: [
            {
                text: 'Exportar Excel',
                extend: 'excelHtml5',
                title: '',
                filename: 'Reporte Ventas',
            }, 'pageLength'
        ],
        language: {
            url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
        },
    });

})

$("#btnBuscar").click(function () {

    if ($("#txtFechaInicio").val().trim() == "" || $("#txtFechaFin").val().trim() == "") {
        toastr.warning("", "Debe ingresar fecha de inicio y fin")
        return;
    }

    let fechaInicio = $("#txtFechaInicio").val().trim();
    let fechaFin = $("#txtFechaFin").val().trim();

    let nuevaUrl = `/Reportes/ReporteVenta?fechaInicio=${fechaInicio}&fechaFin=${fechaFin}`;

    dataTable.ajax.url(nuevaUrl).load();
    
})