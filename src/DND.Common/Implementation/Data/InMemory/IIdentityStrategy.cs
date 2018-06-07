using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Common.Implementation.Data.InMemory
{
    public interface IIdentityStrategy<in T>
        where T : class
    {
        /// <summary>
        /// Assigns an identity value to the given <paramref name="entity"/>.
        /// </summary>
        /// <param name="entity"></param>
        void Assign(T entity);
    }
}
