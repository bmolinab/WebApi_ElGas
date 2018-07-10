using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using WebApi_ElGas.Context;
using WebApi_ElGas.Hubs;
using WebApi_ElGas.Models;
using WebApi_ElGas.Utils;

namespace WebApi_ElGas.Controllers
{/// <summary>
 /// Para las compras los estados de compra seran
 /// -1 que se cancelo
 /// 0 que nadie la acepta aún
 /// 1 que ya un repartidor la está atendiendo
 /// 2 que ya se realizo la venta
 /// 
 /// </summary>

    [RoutePrefix("api/Compras")]
    public class ComprasController : ApiController
    {
        private Model1 db = new Model1();

        // GET: api/Compras
        public IQueryable<Compra> GetCompra()
        {
            return db.Compra;
        }

        // GET: api/Compras/5
        [ResponseType(typeof(Compra))]
        public IHttpActionResult GetCompra(int id)
        {
            Compra compra = db.Compra.Find(id);
            if (compra == null)
            {
                return NotFound();
            }

            return Ok(compra);
        }

        // PUT: api/Compras/5
        [Route("PutCompras")]
        [ResponseType(typeof(void))]
        public IHttpActionResult PutCompra(int id, Compra compra)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != compra.IdCompra)
            {
                return BadRequest();
            }

