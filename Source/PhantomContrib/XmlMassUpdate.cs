using System;
using System.IO;
using System.Linq;

namespace PhantomContrib
{
    public class XmlMassUpdate
    {
        public void UpdateEverythingInFolder(string sourceFolder, string configurationName, string destinationFolder)
        {
            var files = Directory.GetFiles(sourceFolder.Replace('\\', '/'));
            var masterFiles = files.Where(x => x.EndsWith(".master.config", StringComparison.InvariantCultureIgnoreCase));
            var xmlUpdater = new XmlUpdate();

            var updateFileNameEnding = configurationName + ".config";

            foreach (var masterFile in masterFiles)
            {
                var updateFileName = masterFile.Substring(0, "master.config".Length) + updateFileNameEnding;
                var updateFile = files.FirstOrDefault(x => x.Equals(updateFileName, StringComparison.InvariantCultureIgnoreCase));

                xmlUpdater.UpdateConfigFile(masterFile, updateFile, destinationFolder.Replace('\\', '/'));
             }

        }
    }
}
