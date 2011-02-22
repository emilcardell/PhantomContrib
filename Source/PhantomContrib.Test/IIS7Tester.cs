using Microsoft.Web.Administration;
using NUnit.Framework;

namespace PhantomContrib.Test
{
    [TestFixture, Explicit, NUnit.Framework.Category("iis7")]
    public class IIS7Tester : ScriptTest
    {
        const string ApplicationPoolName = "testApplicationPoolName";
        const string SiteName = "newTestSite";
        
        [Test]
        public void StandardSiteLifeCycle_Create_To_Remove()
        {
            ScriptFile = "Scripts/IIS7.boo";
            //Set pre conditions
            using (var iisManager = new ServerManager()) 
            {

                if (iisManager.ApplicationPools[ApplicationPoolName] != null) 
                {
                    iisManager.ApplicationPools.Remove(iisManager.ApplicationPools[ApplicationPoolName]);
                    iisManager.CommitChanges();
                }
                if (iisManager.Sites[SiteName] != null) 
                {
                    iisManager.Sites.Remove(iisManager.Sites[SiteName]);
                    iisManager.CommitChanges();
                }
            }

            //Creation
            Execute("createSiteWithAppPool", "siteExists");
            AssertOutput("createSiteWithAppPool:", SiteName + " created", string.Empty, "siteExists:", "site exist");
            using (var iisManager = new ServerManager()) 
            {
                Assert.IsTrue(iisManager.Sites[SiteName].Applications[0].ApplicationPoolName == ApplicationPoolName);
                Assert.IsNotNull(iisManager.ApplicationPools[ApplicationPoolName], ApplicationPoolName);
            }

            BaseSetup();
            ScriptFile = "Scripts/IIS7.boo";
            
            //Deletion
            Execute("removeSiteAndAppPool", "siteExists");
            AssertOutput("removeSiteAndAppPool:", SiteName + " and application pool removed", string.Empty, "siteExists:", "site don't exist");
            using (var iisManager = new ServerManager()) 
            {
                Assert.IsNull(iisManager.ApplicationPools[ApplicationPoolName]);
            }
        }

        [Test]
        public void ExistingApplicationPoolSiteLifeCycle_Create_To_RemoveWithoutApplicationPool()
        {
            ScriptFile = "Scripts/IIS7.boo";

            using (var iisManager = new ServerManager()) 
            {
                //Set pre conditions
                if (iisManager.Sites[SiteName] != null) {
                    iisManager.Sites.Remove(iisManager.Sites[SiteName]);
                    iisManager.CommitChanges();
                }

                if (iisManager.ApplicationPools[ApplicationPoolName] == null) {
                    iisManager.ApplicationPools.Add(ApplicationPoolName);
                    iisManager.CommitChanges();
                }
            }

            //Creation
            Execute("createSiteWithAppPool", "siteExists");
            AssertOutput("createSiteWithAppPool:", SiteName + " created", string.Empty, "siteExists:", "site exist");
            using (var iisManager = new ServerManager()) 
            {
                Assert.IsTrue(iisManager.Sites[SiteName].Applications[0].ApplicationPoolName == ApplicationPoolName);
                Assert.IsNotNull(iisManager.ApplicationPools[ApplicationPoolName]);
            }

            BaseSetup();
            ScriptFile = "Scripts/IIS7.boo";
                
            //Deletion
            Execute("removeSiteAndNotAppPool", "siteExists");
            AssertOutput("removeSiteAndNotAppPool:", SiteName + " removed", string.Empty, "siteExists:", "site don't exist");
            using (var iisManager = new ServerManager())
            {
                Assert.IsNotNull(iisManager.ApplicationPools[ApplicationPoolName]);

                iisManager.ApplicationPools.Remove(iisManager.ApplicationPools[ApplicationPoolName]);
                iisManager.CommitChanges();
            }
        }

        [TearDown]
        public void CleanUp() 
        {
            using (var iisManager = new ServerManager())
            {
                if (iisManager.ApplicationPools[ApplicationPoolName] != null)
                    iisManager.ApplicationPools.Remove(iisManager.ApplicationPools[ApplicationPoolName]);

                if (iisManager.Sites[SiteName] != null)
                    iisManager.Sites.Remove(iisManager.Sites[SiteName]);

                iisManager.CommitChanges();
            }
        }
    }
}
