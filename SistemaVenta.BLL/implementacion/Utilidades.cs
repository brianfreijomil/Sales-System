using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SistemaVenta.BLL.interfaces;
using System.Security.Cryptography;

namespace SistemaVenta.BLL.implementacion
{
    public class Utilidades : IUtilidades
    {
        //este metodo encripta la password
        public string ConvertSha256(string text)
        {
            StringBuilder sb = new StringBuilder();

            using (SHA256 hash = SHA256Managed.Create())
            {
                Encoding enc = Encoding.UTF8;

                byte[] result = hash.ComputeHash(enc.GetBytes(text));

                foreach (byte b in result)
                {
                    sb.Append(b.ToString("x2"));
                }
            }
            return sb.ToString();
        }

        public string GeneratePassword()
        {
            string password = Guid.NewGuid().ToString("N").Substring(0, 6);
            return password;
        }
    }
}
