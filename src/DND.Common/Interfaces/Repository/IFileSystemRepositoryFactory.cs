using DND.Common.Interfaces.DomainServices;
using System;
using System.Threading;

namespace DND.Common.Interfaces.Repository
{
    public interface IFileSystemGenericRepositoryFactory : IBaseDomainService
    {      
        IBaseFileRepository CreateFileRepository(CancellationToken cancellationToken, string physicalPath, Boolean includeSubDirectories = false, string searchPattern = "*.*", params string[] extensions);
        IBaseFileReadOnlyRepository CreateFileRepositoryReadOnly(CancellationToken cancellationToken, string physicalPath, Boolean includeSubDirectories = false, string searchPattern = "*.*", params string[] extensions);

        IBaseJpegMetadataRepository CreateJpegMetadataRepository(CancellationToken cancellationToken, string physicalPath, Boolean includeSubDirectories = false);
        IBaseJpegMetadataReadOnlyRepository CreateJpegMetadataRepositoryReadOnly(CancellationToken cancellationToken, string physicalPath, Boolean includeSubDirectories = false);

        IBaseFolderRepository CreateFolderRepository(CancellationToken cancellationToken, string physicalPath, Boolean includeSubDirectories = false, string searchPattern = "*");
        IBaseFolderReadOnlyRepository CreateFolderRepositoryReadOnly(CancellationToken cancellationToken, string physicalPath, Boolean includeSubDirectories = false, string searchPattern = "*");
    }
}
