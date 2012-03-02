using Phantom.Core;
using Phantom.Core.Language;
using Raven.Client.Document;
using Raven.Client.Extensions;

namespace PhantomContrib
{
    public class ravendb_create_tenant : IRunnable<ravendb_create_tenant>
    {
        public ravendb_create_tenant()
        {
            serverUrl = "http://localhost:8080";
        }

        public string serverUrl { get; set; }
        public string tenantName { get; set; }

        public ravendb_create_tenant Run()
        {
            if(string.IsNullOrEmpty(serverUrl))
                throw new StringIsNullOrEmptyException("Server url can't be empty");

            if (string.IsNullOrEmpty(tenantName))
                throw new StringIsNullOrEmptyException("Tenant name url can't be empty");

            using (var documentStore = new DocumentStore { Url = serverUrl })
            {
                documentStore.DatabaseCommands.EnsureDatabaseExists(tenantName);
            }
            return this;
        }
    }
}
