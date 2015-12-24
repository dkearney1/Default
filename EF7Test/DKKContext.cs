using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;

namespace EF7Test
{
    public partial class DKKContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(@"Server=.;Database=DKK;Trusted_Connection=true;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AutomatedLocation>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasDefaultValueSql("newsequentialid()")
                    .ValueGeneratedNever();

                entity.Property(e => e.CreateDate).HasDefaultValueSql("sysdatetimeoffset()");

                entity.Property(e => e.Creator)
                    .IsRequired()
                    .HasMaxLength(128)
                    .HasDefaultValueSql("suser_sname()");

                entity.Property(e => e.LastUpdater).HasMaxLength(128);

                entity.Property(e => e.LocalFolder)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.Machine)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.Property(e => e.RowVersion).HasDefaultValue(0);
            });

            modelBuilder.Entity<Client>(entity =>
            {
                entity.HasIndex(e => e.ClientId).HasName("IX_Client_ClientId").IsUnique();

                entity.Property(e => e.Id)
                    .HasDefaultValueSql("newsequentialid()")
                    .ValueGeneratedNever();

                entity.Property(e => e.ClientId)
                    .IsRequired()
                    .HasMaxLength(32);

                entity.Property(e => e.CreateDate).HasDefaultValueSql("sysdatetime()");

                entity.Property(e => e.Creator)
                    .IsRequired()
                    .HasMaxLength(128)
                    .HasDefaultValueSql("suser_sname()");

                entity.Property(e => e.IsActive).HasDefaultValue(true);

                entity.Property(e => e.LastUpdater).HasMaxLength(128);

                entity.Property(e => e.Name).HasMaxLength(64);

                entity.Property(e => e.RowVersion).HasDefaultValue(0);
            });

            modelBuilder.Entity<ClientReportingGroup>(entity =>
            {
                entity.HasIndex(e => e.ClientId).HasName("IX_ClientReportingGroup_ClientId");

                entity.HasIndex(e => e.ParentId).HasName("IX_ClientReportingGroup_ParentId");

                entity.Property(e => e.Id)
                    .HasDefaultValueSql("newsequentialid()")
                    .ValueGeneratedNever();

                entity.Property(e => e.ClientId)
                    .IsRequired()
                    .HasMaxLength(32);

                entity.Property(e => e.CreateDate).HasDefaultValueSql("sysdatetime()");

                entity.Property(e => e.Creator)
                    .IsRequired()
                    .HasMaxLength(128)
                    .HasDefaultValueSql("suser_sname()");

                entity.Property(e => e.LastUpdater).HasMaxLength(128);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(64);

                entity.Property(e => e.RowVersion).HasDefaultValue(0);

                entity.HasOne(d => d.Client).WithMany(p => p.ClientReportingGroup).HasPrincipalKey(p => p.ClientId).HasForeignKey(d => d.ClientId);

                entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent).HasForeignKey(d => d.ParentId);
            });

            modelBuilder.Entity<ClientSetup>(entity =>
            {
                entity.HasIndex(e => new { e.ClientReportingGroup, e.ClientSetupProperty, e.DateFrom, e.DateTo }).HasName("IX_ClientSetup_ClientReportingGroup_ClientSetupProperty_DateFrom_DateTo").IsUnique();

                entity.Property(e => e.Id)
                    .HasDefaultValueSql("newsequentialid()")
                    .ValueGeneratedNever();

                entity.Property(e => e.CreateDate).HasDefaultValueSql("sysdatetime()");

                entity.Property(e => e.Creator)
                    .IsRequired()
                    .HasMaxLength(128)
                    .HasDefaultValueSql("suser_sname()");

                entity.Property(e => e.DateFrom).HasColumnType("date");

                entity.Property(e => e.DateTo).HasColumnType("date");

                entity.Property(e => e.LastUpdater).HasMaxLength(128);

                entity.Property(e => e.MoneyValue).HasColumnType("money");

                entity.Property(e => e.RowVersion).HasDefaultValue(0);

                entity.Property(e => e.StringValue).HasMaxLength(64);

                entity.HasOne(d => d.ClientReportingGroupNavigation).WithMany(p => p.ClientSetup).HasForeignKey(d => d.ClientReportingGroup);

                entity.HasOne(d => d.ClientSetupPropertyNavigation).WithMany(p => p.ClientSetup).HasForeignKey(d => d.ClientSetupProperty);
            });

            modelBuilder.Entity<ClientSetupProperty>(entity =>
            {
                entity.HasIndex(e => e.Code).HasName("IX_ClientSetupProperty_Code").IsUnique();

                entity.Property(e => e.Id)
                    .HasDefaultValueSql("newsequentialid()")
                    .ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(32);

                entity.Property(e => e.CreateDate).HasDefaultValueSql("sysdatetime()");

                entity.Property(e => e.Creator)
                    .IsRequired()
                    .HasMaxLength(128)
                    .HasDefaultValueSql("suser_sname()");

                entity.Property(e => e.IsActive).HasDefaultValue(true);

                entity.Property(e => e.LastUpdater).HasMaxLength(128);

                entity.Property(e => e.RowVersion).HasDefaultValue(0);

                entity.Property(e => e.ShortDescription).HasMaxLength(64);
            });

            modelBuilder.Entity<TestUDT>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasDefaultValueSql("newsequentialid()")
                    .ValueGeneratedNever();

                entity.Property(e => e.DateFrom).HasColumnType("date");

                entity.Property(e => e.DateTo).HasColumnType("date");
            });

            modelBuilder.Entity<sysdiagrams>(entity =>
            {
                entity.HasKey(e => e.diagram_id);

                entity.Property(e => e.definition).HasColumnType("varbinary");
            });
        }

        public virtual DbSet<AutomatedLocation> AutomatedLocation { get; set; }
        public virtual DbSet<Client> Client { get; set; }
        public virtual DbSet<ClientReportingGroup> ClientReportingGroup { get; set; }
        public virtual DbSet<ClientSetup> ClientSetup { get; set; }
        public virtual DbSet<ClientSetupProperty> ClientSetupProperty { get; set; }
        public virtual DbSet<TestUDT> TestUDT { get; set; }
        public virtual DbSet<sysdiagrams> sysdiagrams { get; set; }
    }
}