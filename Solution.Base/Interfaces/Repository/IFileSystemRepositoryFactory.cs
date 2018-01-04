using Solution.Base.Interfaces.Repository;
using Solution.Base.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Solution.Base.Interfaces.Repository
{
    public interface IFileSystemRepositoryFactory : IBaseBusinessService
    {      
        IBaseFileRepository CreateFileRepository(CancellationToken cancellationToken, string physicalPath, Boolean includeSubDirectories = false, string searchPattern = "*.*", params string[] extensions);
        IBaseFileReadOnlyRepository CreateFileRepositoryReadOnly(CancellationToken cancellationToken, string physicalPath, Boolean includeSubDirectories = false, string searchPattern = "*.*", params string[] extensions);

        IBaseJpegMetadataRepository CreateJpegMetadataRepository(CancellationToken cancellationToken, string physicalPath, Boolean includeSubDirectories = false);
        IBaseJpegMetadataReadOnlyRepository CreateJpegMetadataRepositoryReadOnly(CancellationToken cancellationToken, string physicalPath, Boolean includeSubDirectories = false);

        IBaseFolderRepository CreateFolderRepository(CancellationToken cancellationToken, string physicalPath, Boolean includeSubDirectories = false, string searchPattern = "*");
        IBaseFolderReadOnlyRepository CreateFolderRepositoryReadOnly(CancellationToken cancellationToken, string physicalPath, Boolean includeSubDirectories = false, string searchPattern = "*");
    }
}
