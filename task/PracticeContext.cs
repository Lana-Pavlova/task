using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace task;

public partial class PracticeContext : DbContext
{
    public PracticeContext()
    {
    }

    public PracticeContext(DbContextOptions<PracticeContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Agent> Agents { get; set; }

    public virtual DbSet<Agentpriorityhistory> Agentpriorityhistories { get; set; }

    public virtual DbSet<Agenttype> Agenttypes { get; set; }

    public virtual DbSet<Material> Materials { get; set; }

    public virtual DbSet<Materialcounthistory> Materialcounthistories { get; set; }

    public virtual DbSet<Materialtype> Materialtypes { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Productcosthistory> Productcosthistories { get; set; }

    public virtual DbSet<Productmaterial> Productmaterials { get; set; }

    public virtual DbSet<Productsale> Productsales { get; set; }

    public virtual DbSet<Producttype> Producttypes { get; set; }

    public virtual DbSet<Shop> Shops { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=127.0.0.1;persist security info=False;user=root;password=70737;database=practice", Microsoft.EntityFrameworkCore.ServerVersion.Parse("9.3.0-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Agent>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("agent");

            entity.HasIndex(e => e.AgentTypeId, "FK_Agent_AgentType");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Address).HasMaxLength(300);
            entity.Property(e => e.AgentTypeId).HasColumnName("AgentTypeID");
            entity.Property(e => e.DirectorName).HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.Inn)
                .HasMaxLength(12)
                .HasColumnName("INN");
            entity.Property(e => e.Kpp)
                .HasMaxLength(9)
                .HasColumnName("KPP");
            entity.Property(e => e.Logo).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Title).HasMaxLength(150);

            entity.HasOne(d => d.AgentType).WithMany(p => p.Agents)
                .HasForeignKey(d => d.AgentTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Agent_AgentType");
        });

        modelBuilder.Entity<Agentpriorityhistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("agentpriorityhistory");

            entity.HasIndex(e => e.AgentId, "FK_AgentPriorityHistory_Agent");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AgentId).HasColumnName("AgentID");
            entity.Property(e => e.ChangeDate).HasMaxLength(6);

            entity.HasOne(d => d.Agent).WithMany(p => p.Agentpriorityhistories)
                .HasForeignKey(d => d.AgentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AgentPriorityHistory_Agent");
        });

        modelBuilder.Entity<Agenttype>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("agenttype");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Image).HasMaxLength(100);
            entity.Property(e => e.Title).HasMaxLength(50);
        });

        modelBuilder.Entity<Material>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("material");

            entity.HasIndex(e => e.MaterialTypeId, "FK_Material_MaterialType");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Cost).HasPrecision(10, 2);
            entity.Property(e => e.Image).HasMaxLength(100);
            entity.Property(e => e.MaterialTypeId).HasColumnName("MaterialTypeID");
            entity.Property(e => e.Title).HasMaxLength(100);
            entity.Property(e => e.Unit).HasMaxLength(10);

            entity.HasOne(d => d.MaterialType).WithMany(p => p.Materials)
                .HasForeignKey(d => d.MaterialTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Material_MaterialType");

            entity.HasMany(d => d.Suppliers).WithMany(p => p.Materials)
                .UsingEntity<Dictionary<string, object>>(
                    "Materialsupplier",
                    r => r.HasOne<Supplier>().WithMany()
                        .HasForeignKey("SupplierId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_MaterialSupplier_Supplier"),
                    l => l.HasOne<Material>().WithMany()
                        .HasForeignKey("MaterialId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_MaterialSupplier_Material"),
                    j =>
                    {
                        j.HasKey("MaterialId", "SupplierId")
                            .HasName("PRIMARY")
                            .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });
                        j.ToTable("materialsupplier");
                        j.HasIndex(new[] { "SupplierId" }, "FK_MaterialSupplier_Supplier");
                        j.IndexerProperty<int>("MaterialId").HasColumnName("MaterialID");
                        j.IndexerProperty<int>("SupplierId").HasColumnName("SupplierID");
                    });
        });

        modelBuilder.Entity<Materialcounthistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("materialcounthistory");

            entity.HasIndex(e => e.MaterialId, "FK_MaterialCountHistory_Material");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ChangeDate).HasMaxLength(6);
            entity.Property(e => e.MaterialId).HasColumnName("MaterialID");

            entity.HasOne(d => d.Material).WithMany(p => p.Materialcounthistories)
                .HasForeignKey(d => d.MaterialId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MaterialCountHistory_Material");
        });

        modelBuilder.Entity<Materialtype>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("materialtype");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Title).HasMaxLength(50);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("product");

            entity.HasIndex(e => e.ProductTypeId, "FK_Product_ProductType");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ArticleNumber).HasMaxLength(10);
            entity.Property(e => e.Image).HasMaxLength(100);
            entity.Property(e => e.MinCostForAgent).HasPrecision(10, 2);
            entity.Property(e => e.ProductTypeId).HasColumnName("ProductTypeID");
            entity.Property(e => e.Title).HasMaxLength(100);

            entity.HasOne(d => d.ProductType).WithMany(p => p.Products)
                .HasForeignKey(d => d.ProductTypeId)
                .HasConstraintName("FK_Product_ProductType");
        });

        modelBuilder.Entity<Productcosthistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("productcosthistory");

            entity.HasIndex(e => e.ProductId, "FK_ProductCostHistory_Product");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ChangeDate).HasMaxLength(6);
            entity.Property(e => e.CostValue).HasPrecision(10, 2);
            entity.Property(e => e.ProductId).HasColumnName("ProductID");

            entity.HasOne(d => d.Product).WithMany(p => p.Productcosthistories)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductCostHistory_Product");
        });

        modelBuilder.Entity<Productmaterial>(entity =>
        {
            entity.HasKey(e => new { e.ProductId, e.MaterialId })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("productmaterial");

            entity.HasIndex(e => e.MaterialId, "FK_ProductMaterial_Material");

            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.MaterialId).HasColumnName("MaterialID");

            entity.HasOne(d => d.Material).WithMany(p => p.Productmaterials)
                .HasForeignKey(d => d.MaterialId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductMaterial_Material");

            entity.HasOne(d => d.Product).WithMany(p => p.Productmaterials)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductMaterial_Product");
        });

        modelBuilder.Entity<Productsale>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("productsale");

            entity.HasIndex(e => e.AgentId, "FK_ProductSale_Agent");

            entity.HasIndex(e => e.ProductId, "FK_ProductSale_Product");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AgentId).HasColumnName("AgentID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");

            entity.HasOne(d => d.Agent).WithMany(p => p.Productsales)
                .HasForeignKey(d => d.AgentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductSale_Agent");

            entity.HasOne(d => d.Product).WithMany(p => p.Productsales)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductSale_Product");
        });

        modelBuilder.Entity<Producttype>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("producttype");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Title).HasMaxLength(50);
        });

        modelBuilder.Entity<Shop>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("shop");

            entity.HasIndex(e => e.AgentId, "FK_Shop_Agent");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Address).HasMaxLength(300);
            entity.Property(e => e.AgentId).HasColumnName("AgentID");
            entity.Property(e => e.Title).HasMaxLength(150);

            entity.HasOne(d => d.Agent).WithMany(p => p.Shops)
                .HasForeignKey(d => d.AgentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Shop_Agent");
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("supplier");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Inn)
                .HasMaxLength(12)
                .HasColumnName("INN");
            entity.Property(e => e.SupplierType).HasMaxLength(20);
            entity.Property(e => e.Title).HasMaxLength(150);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
