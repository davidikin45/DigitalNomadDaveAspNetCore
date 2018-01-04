namespace Solution.Base.Interfaces.Repository
{
    public interface IBaseFileRepository : IBaseFileReadOnlyRepository
    {
        void Save(string path, byte[] bytes);

        void Save(string path, string text);

        void Move(string sourcePath, string destinationPath);

        void Rename(string sourcePath, string newName);

        void Delete(string path);
    }

}
