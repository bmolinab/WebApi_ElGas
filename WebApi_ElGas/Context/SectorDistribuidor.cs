namespace WebApi_ElGas.Context
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SectorDistribuidor")]
    public partial class SectorDistribuidor
    {
        [Key]
        public int IdSectorDistribuidor { get; set; }

        public int? IdSector { get; set; }

        public int? IdDistribuidor { get; set; }

        public virtual Distribuidor Distribuidor { get; set; }

        public virtual Sector Sector { get; set; }
    }
}
