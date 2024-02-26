using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.BLL.interfaces
{
    public interface IUtilidades
    {

        string GeneratePassword();

        string ConvertSha256(string text);
    }
}
