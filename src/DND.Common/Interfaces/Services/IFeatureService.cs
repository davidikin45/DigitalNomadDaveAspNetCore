﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Common.Interfaces.Services
{
    public interface IFeatureService
    {
        bool IsFeatureActive(string featureName);
    }
}
