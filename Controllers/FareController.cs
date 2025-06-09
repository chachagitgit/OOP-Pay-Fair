using Microsoft.AspNetCore.Mvc;
using System;

namespace OOP_Fair_Fare.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FareController : ControllerBase
    {
        private readonly FareCalculator _fareCalculator;

        public FareController(FareCalculator fareCalculator)
        {
            _fareCalculator = fareCalculator;
        }

        [HttpPost("calculate")]
        public IActionResult Calculate([FromBody] FareRequest request)
        {
            try
            {
                var result = _fareCalculator.Calculate(
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
    }

    public class FareRequest
    {
        public double DistanceTraveled { get; set; }
        public int VehicleChoice { get; set; }
        public bool IsDiscounted { get; set; }
    }
} 