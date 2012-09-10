using System;
using Microsoft.Web.Administration;
using System.Linq;

namespace PhantomContrib
{
    public static class ServerManagerExtentions
    {
        public static Site GetSiteByName(this ServerManager iisManager, string siteName)
        {
            var site = iisManager.Sites.FirstOrDefault(s => s.Name.Equals(siteName, StringComparison.InvariantCultureIgnoreCase));
            if (site == null)
                throw new Exception("Site with name " + siteName + " doesn't exist.");

            return site;
        }

        public static Site GetSiteByNameSupressErrors(this ServerManager iisManager, string siteName)
        {
            return iisManager.Sites.FirstOrDefault(s => s.Name.Equals(siteName, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
