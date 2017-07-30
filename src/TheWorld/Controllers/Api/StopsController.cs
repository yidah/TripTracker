using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheWorld.Models;
using TheWorld.Services;
using TheWorld.ViewModels;

namespace TheWorld.Controllers.Api
{
    [Authorize]
    [Route("api/trips/{tripName}/stops")]
    public class StopsController : Controller
    {
        public IWorldRepository repository { get; set; }

        public ILogger<StopsController> logger { get; set; }

        public GeoCoordsService coordsService { get; set; }

        public StopsController(IWorldRepository repository, 
            ILogger<StopsController> logger,
            GeoCoordsService coordsService)
        {
            this.repository = repository;
            this.logger = logger;
            this.coordsService = coordsService;
        }

        [HttpGet("")]
        public IActionResult Get(string tripName)
        {
            try
            {
                var trip = this.repository.GetUserTripByName(tripName, User.Identity.Name);
                return Ok(Mapper.Map<IEnumerable<StopViewModel>>(trip.Stops.OrderBy(s => s.Order).ToList()));
            }
            catch (Exception ex)
            {
                this.logger.LogError($"An error ocurred while getting stops {ex}");
            }

            return BadRequest($"An error ocurred while getting stops");
        }

        [HttpPost("")]
        public async Task<IActionResult> Post(string tripName, [FromBody]StopViewModel vm)
        {
            try
            {
                //If the VM is valid
                if (ModelState.IsValid)
                {
                    var newStop = Mapper.Map<Stop>(vm);

                    // Get Geocodes from our service 
                    var result = await this.coordsService.GetCoordsAsync(newStop.Name);

                    if (!result.Success)
                    {
                        this.logger.LogError(result.Message);
                    }
                    else
                    {
                        newStop.Latitude = result.Latitude;
                        newStop.Longitude = result.Longitude;

                        // Save to context
                        this.repository.AddStop(tripName, newStop, User.Identity.Name);

                        // Save in db and check if it was save
                        if (await this.repository.SaveChangesAsync())
                        {
                            return Created($"api/trips/{tripName}/stops/{newStop}", Mapper.Map<StopViewModel>(newStop));
                        }
                    }                    
                }
                
            }
            catch (Exception ex)
            {
                this.logger.LogError($"An error ocurred while saving stops {ex}");
            }

            return BadRequest($"An error ocurred while saving stops");
        }
    }
}
