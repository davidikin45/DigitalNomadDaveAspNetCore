using System;

namespace DND.Common.ModelMetadataCustom.FileAttributes
{
    public class FileSaveAttribute: Attribute
    {
        public string SavePath { get; set; }

        public FileSaveAttribute(string savePath)
        {
            SavePath = savePath;
        }
    }

    public class FileSaveToFolderAttribute : Attribute
    {
        public string FolderId { get; set; }

        public FileSaveToFolderAttribute(string folderId)
        {
            FolderId = folderId;
        }
    }
}
