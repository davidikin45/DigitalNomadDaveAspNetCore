using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace DND.Common.Reflection
{
    public class AssemblyProvider : IAssemblyProvider
    {
        private AssemblyProviderOptions Options;
        public AssemblyProvider(IOptions<AssemblyProviderOptions> options)
        {
            Options = options.Value;
        }

        public IEnumerable<Assembly> GetAssemblies(IEnumerable<string> paths = null, Func<string, Boolean> filter = null)
        {
            if(paths == null)
            {
                paths = new List<string>() { Options.BinPath };
                filter = Options.AssemblyFilter;
            }

            return GetAssembliesFromPaths(paths, filter);
        }

        private IEnumerable<Assembly> GetAssembliesFromPaths(IEnumerable<string> paths, Func<string, Boolean> filter = null)
        {
            List<Assembly> assemblies = new List<Assembly>();

            foreach (string path in paths)
            {
                IEnumerable<string> files = Directory.GetFiles(path, "*.dll").ToList();

                if(filter != null)
                {
                    files = files.Where(filter);
                }

                assemblies.AddRange(files.Select(System.Reflection.Assembly.LoadFrom));
            }

            return assemblies;
        }
    }
}