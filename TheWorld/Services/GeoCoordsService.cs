using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace TheWorld.Services
{
    public class GeoCoordsService
    {
        private ILogger<GeoCoordsService> _logger;
        private IConfigurationRoot _config;

        public GeoCoordsService(ILogger<GeoCoordsService> logger, IConfigurationRoot config)
        {
            _logger = logger;
            _config = config;
        }

        public async Task<GeoCoordsResult> GetCoordsAsync(string name)
        {
            var result = new GeoCoordsResult()
            {
                Success = false,
                Message = "Failed to get coordinates"
            };

            var apiKey = _config["Keys:BingKey"];
            var encodedName = WebUtility.UrlEncode(name);
            //Bing Map
            var url = $"http://dev.virtualearth.net/REST/v1/Locations?q={encodedName}&key={apiKey}";

            //Creates client
            var client = new HttpClient();
            //Get results of query
            var json = await client.GetStringAsync(url);
            var results = JObject.Parse(json);
            var resources = results["resourceSets"][0]["resources"];
            if (!results["resourceSets"][0]["resources"].HasValues)
            {
                //Could not find place name
                result.Message = $"Could not find '{name}' as a location";
            }
            else
            {
                var confidence = (string)resources[0]["confidence"];
                //If confidence is not high
                if (confidence != "High")
                {
                    result.Message = $"Could not find a confident match for '{name}' as a location";
                }
                else
                {
                    //Return coordinates
                    var coords = resources[0]["geocodePoints"][0]["coordinates"];
                    result.Latitude = (double)coords[0];
                    result.Longitude = (double)coords[1];
                    result.Success = true;
                    result.Message = "Success";
                }
            }
            return result;
        }
    }
}