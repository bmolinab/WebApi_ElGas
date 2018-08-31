namespace WebApi_ElGas.Context
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Cliente")]
    public partial class Cliente
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Cliente()
        {
            CompraCancelada = new HashSet<CompraCancelada>();
            Compra = new HashSet<Compra>();
        }

        [Key]
        public int IdCliente { get; set; }

        [StringLength(13)]
        public string Identificacion { get; set; }

        [StringLength(30)]
        public string Nombres { get; set; }

        [StringLength(30)]
        public string Apellidos { get; set; }

        [StringLength(120)]
        public string Direccion { get; set; }

        public double? Longitud { get; set; }

        public double? Latitud { get; set; }

        [StringLength(20)]
        public string Telefono { get; set; }

        [StringLength(80)]
        public string Correo { get; set; }

        [StringLength(128)]
        public string IdAspNetUser { get; set; }

        [StringLength(250)]
        public string DeviceID { get; set; }

        public bool? Habilitado { get; set; }

        public DateTime? FechaRegistro { get; set; }

        public int? IdSector { get; set; }

        public virtual Sector Sector { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CompraCancelada> CompraCancelada { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Compra> Compra { get; set; }
    }
}
