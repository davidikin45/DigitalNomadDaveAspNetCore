﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DND.Common.Implementation.Persistance.InMemory
{
    public class GuidIdentityStrategy<T> : IdentityStrategy<T, Guid>
        where T : class
    {
        /// <summary>
        /// Creates an instance of <see cref="IdentityStrategy{TType,TIdentity}"/> for entities where the identity property has type Guid.  Uses the provided identity <paramref name="property"/> setter.
        /// </summary>
        /// <param name="property">The property setter used to set the identity value of an entity.</param>
        public GuidIdentityStrategy(Expression<Func<T, Guid>> property)
            : base(property)
        {
            Generator = GenerateGuid;
        }

        /// <summary>
        /// Returns a value indicating whether a given value equals the default, unset identity value.
        /// </summary>
        /// <param name="id">The identity value to examine.</param>
        /// <returns>A value indicating whether a given value equals the default, unset identity value.</returns>
        protected override bool IsDefaultUnsetValue(Guid id)
        {
            return id == Guid.Empty;
        }

        private Guid GenerateGuid()
        {
            SetLastValue(Guid.NewGuid());
            return LastValue;
        }
    }
}
