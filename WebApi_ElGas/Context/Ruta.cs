namespace WebApi_ElGas.Context
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Ruta")]
    public partial class Ruta
    {
        [Key]
        public int IdRuta { get; set; }
        public int? IdDistribuidor { get; set; }
        public double? Longitud { get; set; }
        public double? Latitud { get; set; }

        public DateTime Fecha { get; set; }
        public virtual Distribuidor Distribuidor { get; set; }
    }
}
