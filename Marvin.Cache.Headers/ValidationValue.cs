// Any comments, input: @KevinDockx
// Any issues, requests: https://github.com/KevinDockx/HttpCacheHeaders

using System;

namespace Marvin.Cache.Headers
{
    public class ValidationValue
    {
        public ETag ETag { get; private set; }
        public DateTime LastModified { get; private set; }

        public ValidationValue(ETag eTag, DateTime lastModified)
        {
            ETag = eTag;
            LastModified = lastModified;
        }
    }
}
