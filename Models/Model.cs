namespace HeedeMoestrup.Models
{
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Linq;

    public class ModelContext : IdentityDbContext<IdentityUser>
    {
        // Your context has been configured to use a 'Model' connection string from your application's 
        // configuration file (App.config or Web.config). By default, this connection string targets the 
        // 'HeedeMoestrup.Models.Model' database on your LocalDb instance. 
        // 
        // If you wish to target a different database and/or database provider, modify the 'Model' 
        // connection string in the application configuration file.
        public ModelContext()
            : base("name=Model")
        {
        }

        // Add a DbSet for each entity type that you want to include in your model. For more information 
        // on configuring and using a Code First model, see http://go.microsoft.com/fwlink/?LinkId=390109.

        // public virtual DbSet<MyEntity> MyEntities { get; set; }


        public virtual DbSet<PageHeader> PageHeaders { get; set; }
        public virtual DbSet<Artist> Artists { get; set; }
        public virtual DbSet<FilePath> FilePaths { get; set; }
        public virtual DbSet<Painting> Paintings { get; set; }
        public virtual DbSet<PaintingCat> PaintingCats { get; set; }
        public virtual DbSet<NewsArticle> NewsArticle { get; set; }
    }
}