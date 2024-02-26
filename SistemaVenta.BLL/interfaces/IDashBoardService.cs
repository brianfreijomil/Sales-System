using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SistemaVenta.BLL.interfaces
{
    public interface IDashBoardService
    {

        Task<int> TotalSalesLastWeek();

        Task<string> TotalIncomeLastWeek();

        Task<int> TotalProducts();

        Task<int> TotalCategories();

        Task<Dictionary<string, int>> SalesLastWeek();

        Task<Dictionary<string, int>> ProductsTopLastWeek();
    }
}
