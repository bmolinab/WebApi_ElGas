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
    [RoutePrefix("api/Clientes")]
    public class ClientesController : ApiController
    {
        private Model1 db = new Model1();

        // GET: api/Clientes
        public IQueryable<Cliente> GetCliente()
        {
            return db.Cliente;
        }

        // GET: api/Clientes/5
        [ResponseType(typeof(Cliente))]
        public IHttpActionResult GetCliente(int id)
        {
            Cliente cliente = db.Cliente.Find(id);
            if (cliente == null)
            {
                return NotFound();
            }

            return Ok(cliente);
        }

        // PUT: api/Clientes/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutCliente(int id, Cliente cliente)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != cliente.IdCliente)
            {
                return BadRequest();
            }

            db.Entry(cliente).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClienteExists(id))
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

        // POST: api/Clientes
        [Route("PostClient")]
        [ResponseType(typeof(Cliente))]
        public IHttpActionResult PostCliente(Cliente cliente)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);

            }
            cliente.FechaRegistro = DateTime.Now;

            db.Cliente.Add(cliente);
            db.SaveChanges();

            //  return CreatedAtRoute("DefaultApi", new { id = cliente.IdCliente }, cliente);
            return Ok(cliente);

        }
        [HttpPost]
        [Route("GetClientData")]
        [ResponseType(typeof(Cliente))]
        public Response GetClientData(Cliente cliente)
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                var Cliente = db.Cliente.Where(x => x.Correo == cliente.Correo).FirstOrDefault();
                if (Cliente != null)
                {
                    if (cliente.DeviceID != null)
                    {
                        Cliente.DeviceID = cliente.DeviceID;
                        db.SaveChanges();
                    }
                    return new Response
                    {
                        IsSuccess = true,
                        Message = "El cliente Existe",
                        Result = Cliente
                    };
                }

                else
                    return new Response
                    {
                        IsSuccess = true,
                        Message = "El cliente no xiste",
                        Result = null
                    };
            }
            catch (Exception ex)
            {

                return new Response
                {
                    IsSuccess = false,
                    Message = "Ocurrion un problema " + ex.Message,
                    Result = null
                };
            }

            //  return CreatedAtRoute("DefaultApi", new { id = cliente.IdCliente }, cliente);

        }

        // DELETE: api/Clientes/5
        [ResponseType(typeof(Cliente))]
        public IHttpActionResult DeleteCliente(int id)
        {
            Cliente cliente = db.Cliente.Find(id);
            if (cliente == null)
            {
                return NotFound();
            }

            db.Cliente.Remove(cliente);
            db.SaveChanges();

            return Ok(cliente);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ClienteExists(int id)
        {
            return db.Cliente.Count(e => e.IdCliente == id) > 0;
        }
    }
}