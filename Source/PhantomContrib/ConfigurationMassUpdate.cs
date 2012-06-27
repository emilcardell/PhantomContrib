using System;
using Phantom.Core.Language;

namespace PhantomContrib
{
    public class configurationMassUpdate : IRunnable<configurationMassUpdate>
    {
        public string destinationFolder;
        public string sourceFolder;
        public string configurationName;

        public configurationMassUpdate()
        {
            configurationName = "local";
        }

        public configurationMassUpdate Run()
        {
            if (string.IsNullOrEmpty(destinationFolder))
                throw new InvalidOperationException("Please specify a destination folder.");

            if (string.IsNullOrEmpty(sourceFolder))
                throw new InvalidOperationException("Please specify a source folder.");

            new XmlMassUpdate().UpdateEverythingInFolder(sourceFolder, configurationName, destinationFolder);
            
            return this;
        }
    }
}
