﻿using Newtonsoft.Json;

namespace ProductShop.DTOs.Export
{
    [JsonObject]
    public class ExportProductsInRange
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; }

        [JsonProperty("seller")]
        public string SellerFullName { get; set; }
    }
}
