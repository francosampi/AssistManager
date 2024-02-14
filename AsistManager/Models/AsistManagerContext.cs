using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

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

    public virtual DbSet<Evento> Eventos { get; set; }

    public virtual DbSet<Ingreso> Ingresos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Acreditado>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__acredita__3213E83F7B56F3CE");

            entity.ToTable("acreditado");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Apellido)
                .HasMaxLength(51)
                .HasColumnName("apellido");
            entity.Property(e => e.Celular)
                .HasMaxLength(51)
                .HasColumnName("celular");
            entity.Property(e => e.Cuit)
                .HasMaxLength(51)
                .HasColumnName("CUIT");
            entity.Property(e => e.Dni)
                .HasMaxLength(51)
                .HasColumnName("DNI");
            entity.Property(e => e.Grupo)
                .HasMaxLength(51)
                .HasColumnName("grupo");
            entity.Property(e => e.Habilitado).HasColumnName("habilitado");
            entity.Property(e => e.IdEvento).HasColumnName("id_evento");
            entity.Property(e => e.Nombre)
                .HasMaxLength(51)
                .HasColumnName("nombre");

            entity.HasOne(d => d.IdEventoNavigation).WithMany(p => p.Acreditados)
                .HasForeignKey(d => d.IdEvento)
                .HasConstraintName("FK__acreditad__id_ev__45F365D3");
        });

        modelBuilder.Entity<Evento>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__evento__3213E83F4B52FAD5");

            entity.ToTable("evento");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.FechaInicio)
                .HasColumnType("datetime")
                .HasColumnName("fecha_inicio");
            entity.Property(e => e.Nombre)
                .HasMaxLength(255)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Ingreso>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ingreso__3213E83F53D6BF73");

            entity.ToTable("ingreso");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.FechaOperacion)
                .HasColumnType("datetime")
                .HasColumnName("fecha_operacion");
            entity.Property(e => e.IdAcreditado).HasColumnName("id_acreditado");

            entity.HasOne(d => d.IdAcreditadoNavigation).WithMany(p => p.Ingresos)
                .HasForeignKey(d => d.IdAcreditado)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ingreso__id_acre__49C3F6B7");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
