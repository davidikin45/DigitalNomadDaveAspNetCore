using DND.Common.Interfaces.Services;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Common.Implementation.Services
{
    public class FeatureService : IFeatureService
    {
        private IHostingEnvironment _hostingEnvironment;

        private Dictionary<string, bool> featureStates = new Dictionary<string, bool>();

        public FeatureService(IHostingEnvironment environment)
        {
            this._hostingEnvironment = environment;
            var path = Path.Combine(_hostingEnvironment.ContentRootPath, "features.json");

            this.featureStates =
                JsonConvert.DeserializeObject<Dictionary<string, bool>>
                (File.ReadAllText(path));
        }

        public bool IsFeatureActive(string featureName)
        {
            return featureStates[featureName];
        }
    }
}
