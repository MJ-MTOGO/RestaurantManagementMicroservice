using Microsoft.AspNetCore.Mvc;
using RestaurantManagementService.Application.Ports;
using RestaurantManagementService.Application.DTOs;
using RestaurantManagementService.Domain.ValueObjects;

namespace RestaurantManagementService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RestaurantController : ControllerBase
    {
        private readonly IRestaurantService _restaurantService;

        public RestaurantController(IRestaurantService restaurantService)
        {
            _restaurantService = restaurantService ?? throw new ArgumentNullException(nameof(restaurantService));
        }

        [HttpPost]
        public IActionResult CreateRestaurant([FromBody] CreateRestaurantDto dto)
        {
            if (dto == null)
                return BadRequest("Invalid restaurant data.");

            try
            {
                var address = new Address(dto.Street, dto.City, dto.PostalCode);
                var restaurant = _restaurantService.CreateRestaurant(dto.Name, address);
                return CreatedAtAction(nameof(GetRestaurantById), new { id = restaurant.RestaurantId }, restaurant);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while creating the restaurant: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetRestaurantById(Guid id)
        {
            try
            {
                var restaurant = _restaurantService.GetRestaurantById(id);
                if (restaurant == null)
                    return NotFound($"Restaurant with ID {id} not found.");

                return Ok(restaurant);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while fetching the restaurant: {ex.Message}");
            }
        }

        [HttpGet("by-city/{city}")]
        public IActionResult GetRestaurantsByCity(string city)
        {
            if (string.IsNullOrWhiteSpace(city))
                return BadRequest("City cannot be empty.");

            try
            {
                var restaurants = _restaurantService.GetRestaurantsByAddress(city);
                if (!restaurants.Any())
                    return NotFound($"No restaurants found in city: {city}.");

                return Ok(restaurants);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while fetching restaurants: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateRestaurant(Guid id, [FromBody] UpdateRestaurantDto dto)
        {
            if (dto == null)
                return BadRequest("Invalid restaurant update data.");

            try
            {
                var address = new Address(dto.Street, dto.City, dto.PostalCode);
                _restaurantService.UpdateRestaurantDetails(id, dto.Name, address);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while updating the restaurant: {ex.Message}");
            }
        }

        [HttpPost("{id}/menu-item")]
        public IActionResult AddMenuItem(Guid id, [FromBody] AddMenuItemDto dto)
        {
            try
            {
                dto.Validate(); // Validate DTO

                var menuItem = new MenuItem(Guid.NewGuid(), dto.Name, dto.Price, id); // Use route id
                _restaurantService.AddMenuItem(id, menuItem);

                return Ok("Menu item added successfully.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

    }
}
