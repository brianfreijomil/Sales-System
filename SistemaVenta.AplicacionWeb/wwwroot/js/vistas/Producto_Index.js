const MODELO_BASE = {
    idProducto: 0,
    codigoBarra: "",
    marca: "",
    descripcion: "",
    idCategoria: 0,
    stock: 1,
    urlImagen: "",
    precio: 0,
    esActivo: ""
}

let dataTable;

$(document).ready(function () {

    fetch("/Categoria/Lista")
        .then(response => {
            return response.ok ? response.json() : Promise.reject(response);

        })
        .then(responseJson => {
            console.log(responseJson)
            if (responseJson.data.length > 0) {
                responseJson.data.forEach((item) => {
                    $("#cboCategoria").append(
                        $("<option>").val(item.idCategoria).text(item.descripcion)
                    )
                })
            }
        })
    
    dataTable = $('#tbdata').DataTable({
        responsive: true,
        "ajax": {
            "url": '/Producto/Lista',
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "idProducto", "visible": false, "searchable": false },
            {
                "data": "urlImagen", render: function (data) {
                    return `<img style="height:60px" src=${data} class="rounded mx-auto d-block"/>`
                }
            },
            { "data": "codigoBarra" },
            { "data": "marca" },
            { "data": "descripcion" },
            { "data": "nombreCategoria", "visible": false, "searchable": false },
            { "data": "stock" },
            { "data": "precio" },
            {
                "data": "esActivo", render: function (data) {
                    if (data == 1)
                        return '<span class="badge badge-info">Activo</span>';
                    else
                        return '<span class="badge badge-danger">No Activo</span>';
                }
            },
            {
                "defaultContent": '<button class="btn btn-primary btn-editar btn-sm mr-2"><i class="fas fa-pencil-alt"></i></button>' +
                    '<button class="btn btn-danger btn-eliminar btn-sm"><i class="fas fa-trash-alt"></i></button>',
                "orderable": false,
                "searchable": false,
                "width": "80px"
            }
        ],
        order: [[0, "desc"]],
        dom: "Bfrtip",
        buttons: [
            {
                text: 'Exportar Excel',
                extend: 'excelHtml5',
                title: '',
                filename: 'Reporte Productos',
                exportOptions: {
                    columns: [2, 3, 4, 5, 6]
                }
            }, 'pageLength'
        ],
        language: {
            url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
        },
    });
})

function mostrarModal(model = MODELO_BASE) {

    $("#txtId").val(model.idProducto)
    console.log(model.idProducto)

    $("#txtCodigoBarra").val(model.codigoBarra)
    $("#txtMarca").val(model.marca)
    $("#txtDescripcion").val(model.descripcion)
    $("#cboCategoria").val(model.idCategoria == 0 ? $("#cboCategoria option:first").val() : model.idCategoria)
    $("#txtStock").val(model.stock)
    $("#txtPrecio").val(model.precio)
    $("#cboEstado").val(model.esActivo)
    $("#txtImagen").val("")
    $("#imgProducto").attr("src", model.urlImagen)

    $("#modalData").modal("show")
}

$("#btnNuevo").click(function () {
    mostrarModal();
})

$("#btnGuardar").click(function () {

    const inputs = $("input.input-validar").serializeArray();
    const inputSinValor = inputs.filter((item) => item.value.trim() == "")

    if (inputSinValor.length > 0) {
        const mensaje = `Debe completar el campo : "${inputSinValor[0].name}"`;
        toastr.warning("", mensaje)
        $(`input[name="${inputSinValor[0].name}"]`).focus();
        return;
    }

    const modelo = structuredClone(MODELO_BASE);
    modelo["idProducto"] = parseInt($("#txtId").val())
    modelo["codigoBarra"] = $("#txtCodigoBarra").val()
    modelo["marca"] = $("#txtMarca").val()
    modelo["descripcion"] = $("#txtDescripcion").val()
    modelo["idCategoria"] = $("#cboCategoria").val()
    modelo["stock"] = $("#txtStock").val()
    modelo["precio"] = $("#txtPrecio").val()
    modelo["esActivo"] = $("#cboEstado").val()

    const inputFoto = document.getElementById("txtImagen")

    const formData = new FormData();

    formData.append("foto", inputFoto.files[0])
    formData.append("modelo", JSON.stringify(modelo))

    $("#modalData").find("div.modal-content").LoadingOverlay("show")

    if (modelo.idProducto == 0) {

        fetch("/Producto/Crear", {
            method: "POST",
            body: formData
        })
            .then(response => {
                $("#modalData").find("div.modal-content").LoadingOverlay("hide")
                return response.ok ? response.json() : Promise.reject(response);

            })
            .then(responseJson => {

                if (responseJson.estado) {
                    dataTable.row.add(responseJson.objeto).draw(false)
                    $("#modalData").modal("hide")
                    swal("Listo!", "El Producto fue creado", "success")
                }
                else {
                    swal("Lo sentimos!", responseJson.mensaje, "error")
                }
            })
    }
    else {
        fetch("/Producto/Editar", {
            method: "PUT",
            body: formData
        })
            .then(response => {
                $("#modalData").find("div.modal-content").LoadingOverlay("hide")
                return response.ok ? response.json() : Promise.reject(response);

            })
            .then(responseJson => {

                if (responseJson.estado) {
                    dataTable.row(filaSeleccionada).data(responseJson.objeto).draw(false);
                    filaSeleccionada = null;
                    $("#modalData").modal("hide")
                    swal("Listo!", "El producto fue modificado", "success")
                }
                else {
                    swal("Lo sentimos!", responseJson.mensaje, "error")
                }
            })
    }
})

let filaSeleccionada;
$("#tbdata tbody").on("click", ".btn-editar", function () {
    if ($(this).closest("tr").hasClass("child")) {
        filaSeleccionada = $(this).closest("tr").prev();
    }
    else {
        filaSeleccionada = $(this).closest("tr");
    }

    const data = dataTable.row(filaSeleccionada).data();

    mostrarModal(data);
})

$("#tbdata tbody").on("click", ".btn-eliminar", function () {

    let fila;

    if ($(this).closest("tr").hasClass("child")) {
        fila = $(this).closest("tr").prev();
    }
    else {
        fila = $(this).closest("tr");
    }

    const data = dataTable.row(fila).data();

    swal(
        {
            title: "¿Esta seguro?",
            text: `Eliminar el producto "${data.descripcion}"`,
            type: "warning",
            showCancelButton: true,
            confirmButtonClass: "btn-danger",
            confirmButtonText: "Si, eliminar",
            CancelButtonText: "No, cancelar",
            closeOnConfirm: false,
            closeOnCancel: true,
        },
        function (respuesta) {
            if (respuesta) {

                $(".showSweetAlert").LoadingOverlay("show")

                fetch(`/Producto/Eliminar?IdProducto=${data.idProducto}`, {
                    method: "DELETE"
                })
                    .then(response => {
                        $(".showSweetAlert").LoadingOverlay("hide")
                        return response.ok ? response.json() : Promise.reject(response);
                    })
                    .then(responseJson => {
                        if (responseJson.estado) {
                            dataTable.row(fila).remove().draw()

                            swal("Listo!", "El producto fue eliminado", "success")
                        }
                        else {
                            swal("Lo sentimos!", responseJson.mensaje, "error")
                        }
                    })
            }
        }
    )
})