            db.Entry(compra).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CompraExists(id))
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

        // POST: api/Compras
        [HttpPost]
        [Route("PostCompras")]
        [ResponseType(typeof(Compra))]
        public async Task<Response> PostCompraAsync(Compra compra)
        {
            db.Configuration.ProxyCreationEnabled = false;
            db.Compra.Add(compra);           
            db.SaveChanges();
            //Aquí debe enviarse la notificacion para los vendedores cercanos
        await Notificacion(compra, 1);
            return new Response
            {
                IsSuccess = true,
                Message = "El Compra realizada con exito",
                Result = compra
            };
        }

        [HttpPost]
        [Route("MisVentasPendientes")]        
        public Response misventas(Distribuidor distribuidor)
        {
            try
            {
                if (distribuidor == null)
                {
                    return new Response {
                        IsSuccess = true,
                        Message = "El Distribuidor no existe",
                        Result = null

                    };
                }
                db.Configuration.ProxyCreationEnabled = false;
              //  var compraspendientes = db.Compra.Where(x => x.Distribuidor.IdDistribuidor == distribuidor.IdDistribuidor && x.Estado == 1).ToList();

                var compraspendientes = db.Compra.Where(x => x.IdDistribuidor == distribuidor.IdDistribuidor && x.Estado == 1).Select(x => new CompraResponse
                {
                    IdCliente = x.IdCliente,
                    IdCompra = x.IdCompra,
                    IdDistribuidor = x.IdDistribuidor,
                    Latitud = x.Latitud,
                    Longitud= x.Longitud,
                    Cantidad= x.Cantidad,
                    NombreCliente=x.Cliente.Nombres+" "+x.Cliente.Apellidos,
                    Telefono=x.Cliente.Telefono,
                    ValorTotal=x.ValorTotal,
                }
                ).ToList();

                return new Response
                { IsSuccess = true,
                Message="Estos son las ventas pendientes",
                Result= compraspendientes
                } ;
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = true,
                    Message = "Tenemos un error: "+ex.Message,
                    Result = null
                };
            }
        
        }

        [HttpPost]
        [Route("GetCompra")]
        public Response Getcompra(Compra compra)
        {
            db.Configuration.ProxyCreationEnabled = false;
            Compra compraresult = db.Compra.Find(compra.IdCompra);
            if (compraresult == null)
            {
                return new Response
                {
                    Message = "Error",
                    IsSuccess = false,
                    Result = null
                };
            }

            return new Response
            {
                Message = "Datos de compra",
                IsSuccess = true,
                Result = compraresult
            };
        }

        [HttpPost]
        [Route("Aplicar")]
        public async Task < Response> Aplicar(Compra compra)
        {
            try
            {
                if (compra == null)
                {
                    return new Response
                    {
                        IsSuccess = true,
                        Message = "El Distribuidor no existe",
                        Result = null

                    };
                }
                db.Configuration.ProxyCreationEnabled = false;
                Compra compraresult = db.Compra.Find(compra.IdCompra);
                if( compraresult.Estado==0)
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    db.Entry(compraresult).State = EntityState.Modified;
                    compraresult.Estado = 1;
                    compraresult.IdDistribuidor = compra.IdDistribuidor;

                    db.SaveChanges();

                    Compra compraresult2 = new Compra { IdCompra = compraresult.IdCompra, IdDistribuidor = compraresult.IdDistribuidor };


                    await Notificacion(compra, 2);


                    return new Response
                    {
                        IsSuccess = true,
                        Message = "Compra aplicada con exito",
                        Result = compraresult2
                    };
                }


                return new Response
                {
                    IsSuccess = false,
                    Message = "la Compra no pudo ser aplicada",
                    Result = compraresult
                };


            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = "Tenemos un error: " + ex.Message,
                    Result = null
                };
            }

        }

        [HttpPost]
        [Route("Vender")]
        public Response Vender(CompraResponse compraResponse)
        {       
            try
            {
                if (compraResponse == null)
                {
                    return new Response
                    {
                        IsSuccess = true,
                        Message = "El Distribuidor no existe",
                        Result = null

                    };
                }
                db.Configuration.ProxyCreationEnabled = false;
                Compra compra = db.Compra.Find(compraResponse.IdCompra);
                db.Entry(compra).State = EntityState.Modified;
                compra.Estado = 2;
                db.SaveChanges();
                return new Response
                {
                    IsSuccess = true,
                    Message = "Estos son las ventas pendientes",
                    Result = compra
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = true,
                    Message = "Tenemos un error: " + ex.Message,
                    Result = null
                };
            }

        }


        public async Task <bool> Notificacion(Compra compra, int tipo)
        {
            switch (tipo)
            {
                case 1:
                    try
                    {
                        db.Configuration.ProxyCreationEnabled = false;
                        var usuarios = db.Distribuidor.ToList();
                        List<Ruta> ultimapos = new List<Ruta>();
                        foreach (var item in usuarios)
                        {
                            var rutausuario = db.Ruta.Where(x => x.IdDistribuidor == item.IdDistribuidor).OrderByDescending(x => x.Fecha).ToList();
                            ultimapos.Add(rutausuario.FirstOrDefault());
                        }
                        if (ultimapos.Count > 0)
                        {
                            Posicion myposicion = new Posicion { Latitud = (double)compra.Latitud, Longitud = (double)compra.Longitud };
                            foreach (var item in ultimapos)
                            {
                                Posicion posicionVendedor = new Posicion
                                {
                                    Latitud = (double)item.Latitud,
                                    Longitud = (double)item.Longitud,
                                };

                                if (Geo.EstaCercaDeMi(myposicion, posicionVendedor, 10))
                                {
                                    Debug.WriteLine("se debe notifica a {0}", item.IdDistribuidor);
                                    List<string> tags = new List<string>();
                                    tags.Add("camion");
                                    await AzureHubUtils.SendNotificationAsync(string.Format("Un cliente desea {0} tanque(s)", compra.Cantidad), tags, item.Distribuidor.DeviceID, "1", compra.IdCompra);
                                }



                            }


                            //algoritmo para obtener los que estan cercas
                        }
                        return true;


                    }
                    catch (Exception)
                    {
                        return false;

                        throw;
                    }
                case 2:
                    try
                    {
                        db.Configuration.ProxyCreationEnabled = false;
                        var cliente = db.Cliente.Where(x => x.IdCliente == compra.IdCliente).FirstOrDefault();

                        Debug.WriteLine("se debe notifica a {0}", cliente.DeviceID);
                        List<string> tags = new List<string>();
                        tags.Add("cliente");
                        await AzureHubUtils.SendNotificationAsync("Su pedido ha sido confirmado, un distribuidor está en camino para realizar la entrega", tags, cliente.DeviceID, "1", compra.IdCompra);
                        return true;

                    }
                    catch (Exception)
                    {
                        return false;

                        throw;
                    }
                default:
                    {
                        return false;
                    }
            }
           
            
           
        }

        // DELETE: api/Compras/5

        [ResponseType(typeof(Compra))]
        public IHttpActionResult DeleteCompra(int id)
        {
            Compra compra = db.Compra.Find(id);
            if (compra == null)
            {
                return NotFound();
            }

            db.Compra.Remove(compra);
            db.SaveChanges();

            return Ok(compra);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CompraExists(int id)
        {
            return db.Compra.Count(e => e.IdCompra == id) > 0;
        }
    }
}