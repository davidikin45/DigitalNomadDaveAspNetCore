﻿namespace DND.Common.Interfaces.Repository
{
    public interface IFolderRepository : IFolderReadOnlyRepository
    {
        void Create(string path);

        void Move(string sourcePath, string destinationPath);

        void Rename(string sourcePath, string newName);

        void Delete(string path);
    }

}
