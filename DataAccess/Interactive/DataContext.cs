using Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Interactive
{
  public class DataContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
  {

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //{
    //  optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=TresPlus;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False;Command Timeout=30");
    //}

    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
      base.OnModelCreating(builder);

      // -----------------------------
      // Composite Keys
      // -----------------------------
      builder.Entity<ModelTag>()
          .HasKey(x => new { x.Model3DId, x.TagId });

      builder.Entity<ProjectCollaborator>()
          .HasKey(x => new { x.ProjectId, x.UserId });

      builder.Entity<ModelLike>()
          .HasKey(x => new { x.Model3DId, x.UserId });

      builder.Entity<UserFollow>(Entity =>
      {
        Entity.HasKey(x => new { x.FollowerUserId, x.UserFollowId });
        Entity.HasOne(uf => uf.Follower)
          .WithMany(u => u.Followers)
          .HasForeignKey(uf => uf.FollowerUserId)
          .OnDelete(DeleteBehavior.Restrict);

        Entity.HasOne(uf => uf.Follower)
          .WithMany(u => u.Followers)
          .HasForeignKey(uf => uf.FollowerUserId)
          .OnDelete(DeleteBehavior.Restrict);
      });

      // -----------------------------
      // ModelTag relations
      // -----------------------------
      builder.Entity<ModelTag>()
          .HasOne(x => x.Model3D)
          .WithMany(m => m.ModelTags)
          .HasForeignKey(x => x.Model3DId);

      builder.Entity<ModelTag>()
          .HasOne(x => x.Tag)
          .WithMany(t => t.ModelTags)
          .HasForeignKey(x => x.TagId);

      // -----------------------------
      // ProjectCollaborator relations
      // -----------------------------
      builder.Entity<ProjectCollaborator>()
          .HasOne(x => x.Project)
          .WithMany(p => p.Collaborators)
          .HasForeignKey(x => x.ProjectId);

      builder.Entity<ProjectCollaborator>()
          .HasOne(x => x.User)
          .WithMany(u => u.ProjectCollaborations)
          .HasForeignKey(x => x.UserId);

      // -----------------------------
      // ModelLike relations
      // -----------------------------
      builder.Entity<ModelLike>()
          .HasOne(l => l.Model3D)
          .WithMany(m => m.Likes)
          .HasForeignKey(l => l.Model3DId);

      builder.Entity<ModelLike>()
          .HasOne(l => l.User)
          .WithMany(u => u.Likes)
          .HasForeignKey(l => l.UserId);

      // -----------------------------
      // UserFollow relations (restrict deletes)
      // -----------------------------
      builder.Entity<UserFollow>()
          .HasOne(f => f.Follower)
          .WithMany(u => u.Followers)
          .HasForeignKey(f => f.FollowerUserId)
          .OnDelete(DeleteBehavior.Restrict);

      builder.Entity<UserFollow>()
          .HasOne(f => f.Follower)
          .WithMany(u => u.Followers)
          .HasForeignKey(f => f.FollowerUserId)
          .OnDelete(DeleteBehavior.Restrict);

      // -----------------------------
      // ModelSettings one-to-one
      // -----------------------------
      builder.Entity<ModelSettings>()
          .HasOne(s => s.Model3D)
          .WithOne(m => m.Settings)
          .HasForeignKey<ModelSettings>(s => s.Model3DId);

      // -----------------------------
      // Indexes
      // -----------------------------
      builder.Entity<Model3D>()
          .HasIndex(m => new { m.IsPublic, m.UploadedAt });

      builder.Entity<Model3D>()
          .HasIndex(m => m.ProjectId);

      builder.Entity<Tag>()
          .HasIndex(t => t.Name)
          .IsUnique();

      // -----------------------------
      // Cascade rules for Model versions
      // -----------------------------
      builder.Entity<Model3D>()
          .HasMany(m => m.Versions)
          .WithOne(v => v.Model3D)
          .HasForeignKey(v => v.Model3DId)
          .OnDelete(DeleteBehavior.Cascade);

      // Tag entity güncellemesi (Glas için)
      builder.Entity<Tag>(entity =>
      {
        // Mevcut unique index korunuyor
        entity.HasIndex(t => t.Name).IsUnique();

        // Usage count index ekleniyor
        entity.HasIndex(t => t.UsageCount);
      });

      // -----------------------------
      // AppUser default values
      // -----------------------------
      builder.Entity<AppUser>()
          .Property(x => x.CreatedAt)
          .HasDefaultValueSql("CONVERT(date, GETUTCDATE())");

      builder.Entity<AppUser>()
          .Property(x => x.OnlineState)
          .HasDefaultValue(false);

      builder.Entity<Glas>()
      .Property(x => x.GlasId)
      .ValueGeneratedOnAdd()
      .HasDefaultValueSql("NEWSEQUENTIALID()");
    }



    // -----------------------------
    // DbSets
    // -----------------------------
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<ProjectCollaborator> ProjectCollaborators => Set<ProjectCollaborator>();

    public DbSet<Model3D> Models => Set<Model3D>();
    public DbSet<ModelVersion> ModelVersions => Set<ModelVersion>();
    public DbSet<ModelConvertedFile> ConvertedFiles => Set<ModelConvertedFile>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<ModelTag> ModelTags => Set<ModelTag>();
    public DbSet<ModelSettings> ModelSettings => Set<ModelSettings>();
    public DbSet<ModelShare> ModelShares => Set<ModelShare>();
    public DbSet<ModelShareLink> ModelShareLinks => Set<ModelShareLink>();

    public DbSet<ModelLike> ModelLikes => Set<ModelLike>();
    public DbSet<ModelComment> ModelComments => Set<ModelComment>();
    public DbSet<UserFollow> UserFollows => Set<UserFollow>();
    public DbSet<ModelReport> ModelReports => Set<ModelReport>();

    public DbSet<Glas> Glasses => Set<Glas>();

  }
}
