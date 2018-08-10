using DND.Common.Interfaces.Data;
using DND.Common.Interfaces.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Common.Testing
{
    public class FakeSingleDbContextFactory<TIDbContext> : IDbContextFactory<TIDbContext> where TIDbContext : IBaseDbContext
    {
        private IBaseDbContext _dbContext;
        public FakeSingleDbContextFactory(IBaseDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IBaseDbContext CreateBaseDbContext()
        {
            return _dbContext;
        }

        public TIDbContext CreateDbContext()
        {
            return (TIDbContext)_dbContext;
        }
    }
}
