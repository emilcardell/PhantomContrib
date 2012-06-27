using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.Web.Administration;
using Phantom.Core.Language;

namespace PhantomContrib
{
    public class iis7_create_site : IRunnable<iis7_create_site>
    {
        public iis7_create_site()
        {
            managedRuntimeVersion = "v4.0";
            port = 80;
            managedPipelineMode = "Integrated";
        }

        public string managedRuntimeVersion { get; set; }
        public string path { get; set; }
        public string siteName { get; set; }
        public string bindingProtocol { get; set; }
        public string bindingInformation { get; set; }
        public string applicationPoolName { get; set; }
        public string managedPipelineMode { get; set; }
        public byte[] certificateHash { get; set; }

        public int port { get; set; }

        public iis7_create_site Run()
        {
            if (string.IsNullOrEmpty(siteName))
                throw new InvalidOperationException("Please specify a site name");

            if (string.IsNullOrEmpty(path))
                throw new InvalidOperationException("Please specify a site path");



            if (string.IsNullOrEmpty(applicationPoolName))
                applicationPoolName = siteName;

            using (var iisManager = new ServerManager())
            {
                if (iisManager.Sites[siteName] != null)
                    throw new SiteAlreadyExistsException(siteName);

                var appPool = iisManager.ApplicationPools.FirstOrDefault(x => x.Name.Equals(applicationPoolName, StringComparison.InvariantCultureIgnoreCase)) ??
                              iisManager.ApplicationPools.Add(applicationPoolName);

                appPool.ManagedPipelineMode = managedPipelineMode.Equals("Classic", StringComparison.InvariantCultureIgnoreCase) ? 
                    ManagedPipelineMode.Classic : ManagedPipelineMode.Integrated;

                appPool.ManagedRuntimeVersion = managedRuntimeVersion;
                
                var fixedPath = new DirectoryInfo(path.Replace('\\', '/')).FullName;

                Site siteToCreate;
                
                if(certificateHash != null)
                    siteToCreate = iisManager.Sites.Add(siteName, bindingInformation, fixedPath, certificateHash);
                else if(string.IsNullOrEmpty(bindingInformation))
                    siteToCreate = iisManager.Sites.Add(siteName, fixedPath, port);
                else
                    siteToCreate = iisManager.Sites.Add(siteName, bindingProtocol, bindingInformation, fixedPath);
                
                siteToCreate.Applications.First().ApplicationPoolName = applicationPoolName;
                iisManager.CommitChanges();
            }


            return this;
        }
    }
}
