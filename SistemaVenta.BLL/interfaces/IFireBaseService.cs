using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.BLL.interfaces
{
    public interface IFireBaseService
    {
        Task<string> UploadStorage(Stream StreamFile, string FolderDestination, string NameFile);

        Task<bool> DeleteStorage(string FolderDestination, string NameFile);
    }
}
