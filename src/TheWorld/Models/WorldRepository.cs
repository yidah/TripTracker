using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheWorld.Models
{
    public class WorldRepository : IWorldRepository
    {
        public WorldContext context { get; set; }

        public ILogger<WorldRepository> logger { get; set; }
        public WorldRepository(WorldContext context, ILogger<WorldRepository> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public IEnumerable<Trip> GetAllTrips()
        {
            this.logger.LogInformation("Getting all trips from the database");
            return this.context.Trips.ToList();
        }

        public void AddTrip(Trip trip)
        {
            context.Add(trip);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await this.context.SaveChangesAsync()) > 0;
        }

        public Trip GetTripByName(string tripName)
        {
            return this.context.Trips
                .Include(t => t.Stops)
                .Where(t => t.Name == tripName).FirstOrDefault();
        }

        public void AddStop(string tripName, Stop newStop, string username)
        {
            var trip = GetUserTripByName(tripName, username);
            if(trip != null)
            {
                // With this version of EF we need to perform 2 steps first to add foreign key
                // and second to add the stop to the context
                trip.Stops.Add(newStop);
                this.context.Add(newStop);
            }
        }

        public IEnumerable<Trip> GetTripsByUsername(string name)
        {
            return this.context.Trips
                .Include(t=>t.Stops)
                .Where(t => t.UserName == name)
                .ToList();
        }

        public Trip GetUserTripByName(string tripName, string username)
        {
            return this.context.Trips
                .Include(t => t.Stops)
                .Where(t => t.Name == tripName && t.UserName == username).FirstOrDefault();
        }
    }
}
