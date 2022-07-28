using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.Persistence.Interface
{
    public interface IRetailModelDao
    {
        List<RetailModel> GetRetailByYear(Int32 Year);
        RetailModel GetRetailByUniqueId(Guid UniqueId);
        int SaveRetail(RetailModel R);
        int DeleteRetail(RetailModel R);
        int SaveDisplayRetailLog(Int32 Year);
    }
}
