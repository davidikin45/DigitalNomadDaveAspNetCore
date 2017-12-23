using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace DND.Base.Interfaces.Persistance
{
    public interface IBaseIdentityDbContext<TUser> : IBaseDbContext where TUser : IdentityUser
    { 
        DbSet<TUser> Users { get; set; }
        DbSet<IdentityRole> Roles { get; set; }
    }
}
