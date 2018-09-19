using System;

namespace DND.Common.Infrastructure.Dtos
{
    public class BulkDto<T> where T :class
    {
        public Object Id { get; set; }
        public T Value { get; set; }
    }
}
