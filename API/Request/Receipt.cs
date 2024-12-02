using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FetchPoints.API.Request
{
    /// <summary>
    /// Represents a request, submitted via the API, to add points.
    /// </summary>
    public class Receipt
    {
        [JsonPropertyName("retailer")]
        [Required]
        [RegularExpression("^.*\\w.*$")]
        public string Retailer { get; set; }

        [JsonPropertyName("purchaseDate")]
        [Required]
        [RegularExpression("^\\d{4}-\\d{2}-\\d{2}$", ErrorMessage = "Invalid date format. Use YYYY-MM-DD, e.g. 2024-12-25")]
        public string PurchaseDate { get; set; }

        [JsonPropertyName("purchaseTime")]
        [Required]
        [RegularExpression("^\\d{2}:\\d{2}$", ErrorMessage = "Invalid time format. Use HH:MM, e.g. 13:01")]
        public string PurchaseTime { get; set; }

        [JsonPropertyName("items")]
        [Required]
        [MinLength(1, ErrorMessage = "Receipt must contain at least one item")]
        public List<Item> Items { get; set; }

        [JsonPropertyName("total")]
        [Required]
        [Range(0.01, int.MaxValue, ErrorMessage = "Receipt total must be positive number")]
        public string Total { get; set; }
    }

    public class Item
    {
        [JsonPropertyName("shortDescription")]
        [Required]
        [RegularExpression("^.*\\w.*")]
        public string ShortDescription { get; set; }

        [JsonPropertyName("price")]
        [Required]
        [Range(0.01, int.MaxValue)]
        public string Price { get; set; }
    }

}
