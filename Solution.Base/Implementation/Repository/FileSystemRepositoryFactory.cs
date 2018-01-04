using AutoMapper;
using Solution.Base.Implementation.Services;
using Solution.Base.Interfaces.Services;
using Solution.Base.Interfaces.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Solution.Base.Interfaces.Repository;
using System.Threading;
using System.Collections.Concurrent;
using Solution.Base.Implementation.Repository.File;
using Solution.Base.Implementation.Repository.Folder;

namespace Solution.Base.Implementation.Repository
{
    public class FileSystemRepositoryFactory : BaseBusinessService, IFileSystemRepositoryFactory
    {
        public FileSystemRepositoryFactory(IBaseUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory, IMapper mapper)
            :base(baseUnitOfWorkScopeFactory, mapper)
        {

        }

        public IBaseFileRepository CreateFileRepository(CancellationToken cancellationToken, string physicalPath, bool includeSubDirectories = false, string searchPattern = "*.*", params string[] extensions)
        {
            return new BaseFileRepository(physicalPath, includeSubDirectories, searchPattern, cancellationToken, extensions);
        }

        public IBaseFileReadOnlyRepository CreateFileRepositoryReadOnly(CancellationToken cancellationToken, string physicalPath, bool includeSubDirectories = false, string searchPattern = "*.*", params string[] extensions)
        {
            return new BaseFileReadOnlyRepository(physicalPath, includeSubDirectories, searchPattern, cancellationToken, extensions);
        }

        public IBaseFolderRepository CreateFolderRepository(CancellationToken cancellationToken, string physicalPath, bool includeSubDirectories = false, string searchPattern = "*")
        {
            return new BaseFolderRepository(physicalPath, includeSubDirectories, searchPattern, cancellationToken);
        }

        public IBaseFolderReadOnlyRepository CreateFolderRepositoryReadOnly(CancellationToken cancellationToken, string physicalPath, bool includeSubDirectories = false, string searchPattern = "*")
        {
            return new BaseFolderReadOnlyRepository(physicalPath, includeSubDirectories, searchPattern, cancellationToken);
        }

        public IBaseJpegMetadataReadOnlyRepository CreateJpegMetadataRepositoryReadOnly(CancellationToken cancellationToken, string physicalPath, bool includeSubDirectories = false)
        {
            return new BaseJpegMetadataReadOnlyRepository(physicalPath, includeSubDirectories, cancellationToken);
        }

        public IBaseJpegMetadataRepository CreateJpegMetadataRepository(CancellationToken cancellationToken, string physicalPath, bool includeSubDirectories = false)
        {
            return new BaseJpegMetadataRepository(physicalPath, includeSubDirectories, cancellationToken);
        }
        
    }

}
