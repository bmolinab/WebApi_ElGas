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
using WebApi_ElGas.Utils;

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

        [HttpPost]
        [Route("GetDistribuidorData")]
        [ResponseType(typeof(Distribuidor))]
        public Response GetDistribuidorData(Distribuidor distribuidor)
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                
                var Distribuidor = db.Distribuidor.Where(x => x.Correo == distribuidor.Correo).FirstOrDefault();
                if (Distribuidor != null)
                {
                    if(distribuidor.DeviceID!=null)
                    {
                        Distribuidor.DeviceID = distribuidor.DeviceID;
                        db.SaveChanges();
                    }
                    return new Response
                    {
                        IsSuccess = true,
                        Message = "El distribuidor Existe",
                        Result = Distribuidor
                    };
                }
                else
                    return new Response
                    {
                        IsSuccess = true,
                        Message = "El distribuidor no xiste",
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


        /// <summary>
        /// Devuelve los distribuidores cercanos segun la posicion
        /// </summary>
        /// <param name="myPosicion"></param>
        /// <returns></returns>
        [Route("NearDistribuidor")]
        [ResponseType(typeof(Distribuidor))]
        [HttpPost]
        
        public IHttpActionResult NearDistribuidor(Posicion myPosicion)
        {
            List<DistribuidorResponse> distribuidores = new List<DistribuidorResponse>();

            db.Configuration.ProxyCreationEnabled = false;

            foreach (var item in db.Ruta.Where(x => DbFunctions.TruncateTime(x.Fecha) == DateTime.Today).ToList())
            {
                if(Geo.EstaCercaDeMi(myPosicion, new Posicion {Latitud=(Double)item.Latitud, Longitud=(Double)item.Longitud }, 10))
                {
                    var distribuidor = db.Distribuidor.Where(x => x.IdDistribuidor == item.IdDistribuidor).FirstOrDefault();
                    if (distribuidores.Count==0 )
                    {
                        distribuidores.Add(new DistribuidorResponse {

                            IdDistribuidor= distribuidor.IdDistribuidor,
                            Identificacion= distribuidor.Identificacion,
                            Latitud= item.Latitud,
                            Longitud=item.Longitud
                        });
                    }
                    else
                    {
                        foreach (var dis in distribuidores)
                        {
                            if (dis.IdDistribuidor != item.IdDistribuidor )
                            {
                                distribuidores.Add(new DistribuidorResponse
                                {

                                    IdDistribuidor = distribuidor.IdDistribuidor,
                                    Identificacion = distribuidor.Identificacion,
                                    Latitud = item.Latitud,
                                    Longitud = item.Longitud
                                });
                            }
                        }                       
                    }                    
                }
            }
           
            return Ok(distribuidores.ToList());
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