namespace Solution.Base.Interfaces.Repository
{
    public interface IBaseFolderRepository : IBaseFolderReadOnlyRepository
    {
        void Create(string path);

        void Move(string sourcePath, string destinationPath);

        void Rename(string sourcePath, string newName);

        void Delete(string path);
    }

}
