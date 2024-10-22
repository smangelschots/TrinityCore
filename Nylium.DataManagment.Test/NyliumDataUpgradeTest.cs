using Nylium.DataManagement;

namespace Nylium.DataManagment.Test
{
    [TestClass]
    public class NyliumDataUpgradeTest
    {
        [TestMethod]
        public void TestSync()
        {

            var sync = new RaesSync();


            sync.SyncProjects();

        }
    }
}