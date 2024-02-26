using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SistemaVenta.BLL.interfaces;
using SistemaVenta.DAL.interfaces;
using SistemaVenta.Entity;

namespace SistemaVenta.BLL.implementacion
{
    public class NegocioService : INegocioService
    {

        private readonly IGenericRepository<Negocio> _repository;
        private readonly IFireBaseService _fireBaseService;

        public NegocioService( IGenericRepository<Negocio> repository, IFireBaseService fireBaseService)
        {
            _repository = repository;
            _fireBaseService = fireBaseService;
        }

        public async Task<Negocio> Get()
        {
            try
            {
                IQueryable<Negocio> businessFound = await _repository.GetByFilter(n => n.IdNegocio == 1);
                return businessFound.First();
            }
            catch
            {
                throw;
            }
        }

        public async Task<Negocio> SaveChanges(Negocio entity, Stream Logo = null, string NameLogo = "")
        {
            
            try
            {
                IQueryable<Negocio> businessFoundQuery = await _repository.GetByFilter(n => n.IdNegocio == 1);
                Negocio businessFound = businessFoundQuery.First();

                businessFound.NumeroDocumento = entity.NumeroDocumento;
                businessFound.Nombre = entity.Nombre;
                businessFound.Correo = entity.Correo;
                businessFound.Direccion = entity.Direccion;
                businessFound.Telefono = entity.Telefono;
                businessFound.PorcentajeImpuesto = entity.PorcentajeImpuesto;
                businessFound.SimboloMoneda = entity.SimboloMoneda;

                businessFound.NombreLogo = businessFound.NombreLogo == "" ? NameLogo : businessFound.NombreLogo;

                if(Logo != null)
                {
                    string urlLogo = await _fireBaseService.UploadStorage(Logo, "carpeta_logo", businessFound.NombreLogo);
                    businessFound.UrlLogo = urlLogo;
                }

                await _repository.Update(businessFound);
                return businessFound;
            }
            catch
            {
                throw;
            }
        }

    }
}
