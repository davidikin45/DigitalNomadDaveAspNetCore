using DND.Common.Infrastructure;
using DND.Common.Testing;
using DND.Domain;
using DND.Domain.Models;
using DND.EFPersistance;
using DND.EFPersistance.Identity;
using DND.EFPersistance.Initializers;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DND.IntegrationTestsNUnit
{
    [SetUpFixture]
    public class GlobalDbSetupFixture : DND.TestSetup.DbSetupFixture
    {

    }
}