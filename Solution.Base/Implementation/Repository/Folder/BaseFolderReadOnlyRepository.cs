using Solution.Base.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Solution.Base.Implementation.Repository.Folder
{
    public class BaseFolderReadOnlyRepository : IBaseFolderReadOnlyRepository
    {
        protected readonly string _physicalPath;
        protected readonly string _searchPattern;
        protected readonly SearchOption _searchOption;
        protected readonly CancellationToken _cancellationToken;
        protected readonly Boolean _atLeastOneFile;

        public BaseFolderReadOnlyRepository(string physicalPath, Boolean includeSubDirectories, string searchPattern = "*",  CancellationToken cancellationToken = default(CancellationToken), Boolean atLeastOneFile = true)
        {
            if (!physicalPath.EndsWith("\\"))
                {
                physicalPath = physicalPath + "\\";
            }

            if (!System.IO.Directory.Exists(physicalPath))
            {
                throw new Exception("Path: " + physicalPath + " does not exist");
            }
                this._physicalPath = physicalPath;
            
            _searchOption = SearchOption.TopDirectoryOnly;
            if (includeSubDirectories)
            {
                _searchOption = SearchOption.AllDirectories;
            }
            _searchPattern = searchPattern;
            _cancellationToken = cancellationToken;
            _atLeastOneFile = atLeastOneFile;
        }

        protected virtual IQueryable<DirectoryInfo> GetQueryable(
            string search = "",
            Expression<Func<DirectoryInfo, bool>> filter = null,
            Func<IQueryable<DirectoryInfo>, IOrderedQueryable<DirectoryInfo>> orderBy = null,
            int? skip = null,
            int? take = null)
        {
            IQueryable<DirectoryInfo> query = new DirectoryInfo(_physicalPath).EnumerateDirectories(_searchPattern, _searchOption).AsQueryable();

            Expression<Func<DirectoryInfo, bool>> atLeastOneFileFilter = d => d.GetFiles("*.*", SearchOption.AllDirectories).Where(f => f.Name != "delete.txt").Count() > 0;

            if (_atLeastOneFile)
            {
                query = query.Where(atLeastOneFileFilter);
            }

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(f => f.FullName.Contains(search));
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            if (skip.HasValue)
            {
                query = query.Skip(skip.Value);
            }

            if (take.HasValue)
            {
                query = query.Take(take.Value);
            }

            return query;
        }

        public virtual IEnumerable<DirectoryInfo> GetAll(
          Func<IQueryable<DirectoryInfo>, IOrderedQueryable<DirectoryInfo>> orderBy = null,
          int? skip = null,
          int? take = null)
        {
            return GetQueryable(null, null, orderBy, skip, take).ToList();
        }

        public async virtual Task<IEnumerable<DirectoryInfo>> GetAllAsync(
         Func<IQueryable<DirectoryInfo>, IOrderedQueryable<DirectoryInfo>> orderBy = null,
         int? skip = null,
         int? take = null)
        {
            return GetQueryable(null, null, orderBy, skip, take).ToList();
        }

        public virtual IEnumerable<DirectoryInfo> Get(
            Expression<Func<DirectoryInfo, bool>> filter = null,
            Func<IQueryable<DirectoryInfo>, IOrderedQueryable<DirectoryInfo>> orderBy = null,
            int? skip = null,
            int? take = null)
        {
            return GetQueryable(null, filter, orderBy, skip, take).ToList();
        }

        public async virtual Task<IEnumerable<DirectoryInfo>> GetAsync(
           Expression<Func<DirectoryInfo, bool>> filter = null,
           Func<IQueryable<DirectoryInfo>, IOrderedQueryable<DirectoryInfo>> orderBy = null,
           int? skip = null,
           int? take = null)
        {
            return GetQueryable(null, filter, orderBy, skip, take).ToList();
        }

        public virtual IEnumerable<DirectoryInfo> Search(
           string search = "",
           Expression<Func<DirectoryInfo, bool>> filter = null,
           Func<IQueryable<DirectoryInfo>, IOrderedQueryable<DirectoryInfo>> orderBy = null,
           int? skip = null,
           int? take = null)
        {
            return GetQueryable(search, filter, orderBy, skip, take).ToList();
        }

        public async virtual Task<IEnumerable<DirectoryInfo>> SearchAsync(
            string search = "",
           Expression<Func<DirectoryInfo, bool>> filter = null,
           Func<IQueryable<DirectoryInfo>, IOrderedQueryable<DirectoryInfo>> orderBy = null,
           int? skip = null,
           int? take = null)
        {
            return GetQueryable(search, filter, orderBy, skip, take).ToList();
        }

        public virtual DirectoryInfo GetOne(
            Expression<Func<DirectoryInfo, bool>> filter = null)
        {
            return GetQueryable(null, filter, null, null, null).SingleOrDefault();
        }

        public async virtual Task<DirectoryInfo> GetOneAsync(
           Expression<Func<DirectoryInfo, bool>> filter = null)
        {
            return GetQueryable(null, filter, null, null, null).SingleOrDefault();
        }

        public virtual DirectoryInfo GetFirst(
           Expression<Func<DirectoryInfo, bool>> filter = null,
           Func<IQueryable<DirectoryInfo>, IOrderedQueryable<DirectoryInfo>> orderBy = null)
        {
            return GetQueryable(null, filter, orderBy, null, null).FirstOrDefault();
        }

        public async virtual Task<DirectoryInfo> GetFirstAsync(
          Expression<Func<DirectoryInfo, bool>> filter = null,
          Func<IQueryable<DirectoryInfo>, IOrderedQueryable<DirectoryInfo>> orderBy = null)
        {
            return GetQueryable(null, filter, orderBy, null, null).FirstOrDefault();
        }

        public virtual DirectoryInfo GetByPath(string path)
        {
            return GetQueryable(null, f => f.FullName.ToLower().EndsWith(path.ToLower())  , null, null, null).FirstOrDefault();
        }

        public async virtual Task<DirectoryInfo> GetByPathAsync(string path)
        {
            return GetQueryable(null, f => f.FullName.ToLower().EndsWith(path.ToLower()), null, null, null).FirstOrDefault();
        }

        public virtual int GetCount(Expression<Func<DirectoryInfo, bool>> filter = null)
        {
            return GetQueryable(null, filter).Count();
        }

        public async virtual Task<int> GetCountAsync(Expression<Func<DirectoryInfo, bool>> filter = null)
        {
            return GetQueryable(null, filter).Count();
        }

        public virtual int GetSearchCount(string search= "", Expression<Func<DirectoryInfo, bool>> filter = null)
        {
            return GetQueryable(search, filter).Count();
        }

        public async virtual Task<int> GetSearchCountAsync(string search = "", Expression<Func<DirectoryInfo, bool>> filter = null)
        {
            return GetQueryable(search, filter).Count();
        }

        public virtual bool GetExists(Expression<Func<DirectoryInfo, bool>> filter = null)
        {
            return GetQueryable(null, filter).Any();
        }

        public async virtual Task<bool> GetExistsAsync(Expression<Func<DirectoryInfo, bool>> filter = null)
        {
            return GetQueryable(null,filter).Any();
        }

        public void Dispose()
        {
            
        }
    }
}
