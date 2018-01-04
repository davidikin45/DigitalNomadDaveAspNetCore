using DND.Domain.Models;
using System;
using System.Linq;

namespace DND.EFPersistance.Initializers
{
    public class DBSeed
    {
        public static void Seed(ApplicationDbContext context)
        {
            AddRoles(context);
            AddUsers(context);
            AddContentHtml(context);
            context.SaveChanges();
        }

        private static void AddRoles(ApplicationDbContext context)
        {
            string[] roles = new string[] {};

            foreach (string role in roles)
            {


                //if (!context.Roles.Any(r => r.Name == role))
                //{
                //    roleManager.Create(new IdentityRole(role));
                //}
            }
        }

        private static void AddUsers(ApplicationDbContext context)
        {
            //if (!(context.Users.Any(u => u.UserName == "admin")))
            //{
            //    var userStore = new UserStore<User>(context);
            //    var userManager = new UserManager<User>(userStore);
            //    var userToInsert = new User { UserName = "admin", Name = "admin" };
            //    userManager.Create(userToInsert, "password");

            //    var user = userManager.FindByName("admin");
            //    userManager.AddToRole(user.Id, "admin");
            //}
        }

        private static void AddContentText(ApplicationDbContext context)
        {
           
        }

        private static void AddContentHtml(ApplicationDbContext context)
        {
            AddContentHTML(context, DND.Domain.Constants.CMS.ContentHtml.About, "<p>About Me</p>");
            AddContentHTML(context, DND.Domain.Constants.CMS.ContentHtml.SideBarAbout, "<p>About Me</p>");
            AddContentHTML(context, DND.Domain.Constants.CMS.ContentHtml.WorkWithMe, "<p>Work With Me</p>");
            AddContentHTML(context, DND.Domain.Constants.CMS.ContentHtml.Affiliates, "<p>Affiliates</p>");
            AddContentHTML(context, DND.Domain.Constants.CMS.ContentHtml.Resume, "<p>Resume</p>");
            AddContentHTML(context, Solution.Base.Constants.CMS.ContentHtml.Contact, "<p>Contact</p>");
            AddContentHTML(context, Solution.Base.Constants.CMS.ContentHtml.Head, "");
            AddContentHTML(context, Solution.Base.Constants.CMS.ContentHtml.Main, "");
        }

        private static void AddContentHTML(ApplicationDbContext context, string id, string content)
        {
            if (!context.ContentHtml.Any(c => c.Id == id))
            {
                context.ContentHtml.Add(new ContentHtml() { Id = id, HTML = content, PreventDelete = true, DateCreated = DateTime.Now, UserCreated = "SYSTEM" });
            }
        }
    }
}
