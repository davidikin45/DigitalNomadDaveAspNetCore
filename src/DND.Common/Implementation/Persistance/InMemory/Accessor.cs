using System;

namespace DND.Common.Implementation.Persistance.InMemory
{
    internal class Accessor
    {
        public Accessor(Action removeAction, Func<object, object, object> getterFunc)
        {
            RemoveAction = removeAction;
            GetterFunc = getterFunc;
        }

        internal Action RemoveAction { get; set; }
        internal Func<object, object, object> GetterFunc { get; set; }
    }
}
