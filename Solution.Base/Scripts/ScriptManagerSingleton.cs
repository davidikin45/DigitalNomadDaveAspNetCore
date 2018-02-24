using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solution.Base.Scripts
{
    /// <summary>
    ///     ScriptManager keeps track of all the scripts (referenced javascript files) and scriptTexts (blocks of actual
    ///     javascript)
    ///     that have been added to the project.  ScriptManager makes sure there are no duplicates add so when it is time to
    ///     output the
    ///     javascript files, they are already deduped.
    /// </summary>
    public class ScriptManagerSingleton : IScriptManagerSingleton
    {

        public List<string> ScriptTexts { get; } = new List<string>();

        public void AddScriptText(string scriptTextExecute)
        {
            if (!ScriptTexts.Contains(scriptTextExecute))
                ScriptTexts.Add(scriptTextExecute);
        }
    }
}
