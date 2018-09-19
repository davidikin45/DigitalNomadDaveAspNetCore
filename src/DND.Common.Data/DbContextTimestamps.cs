using DND.Common.Infrastrucutre.Interfaces.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DND.Common.Data
{
    public class DbContextTimestamps
    {
        public void AddTimestamps(IEnumerable<object> addedObjects, IEnumerable<object> modifiedObjects, IEnumerable<object> deletedObjects)
        {
            var added = addedObjects.Where(x => x is IEntityAuditable);
            var modified = modifiedObjects.Where(x => x is IEntityAuditable);
            var deleted = modifiedObjects.Where(x => x is IEntityAuditable);

            foreach (var entity in added)
            {

                ((IEntityAuditable)entity).DateCreated = DateTime.UtcNow;
                if (String.IsNullOrEmpty(((IEntityAuditable)entity).UserCreated))
                {
                    ((IEntityAuditable)entity).UserCreated = "Anonymous";
                }

                ((IEntityAuditable)entity).DateModified = DateTime.UtcNow;
                if (String.IsNullOrEmpty(((IEntityAuditable)entity).UserModified))
                {
                    ((IEntityAuditable)entity).UserModified = "Anonymous";
                }
            }

            foreach (var entity in modified)
            {

                ((IEntityAuditable)entity).DateModified = DateTime.UtcNow;
                if (String.IsNullOrEmpty(((IEntityAuditable)entity).UserModified))
                {
                    ((IEntityAuditable)entity).UserModified = "Anonymous";
                }
            }

            foreach (var entity in deleted)
            {

                ((IEntityAuditable)entity).DateDeleted = DateTime.UtcNow;
                if (String.IsNullOrEmpty(((IEntityAuditable)entity).UserDeleted))
                {
                    ((IEntityAuditable)entity).UserDeleted = "Anonymous";
                }
                //((IBaseEntityAuditable)entity).UserModified = currentUsername;
            }
        }
    }
}
