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

        //add attribute to endPoint
        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            CelestialObject celestialObject = 
                new CelestialObject();
            if (_context.CelestialObjects.Where(x => x.Name == name) == null)
                NotFound();
            else
            {
                //wrap DbSet into List
                _context.CelestialObjects.ToList().ForEach(delegate(CelestialObject celesOb)
                {
                    //celestialObject match OrbitedObjectId and Id
                    if (celesOb.OrbitedObjectId == celesOb.Id && celesOb.Name == name)
                        celestialObject.Satellites.Add(celesOb);
                });
            }

            //return status code OK and with all celestialObjects
            //match by name
            return Ok(celestialObject);
        }

        [HttpGet]
        public IActionResult GetAll() 
        {
            CelestialObject celestialObject = new CelestialObject();
            _context.CelestialObjects.ToList().ForEach(delegate(CelestialObject celesOb) 
            {
                celestialObject.Satellites.Add(celesOb);
            });

            return Ok(celestialObject);
        }
    }
}
