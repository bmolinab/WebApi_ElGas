using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using WebApi_ElGas.Context;
using WebApi_ElGas.Hubs;

namespace WebApi_ElGas.Controllers
{
    /// 1 que ya un repartidor la está atendiendo

    [RoutePrefix("api/Rutas")]

    public class RutasController : ApiController
    {
        private Model1 db = new Model1();

        // GET: api/Rutas
        public IQueryable<Ruta> GetRuta()
        {
            return db.Ruta;
        }

        // GET: api/Rutas/5
        [ResponseType(typeof(Ruta))]
        public IHttpActionResult GetRuta(int id)
        {
            Ruta ruta = db.Ruta.Find(id);
            if (ruta == null)
            {
                return NotFound();
            }
            return Ok(ruta);
        }



        // PUT: api/Rutas/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutRuta(int id, Ruta ruta)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != ruta.IdRuta)
            {
                return BadRequest();
            }
            db.Entry(ruta).State = EntityState.Modified;
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RutaExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Rutas
        [Route("PostRutas")]
        [HttpPost]
        [ResponseType(typeof(Ruta))]
        public async System.Threading.Tasks.Task<IHttpActionResult> PostRutaAsync(Ruta ruta)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            ruta.Fecha = DateTime.Now;
            db.Ruta.Add(ruta);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (RutaExists(ruta.IdRuta))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            db.Configuration.ProxyCreationEnabled = false;
            var compra = db.Compra.Where(x => x.IdDistribuidor == ruta.IdDistribuidor && x.Estado == 1).FirstOrDefault();
            var posactual = new Models.Posicion() { Latitud = (double)ruta.Latitud, Longitud = (double)ruta.Longitud };
            var poscompra = new Models.Posicion() { Latitud = (double)compra.Latitud, Longitud = (double)compra.Longitud };
            // esto es para saber si esta cerca y enviarle la notificacion
            if (Utils.Geo.EstaCercaDeMi(posactual, poscompra, 2))
            {
                //notificar

                var cliente = db.Cliente.Where(x => x.IdCliente == compra.IdCliente).FirstOrDefault();
                Debug.WriteLine("se debe notifica a {0}", cliente.DeviceID);
                List<string> tags = new List<string>();
                tags.Add(cliente.DeviceID);
                await AzureHubUtils.SendNotificationAsync("su pedido está a pocos metros de llegar", tags, cliente.DeviceID, "2", compra.IdCompra, compra.IdDistribuidor.ToString());

            }

            return CreatedAtRoute("DefaultApi", new { id = ruta.IdRuta }, ruta);
        }


        [Route("GetLastPosition")]
        [HttpPost]
        [ResponseType(typeof(Response))]
        public Response GetLastPosition(Distribuidor distribuidor)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var posicion = db.Ruta.Where(x => x.Distribuidor.IdDistribuidor == distribuidor.IdDistribuidor).OrderByDescending(c => c.IdRuta).FirstOrDefault();
            try
            {
                return new Response
                {
                    IsSuccess = true,
                    Message = "Ultima Ruta",
                    Result = posicion,
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = "Error: " + ex.Message,
                    Result = ex,
                };
            }
        }



        // DELETE: api/Rutas/5
        [ResponseType(typeof(Ruta))]
        public IHttpActionResult DeleteRuta(int id)
        {
            Ruta ruta = db.Ruta.Find(id);
            if (ruta == null)
            {
                return NotFound();
            }
            db.Ruta.Remove(ruta);
            db.SaveChanges();
            return Ok(ruta);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool RutaExists(int id)
        {
            return db.Ruta.Count(e => e.IdRuta == id) > 0;
        }
    }
}