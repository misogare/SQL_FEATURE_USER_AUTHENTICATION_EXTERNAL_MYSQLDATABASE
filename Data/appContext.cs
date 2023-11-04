using Microsoft.EntityFrameworkCore;


namespace App.Data
{
    public class appContext : DbContext
    {
        public appContext(DbContextOptions<appContext> options)
            : base(options)
        {
        }
        public DbSet<ERDData> ERDData { get; set; }
        public DbSet<Element> Elements { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Element>()
        .HasOne(e => e.ERDData)
        .WithMany(d => d.Elements)
        .HasForeignKey(e => e.ERDDataId);
        }
        public DbSet<CsvFileModel>? CsvFileModel { get; set; }
        public DbSet<ApplicationUser> UserLogins { get; set; }

    }
}
