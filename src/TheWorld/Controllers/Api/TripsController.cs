using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheWorld.Models;
using TheWorld.ViewModels;

namespace TheWorld.Controllers.Api
{
    [Authorize]
    [Route("api/trips")]    
    public class TripsController : Controller
    {
        public IWorldRepository repository { get; set; }

        public ILogger<TripsController> logger { get; set; }

        public TripsController(IWorldRepository repository, ILogger<TripsController> logger)
        {
            this.repository = repository;
            this.logger = logger;
        }

        [HttpGet("")]
        public IActionResult Get()
        {
            try
            {
                var results = repository.GetTripsByUsername(this.User.Identity.Name);
                return Ok(Mapper.Map<IEnumerable<TripViewModel>>(results));
            }
            catch(Exception ex)
            {
                this.logger.LogError($"Failedto get All Trips: {ex}");
                return BadRequest("Error ocurred");
            }

        }

        [HttpPost("")]
        public async Task<IActionResult> Post([FromBody]TripViewModel trip)
        {
            if(ModelState.IsValid)
            {
                // save in the database 
                var newTrip = Mapper.Map<Trip>(trip);

                newTrip.UserName = User.Identity.Name;

                // Only saves in the context 
                this.repository.AddTrip(newTrip);

                // Save in the database and returns false or true 
                if (await this.repository.SaveChangesAsync())
                {
                    return Created($"api/trips/{trip.Name}", Mapper.Map<TripViewModel>(newTrip));
                }
                else
                {
                    BadRequest("Failed to save changes to the database");
                }

                
            }

            return BadRequest("Error ocurred");
            
        }
    }
}
