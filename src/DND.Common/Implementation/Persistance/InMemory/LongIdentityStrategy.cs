using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DND.Common.Implementation.Persistance.InMemory
{
    public class LongIdentityStrategy<T> : IdentityStrategy<T, long>
         where T : class
    {
        /// <summary>
        /// Creates an instance of <see cref="IdentityStrategy{TType,TIdentity}"/> for entities where the identity property has type long.  Uses the provided identity <paramref name="property"/> setter.
        /// </summary>
        /// <param name="property">The property setter used to set the identity value of an entity.</param>
        public LongIdentityStrategy(Expression<Func<T, long>> property)
            : base(property)
        {
            Generator = GenerateLong;
        }

        /// <summary>
        /// Returns a value indicating whether a given value equals the default, unset identity value.
        /// </summary>
        /// <param name="id">The identity value to examine.</param>
        /// <returns>A value indicating whether a given value equals the default, unset identity value.</returns>
        protected override bool IsDefaultUnsetValue(long id)
        {
            return id == 0;
        }

        private long GenerateLong()
        {
            SetLastValue(++LastValue);
            return LastValue;
        }
    }
}
