using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using HjulinstallningAPI.Data;
using HjulinstallningAPI.Models;

namespace HjulinstallningAPI.Controllers
{
    [ApiController]
    [Route("api/vehicle")]
    public class VehicleController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly Dictionary<string, string> _headers;
        private readonly string _baseUrl;
        private readonly VehicleDbContext _dbContext;

        public VehicleController(IConfiguration configuration, VehicleDbContext dbContext)
        {
            _configuration = configuration;
            _httpClient = new HttpClient();
            _dbContext = dbContext;

            _baseUrl = _configuration["DnbApi:BaseUrl"] 
                ?? throw new InvalidOperationException("Base URL is missing in appsettings.json.");

            _headers = new Dictionary<string, string>
            {
                { "IdKey", _configuration["DnbApi:IdKey"] ?? throw new InvalidOperationException("IdKey is missing in appsettings.json.") },
                { "Lk", _configuration["DnbApi:Lk"] ?? throw new InvalidOperationException("Lk is missing in appsettings.json.") },
                { "KundId", _configuration["DnbApi:KundId"] ?? throw new InvalidOperationException("KundId is missing in appsettings.json.") },
                { "Password", _configuration["DnbApi:Password"] ?? throw new InvalidOperationException("Password is missing in appsettings.json.") },
                { "ProduktId", _configuration["DnbApi:ProduktId"] ?? throw new InvalidOperationException("ProduktId is missing in appsettings.json.") }
            };
        }




        private static readonly Dictionary<string, (DateTime, string)> _cache = new();

        [HttpGet("{licensePlate}")]
        public async Task<IActionResult> GetVehicleData(string licensePlate)
        {
            // 1️⃣ Check Cache First
            if (_cache.TryGetValue(licensePlate, out var cachedEntry) && (DateTime.UtcNow - cachedEntry.Item1).TotalMinutes < 60)
            {
                Console.WriteLine($"🔹 Returning cached data for {licensePlate}");
                return Ok(JsonConvert.DeserializeObject<Vehicle>(cachedEntry.Item2));
            }

            // 2️⃣ Check Database for Cached Data
            var existingVehicle = await _dbContext.Vehicles.FirstOrDefaultAsync(v => v.LicensePlate == licensePlate);

            if (existingVehicle != null && (DateTime.UtcNow - existingVehicle.LastUpdated).TotalMinutes < 60)
            {
                Console.WriteLine($"🔹 Returning database data for {licensePlate}");
                return Ok(existingVehicle);
            }

            Console.WriteLine($"🔹 Sending request to {_baseUrl}/{licensePlate}");

            try
            {
                // 3️⃣ Make API Request
                var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}/{licensePlate}");
                foreach (var header in _headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }

                var response = await _httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"❌ API Request Failed: {errorMessage}");
                    return StatusCode((int)response.StatusCode, new { message = "D&B API Request Failed", error = errorMessage });
                }

                var data = JsonConvert.DeserializeObject<Vehicle>(await response.Content.ReadAsStringAsync());

                // 4️⃣ Cache API Response
                _cache[licensePlate] = (DateTime.UtcNow, JsonConvert.SerializeObject(data));

                // 5️⃣ Store API Response in Database
                if (data == null)
                {
                    throw new InvalidOperationException("Vehicle data cannot be null.");
                }
                if (existingVehicle == null)
                {
                    _dbContext.Vehicles.Add(data);
                }
                else
                {
                    existingVehicle.Make = data.Make;
                    existingVehicle.Model = data.Model;
                    existingVehicle.Year = data.Year;
                    existingVehicle.LastUpdated = DateTime.UtcNow;
                }


                await _dbContext.SaveChangesAsync();
                return Ok(data);
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"❌ Network Error: {ex.Message}");
                return StatusCode(503, new { message = "Service Unavailable", error = ex.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Unexpected Error: {ex.Message}");
                return StatusCode(500, new { message = "Internal Server Error", error = ex.Message });
            }
        }





        //SET TEST DATA ############################################################################################## TEST
        [HttpPost("set")]
        public async Task<IActionResult> SetTestVehicleData([FromBody] Vehicle vehicle)
        {
            if (vehicle == null || string.IsNullOrEmpty(vehicle.LicensePlate))
            {
                return BadRequest(new { message = "Invalid vehicle data" });
            }

            Console.WriteLine($"🔹 Adding test vehicle data: {vehicle.LicensePlate}, {vehicle.Make}, {vehicle.Model}, {vehicle.Year}");

            try
            {
                var existingVehicle = await _dbContext.Vehicles.FirstOrDefaultAsync(v => v.LicensePlate == vehicle.LicensePlate);
                
                if (existingVehicle == null)
                {
                    vehicle.LastUpdated = DateTime.UtcNow;
                    _dbContext.Vehicles.Add(vehicle);
                }
                else
                {
                    existingVehicle.Make = vehicle.Make;
                    existingVehicle.Model = vehicle.Model;
                    existingVehicle.Year = vehicle.Year;
                    existingVehicle.LastUpdated = DateTime.UtcNow;
                }

                await _dbContext.SaveChangesAsync();
                return Ok(new { message = "Test vehicle data saved successfully", vehicle });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error saving test vehicle: {ex.Message}");
                return StatusCode(500, new { message = "Internal Server Error", error = ex.Message });
            }
        }

        
    }
}
