using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Common.Scripts
{
    public interface IScriptManagerSingleton
    {
        List<string> ScriptTexts { get; }
        void AddScriptText(string scriptTextExecute);
    }
}
