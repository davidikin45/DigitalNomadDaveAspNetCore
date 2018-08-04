using System.Collections.Generic;
using System.Reflection;

namespace DND.Common.Reflection
{
    public interface IAssemblyProvider
    {
        IEnumerable<Assembly> GetAssemblies(string path = null);
    }
}