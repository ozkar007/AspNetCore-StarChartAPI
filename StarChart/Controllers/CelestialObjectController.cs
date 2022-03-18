using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
