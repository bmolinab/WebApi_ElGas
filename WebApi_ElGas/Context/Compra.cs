namespace WebApi_ElGas.Context
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Compra")]
    public partial class Compra
    {
        [Key]
        public int IdCompra { get; set; }

        public int? IdCliente { get; set; }

        public int? IdDistribuidor { get; set; }

        public int? Estado { get; set; }

        public double? Longitud { get; set; }

        public double? Latitud { get; set; }

        public int? Cantidad { get; set; }

        public double? ValorTotal { get; set; }

        public int? Calificacion { get; set; }

        public virtual Cliente Cliente { get; set; }

        public virtual Distribuidor Distribuidor { get; set; }
    }
}
