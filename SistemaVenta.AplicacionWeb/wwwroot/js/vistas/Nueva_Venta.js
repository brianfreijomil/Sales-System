




let ValorImpuesto = 0;

$(document).ready(function () {

    fetch("/Venta/ListaTipoDocumentoVenta")
        .then(response => {
            return response.ok ? response.json() : Promise.reject(response);

        })
        .then(responseJson => {
            if (responseJson.length > 0) {
                responseJson.forEach((item) => {
                    $("#cboTipoDocumentoVenta").append(
                        $("<option>").val(item.idTipoDocumentoVenta).text(item.descripcion)
                    )
                })
            }
        })


    fetch("/Negocio/Obtener")
        .then(response => {
            return response.ok ? response.json() : Promise.reject(response);

        })
        .then(responseJson => {
            if (responseJson.estado) {
                const d = responseJson.objeto;

                $("#inputGroupSubTotal").text(`Sub total - ${d.simboloMoneda}`)
                $("#inputGroupIGV").text(`IGV(${d.porcentajeImpuesto}) - ${d.simboloMoneda}`)
                $("#inputGroupTotal").text(`Total - ${d.simboloMoneda}`)

                ValorImpuesto = parseFloat(d.porcentajeImpuesto);
                
            }
        })

    $("#cboBuscarProducto").select2({
        ajax: {
            url: "/Venta/ObtenerProductos",
            dataType: 'json',
            contentType: "application/json; charset=utf-8",
            delay: 250,
            data: function (params) {
                return {
                    search: params.term
                };
            },
            processResults: function (data) {

                return {
                    results: data.map((item) => ({
                        id: item.idProducto,
                        text: item.descripcion,

                        marca: item.marca,
                        categoria: item.nombreCategoria,
                        urlImagen: item.urlImagen,
                        precio: parseFloat(item.precio)
                    }))
                };
            },
        },
        language: 'es',
        placeholder: 'Buscar Producto...',
        minimumInputLength: 1,
        templateResult: formatoResultado,
    });




})

function formatoResultado(data) {
    //by default...
    if (data.loading)
        return data.text;


    var contenedor = $(
        `<table width="100%">
            <tr>
                <td>
                    <img style="height:60px;width:60px;margin-right:10px" src="${data.urlImagen}"/>
                </td>
                <td>
                    <p style="font-weight: bolder;margin:2px">${data.marca}</p>
                    <p style="margin:2px">${data.text}</p>
                </td>
            </tr>
        </table>`
    )
    return contenedor;
}

$(document).on("select2:open", function () {
    document.querySelector(".select2-search__field").focus()
})

let ProductosParaVenta = [];

$("#cboBuscarProducto").on("select2:select", function (e) {
    const data = e.params.data;

    let productoEncontrado = ProductosParaVenta.filter(p => p.idProducto == data.id)

    if (productoEncontrado.length > 0) {
        $("#cboBuscarProducto").val("").trigger("change")
        toastr.warning("", "El producto ya fue agregado")
        return false
    }

    swal(
        {
            title: data.marca,
            text: data.text,
            imageUrl: data.urlImagen,
            type: "input",
            showCancelButton: true,
            closeOnConfirm: false,
            inputPlaceHolder: "Ingrese Cantidad"
        },
        function (valor) {
            if (valor === false) return false;

            if (valor === "") {
                toastr.warning("", "Necesita ingresar la cantidad")
                return false;
            }

            if (isNaN(parseInt(valor))) {
                toastr.warning("", "Debe ingresar un valor numerico")
                return false
            }

            let producto = {
                idProducto: data.id,
                marcaProducto: data.marca,
                descripcionProducto: data.text,
                categoriaProducto: data.categoria,
                cantidad: parseInt(valor),
                precio: data.precio.toString(),
                total: (parseFloat(valor)*data.precio).toString()
            }

            ProductosParaVenta.push(producto)
            mostrarProductos_Precios()
            $("#cboBuscarProducto").val("").trigger("change")
            swal.close()
        }
    )
})



function mostrarProductos_Precios() {

    let total = 0;
    let igv = 0;
    let subTotal = 0;
    let porcentaje = ValorImpuesto / 100;

    $("#tbProducto tbody").html("")

    ProductosParaVenta.forEach((item) => {

        total = total + parseFloat(item.total)
        $("#tbProducto tbody").append(
            $("<tr>").append(
                $("<td>").append(
                    $("<button>").addClass("btn btn-danger btn-eliminar btn-sm").append(
                        $("<i>").addClass("fas fa-trash-alt")
                    ).data("idProducto", item.idProducto)
                ),
                $("<td>").text(item.descripcionProducto),
                $("<td>").text(item.cantidad),
                $("<td>").text(item.precio),
                $("<td>").text(item.total)
            )
        )
    })

    subTotal = total / (1 + porcentaje);
    igv = total - subTotal;

    $("#txtSubTotal").val(subTotal.toFixed(2))
    $("#txtIGV").val(igv.toFixed(2))
    $("#txtTotal").val(total.toFixed(2))

}

$(document).on("click", "button.btn-eliminar", function () {

    const _idProducto = $(this).data("idProducto")

    ProductosParaVenta = ProductosParaVenta.filter(p => p.idProducto != _idProducto);

    mostrarProductos_Precios();


})

$("#btnTerminarVenta").click(function () {
    if (ProductosParaVenta.length < 1) {
        toastr.warning("", "Debe ingresar productos")
        return;
    }

    const vmDetalleVenta = ProductosParaVenta;

    const venta = {
        idTipoDocumentoVenta: $("#cboTipoDocumentoVenta").val(),
        documentoCliente: $("#txtDocumentoCliente").val(),
        nombreCliente: $("#txtNombreCliente").val(),
        subTotal: $("#txtSubTotal").val(),
        impuestoTotal: $("#txtIGV").val(),
        total: $("#txtTotal").val(),
        DetalleVenta: vmDetalleVenta
    }

    $("#btnTerminarVenta").LoadingOverlay("show")

    fetch("/Venta/RegistrarVenta", {
        method: "POST",
        headers: { "Content-Type": "application/json; charset=utf-8" },
        body: JSON.stringify(venta)
    })
        .then(response => {
            $("#btnTerminarVenta").LoadingOverlay("hide")
            return response.ok ? response.json() : Promise.reject(response);

        })
        .then(responseJson => {

            if (responseJson.estado) {
                ProductosParaVenta = [];
                mostrarProductos_Precios();

                $("#txtDocumentoCliente").val("")
                $("#txtNombreCliente").val("")
                $("#cboTipoDocumentoVenta").val($("#cboTipoDocumentoVenta option:first").val())



                swal("Registrado!", "Numero de Venta: " + responseJson.objeto.numeroVenta, "success")
                //swal("Registrado!", `Numero Venta : ${responseJson.objeto.numeroVenta}`, "succes")
            }
            else {
                swal("Lo sentimos!", "No se pudo registrar la venta", "error")
            }

        })
})

