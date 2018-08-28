using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi_ElGas.Models
{
    public class ContrasenaRequest
    {
        public string Email { get; set; }

        public string Codigo { get; set; }
    }
}