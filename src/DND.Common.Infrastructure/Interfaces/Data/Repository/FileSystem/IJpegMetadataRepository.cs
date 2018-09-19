namespace DND.Common.Interfaces.Repository
{
    public interface IJpegMetadataRepository : IJpegMetadataReadOnlyRepository
    {
        void Save(string path, byte[] bytes);

        void Save(string path, string text);

        void Move(string sourcePath, string destinationPath);

        void Rename(string sourcePath, string newName);

        void Delete(string path);
    }

}
