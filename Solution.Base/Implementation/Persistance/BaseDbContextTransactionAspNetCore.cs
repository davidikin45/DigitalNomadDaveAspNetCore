using Microsoft.EntityFrameworkCore.Storage;
using Solution.Base.Interfaces.Persistance;
using System;

namespace Solution.Base.Implementation.Persistance
{
    public class BaseDbContextTransactionAspNetcore : IBaseDbContextTransaction
    {
        readonly IDbContextTransaction _transaction;
        bool _commited;

        public BaseDbContextTransactionAspNetcore(IDbContextTransaction transaction)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            this._transaction = transaction;
        }

        public void Commit()
        {
            this._transaction.Commit();
            this._commited = true;
        }

        public void Rollback()
        {
            this._transaction.Rollback();
        }

        public void Dispose()
        {
            if (!this._commited)
            {
                try
                {
                    this._transaction.Rollback();
                }
                catch (Exception) { }
            }

            this._transaction.Dispose();
        }

    }
}
