namespace WebApi_ElGas.Context
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Distribuidor")]
    public partial class Distribuidor
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Distribuidor()
        {
            Compra = new HashSet<Compra>();
            CompraCancelada = new HashSet<CompraCancelada>();
            SectorDistribuidor = new HashSet<SectorDistribuidor>();
            Ruta = new HashSet<Ruta>();
        }

        [Key]
        public int IdDistribuidor { get; set; }

        [StringLength(13)]
        public string Identificacion { get; set; }

        [StringLength(30)]
        public string Nombres { get; set; }

        [StringLength(30)]
        public string Apellidos { get; set; }

        [StringLength(20)]
        public string Telefono { get; set; }

        [StringLength(80)]
        public string Correo { get; set; }

        [StringLength(10)]
        public string PlacaVehiculo { get; set; }

        public int? Prioridad { get; set; }

        public int? IdTipoSuscripcion { get; set; }

        [StringLength(128)]
        public string IdAspNetUser { get; set; }

        [StringLength(250)]
        public string DeviceID { get; set; }

        public bool? Habilitado { get; set; }

        [StringLength(100)]
        public string FirebaseID { get; set; }

        public int? IdSector { get; set; }

        [StringLength(120)]
        public string Direccion { get; set; }

        public DateTime? FechaRegistro { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Compra> Compra { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CompraCancelada> CompraCancelada { get; set; }

        public virtual Sector Sector { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SectorDistribuidor> SectorDistribuidor { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Ruta> Ruta { get; set; }
    }
}
