using DND.Common.Interfaces.Persistance;
using DND.Common.Interfaces.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Common.Testing
{
    public class FakeSingleDbContextFactory : IDbContextFactory
    {
        private IBaseDbContext _dbContext;
        public FakeSingleDbContextFactory(IBaseDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public TIDbContext Create<TIDbContext>() where TIDbContext : IBaseDbContext
        {
            return (TIDbContext)_dbContext;
        }

        public IBaseDbContext CreateDefault()
        {
            return _dbContext;
        }
    }
}
