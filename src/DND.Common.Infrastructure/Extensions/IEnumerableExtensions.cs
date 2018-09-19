using System.Collections.Generic;

namespace DND.Common.Infrastructure.Extensions
{
    public static class IEnumerableExtensions
    {
        public static List<T> MergeLists<T>(
          this IEnumerable<List<T>> source)
        {
            var newList = new List<T>();
            foreach (var list in source)
            {
                foreach (var item in list)
                {
                    newList.Add(item);
                }
            }
            return newList;
        }  
    }
}
