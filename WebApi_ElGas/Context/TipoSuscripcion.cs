namespace WebApi_ElGas.Context
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TipoSuscripcion")]
    public partial class TipoSuscripcion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdTipoSuscripcion { get; set; }

        [StringLength(20)]
        public string Nombre { get; set; }

        [StringLength(100)]
        public string Descripcion { get; set; }

        public double? Precio { get; set; }

        public int? Prioridad { get; set; }
    }
}
