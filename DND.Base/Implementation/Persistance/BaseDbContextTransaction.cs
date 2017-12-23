using Microsoft.EntityFrameworkCore.Storage;
using DND.Base.Interfaces.Persistance;
using System;
using System.Collections.Generic;
using System.Text;

namespace DND.Base.Implementation.Persistance
{
    class BaseDbContextTransaction : IBaseDbContextTransaction
    {
        readonly IDbContextTransaction _transaction;
        bool _commited;

        public BaseDbContextTransaction(IDbContextTransaction transaction)
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
