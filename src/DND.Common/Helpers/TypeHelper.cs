using System;
using System.Linq;

namespace DND.Common.Helpers
{
    public static class TypeHelper
    {
        public static Type GetType(string assemblyQualifiedName)
        {
            // Throws exception is type was not found
            return Type.GetType(
                assemblyQualifiedName,
                (name) =>
                {
                    // Returns the assembly of the type by enumerating loaded assemblies
                    // in the app domain            
                    return AppDomain.CurrentDomain.GetAssemblies().Where(z => z.FullName == name.FullName).FirstOrDefault();
                },
                null,
                true);
        }
    }
}
