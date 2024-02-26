using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.BLL.interfaces
{
    public interface ICorreoService
    {
        Task<bool> SendMail(string MailDestination, string Subject, string Message);
    }
}
