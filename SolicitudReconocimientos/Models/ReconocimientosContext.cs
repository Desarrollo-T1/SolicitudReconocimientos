namespace SolicitudReconocimientos.Models
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using Rec.Models;
    using System.Data.Entity.Spatial;

    public partial class ReconocimientosContext : DbContext
    {
        public ReconocimientosContext()
            : base("name=ReconocimientosContext")
        {
        }

        //public virtual DbSet<C__MigrationHistory> C__MigrationHistory { get; set; }
        public virtual DbSet<Firma> Firma { get; set; }
        public virtual DbSet<Permiso> Permiso { get; set; }
        public virtual DbSet<Quien> Quien { get; set; }
        public virtual DbSet<Reconocimientos> Reconocimientos { get; set; }
        public virtual DbSet<Rol> Rol { get; set; }
        public virtual DbSet<sysdiagrams> sysdiagrams { get; set; }
        public virtual DbSet<TipoDocumento> TipoDocumento { get; set; }
        public virtual DbSet<Unidad> Unidad { get; set; }
        public virtual DbSet<Usuario> Usuario { get; set; }
        public virtual DbSet<UsuarioReconocimiento> UsuarioReconocimiento { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Firma>()
                .Property(e => e.Nombre)
                .IsUnicode(false);

            modelBuilder.Entity<Permiso>()
                .Property(e => e.Modulo)
                .IsUnicode(false);

            modelBuilder.Entity<Permiso>()
                .Property(e => e.Descripcion)
                .IsUnicode(false);

            modelBuilder.Entity<Permiso>()
                .HasMany(e => e.Rol)
                .WithMany(e => e.Permiso)
                .Map(m => m.ToTable("PermisoDenegadoPorRol").MapLeftKey("PermisoID").MapRightKey("RolID"));

            modelBuilder.Entity<Quien>()
                .Property(e => e.Nombre)
                .IsUnicode(false);

            modelBuilder.Entity<Reconocimientos>()
                .Property(e => e.Folio)
                .IsUnicode(false);

            modelBuilder.Entity<Reconocimientos>()
                .Property(e => e.Foja)
                .IsUnicode(false);

            modelBuilder.Entity<Reconocimientos>()
                .Property(e => e.Libro)
                .IsUnicode(false);

            modelBuilder.Entity<Reconocimientos>()
                .HasMany(e => e.UsuarioReconocimiento)
                .WithRequired(e => e.Reconocimientos)
                .HasForeignKey(e => e.Reconocimiento)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Rol>()
                .Property(e => e.Nombre)
                .IsUnicode(false);

            modelBuilder.Entity<Rol>()
                .HasMany(e => e.Usuario)
                .WithRequired(e => e.Rol)
                .HasForeignKey(e => e.Rol_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TipoDocumento>()
                .HasMany(e => e.Reconocimientos)
                .WithRequired(e => e.TipoDocumento)
                .HasForeignKey(e => e.IdTipoDocumento)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Unidad>()
                .Property(e => e.Nombre)
                .IsUnicode(false);

            modelBuilder.Entity<Unidad>()
                .HasMany(e => e.Usuario)
                .WithRequired(e => e.Unidad)
                .HasForeignKey(e => e.Unidad_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Usuario>()
                .Property(e => e.NombreTipo)
                .IsUnicode(false);

            modelBuilder.Entity<Usuario>()
                .Property(e => e.UserName)
                .IsUnicode(false);

            modelBuilder.Entity<Usuario>()
                .Property(e => e.Contraseña)
                .IsUnicode(false);

            modelBuilder.Entity<Usuario>()
                .Property(e => e.Nombre)
                .IsUnicode(false);

            modelBuilder.Entity<Usuario>()
                .Property(e => e.ApellidoP)
                .IsUnicode(false);

            modelBuilder.Entity<Usuario>()
                .Property(e => e.ApellidoM)
                .IsUnicode(false);

            modelBuilder.Entity<Usuario>()
                .HasMany(e => e.UsuarioReconocimiento)
                .WithRequired(e => e.Usuario)
                .WillCascadeOnDelete(false);
        }
    }
}
