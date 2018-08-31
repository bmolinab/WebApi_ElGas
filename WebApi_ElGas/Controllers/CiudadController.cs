using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using WebApi_ElGas.Context;

namespace WebApi_ElGas.Controllers
{
    [RoutePrefix("api/Ciudades")]
    public class CiudadController : ApiController
    {
        private Model1 db = new Model1();
        /// <summary>
        ///List all cities
        /// </summary>
        /// <returns></returns>
        [Route("GetCiudades")]
        public async Task<List<Ciudad>> GetCiudad()
        {
            db.Configuration.ProxyCreationEnabled = false;
            return await db.Ciudad.ToListAsync();
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}