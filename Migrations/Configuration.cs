namespace HeedeMoestrup.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using static Models.ModelContext;

    internal sealed class Configuration : DbMigrationsConfiguration<HeedeMoestrup.Models.ModelContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            ContextKey = "HeedeMoestrup.Models.ModelContext";
        }

        protected override void Seed(HeedeMoestrup.Models.ModelContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
            
            //if (!(context.Users.Any(u => u.UserName == "Testadmin")))
            //{
            //    var userStore = new UserStore<AppUser>(context);
            //    var manager = new UserManager<AppUser>(userStore);

            //    var user = new AppUser { UserName = "Testadmin" };
            //    manager.Create(user, "123456");
            //}

            if (!(context.Users.Any(u => u.UserName == "MainAdmin")) & !(context.Roles.Any(u => u.Name == "SuperAdmin")))
            {
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
                var UserManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>(context));

                var role = new IdentityRole();
                role.Name = "SuperAdmin";
                roleManager.Create(role);

                var user = new IdentityUser();
                user.UserName = "MainAdmin";
                var result = UserManager.Create(user, "123456");

                if (result.Succeeded)
                {
                    UserManager.AddToRole(user.Id, "SuperAdmin");
                }
            }
        }
    }
}
