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
            compra.FechaPedido = DateTime.Now;
            db.SaveChanges();
            //Aquí debe enviarse la notificacion para los vendedores cercanos
            //  await Notificacion(compra, 1);
            await BuscarSector(compra);


            return new Response
            {
                IsSuccess = true,
                Message = "Compra realizada con exito",
                Result = compra
            };
        }
        
        // POST: api/ListCompraByClient
        [HttpPost]
        [Route("ListCompraByClient")]
        [ResponseType(typeof(Response))]
        public async Task<Response> ListCompraByClient(Cliente cliente)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var ListaCompras = await db.Compra.Where
                (w => w.Cliente.IdCliente == cliente.IdCliente)
                .Select(x => new ComprasRequest
                {
                    IdCliente = x.IdCliente,
                    IdCompra = x.IdCompra,
                    IdDistribuidor = x.IdDistribuidor,
                    Latitud = x.Latitud,
                    Longitud = x.Longitud,
                    Cantidad = x.Cantidad,
                    ValorTotal = x.ValorTotal,
                    FechaPedido= x.FechaPedido,
                    Estado= x.Estado                    
                }).ToListAsync();

            return new Response
            {
                IsSuccess = true,
                Message = "Lista de compras",
                Result = ListaCompras
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
                    return new Response
                    {
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
                    Longitud = x.Longitud,
                    Cantidad = x.Cantidad,
                    NombreCliente = x.Cliente.Nombres + " " + x.Cliente.Apellidos,
                    Telefono = x.Cliente.Telefono,
                    ValorTotal = x.ValorTotal,
                }
                ).ToList();

                return new Response
                {
                    IsSuccess = true,
                    Message = "Estos son las ventas pendientes",
                    Result = compraspendientes
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
        public async Task<Response> Aplicar(Compra compra)
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
                if (compraresult.Estado == 0)
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    db.Entry(compraresult).State = EntityState.Modified;
                    compraresult.Estado = 1;
                    compraresult.FechaAplica = DateTime.Now;

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
                compra.FechaFinalizacion = DateTime.Now;

                db.SaveChanges();
                Notificacion(compra, 3);
                Compra compraresult2 = new Compra { IdCompra = compra.IdCompra, IdDistribuidor = compra.IdDistribuidor };

                return new Response
                {
                    IsSuccess = true,
                    Message = "Venta realizada con exito",
                    Result = compraresult2
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

        [HttpPost]
        [Route("Cancelar")]
        public async Task<Response> Cancelar(CompraCancelada compra)
        {
            if (compra == null)
            {
                return new Response
                {
                    IsSuccess = true,
                    Message = "no existe",
                    Result = null

                };
            }

            db.Configuration.ProxyCreationEnabled = false;
            Compra compraresult = db.Compra.Find(compra.IdCompra);
            compra.Fecha = DateTime.Now;


            switch (compra.CanceladaPor)
            {
                //si es el usuario el que desea cancelar el pedido
                case 1:
                    //Modifica la base y se agrega al registro de comprascancelada
                    if (compraresult.Estado != -1)
                    {
                        db.Configuration.ProxyCreationEnabled = false;
                        db.Entry(compraresult).State = EntityState.Modified;
                        compraresult.Estado = -1;
                        compraresult.IdDistribuidor = compra.IdDistribuidor;
                        db.CompraCancelada.Add(compra);
                        db.SaveChanges();
                        Compra compraresult2 = new Compra { IdCompra = compraresult.IdCompra, IdDistribuidor = compraresult.IdDistribuidor };
                      
                        //Notificar al distribuidor que la compra se cancelo
                       if(compraresult.IdDistribuidor!=null&&compraresult.IdDistribuidor>0)
                        {
                            await Notificacion(compraresult, 4);
                        }


                        return new Response
                        {
                            IsSuccess = true,
                            Message = "Compra cancelada con exito",
                            Result = compraresult2
                        };
                    }
                    return new Response
                    {
                        IsSuccess = false,
                        Message = "la compra fue cancelada antes",
                        Result = null
                    };

                //si es el distribuidor el que desea cancelar el pedido

                case 2:
                    try
                    {


                        db.Configuration.ProxyCreationEnabled = false;
                        db.Entry(compraresult).State = EntityState.Modified;
                        compraresult.Estado = 0;
                        compraresult.IdDistribuidor = compra.IdDistribuidor;

                        db.CompraCancelada.Add(compra);

                        db.SaveChanges();

                        await Notificacion(compraresult, 5);

                        await BuscarSector(compraresult);

                        Compra compraresult3 = new Compra { IdCompra = compraresult.IdCompra, IdDistribuidor = compraresult.IdDistribuidor };

                        return new Response
                        {
                            IsSuccess = true,
                            Message = "Compra cancelada con exito",
                            Result = compraresult3
                        };

                    }
                    catch (Exception ex)
                    {
                        Debug.Write(ex.Message);
                        return new Response
                        {
                            IsSuccess = false,
                            Message = "la compra no se pudo cancelar",
                            Result = ex.Message
                        };
                    }
                //Modifica la base y se agrega al registro de comprascancelada
                //Notificar al usuario que la compra se cancelo y que se buscara otro Distribuidor
                //Notificar a los distribuidores excepto el anterior
                default:
                    return new Response
                    {
                        IsSuccess = false,
                        Message = "llamada erronea",
                        Result = null
                    };
            }


        }

        [HttpPost]
        [Route("Calificar")]
        public async Task<Response> Calificar(Compra compra)
        {
            try
            {
                if (compra == null)
                {
                    return new Response
                    {
                        IsSuccess = true,
                        Message = "no existe",
                        Result = null

                    };
                }
                db.Configuration.ProxyCreationEnabled = false;
                Compra compraresult = db.Compra.Find(compra.IdCompra);
                if (compraresult.Estado != -1)
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    db.Entry(compraresult).State = EntityState.Modified;
                    compraresult.Calificacion = compra.Calificacion;
                    compraresult.IdDistribuidor = compra.IdDistribuidor;

                    db.SaveChanges();

                    Compra compraresult2 = new Compra { IdCompra = compraresult.IdCompra, IdDistribuidor = compraresult.IdDistribuidor };


                    // await Notificacion(compra, 2);


                    return new Response
                    {
                        IsSuccess = true,
                        Message = "Compra calificada con exito",
                        Result = compraresult2
                    };
                }


                return new Response
                {
                    IsSuccess = false,
                    Message = "la calificacion no pudo ser aplicada",
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
        /// <summary>
        /// Método para la busqueda del sector idoneo de la compra
        /// </summary>
        /// <param name="compra"></param>
        public async Task BuscarSector(Compra compra)
        {
            var MiPosicion = new Posicion { Latitud = (double)compra.Latitud, Longitud = (double)compra.Longitud };

            var Sectores = db.Sector.ToList();

            if (Sectores != null && Sectores.Count > 0)
            {
                foreach (var sector in Sectores)
                {
                    try
                    {
                        var puntosSector = db.PuntoSector.Where(w => w.IdSector == sector.IdSector).Select(x => new Posicion
                        { Latitud = (double)x.Latitud, Longitud = (double)x.Longitud }).ToList();



                        if (puntosSector != null && puntosSector.Count > 0)
                        {
                            var Poligono = new System.Collections.ObjectModel.ObservableCollection<Posicion>(puntosSector);
                            if (Geo.IsPointInPolygonV2(Poligono, MiPosicion))
                            {
                                //Retornamos un objeto tipo response con el sector de la compra
                                DistribuidoresPorSector(sector.IdSector, compra);
                            }
                            else
                            {

                            }
                        }
                        else
                        {

                        }

                    }
                    catch (Exception ex)
                    {
                        Debug.Write(ex.Message);
                    }
                }
                //Buscar el sector más cercano
                await SectorCercano(Sectores, MiPosicion, compra);
            }
        }

        public async void DistribuidoresPorSector(int IdSector, Compra compra)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var SectorDistribuidor = db.SectorDistribuidor.Where(sd => sd.IdSector == IdSector).ToList();
            List<Distribuidor> Distribuidores = new List<Distribuidor>();
            foreach (var item in SectorDistribuidor)
            {
                Distribuidores.Add(db.Distribuidor.Where(w => w.IdDistribuidor == item.IdDistribuidor).FirstOrDefault());
            }

            await Notificar(Distribuidores, compra);

        }

        public async Task SectorCercano(List<Sector> Sectores, Posicion MiPosicion, Compra compra)
        {
            try
            {
                List<SectorDistancia> sectorDistancias = new List<SectorDistancia>();

                foreach (var item in Sectores)
                {
                    var puntosSector = db.PuntoSector.Where(w => w.IdSector == item.IdSector).Select(x => new Posicion
                    { Latitud = (double)x.Latitud, Longitud = (double)x.Longitud }).ToList();

                    foreach (var punto in puntosSector)
                    {
                        var distancia = Geo.Distancia(MiPosicion.Latitud, MiPosicion.Longitud, (double)punto.Latitud, (double)punto.Longitud);
                        var idSector = item.IdSector;
                        sectorDistancias.Add(new SectorDistancia { Distancia = distancia, IdSector = idSector });
                    }
                }
                var DistanciaMenor = sectorDistancias.OrderBy(o => o.Distancia).FirstOrDefault();


                Console.WriteLine(sectorDistancias.Min(x => x.Distancia));

                DistribuidoresPorSector(DistanciaMenor.IdSector, compra);

            }
            catch (Exception ex)
            {
                Debug.Write(ex.Message);
                throw;
            }

        }
        public async Task<bool> Notificar(List<Distribuidor> distribuidors, Compra compra)
        {
            try
            {


                //Posicion myposicion = new Posicion { Latitud = (double)compra.Latitud, Longitud = (double)compra.Longitud };

                foreach (var item in distribuidors)
                {

                    Debug.WriteLine("se debe notifica a {0}", item.IdDistribuidor);
                    List<string> tags = new List<string>();
                    tags.Add(item.DeviceID);

                    await AzureHubUtils.SendNotificationAsync(string.Format("Un cliente desea {0} tanque(s)", compra.Cantidad), tags, item.DeviceID, "1", compra.IdCompra, item.IdDistribuidor.ToString());
                }
                //algoritmo para obtener los que estan cercas

                return true;


            }
            catch (Exception ex)
            {
                Debug.Write(ex.Message);
                return false;

                throw;
            }


        }
        /// <summary>
        /// Metodo para notificar segun la 
        /// </summary>
        /// <param name="compra"></param>
        /// <param name="tipo"></param>
        /// <returns></returns>
        public async Task<bool> Notificacion(Compra compra, int tipo)
        {
            switch (tipo)
            {
                case 1:
                    try
                    {
                        db.Configuration.ProxyCreationEnabled = false;
                        var usuarios = db.Distribuidor.Where(x => x.Habilitado == true).ToList();
                        List<Ruta> ultimapos = new List<Ruta>();
                        foreach (var item in usuarios)
                        {
                            var rutausuario = db.Ruta.Where(x => x.IdDistribuidor == item.IdDistribuidor).OrderByDescending(x => x.Fecha).ToList();
                            ultimapos.Add(rutausuario.FirstOrDefault());
                        }
                        if (ultimapos.Count > 0)
                        {
                            //Posicion myposicion = new Posicion { Latitud = (double)compra.Latitud, Longitud = (double)compra.Longitud };

                            foreach (var item in ultimapos)
                            {
                                //Posicion posicionVendedor = new Posicion
                                //{
                                //    Latitud = (double)item.Latitud,
                                //    Longitud = (double)item.Longitud,
                                //};

                                //if (Geo.EstaCercaDeMi(myposicion, posicionVendedor, 10))
                                //{
                                Debug.WriteLine("se debe notifica a {0}", item.IdDistribuidor);
                                List<string> tags = new List<string>();
                                tags.Add(item.Distribuidor.DeviceID);

                                await AzureHubUtils.SendNotificationAsync(string.Format("Un cliente desea {0} tanque(s)", compra.Cantidad), tags, item.Distribuidor.DeviceID, "1", compra.IdCompra, item.IdDistribuidor.Value.ToString());
                                //  }




                            }


                            //algoritmo para obtener los que estan cercas
                        }
                        return true;


                    }
                    catch (Exception ex)
                    {
                        Debug.Write(ex.Message);
                        return false;

                        throw;
                    }
                case 2:
                    try
                    {
                        db.Configuration.ProxyCreationEnabled = false;
                        var cliente = db.Cliente.Where(x => x.IdCliente == compra.IdCliente).FirstOrDefault();
                        Debug.WriteLine(string.Format("se debe notifica a {0}", cliente.DeviceID));
                        List<string> tags = new List<string>();
                        tags.Add(cliente.DeviceID);
                        await AzureHubUtils.SendNotificationAsync("Su pedido ha sido confirmado, un distribuidor está en camino para realizar la entrega", tags, cliente.DeviceID, "1", compra.IdCompra, compra.IdDistribuidor.ToString());
                        return true;

                    }
                    catch (Exception)
                    {
                        return false;

                        throw;
                    }
                case 3:
                    try
                    {
                        db.Configuration.ProxyCreationEnabled = false;
                        var cliente = db.Cliente.Where(x => x.IdCliente == compra.IdCliente).FirstOrDefault();

                        Debug.WriteLine("se debe notifica a {0}", cliente.DeviceID);
                        List<string> tags = new List<string>();
                        tags.Add(cliente.DeviceID);
                        await AzureHubUtils.SendNotificationAsync("Gracias por  confiar en nosotros, Por favor califica el servicio brindado", tags, cliente.DeviceID, "3", compra.IdCompra, compra.IdDistribuidor.ToString());
                        return true;

                    }
                    catch (Exception)
                    {
                        return false;

                        throw;
                    }
                // notificar cancelación de compra por parte del cliente
                case 4:
                    try
                    {
                        var Distribuidor = db.Distribuidor.Where(x => x.IdDistribuidor == compra.IdDistribuidor).FirstOrDefault();

                        Debug.WriteLine("se debe notifica a {0}", Distribuidor.DeviceID);
                        List<string> tags = new List<string>();
                        tags.Add(Distribuidor.DeviceID);
                        await AzureHubUtils.SendNotificationAsync("El cliente cancelo el pedido ", tags, Distribuidor.DeviceID, "2", compra.IdCompra, compra.IdDistribuidor.ToString());

                        return true;

                    }
                    catch (Exception)
                    {
                        return false;

                        throw;
                    }
                // notificar cancelación de compra por parte del Distribuidor
                case 5:
                    try
                    {
                        db.Configuration.ProxyCreationEnabled = false;
                        var cliente = db.Cliente.Where(x => x.IdCliente == compra.IdCliente).FirstOrDefault();

                        Debug.WriteLine("se debe notifica a {0}", cliente.DeviceID);
                        List<string> tags = new List<string>();
                        tags.Add(cliente.DeviceID);
                        await AzureHubUtils.SendNotificationAsync("El distribuidor a cancelado su pedido, estamos buscando un nuevo distribuidor para atender tu pedido.", tags, cliente.DeviceID, "5", compra.IdCompra, compra.IdDistribuidor.ToString());
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