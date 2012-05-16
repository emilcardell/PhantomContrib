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
using System.IO;
using System.Linq;
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
                var sites = iisManager.Sites.Where(site => site.Name.ToLowerInvariant() == siteName.ToLowerInvariant());
                return sites.Count() > 0;
            }
        }

        public static void iis7_create_site(string siteName, string bindingProtocol, string bindingInformation, string path) 
        {
            if (string.IsNullOrEmpty(siteName))
                throw new StringIsNullOrEmptyException("siteName");

            if (string.IsNullOrEmpty(bindingProtocol))
                throw new StringIsNullOrEmptyException("bindingProtocol");

            if (string.IsNullOrEmpty(bindingInformation))
                throw new StringIsNullOrEmptyException("bindingInformation");

            if (string.IsNullOrEmpty(path))
                throw new StringIsNullOrEmptyException("path");


            using (var iisManager = new ServerManager()) 
            {
                if (iisManager.Sites[siteName] != null)
                    throw new SiteAlreadyExistsException(siteName);

                iisManager.Sites.Add(siteName, bindingProtocol, bindingInformation, new DirectoryInfo(path.Replace('\\', '/')).FullName);
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
                if (iisManager.Sites[siteName] == null)
                    return;

                if (removeApplicationPool && iisManager.Sites[siteName].Applications[0] != null) 
                {
                    string applicationPoolName = iisManager.Sites[siteName].Applications[0].ApplicationPoolName;

                    if (applicationPoolName != "Classic .NET AppPool" && applicationPoolName != "DefaultAppPool" && iisManager.ApplicationPools[applicationPoolName] != null)
                        iisManager.ApplicationPools.Remove(iisManager.ApplicationPools[applicationPoolName]);
                }

                iisManager.Sites.Remove(iisManager.Sites[siteName]);
                iisManager.CommitChanges();
            }
        }

    }
}
