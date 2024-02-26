using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Mail;

using SistemaVenta.BLL.interfaces;
using SistemaVenta.DAL.interfaces;
using SistemaVenta.Entity;

namespace SistemaVenta.BLL.implementacion
{
    public class CorreoService : ICorreoService
    {

        private readonly IGenericRepository<Configuracion> _repository;

        public CorreoService(IGenericRepository<Configuracion> repository)
        {
            _repository = repository;
        }

        public async Task<bool> SendMail(string destinationEmail, string subject, string msj)
        {
            try
            {
                IQueryable<Configuracion> query = await _repository.GetByFilter(c => c.Recurso.Equals("Servicio_Correo"));

                Dictionary<string, string> Config = query.ToDictionary(keySelector: c => c.Propiedad, elementSelector: c => c.Valor);

                var credentials = new NetworkCredential(Config["correo"], Config["clave"]);

                //configuracion del mail
                var correo = new MailMessage()
                {
                    From = new MailAddress(Config["correo"], Config["alias"]),
                    Subject = subject,
                    Body = msj,
                    IsBodyHtml = true
                };

                correo.To.Add(new MailAddress(destinationEmail));

                //configuracion del envio del mail
                var clientServer = new SmtpClient()
                {
                    Host = Config["host"],
                    Port = int.Parse(Config["puerto"]),
                    Credentials = credentials,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    EnableSsl = true
                };

                clientServer.Send(correo);
                return true;

            }
            catch
            {
                return false;
            }
        }
    }
}
