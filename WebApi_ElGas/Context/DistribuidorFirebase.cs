using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi_ElGas.Context
{
    public class DistribuidorFirebase
    {
        public double Latitud { get; set; }
        public double Longitud { get; set; }
        public int id { get; set; }

    }

    public class ResultFirbase
    {
        public string name { get; set; }
    }
}