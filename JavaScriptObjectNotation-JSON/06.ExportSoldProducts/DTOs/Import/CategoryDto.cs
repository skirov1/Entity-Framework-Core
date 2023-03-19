using Newtonsoft.Json;

namespace ProductShop.DTOs.Import
{
    [JsonObject]
    public class CategoryDto
    {
        [JsonProperty("Name")]
        public string Name { get; set; }
    }
}
