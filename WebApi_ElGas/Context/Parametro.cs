namespace WebApi_ElGas.Context
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Parametro")]
    public partial class Parametro
    {
        [Key]
        public int IdParametro { get; set; }

        [StringLength(20)]
        public string Nombre { get; set; }

        public double? Valor { get; set; }

        [StringLength(120)]
        public string Mensaje { get; set; }
    }
}
