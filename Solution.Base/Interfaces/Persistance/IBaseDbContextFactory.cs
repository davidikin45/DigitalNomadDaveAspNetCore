using Solution.Base.Implementation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solution.Base.Interfaces.Persistance
{
    public interface IDbContextFactory
    {
        IBaseDbContext CreateDefault();
        TIDbContext Create<TIDbContext>() where TIDbContext : IBaseDbContext;
    }
}
