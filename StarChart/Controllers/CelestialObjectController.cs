using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
    /// <summary>
    /// Controller with routes to the API end-points
    /// end-points accept different attributes
    /// </summary>
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        //bind db context
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id) 
        {
            //locate ALL CelestialObjects that have the same id/OrbitObjectId to delete
            
            if (_context.CelestialObjects.FirstOrDefault(x => x.Id == id) == null )
            {
                return NotFound();
            }

            //remove sequence of records
            var celObjects = _context.CelestialObjects.ToList().Where(x => x.Id == id || x.OrbitedObjectId == id);
            _context.RemoveRange(celObjects);
            _context.SaveChanges();

            return NoContent();

        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name) 
        {
            //locate CelestialObject based on id
            var patchCelObj = _context.CelestialObjects.FirstOrDefault(x => x.Id == id);
            if (patchCelObj == null)
            {
                return NotFound();
            }

            //patch patchCelObj
            patchCelObj.Name = name;
            _context.CelestialObjects.Update(patchCelObj);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject celestialObject) 
        {
            //find CelestialObject based on param Id
            //can we reuse by calling GetById(id),
            //not sure how to work with IActionResult, to access CelestialObject
            var updateCelObj = _context.CelestialObjects.FirstOrDefault(c => c.Id == id);
            if (updateCelObj == null)
            {
                return NotFound();
            }
            else 
            {
                //update found CelestialObject with data
                //from param CelestialObject
                updateCelObj.Name = celestialObject.Name;
                updateCelObj.OrbitalPeriod = celestialObject.OrbitalPeriod;
                updateCelObj.OrbitedObjectId = celestialObject.OrbitedObjectId;

                _context.CelestialObjects.Update(updateCelObj);
                _context.SaveChanges();
            }

            return NoContent();
        }

        [HttpPost]
        public IActionResult Create([FromBody]CelestialObject payload) 
        {
            //happy path:
            //we assume CelestialObject payload has all properties set
            //alternative option wrap in try/catch block
            _context.CelestialObjects.AddAsync(payload);//adds entity DbSet in memory
            _context.SaveChangesAsync();//saves entity to db

            var cObject = _context.CelestialObjects.Find(1);
            
            return CreatedAtRoute("GetById", new { cObject.Id }, cObject);

            /*
            try
            {

            }
            catch (Exception)
            {

                throw new InvalidOperationException();
            }
            */
        }

        [HttpGet("{id:int}", Name = "GetById")]
        public IActionResult GetById(int id) 
        {
            CelestialObject celestialObject;
            if (_context.CelestialObjects.FirstOrDefault(t => t.Id == id) == null)
            {
                return NotFound();//record not found CelestialObject
            }
            else
            {
                //search DbSet
                //***match based on Id and parameter id
                celestialObject = _context.CelestialObjects.FirstOrDefault(x => x.Id == id);
                //need to instantiate the new CelestialObject
                celestialObject.Satellites = new List<CelestialObject>() { celestialObject };
            }

            //return OK status code with matching CelestialObject object
            return Ok(celestialObject);
        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            CelestialObject celestialObject = new CelestialObject();
            if (_context.CelestialObjects.FirstOrDefault(x => x.Name == name) == null)
                return NotFound();
            else
            {
                //wrap DbSet into List
                _context.CelestialObjects.ToList().ForEach(delegate(CelestialObject celesOb)
                {
                    //filter celestialObject by name and set ids
                    if (celesOb.Name == name)
                    {
                        celestialObject.OrbitedObjectId = celesOb.Id;
                        celestialObject.Id = celesOb.Id;
                        celestialObject.Name = celesOb.Name;
                        celestialObject.Satellites = new List<CelestialObject>() { celestialObject };
                    }
                });
            }

            //return status code OK and with all celestialObjects
            //match by name
            return Ok(celestialObject.Satellites);
        }

        [HttpGet]
        public IActionResult GetAll() 
        {
            CelestialObject celestialObject = new CelestialObject();
            _context.CelestialObjects.ToList().ForEach(delegate(CelestialObject celesOb) 
            {
                celestialObject.OrbitedObjectId = celesOb.Id;
                celestialObject.Id = celesOb.Id;
                celestialObject.Name = celesOb.Name;
                celestialObject.Satellites = new List<CelestialObject>() { celestialObject };
            });

            return Ok(celestialObject.Satellites);
        }
    }
}
