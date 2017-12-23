using System;
using System.Collections.Generic;
using System.Text;

namespace DND.Base.Interfaces.Persistance
{
    public interface IBaseDbContextTransaction : IDisposable
    {
        void Commit();
        void Rollback();
    }
}
