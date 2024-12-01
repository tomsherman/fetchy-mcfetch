using System;
using System.Reflection.Metadata;
using System.Globalization;
using FetchPoints;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http.Features;
using System.Runtime.Serialization;

namespace FetchPoints.DataClass {

    public class ValidatedTransaction
    {

        public DateTime Timestamp { get; private set; }
        public string Retailer { get; private set;}
        public List<ValidatedTransactionItem> Items { get; private set; }
        public double Total { get; private set; }


        private ValidatedTransaction(DateTime timestamp, string retailer, List<ValidatedTransactionItem> items, double total) {
            Timestamp = timestamp;
            Retailer = retailer;
            Items = items;
            Total = total;
        }

        // Factory method; perform validation
        // If invalid, returns null
        public static ValidatedTransaction Create(Input.Receipt receipt) {
            CultureInfo enUS = new CultureInfo("en-US");

            // Example:
            // "purchaseDate": "2022-01-01",
            // "purchaseTime": "13:01",
            DateTime timestamp;
            var dateTimeText = receipt.PurchaseDate + " " + receipt.PurchaseTime;
            var dateTimeFormat = "yyyy-MM-dd HH:mm";
            var validDate = DateTime.TryParseExact(dateTimeText, dateTimeFormat, enUS, DateTimeStyles.AssumeLocal, out timestamp);
            if (!validDate) {
                throw new ArgumentException("Invalid date or time");
            }

            string retailer = receipt.Retailer.Trim();
            if (retailer.Length == 0) {
                throw new ArgumentException("Invalid retailer");
            }

            var items = new List<ValidatedTransactionItem>();
            foreach (var item in receipt.Items) {
                string desc = item.ShortDescription.Trim();
                if (desc.Length == 0) {
                    throw new ArgumentException("Invalid item description");
                }
                double price;
                var validPrice = Double.TryParse(item.Price, out price);
                if (!validPrice) throw new ArgumentException("Invalid item price");

                items.Add(new ValidatedTransactionItem(desc, price));
            }

            double total;
            var validTotal = Double.TryParse(receipt.Total, out total);
            if (!validTotal) throw new ArgumentException("Invalid total");

            double itemTotal = 0;
            items.ForEach(item => { itemTotal += item.Price;});

            // double precision :/
            if (total.ToString("F2") != itemTotal.ToString("F2")) throw new ArgumentException($"Receipt total does not match item total");

            return new ValidatedTransaction(timestamp, retailer, items, total);
        }

        // Unique ID for receipt; deterministic
        public string Id() {
            return $"Spent ${Total} at {Retailer}";
        }

        public int Points() {
            return 42;
        }

    }

    public class ValidatedTransactionItem {
        public string ShortDescription { get; }
        public double Price { get; }

        public ValidatedTransactionItem(string shortDescription, double price) {
            ShortDescription = shortDescription;
            Price = price;
        }
    }

}
