using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Common.Interfaces.Services
{
    public interface ITypeHelperService
    {
        bool TypeHasProperties(Type type, string fields);
        bool TypeHasProperties<T>(string fields);
    }
}
