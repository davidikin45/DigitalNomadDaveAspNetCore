using DND.Interfaces.Blog.Data;
using DND.Interfaces.CMS.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Interfaces.Data
{
    public interface IApplicationDbContext : ICMSDbContext, IBlogDbContext
    {

    }
}
