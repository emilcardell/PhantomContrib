using NUnit.Framework;

namespace PhantomContrib.Test
{
    public class XmlUpdateTest
    {
        [Test]
        public void TestWithStaticFiles()
        {
            var masterFile = @"C:\myproject\config\Web.master.Config";
            var updateFile = @"C:\myproject\config\web.local.config";
            var destinationFolder = @"C:\myproject\Result\";

            new XmlUpdate().UpdateConfigFile(masterFile, updateFile, destinationFolder);
        }

        [Test]
        public void TestWithStaticFolder()
        {
            var masterFolder = @"C:\myproject\config\";
            var configurationName = "local";
            var destinationFolder = @"C:\myproject\Result\";

            new XmlMassUpdate().UpdateEverythingInFolder(masterFolder, configurationName, destinationFolder);

        }

        [Test]
        public void TestWithRelativeFolder()
        {
            var masterFolder = "config";
            var configurationName = "local";
            var destinationFolder = "Result";

            new XmlMassUpdate().UpdateEverythingInFolder(masterFolder, configurationName, destinationFolder);

        }
    }
}
