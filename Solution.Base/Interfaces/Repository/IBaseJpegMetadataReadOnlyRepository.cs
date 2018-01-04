using Solution.Base.Implementation.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Solution.Base.Interfaces.Repository
{
    public interface IBaseJpegMetadataReadOnlyRepository : IBaseFileReadOnlyRepository
    {

        IEnumerable<JpegMetadata> MetadataGetAll(
            Func<IQueryable<FileInfo>, IOrderedQueryable<FileInfo>> orderBy = null,
            int? skip = null,
            int? take = null);

        Task<IEnumerable<JpegMetadata>> MetadataGetAllAsync(
          Func<IQueryable<FileInfo>, IOrderedQueryable<FileInfo>> orderBy = null,
          int? skip = null,
          int? take = null);

        IEnumerable<JpegMetadata> MetadataGet(
            Expression<Func<FileInfo, bool>> filter = null,
            Func<IQueryable<FileInfo>, IOrderedQueryable<FileInfo>> orderBy = null,
            int? skip = null,
            int? take = null)
            ;

        Task<IEnumerable<JpegMetadata>> MetadataGetAsync(
           Expression<Func<FileInfo, bool>> filter = null,
           Func<IQueryable<FileInfo>, IOrderedQueryable<FileInfo>> orderBy = null,
           int? skip = null,
           int? take = null)
           ;

        IEnumerable<JpegMetadata> MetadataSearch(
           string search= "",
           Expression<Func<FileInfo, bool>> filter = null,
           Func<IQueryable<FileInfo>, IOrderedQueryable<FileInfo>> orderBy = null,
           int? skip = null,
           int? take = null)
           ;

        Task<IEnumerable<JpegMetadata>> MetadataSearchAsync(
             string search = "",
           Expression<Func<FileInfo, bool>> filter = null,
           Func<IQueryable<FileInfo>, IOrderedQueryable<FileInfo>> orderBy = null,
           int? skip = null,
           int? take = null)
           ;

        JpegMetadata MetadataGetOne(
            Expression<Func<FileInfo, bool>> filter = null)
            ;

        Task<JpegMetadata> MetadataGetOneAsync(
         Expression<Func<FileInfo, bool>> filter = null)
         ;

        JpegMetadata MetadataGetFirst(
            Expression<Func<FileInfo, bool>> filter = null,
            Func<IQueryable<FileInfo>, IOrderedQueryable<FileInfo>> orderBy = null)
            ;

        Task<JpegMetadata> MetadataGetFirstAsync(
          Expression<Func<FileInfo, bool>> filter = null,
          Func<IQueryable<FileInfo>, IOrderedQueryable<FileInfo>> orderBy = null)
          ;

        JpegMetadata MetadataGetByPath(string path)
            ;

        Task<JpegMetadata> MetadataGetByPathAsync(string path)
           ;

        JpegMetadata MetadataGetMain()
         ;

        Task<JpegMetadata> MetadataGetMainAsync();

    }

}
