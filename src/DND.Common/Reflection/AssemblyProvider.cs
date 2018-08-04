using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace DND.Common.Reflection
{
    public class AssemblyProvider : IAssemblyProvider
    {
        #region Fields
        //private bool _ignoreReflectionErrors = true;
        private string _assemblySkipLoadingPattern = "^System|^mscorlib|^Microsoft|^AjaxControlToolkit|^Antlr3|^Autofac|^AutoMapper|^Castle|^ComponentArt|^CppCodeProvider|^DotNetOpenAuth|^EntityFramework|^EPPlus|^FluentValidation|^ImageResizer|^itextsharp|^log4net|^MaxMind|^MbUnit|^MiniProfiler|^Mono.Math|^MvcContrib|^Newtonsoft|^NHibernate|^nunit|^Org.Mentalis|^PerlRegex|^QuickGraph|^Recaptcha|^Remotion|^RestSharp|^Rhino|^Telerik|^Iesi|^TestDriven|^TestFu|^UserAgentStringLibrary|^VJSharpCodeProvider|^WebActivator|^WebDev|^WebGrease";
        private string _assemblyRestrictToLoadingPattern = ".*";
        #endregion
        public AssemblyProvider()
        {
        }

        /// <summary>Gets the pattern for dlls that we know don't need to be investigated.</summary>
        public string AssemblySkipLoadingPattern
        {
            get { return _assemblySkipLoadingPattern; }
            set { _assemblySkipLoadingPattern = value; }
        }

        /// <summary>Gets or sets the pattern for dll that will be investigated. For ease of use this defaults to match all but to increase performance you might want to configure a pattern that includes assemblies and your own.</summary>
        /// <remarks>If you change this so that Nop assemblies arn't investigated (e.g. by not including something like "^Nop|..." you may break core functionality.</remarks>
        public string AssemblyRestrictToLoadingPattern
        {
            get { return _assemblyRestrictToLoadingPattern; }
            set { _assemblyRestrictToLoadingPattern = value; }
        }

        public IEnumerable<Assembly> GetAssemblies(string path = null)
        {
            return GetAssembliesFromPath(path);
        }


        #region Utilities


        /// <summary>
        /// Check if a dll is one of the shipped dlls that we know don't need to be investigated.
        /// </summary>
        /// <param name="assemblyFullName">
        /// The name of the assembly to check.
        /// </param>
        /// <returns>
        /// True if the assembly should be loaded into Nop.
        /// </returns>
        public virtual bool Matches(string assemblyFullName)
        {
            return !Matches(assemblyFullName, AssemblySkipLoadingPattern)
                   && Matches(assemblyFullName, AssemblyRestrictToLoadingPattern);
        }

        /// <summary>
        /// Check if a dll is one of the shipped dlls that we know don't need to be investigated.
        /// </summary>
        /// <param name="assemblyFullName">
        /// The assembly name to match.
        /// </param>
        /// <param name="pattern">
        /// The regular expression pattern to match against the assembly name.
        /// </param>
        /// <returns>
        /// True if the pattern matches the assembly name.
        /// </returns>
        protected virtual bool Matches(string assemblyFullName, string pattern)
        {
            return Regex.IsMatch(assemblyFullName, pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }

        private IEnumerable<Assembly> GetAssembliesFromPath(string path)
        {
            List<Assembly> assemblies = new List<Assembly>();

            if (!string.IsNullOrEmpty(path) && Directory.Exists(path))
            {

                foreach (string file in Directory.EnumerateFiles(path, "*.dll"))
                {

                    AssemblyName assemblyName = null;
                    try
                    {

                        //#if NETSTANDARD16
                        //                            assembly = System.Runtime.Loader.AssemblyLoadContext.Default.LoadFromAssemblyName(assemblyName);
                        //#else
                        //                            assembly = App.Load(assemblyName);
                        //#endif
                        assemblies.Add(Assembly.Load(assemblyName));

                    }
                    catch
                    {
                        // ignored
                    }
                }
            }

            else
            {

            }

            return assemblies;
        }

        #endregion
    }
}