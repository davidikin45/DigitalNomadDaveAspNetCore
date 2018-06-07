using DND.Common.Implementation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Common.Interfaces.Data
{
    public interface IDbContextFactory
    {
        IBaseDbContext CreateDefault();
        TIDbContext Create<TIDbContext>() where TIDbContext : IBaseDbContext;
    }
}
