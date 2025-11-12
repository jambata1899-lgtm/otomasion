using System;
using System.IO;
using System.Collections.Generic;

namespace PaygirLettersApp.Helpers
{
    using static System.Environment;
    public static class FileAttachmentHelper
    {
        public static string GetBaseFolder()
        {
            return Path.Combine(GetFolderPath(SpecialFolder.MyDocuments), "PaygirLetters_Attachments");
        }

        public static string CopyAttachments(int letterId, IEnumerable<string> sourceFiles)
        {
            var baseF = GetBaseFolder();
            Directory.CreateDirectory(baseF);
            var destFolder = Path.Combine(baseF, letterId.ToString());
            Directory.CreateDirectory(destFolder);
            foreach(var f in sourceFiles)
            {
                try
                {
                    var name = Path.GetFileName(f);
                    var dest = Path.Combine(destFolder, name);
                    File.Copy(f, dest, true);
                }
                catch { }
            }
            return destFolder;
        }

        public static List<string> GetSavedAttachments(int letterId)
        {
            var destFolder = Path.Combine(GetBaseFolder(), letterId.ToString());
            var list = new List<string>();
            if (!Directory.Exists(destFolder)) return list;
            foreach(var f in Directory.GetFiles(destFolder))
            {
                list.Add(f);
            }
            return list;
        }
    }
}
