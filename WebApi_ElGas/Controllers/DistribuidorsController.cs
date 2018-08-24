using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
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
        public async System.Threading.Tasks.Task<IHttpActionResult> PostDistribuidorAsync(Distribuidor distribuidor)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            db.Distribuidor.Add(distribuidor);
            db.SaveChanges();


            #region FireBase

            var FirebaseUri = "https://elgas-f24e8.firebaseio.com/-LJVkHULelfySFjNF9-Q/Equipo-ElGas/Distribuidores.json";
            var USER_AGENT = "firebase-net/1.0";

            var rutajson = new DistribuidorFirebase { id = distribuidor.IdDistribuidor, Latitud = 0, Longitud = 0 };
            var json = JsonConvert.SerializeObject(rutajson);
            var client = new HttpClient();
            var msg = new HttpRequestMessage(new HttpMethod("Post"), FirebaseUri);
            msg.Headers.Add("user-agent", USER_AGENT);
            if (json != null)
            {
                msg.Content = new StringContent(
                    json,
                    UnicodeEncoding.UTF8,
                    "application/json");
            }

            var respuesta = await client.SendAsync(msg);

            var result = await respuesta.Content.ReadAsStringAsync();
            var idFirebase = JsonConvert.DeserializeObject<ResultFirbase>(result);

            Debug.Write(idFirebase.name);

            db.Entry(distribuidor).State = EntityState.Modified;

            distribuidor.FirebaseID = idFirebase.name;

            db.SaveChanges();


            #endregion


            return Ok();
        }

        [Route("GetforUser")]
        [ResponseType(typeof(Distribuidor))]
        [HttpPost]
        public IHttpActionResult GetforUser(RegisterBindingModel registerBindingModel)
        {
            db.Configuration.ProxyCreationEnabled = false;
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
                    if (distribuidor.DeviceID != null)
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

        [HttpPost]
        [Route("GetDistribuidorID")]
        [ResponseType(typeof(Distribuidor))]
        public Response GetDistribuidorID(Distribuidor distribuidor)
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;

                var Distribuidor = db.Distribuidor.Where(x => x.IdDistribuidor == distribuidor.IdDistribuidor).FirstOrDefault();
                if (Distribuidor != null)
                {
                    if (distribuidor.DeviceID != null)
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
            try
            {

                var RutasHoy = db.Ruta.Where(x => DbFunctions.TruncateTime(x.Fecha) == DateTime.Today && x.Distribuidor.Habilitado == true).ToList();




                var result = RutasHoy.GroupBy(g => g.IdDistribuidor).ToList();

                // Loop over groups.
                foreach (var group in result)
                {
                    var value = group.OrderByDescending(x => x.Fecha).FirstOrDefault();

                    var identificacion = db.Distribuidor.Where(x => x.IdDistribuidor == value.IdDistribuidor).Select(s => s.Identificacion).FirstOrDefault();


                    distribuidores.Add(new DistribuidorResponse
                    {

                        IdDistribuidor = (int)value.IdDistribuidor,
                        Identificacion = identificacion,
                        Latitud = value.Latitud,
                        Longitud = value.Longitud
                    });
                }



                Debug.WriteLine(distribuidores.Count());





                return Ok(distribuidores.ToList());

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                throw;
            }
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