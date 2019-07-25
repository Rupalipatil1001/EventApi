using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using EventAPI.Infrastructure;
using EventAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EventAPI.Controllers
{  // [EnableCors("MSPolicy")]
    [Produces("text/json","text/xml")]
    [Route("api/[controller]")]//route prefix
    [ApiController]
    public class EventsController : ControllerBase
    {
        private EventDbContext db;

        public EventsController(EventDbContext dbContext)
        {
            db = dbContext;
        }

        //GET/api/events
        [HttpGet(Name ="GetAll")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public ActionResult<List<EventInfo>> GetEvents()
        {
            var events = db.Events.ToList();
            return Ok(events); //returns with status code 200
        }

        //POST/api/events
        [HttpPost(Name ="AddEvent")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [Authorize]
        public async Task<ActionResult<EventInfo>>AddEventAsync([FromBody]EventInfo eventInfo)
        {
            if (ModelState.IsValid)
            {
                var result = db.Events.Add(eventInfo);
                await db.SaveChangesAsync();
                //  return CreatedAtAction("GetById", new { id = result.Entity.id }, result.Entity);
                return CreatedAtAction(nameof(GetEventAsync), new { id = result.Entity.id }, result.Entity);//returns the status code 201
                                                                                                            //  return Created("", result.Entity);
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        //GET/api/events/{id}
        [HttpGet("{id}", Name ="GetById")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]

        public async Task<ActionResult<EventInfo>> GetEventAsync([FromRoute] int id)
        {
           // throw new Exception();
            var eventInfo = await db.Events.FindAsync(id);
            if (eventInfo != null)
                return Ok(eventInfo);
            else
            return NotFound("Item you are searching not found");
        }
    }
}