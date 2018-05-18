using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow;

namespace DND.UITests
{
    [Binding]
    public sealed class SpecFlowHooksAndFixtures
    {
        // For additional details on SpecFlow hooks see http://go.specflow.org/doc-hooks

    }

    public partial class BrowsePagesFeature : Xunit.IAssemblyFixture<TestSetup.DbSetupAndDotNetRunXUnitFixture>
    {

    }

    public partial class ContactFeature : Xunit.IAssemblyFixture<TestSetup.DbSetupAndDotNetRunXUnitFixture>
    {

    }
}
