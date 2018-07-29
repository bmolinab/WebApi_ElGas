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

        public bool Habilitado { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Compra> Compra { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Ruta> Ruta { get; set; }

        public virtual TipoSuscripcion TipoSuscripcion { get; set; }

        
    }
}
