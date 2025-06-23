using Microsoft.AspNetCore.Mvc;
using System;
using OOP_Fair_Fare.Models;

namespace OOP_Fair_Fare.Controllers
{
    /// <summary>
    /// Controller responsible for calculating public transportation fares.
    /// Handles fare calculations based on:
    /// - Distance traveled
    /// - Vehicle type (from database)
    /// - Discount eligibility
    /// 
    /// Works with the FareCalculator service to compute fares using:
    /// - Base fare for initial kilometers
    /// - Additional fare per kilometer
    /// - Discount rules (20% for eligible passengers)
    /// - Fare rounding rules
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class FareController : ControllerBase
    {
        // Database context for accessing vehicle fare rates and rules
        private readonly AppDbContext _db;

        public FareController(AppDbContext db)
        {
            _db = db;
        }        /// <summary>
        /// Calculates the fare for a journey based on the provided parameters.
        /// 
        /// The calculation process:
        /// 1. Validates the vehicle choice exists in database
        /// 2. Calculates base fare using:
        ///    - Initial fare for first X kilometers (varies by vehicle)
        ///    - Additional fare per kilometer beyond initial distance
        /// 3. Applies discount if eligible (20% off)
        /// 4. Applies rounding rules to final fare
        /// </summary>
        /// <param name="request">
        /// Contains:
        /// - DistanceTraveled: Journey distance in kilometers
        /// - VehicleChoice: ID of the vehicle type (e.g., 1 for jeep, 2 for bus)
        /// - IsDiscounted: Whether passenger is eligible for discount
        /// </param>
        /// <returns>
        /// Success response with:
        /// - BaseFare: Original fare before discount
        /// - DiscountAmount: Amount deducted (if applicable)
        /// - TotalFare: Final fare after discount and rounding
        /// 
        /// Or BadRequest if calculation fails (e.g., invalid vehicle)
        /// </returns>
        [HttpPost("calculate")]
        public IActionResult Calculate([FromBody] FareRequest request)
        {
            try
            {
                var fareCalculator = new FareCalculator(_db);
                var result = fareCalculator.Calculate(
                    request.DistanceTraveled,
                    request.VehicleChoice,
                    request.IsDiscounted
                );

                return Ok(new
                {
                    BaseFare = result.BaseFare,
                    DiscountAmount = result.DiscountAmount,
                    TotalFare = result.TotalFare
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }    /// <summary>
    /// Data model for fare calculation requests.
    /// This class defines the structure of the JSON payload that should be sent
    /// to the /api/Fare/calculate endpoint.
    /// </summary>
    public class FareRequest
    {
        /// <summary>
        /// The total distance of the journey in kilometers
        /// </summary>
        public double DistanceTraveled { get; set; }

        /// <summary>
        /// The ID of the vehicle type from the Vehicles table
        /// Examples: 1 for jeep, 2 for bus, etc.
        /// </summary>
        public int VehicleChoice { get; set; }

        /// <summary>
        /// Whether the passenger is eligible for a discount
        /// True applies 20% discount, False uses regular fare
        /// </summary>
        public bool IsDiscounted { get; set; }
    }
}