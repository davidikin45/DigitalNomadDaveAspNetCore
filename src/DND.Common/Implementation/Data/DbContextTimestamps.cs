using DND.Common.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Common.Implementation.Data
{
    public class DbContextTimestamps
    {
        public void AddTimestamps(IEnumerable<object> addedObjects, IEnumerable<object> modifiedObjects, IEnumerable<object> deletedObjects)
        {
            var added = addedObjects.Where(x => x is IBaseEntityAuditable);
            var modified = modifiedObjects.Where(x => x is IBaseEntityAuditable);
            var deleted = modifiedObjects.Where(x => x is IBaseEntityAuditable);

            //This doesn't work for .NET Core
            //var currentUsername = !string.IsNullOrEmpty(Thread.CurrentPrincipal?.Identity?.Name)
            //  ? Thread.CurrentPrincipal.Identity.Name
            //    : "Anonymous";

            foreach (var entity in added)
            {

                ((IBaseEntityAuditable)entity).DateCreated = DateTime.UtcNow;
                if (String.IsNullOrEmpty(((IBaseEntityAuditable)entity).UserCreated))
                {
                    ((IBaseEntityAuditable)entity).UserCreated = "Anonymous";
                }
                //((IBaseEntityAuditable)entity).UserCreated = currentUsername;

                ((IBaseEntityAuditable)entity).DateModified = DateTime.UtcNow;
                if (String.IsNullOrEmpty(((IBaseEntityAuditable)entity).UserModified))
                {
                    ((IBaseEntityAuditable)entity).UserModified = "Anonymous";
                }
                //((IBaseEntityAuditable)entity).UserModified = currentUsername;
            }

            foreach (var entity in modified)
            {

                ((IBaseEntityAuditable)entity).DateModified = DateTime.UtcNow;
                if (String.IsNullOrEmpty(((IBaseEntityAuditable)entity).UserModified))
                {
                    ((IBaseEntityAuditable)entity).UserModified = "Anonymous";
                }
                //((IBaseEntityAuditable)entity).UserModified = currentUsername;
            }

            foreach (var entity in deleted)
            {

                ((IBaseEntityAuditable)entity).DateDeleted = DateTime.UtcNow;
                if (String.IsNullOrEmpty(((IBaseEntityAuditable)entity).UserDeleted))
                {
                    ((IBaseEntityAuditable)entity).UserDeleted = "Anonymous";
                }
                //((IBaseEntityAuditable)entity).UserModified = currentUsername;
            }
        }
    }
}
