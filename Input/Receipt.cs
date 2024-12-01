using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FetchPoints.Input
{
    /// <summary>
    /// Represents a request, submitted via the API, to add points.
    /// </summary>
    public class Receipt
    {
        [JsonPropertyName("retailer")]
        [Required]
        public string Retailer { get; set; }

        [JsonPropertyName("purchaseDate")]
        [Required]
        public string PurchaseDate { get; set; }

        [JsonPropertyName("purchaseTime")]
        [Required]
        public string PurchaseTime { get; set; }

        [JsonPropertyName("items")]
        [Required]
        public List<Item> Items { get; set; }

        [JsonPropertyName("total")]
        [Required]
        public string Total { get; set; }
    }

    public class Item
    {
        [JsonPropertyName("shortDescription")]
        [Required]
        public string ShortDescription { get; set; }

        [JsonPropertyName("price")]
        [Required]
        public string Price { get; set; }
    }

}
