using Solution.Base.Interfaces.Repository;
using Solution.Base.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Solution.Base.Implementation.Repository.File
{
    public class BaseJpegMetadataReadOnlyRepository : BaseFileReadOnlyRepository, IBaseJpegMetadataReadOnlyRepository
    {

        public BaseJpegMetadataReadOnlyRepository(string physicalPath, Boolean includeSubDirectories, CancellationToken cancellationToken = default(CancellationToken))
           : base(physicalPath, includeSubDirectories, "*.*",  cancellationToken, ".jpg", ".jpeg")
        {
           
        }

        public JpegMetadata MapFileInfoToJpegMetadata(FileInfo fileInfo)
        {
            var metadata = new JpegMetadata(fileInfo.FullName, _physicalPath);
            return metadata;
        }

        public IEnumerable<JpegMetadata> MetadataGetAll(
          Func<IQueryable<FileInfo>, IOrderedQueryable<FileInfo>> orderBy = null,
          int? skip = null,
          int? take = null)
        {
            return base.GetAll(orderBy, skip, take).Select(s => MapFileInfoToJpegMetadata(s));
        }

        public async virtual Task<IEnumerable<JpegMetadata>> MetadataGetAllAsync(
         Func<IQueryable<FileInfo>, IOrderedQueryable<FileInfo>> orderBy = null,
         int? skip = null,
         int? take = null)
        {
            return (await base.GetAllAsync(orderBy, skip, take)).Select(s => MapFileInfoToJpegMetadata(s));
        }

        public virtual IEnumerable<JpegMetadata> MetadataGet(
            Expression<Func<FileInfo, bool>> filter = null,
            Func<IQueryable<FileInfo>, IOrderedQueryable<FileInfo>> orderBy = null,
            int? skip = null,
            int? take = null)
        {
            return base.Get(filter, orderBy, skip, take).Select(s => MapFileInfoToJpegMetadata(s));
        }

        public async virtual Task<IEnumerable<JpegMetadata>> MetadataGetAsync(
           Expression<Func<FileInfo, bool>> filter = null,
           Func<IQueryable<FileInfo>, IOrderedQueryable<FileInfo>> orderBy = null,
           int? skip = null,
           int? take = null)
        {
            return (await base.GetAsync(filter, orderBy, skip, take)).Select(s => MapFileInfoToJpegMetadata(s));
        }

        public virtual IEnumerable<JpegMetadata> MetadataSearch(
          string search = "",
          Expression<Func<FileInfo, bool>> filter = null,
          Func<IQueryable<FileInfo>, IOrderedQueryable<FileInfo>> orderBy = null,
          int? skip = null,
          int? take = null)
        {
            return base.Search(search, filter, orderBy, skip, take).Select(s => MapFileInfoToJpegMetadata(s));
        }

        public async virtual Task<IEnumerable<JpegMetadata>> MetadataSearchAsync(
           string search = "",
           Expression<Func<FileInfo, bool>> filter = null,
           Func<IQueryable<FileInfo>, IOrderedQueryable<FileInfo>> orderBy = null,
           int? skip = null,
           int? take = null)
        {
            return (await base.SearchAsync(search, filter, orderBy, skip, take)).Select(s => MapFileInfoToJpegMetadata(s));
        }

        public virtual JpegMetadata MetadataGetOne(
            Expression<Func<FileInfo, bool>> filter = null)
        {
            return MapFileInfoToJpegMetadata(base.GetOne(filter));
        }

        public async virtual Task<JpegMetadata> MetadataGetOneAsync(
           Expression<Func<FileInfo, bool>> filter = null)
        {
            return MapFileInfoToJpegMetadata(await base.GetOneAsync(filter));
        }

        public virtual JpegMetadata MetadataGetMain()
        {
            return MapFileInfoToJpegMetadata(base.GetMain());
        }

        public async virtual Task<JpegMetadata> MetadataGetMainAsync()
        {
            return MapFileInfoToJpegMetadata(await base.GetMainAsync());
        }

        public virtual JpegMetadata MetadataGetFirst(
           Expression<Func<FileInfo, bool>> filter = null,
           Func<IQueryable<FileInfo>, IOrderedQueryable<FileInfo>> orderBy = null)
        {
            return MapFileInfoToJpegMetadata(base.GetFirst(filter,orderBy));
        }

        public async virtual Task<JpegMetadata> MetadataGetFirstAsync(
          Expression<Func<FileInfo, bool>> filter = null,
          Func<IQueryable<FileInfo>, IOrderedQueryable<FileInfo>> orderBy = null)
        {
            return MapFileInfoToJpegMetadata(await base.GetFirstAsync(filter, orderBy));
        }

        public virtual JpegMetadata MetadataGetByPath(string path)
        {
            return MapFileInfoToJpegMetadata(base.GetByPath(path));
        }

        public async virtual Task<JpegMetadata> MetadataGetByPathAsync(string path)
        {
            return MapFileInfoToJpegMetadata(await base.GetByPathAsync(path));
        }

        public void Dispose()
        {
            
        }
    }
}
