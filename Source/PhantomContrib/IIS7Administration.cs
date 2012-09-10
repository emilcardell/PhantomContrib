#region License

// Copyright Emil Cardell (http://www.unwillingcoder.com) and Contributors
// 
// Licensed under the Microsoft Public License. You may
// obtain a copy of the license at:
// 
// http://www.microsoft.com/opensource/licenses.mspx
// 
// By using this source code in any fashion, you are agreeing
// to be bound by the terms of the Microsoft Public License.
// 
// You must not remove this notice, or any other, from this software.

#endregion
using Phantom.Core;
using System.Runtime.CompilerServices;
using Microsoft.Web.Administration;

namespace PhantomContrib
{
    [CompilerGlobalScope]
    public static class IIS7Administration
    {
        public static bool iis7_site_exists(string siteName) 
        {
            if(string.IsNullOrEmpty(siteName))
                throw  new StringIsNullOrEmptyException("siteName");

            using (var iisManager = new ServerManager())
            {
                var site = iisManager.GetSiteByName(siteName); 
                return site != null;
            }
        }

        public static bool iis7_site_isStopped(string siteName)
        {
            if (string.IsNullOrEmpty(siteName))
                throw new StringIsNullOrEmptyException("siteName");

            using (var iisManager = new ServerManager())
            {
                var site = iisManager.GetSiteByName(siteName);
                var state = site.State;
                return state == ObjectState.Stopped;
            }
        }

        public static bool iis7_site_isRunning(string siteName)
        {
            if (string.IsNullOrEmpty(siteName))
                throw new StringIsNullOrEmptyException("siteName");

            using (var iisManager = new ServerManager())
            {
                var site = iisManager.GetSiteByName(siteName);
                var state = site.State;
                return state == ObjectState.Started;
            }

        }
        

        public static void iis7_stop_site(string siteName)
        {
            if(string.IsNullOrEmpty(siteName))
                throw  new StringIsNullOrEmptyException("siteName");

            using (var iisManager = new ServerManager())
            {
                var site = iisManager.GetSiteByName(siteName);

                site.Stop();
                iisManager.CommitChanges();
            }
        }

        public static void iis7_start_site(string siteName)
        {
            if (string.IsNullOrEmpty(siteName))
                throw new StringIsNullOrEmptyException("siteName");

            using (var iisManager = new ServerManager())
            {
                var site = iisManager.GetSiteByName(siteName);
                site.Start();
                iisManager.CommitChanges();
            }
        }

        public static void iis7_remove_site(string siteName)
        {
            iis7_remove_site(siteName, false);
        }

        public static void iis7_remove_site(string siteName, bool removeApplicationPool)
        {
            using (var iisManager = new ServerManager())
            {
                var site = iisManager.GetSiteByNameSupressErrors(siteName);

                if (site == null)
                    return;

                if (removeApplicationPool && site.Applications[0] != null) 
                {
                    string applicationPoolName = site.Applications[0].ApplicationPoolName;

                    if (applicationPoolName != "Classic .NET AppPool" && applicationPoolName != "DefaultAppPool" && iisManager.ApplicationPools[applicationPoolName] != null)
                        iisManager.ApplicationPools.Remove(iisManager.ApplicationPools[applicationPoolName]);
                }

                iisManager.Sites.Remove(site);
                iisManager.CommitChanges();
            }
        }

    }
}
