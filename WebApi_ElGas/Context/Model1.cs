namespace WebApi_ElGas.Context
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class Model1 : DbContext
    {
        public Model1()
            : base("name=Model1")
        {
        }

        public virtual DbSet<Cliente> Cliente { get; set; }
        public virtual DbSet<Compra> Compra { get; set; }
        public virtual DbSet<CompraCancelada> CompraCancelada { get; set; }
        public virtual DbSet<Distribuidor> Distribuidor { get; set; }
        public virtual DbSet<Parametro> Parametro { get; set; }
        public virtual DbSet<PuntoSector> PuntoSector { get; set; }
        public virtual DbSet<Ruta> Ruta { get; set; }
        public virtual DbSet<Sector> Sector { get; set; }
        public virtual DbSet<SectorDistribuidor> SectorDistribuidor { get; set; }
        public virtual DbSet<sysdiagrams> sysdiagrams { get; set; }
        public virtual DbSet<TipoSuscripcion> TipoSuscripcion { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cliente>()
                .Property(e => e.Identificacion)
                .IsUnicode(false);

            modelBuilder.Entity<Cliente>()
                .Property(e => e.Nombres)
                .IsUnicode(false);

            modelBuilder.Entity<Cliente>()
                .Property(e => e.Apellidos)
                .IsUnicode(false);

            modelBuilder.Entity<Cliente>()
                .Property(e => e.Direccion)
                .IsUnicode(false);

            modelBuilder.Entity<Cliente>()
                .Property(e => e.Telefono)
                .IsUnicode(false);

            modelBuilder.Entity<Cliente>()
                .Property(e => e.Correo)
                .IsUnicode(false);

            modelBuilder.Entity<Cliente>()
                .Property(e => e.DeviceID)
                .IsUnicode(false);

            modelBuilder.Entity<Cliente>()
                .HasMany(e => e.CompraCancelada)
                .WithRequired(e => e.Cliente)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Compra>()
                .HasMany(e => e.CompraCancelada)
                .WithRequired(e => e.Compra)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Distribuidor>()
                .Property(e => e.Identificacion)
                .IsUnicode(false);

            modelBuilder.Entity<Distribuidor>()
                .Property(e => e.Nombres)
                .IsUnicode(false);

            modelBuilder.Entity<Distribuidor>()
                .Property(e => e.Apellidos)
                .IsUnicode(false);

            modelBuilder.Entity<Distribuidor>()
                .Property(e => e.Telefono)
                .IsUnicode(false);

            modelBuilder.Entity<Distribuidor>()
                .Property(e => e.Correo)
                .IsUnicode(false);

            modelBuilder.Entity<Distribuidor>()
                .Property(e => e.PlacaVehiculo)
                .IsUnicode(false);

            modelBuilder.Entity<Distribuidor>()
                .Property(e => e.DeviceID)
                .IsUnicode(false);

            modelBuilder.Entity<Distribuidor>()
                .HasMany(e => e.CompraCancelada)
                .WithRequired(e => e.Distribuidor)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Parametro>()
                .Property(e => e.Nombre)
                .IsUnicode(false);

            modelBuilder.Entity<Parametro>()
                .Property(e => e.Mensaje)
                .IsUnicode(false);

            modelBuilder.Entity<Sector>()
                .Property(e => e.Nombre)
                .IsUnicode(false);

            modelBuilder.Entity<Sector>()
                .HasMany(e => e.PuntoSector)
                .WithRequired(e => e.Sector)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TipoSuscripcion>()
                .Property(e => e.Nombre)
                .IsUnicode(false);

            modelBuilder.Entity<TipoSuscripcion>()
                .Property(e => e.Descripcion)
                .IsUnicode(false);
        }
    }
}
