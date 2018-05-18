﻿using System;
using System.Linq.Expressions;

namespace DND.Common.Implementation.Persistance.InMemory
{
    public class IntegerIdentityStrategy<T> : IdentityStrategy<T, int>
         where T : class
    {
        /// <summary>
        /// Creates an instance of <see cref="IdentityStrategy{TType,TIdentity}"/> for entities where the identity property has type int.  Uses the provided identity <paramref name="property"/> setter.
        /// </summary>
        /// <param name="property">The property setter used to set the identity value of an entity.</param>
        public IntegerIdentityStrategy(Expression<Func<T, int>> property)
            : base(property)
        {
            Generator = GenerateInt;
        }

        /// <summary>
        /// Returns a value indicating whether a given value equals the default, unset identity value.
        /// </summary>
        /// <param name="id">The identity value to examine.</param>
        /// <returns>A value indicating whether a given value equals the default, unset identity value.</returns>
        protected override bool IsDefaultUnsetValue(int id)
        {
            return id == 0;
        }

        private int GenerateInt()
        {
            SetLastValue(++LastValue);
            return LastValue;
        }
    }
}
