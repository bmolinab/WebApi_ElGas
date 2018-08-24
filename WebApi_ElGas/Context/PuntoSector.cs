namespace WebApi_ElGas.Context
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PuntoSector")]
    public partial class PuntoSector
    {
        [Key]
        public int IdPuntoSector { get; set; }

        public double? Latitud { get; set; }

        public double? Longitud { get; set; }

        public int IdSector { get; set; }

        public virtual Sector Sector { get; set; }
    }
}
