using DND.Common.Interfaces.Persistance;
using System;
using System.Data.Entity;

namespace DND.Common.Implementation.Persistance
{
    public class BaseDbContextTransaction : IBaseDbContextTransaction
    {
        readonly DbContextTransaction _transaction;
        bool _commited;

        public BaseDbContextTransaction(DbContextTransaction transaction)
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
