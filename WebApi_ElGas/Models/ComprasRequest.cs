using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi_ElGas.Models
{
    public class ComprasRequest
    {
        public int IdCompra { get; set; }
        public int? IdCliente { get; set; }
        public int? IdDistribuidor { get; set; }
        public int? Estado { get; set; }

        public double? Longitud { get; set; }
        public double? Latitud { get; set; }

        public int? Cantidad { get; set; }
        public double? ValorTotal { get; set; }    
        public DateTime? FechaPedido { get; set; }
    }
}