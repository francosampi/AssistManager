using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace AsistManager.Models;

public partial class AsistManagerContext : DbContext
{
    public AsistManagerContext()
    {
    }

    public AsistManagerContext(DbContextOptions<AsistManagerContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Acreditado> Acreditados { get; set; }

    public virtual DbSet<Egreso> Egresos { get; set; }

    public virtual DbSet<Evento> Eventos { get; set; }

    public virtual DbSet<Ingreso> Ingresos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_general_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Acreditado>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("acreditado");

            entity.HasIndex(e => e.IdEvento, "id_evento");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.Alta)
                .HasColumnType("bit(1)")
                .HasColumnName("alta");
            entity.Property(e => e.Apellido)
                .HasMaxLength(51)
                .HasColumnName("apellido")
                .UseCollation("utf8_general_ci")
                .HasCharSet("utf8");
            entity.Property(e => e.Celular)
                .HasMaxLength(51)
                .HasColumnName("celular")
                .UseCollation("utf8_general_ci")
                .HasCharSet("utf8");
            entity.Property(e => e.Cuit)
                .HasMaxLength(51)
                .HasColumnName("CUIT")
                .UseCollation("utf8_general_ci")
                .HasCharSet("utf8");
            entity.Property(e => e.Dni)
                .HasMaxLength(51)
                .HasColumnName("DNI")
                .UseCollation("utf8_general_ci")
                .HasCharSet("utf8");
            entity.Property(e => e.Grupo)
                .HasMaxLength(51)
                .HasColumnName("grupo")
                .UseCollation("utf8_general_ci")
                .HasCharSet("utf8");
            entity.Property(e => e.Habilitado)
                .HasColumnType("bit(1)")
                .HasColumnName("habilitado");
            entity.Property(e => e.IdEvento)
                .HasColumnType("int(11)")
                .HasColumnName("id_evento");
            entity.Property(e => e.Nombre)
                .HasMaxLength(51)
                .HasColumnName("nombre")
                .UseCollation("utf8_general_ci")
                .HasCharSet("utf8");

            entity.HasOne(d => d.IdEventoNavigation).WithMany(p => p.Acreditados)
                .HasForeignKey(d => d.IdEvento)
                .HasConstraintName("acreditado_ibfk_1");
        });

        modelBuilder.Entity<Egreso>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("egreso");

            entity.HasIndex(e => e.IdAcreditado, "id_acreditado");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.FechaOperacion)
                .HasColumnType("datetime")
                .HasColumnName("fecha_operacion");
            entity.Property(e => e.IdAcreditado)
                .HasColumnType("int(11)")
                .HasColumnName("id_acreditado");

            entity.HasOne(d => d.IdAcreditadoNavigation).WithMany(p => p.Egresos)
                .HasForeignKey(d => d.IdAcreditado)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("egreso_ibfk_1");
        });

        modelBuilder.Entity<Evento>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("evento");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.FechaInicio)
                .HasColumnType("datetime")
                .HasColumnName("fecha_inicio");
            entity.Property(e => e.Nombre)
                .HasMaxLength(255)
                .HasColumnName("nombre")
                .UseCollation("utf8_general_ci")
                .HasCharSet("utf8");
        });

        modelBuilder.Entity<Ingreso>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("ingreso");

            entity.HasIndex(e => e.IdAcreditado, "id_acreditado");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.FechaOperacion)
                .HasColumnType("datetime")
                .HasColumnName("fecha_operacion");
            entity.Property(e => e.IdAcreditado)
                .HasColumnType("int(11)")
                .HasColumnName("id_acreditado");

            entity.HasOne(d => d.IdAcreditadoNavigation).WithMany(p => p.Ingresos)
                .HasForeignKey(d => d.IdAcreditado)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ingreso_ibfk_1");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
