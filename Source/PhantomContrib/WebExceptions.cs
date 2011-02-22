using Phantom.Core;
using PhantomContrib.Core;

namespace PhantomContrib
{
    public class SiteAlreadyExistsException : PhantomException
    {
        public SiteAlreadyExistsException(string siteName)
            : base(string.Format("There is already a site with the name {0}.", siteName))
        {
        }
    }
}
