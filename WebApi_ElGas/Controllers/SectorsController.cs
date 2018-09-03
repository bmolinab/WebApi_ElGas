using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using WebApi_ElGas.Context;
using WebApi_ElGas.Models;

namespace WebApi_ElGas.Controllers
{
    /// <summary>
    /// Controller API for sectors 
    /// </summary>
    [RoutePrefix("api/Sectores")]
    public class SectorsController : ApiController
    {
        private Model1 db = new Model1();

        /// <summary>
        /// Query genérica de sectores
        /// </summary>
        /// <returns></returns>
        private IQueryable<Sector> GetSector()
        {
            return db.Sector;
        }

        /// <summary>
        /// Generic Query to filter the sectors
        /// </summary>
        /// <param name="where">input parameter of the lambda expression which is the filter condition </param>
        /// <returns>
        ///if the "where" parameter is null returns a query of all sectors
        /// if the where parameter is not null, it returns a query of all the sectors that match the lambda expression entered in the "where" parameter.
        /// </returns>
        private IQueryable<Sector> GetSectorCondition(Expression<Func<Sector, bool>> where = null)
        {
            var resposnse = where == null ? GetSector() : GetSector().Where(where);
            return resposnse;
        }


        /// <summary>
        /// 
        ///Public method of the api, list the sectors by the id of the city
        /// </summary>
        /// <param name="id"> city id</param>
        /// <returns>
        /// List of sectors belonging to a city
        /// List <SectorRequest>
        /// SectorRequest has properties
        /// IdSector type of int
        /// Nombre type pof string
        /// 
        /// Object SectorRequest
        /// { IdSector,Nombre }
        /// </returns>
        [HttpPost]
        [Route("GetSectorsByCity/{id:int:min(1)}")]
        public async Task<List<SectorRequest>> GetSectorsByCity(int id)
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                var response = await GetSectorCondition(where: x => x.IdCiudad == id).Select(x=>new SectorRequest { IdSector=x.IdSector,Nombre=x.Nombre}).ToListAsync();
                return response;
            }
            catch (Exception)
            {
                return null;
                throw;
            }
            
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