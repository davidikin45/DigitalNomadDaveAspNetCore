using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solution.Base.Interfaces.Persistance
{
    public interface IBaseDbContextTransaction : IDisposable
    {
        void Commit();
        void Rollback();
    }
}
