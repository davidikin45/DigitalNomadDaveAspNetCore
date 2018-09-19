using DND.Common.Implementation.Repository.FileSystem.File;
using DND.Common.Implementation.Repository.FileSystem.Folder;
using DND.Common.Interfaces.Repository;
using System.Threading;

namespace DND.Common.Data.Repository.FileSystem
{
    public class FileSystemGenericRepositoryFactory : IFileSystemGenericRepositoryFactory
    {
        public FileSystemGenericRepositoryFactory()
            :base()
        {

        }

        public IFileRepository CreateFileRepository(CancellationToken cancellationToken, string physicalPath, bool includeSubDirectories = false, string searchPattern = "*.*", params string[] extensions)
        {
            return new FileRepository(physicalPath, includeSubDirectories, searchPattern, cancellationToken, extensions);
        }

        public IFileReadOnlyRepository CreateFileRepositoryReadOnly(CancellationToken cancellationToken, string physicalPath, bool includeSubDirectories = false, string searchPattern = "*.*", params string[] extensions)
        {
            return new FileReadOnlyRepository(physicalPath, includeSubDirectories, searchPattern, cancellationToken, extensions);
        }

        public IFolderRepository CreateFolderRepository(CancellationToken cancellationToken, string physicalPath, bool includeSubDirectories = false, string searchPattern = "*")
        {
            return new FolderRepository(physicalPath, includeSubDirectories, searchPattern, cancellationToken);
        }

        public IFolderReadOnlyRepository CreateFolderRepositoryReadOnly(CancellationToken cancellationToken, string physicalPath, bool includeSubDirectories = false, string searchPattern = "*")
        {
            return new FolderReadOnlyRepository(physicalPath, includeSubDirectories, searchPattern, cancellationToken);
        }

        public IJpegMetadataReadOnlyRepository CreateJpegMetadataRepositoryReadOnly(CancellationToken cancellationToken, string physicalPath, bool includeSubDirectories = false)
        {
            return new JpegMetadataReadOnlyRepository(physicalPath, includeSubDirectories, cancellationToken);
        }

        public IJpegMetadataRepository CreateJpegMetadataRepository(CancellationToken cancellationToken, string physicalPath, bool includeSubDirectories = false)
        {
            return new JpegMetadataRepository(physicalPath, includeSubDirectories, cancellationToken);
        }
        
    }
}
