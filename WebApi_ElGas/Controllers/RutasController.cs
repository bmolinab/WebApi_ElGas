using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using WebApi_ElGas.Context;

namespace WebApi_ElGas.Controllers
{
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
        [ResponseType(typeof(Ruta))]
        public IHttpActionResult PostRuta(Ruta ruta)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

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

            return CreatedAtRoute("DefaultApi", new { id = ruta.IdRuta }, ruta);
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