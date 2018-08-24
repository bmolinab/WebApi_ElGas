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
    [RoutePrefix("api/Parametroes")]

    public class ParametroesController : ApiController
    {
        private Model1 db = new Model1();

        // GET: api/Parametroes
        public IQueryable<Parametro> GetParametro()
        {
            return db.Parametro;
        }


        [HttpPost]
        [Route("GetAllParameters")]
        [ResponseType(typeof(Distribuidor))]
        public Response GetAllParameters(Cliente cliente)
        {
            try
            {
                var parametros = db.Parametro.ToList();

                if (parametros != null && parametros.Count > 0)
                {
                    return new Response
                    {
                        IsSuccess = true,
                        Message = "Lista de parametros El Gas",
                        Result = parametros
                    };
                }
                else
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = "Lista de parametros El Gas Vacia",
                        Result = null
                    };
                }
            }
            catch
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = "Error en la consulta",
                    Result = null
                };
            }

        }

        // GET: api/Parametroes/5
        [ResponseType(typeof(Parametro))]
        public IHttpActionResult GetParametro(int id)
        {
            Parametro parametro = db.Parametro.Find(id);
            if (parametro == null)
            {
                return NotFound();
            }

            return Ok(parametro);
        }

        // PUT: api/Parametroes/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutParametro(int id, Parametro parametro)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != parametro.IdParametro)
            {
                return BadRequest();
            }

            db.Entry(parametro).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ParametroExists(id))
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

        // POST: api/Parametroes
        [ResponseType(typeof(Parametro))]
        public IHttpActionResult PostParametro(Parametro parametro)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Parametro.Add(parametro);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = parametro.IdParametro }, parametro);
        }

        // DELETE: api/Parametroes/5
        [ResponseType(typeof(Parametro))]
        public IHttpActionResult DeleteParametro(int id)
        {
            Parametro parametro = db.Parametro.Find(id);
            if (parametro == null)
            {
                return NotFound();
            }

            db.Parametro.Remove(parametro);
            db.SaveChanges();

            return Ok(parametro);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ParametroExists(int id)
        {
            return db.Parametro.Count(e => e.IdParametro == id) > 0;
        }
    }
}