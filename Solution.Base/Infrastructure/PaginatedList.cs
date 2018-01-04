using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solution.Base.Infrastructure
{
    public class LoadMoreList<T> : List<T>
    {
        public int TotalCount { get; private set; }

        public LoadMoreList(IEnumerable<T> source, int skip, int take)
        {

            TotalCount = source.Count();
            this.AddRange(source.Skip(skip).Take(take));
        }

        public bool HasMore
        {
            get
            {
                return (TotalCount > this.Count);
            }
        }

    }
}
