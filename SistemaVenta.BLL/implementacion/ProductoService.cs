using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SistemaVenta.BLL.interfaces;
using SistemaVenta.DAL.interfaces;
using SistemaVenta.Entity;
using Microsoft.EntityFrameworkCore;

namespace SistemaVenta.BLL.implementacion
{
    public class ProductoService : IProductoService
    {

        private readonly IGenericRepository<Producto> _repository;
        private readonly IFireBaseService _fireBaseService;

        public ProductoService(IGenericRepository<Producto> repository, IFireBaseService fireBaseService, IUtilidades utilidades)
        {
            _repository = repository;
            _fireBaseService = fireBaseService;

        }

        public async Task<List<Producto>> List()
        {
            IQueryable<Producto> query = await _repository.GetByFilter();
            return query.Include(r => r.IdCategoriaNavigation).ToList();
        }

        public async Task<Producto> Create(Producto entity, Stream img = null, string NameImg = "")
        {

            IQueryable<Producto> productoExiteQuery = await _repository.GetByFilter(p => p.CodigoBarra == entity.CodigoBarra);

            if (productoExiteQuery.Any())
            {

                throw new TaskCanceledException("El codigo de barra ya existe"+entity.CodigoBarra);
            }

            try
            {

                entity.NombreImagen = NameImg;
                if(img != null)
                {
                    string urlImage = await _fireBaseService.UploadStorage(img, "carpeta_producto", NameImg);
                    entity.UrlImagen = urlImage;
                }

                Producto productCreated = await _repository.Create(entity);

                if(productCreated.IdProducto == 0)
                {
                    throw new TaskCanceledException("No se pudo crear el producto");
                }

                IQueryable<Producto> query = await _repository.GetByFilter(p => p.IdProducto == productCreated.IdProducto);
                productCreated = query.Include(c => c.IdCategoriaNavigation).First();

                return productCreated;

            }
            catch(Exception ex) {
                throw;    
            }
        }

        public async Task<Producto> Update(Producto entity, Stream img = null, string NameImg = "")
        {
            IQueryable<Producto> productExistQuery = await _repository.GetByFilter(p => p.CodigoBarra == entity.CodigoBarra && p.IdProducto != entity.IdProducto);

            if (productExistQuery.Any())
            {
                throw new TaskCanceledException("El codigo de barra ya existe");
            }

            try
            {
                IQueryable<Producto> query = await _repository.GetByFilter(p => p.IdProducto == entity.IdProducto);

                Producto productEdit = query.First();

                productEdit.CodigoBarra = entity.CodigoBarra;
                productEdit.Marca = entity.Marca;
                productEdit.Descripcion = entity.Descripcion;
                productEdit.IdCategoria = entity.IdCategoria;
                productEdit.Stock = entity.Stock;
                productEdit.Precio = entity.Precio;
                productEdit.EsActivo = entity.EsActivo;

                if(productEdit.NombreImagen == "")
                {
                    productEdit.NombreImagen = NameImg;
                }

                if(img != null)
                {
                    string urlImage = await _fireBaseService.UploadStorage(img, "carpeta_producto", productEdit.NombreImagen);
                    productEdit.UrlImagen = urlImage;
                }

                bool result = await _repository.Update(productEdit);

                if(!result)
                {
                    throw new TaskCanceledException("No se pudo editar el producto");
                }

                Producto productEdited = query.Include(p => p.IdCategoriaNavigation).First();

                return productEdited;
            }
            catch(Exception ex)
            {
                throw;
            }

        }

        public async Task<bool> Delete(int IdProduct)
        {
            try
            {
                IQueryable<Producto> queryProduct = await _repository.GetByFilter(p => p.IdProducto == IdProduct);
                if (!queryProduct.Any())
                {
                    throw new TaskCanceledException("El producto no existe");
                }

                Producto productFound = queryProduct.First();

                string namePhoto = productFound.NombreImagen;
                bool result = await _repository.Delete(productFound);

                if (result)
                    await _fireBaseService.DeleteStorage("carpeta_producto", namePhoto);

                return true;
            }
            catch
            {
                throw;
            }
        }

    }
}
