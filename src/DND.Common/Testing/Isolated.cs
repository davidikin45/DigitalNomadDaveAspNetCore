using NUnit.Framework;
using NUnit.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace DND.Common.Testing
{
    ////EF Core Doesn't support Ambient Transactions!!!
    public class Isolated : Attribute, ITestAction
    {
        private TransactionScope _transactionScope;

        public void BeforeTest(ITest test)
        {
            _transactionScope = new TransactionScope();
        }

        public void AfterTest(ITest test)
        {
            _transactionScope.Dispose();
        }

        public ActionTargets Targets
        {
            get
            {
                return ActionTargets.Test;
            }
        }

    }
}
