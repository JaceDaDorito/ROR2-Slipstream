using RoR2;
using System.Collections.Generic;
using Zio;
using Zio.FileSystems;

namespace Slipstream.Modules
{
    public static class SlipLanguage
    {
        public static FileSystem FileSystem { get; private set; }

        public static void Initialize()
        {
            PhysicalFileSystem physicalFileSystem = new PhysicalFileSystem();
            FileSystem = new SubFileSystem(physicalFileSystem, physicalFileSystem.ConvertPathFromInternal(Assets.AssemblyDir), true);

            if (FileSystem.DirectoryExists("/Languages/"))
            {
                Language.collectLanguageRootFolders += delegate (List<DirectoryEntry> list)
                {
                    list.Add(FileSystem.GetDirectoryEntry("/Languages/"));
                };
            }
        }
    }
}
