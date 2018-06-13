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
using WebApi_ElGas.Models;

namespace WebApi_ElGas.Controllers
{
    [RoutePrefix("api/Distribuidors")]

    public class DistribuidorsController : ApiController
    {
        private Model1 db = new Model1();

        // GET: api/Distribuidors
        public IQueryable<Distribuidor> GetDistribuidor()
        {
            return db.Distribuidor;
        }

        // GET: api/Distribuidors/5
        [ResponseType(typeof(Distribuidor))]
        public IHttpActionResult GetDistribuidor(int id)
        {
            Distribuidor distribuidor = db.Distribuidor.Find(id);
            if (distribuidor == null)
            {
                return NotFound();
            }

            return Ok(distribuidor);
        }



        // PUT: api/Distribuidors/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutDistribuidor(int id, Distribuidor distribuidor)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != distribuidor.IdDistribuidor)
            {
                return BadRequest();
            }

            db.Entry(distribuidor).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DistribuidorExists(id))
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

        // POST: api/Distribuidors
        [Route("PostDistribuidor")]
        [ResponseType(typeof(Distribuidor))]
        public IHttpActionResult PostDistribuidor(Distribuidor distribuidor)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Distribuidor.Add(distribuidor);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = distribuidor.IdDistribuidor }, distribuidor);
        }

        [Route("GetforUser")]
        [ResponseType(typeof(Distribuidor))]
        [HttpPost]
        public IHttpActionResult GetforUser(RegisterBindingModel registerBindingModel)
        {
            Distribuidor distribuidor = db.Distribuidor.Where(x => x.Correo == registerBindingModel.Email).FirstOrDefault();
            if (distribuidor == null)
            {
                return NotFound();
            }
            return Ok(distribuidor);
        }

        // DELETE: api/Distribuidors/5
        [ResponseType(typeof(Distribuidor))]
        public IHttpActionResult DeleteDistribuidor(int id)
        {
            Distribuidor distribuidor = db.Distribuidor.Find(id);
            if (distribuidor == null)
            {
                return NotFound();
            }

            db.Distribuidor.Remove(distribuidor);
            db.SaveChanges();

            return Ok(distribuidor);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool DistribuidorExists(int id)
        {
            return db.Distribuidor.Count(e => e.IdDistribuidor == id) > 0;
        }
    }
}