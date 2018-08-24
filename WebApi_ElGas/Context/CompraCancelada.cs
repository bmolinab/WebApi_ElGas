namespace WebApi_ElGas.Context
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CompraCancelada")]
    public partial class CompraCancelada
    {
        [Key]
        public int IdCompraCancelada { get; set; }

        public int IdCompra { get; set; }

        public int IdCliente { get; set; }

        public int IdDistribuidor { get; set; }

        public DateTime? Fecha { get; set; }

        public int CanceladaPor { get; set; }

        public virtual Cliente Cliente { get; set; }

        public virtual Compra Compra { get; set; }

        public virtual Distribuidor Distribuidor { get; set; }
    }
}
