using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;


namespace PhantomContrib
{
    [CompilerGlobalScope]
    public static class RemoveEmptyDirectoriesFunctions
    {
        public static void RemoveEmptyDirectories(string startPath)
        {
            DeleteEmptyDirs(startPath.Replace("\\", "/"));
        }

        private static void DeleteEmptyDirs(string dir)
        {
            if (String.IsNullOrEmpty(dir))
                throw new ArgumentException("Starting directory is a null reference or an empty string", "dir");

            try
            {
                foreach (var d in Directory.EnumerateDirectories(dir))
                {
                    DeleteEmptyDirs(d);
                }

                var entries = Directory.EnumerateFileSystemEntries(dir);

                if (!entries.Any())
                {
                    try
                    {
                        Directory.Delete(dir);
                    }
                    catch (UnauthorizedAccessException) { }
                    catch (DirectoryNotFoundException) { }
                }
            }
            catch (UnauthorizedAccessException) { }
        }
    }
}
