using System;
using System.Globalization;
using System.Collections.Generic;

using FetchPoints.API.Request;
using FetchPoints.Entity.PointsRules;
using FetchPoints.API.Exception;

namespace FetchPoints.Entity {

    public class ValidatedTransaction
    {

        public DateTime Timestamp { get; private set; }
        public string Retailer { get; private set;}
        public List<ValidatedTransactionItem> Items { get; private set; }
        public double Total { get; private set; }

        private List<IPointsRule> rules = new List<IPointsRule> {
            new RetailerRules(),
            new ItemRules(),
            new TransactionAmountRules(),
            new TransactionTimestampRules(),
        };

        private ValidatedTransaction(DateTime timestamp, string retailer, List<ValidatedTransactionItem> items, double total) {
            Timestamp = timestamp;
            Retailer = retailer;
            Items = items;
            Total = total;
        }

        // Factory method; perform validation
        // If invalid, returns null
        public static ValidatedTransaction Create(Receipt receipt) {
            // Example:
            // "purchaseDate": "2022-01-01"
            // "purchaseTime": "13:01"
            DateTime timestamp;
            var dateTimeText = receipt.PurchaseDate + " " + receipt.PurchaseTime;
            var dateTimeFormat = "yyyy-MM-dd HH:mm";
            // TODO add note about time assumptions
            var validDate = DateTime.TryParseExact(dateTimeText, dateTimeFormat, new CultureInfo("en-US"), DateTimeStyles.AssumeLocal, out timestamp);
            if (!validDate) {
                throw new InvalidReceipt("Invalid date or time");
            }

            if (timestamp > DateTime.Now) {
                throw new InvalidReceipt("Date cannot be in the future");
            }

/*
            string retailer = receipt.Retailer.Trim();
            if (retailer.Length == 0) {
                throw new ArgumentException("Invalid retailer");
            }
    */

            var items = new List<ValidatedTransactionItem>();
            foreach (var item in receipt.Items) {
                string desc = item.ShortDescription.Trim();
                if (desc.Length == 0) {
                    throw new InvalidReceipt("Invalid item description");
                }
                double price;
                var validPrice = Double.TryParse(item.Price, out price);
                if (!validPrice) throw new InvalidReceipt("Invalid item price");

                items.Add(new ValidatedTransactionItem(desc, price));
            }

            double total;
            var validTotal = Double.TryParse(receipt.Total, out total);
            if (!validTotal) throw new InvalidReceipt("Invalid total");

            double itemTotal = 0;
            items.ForEach(item => { itemTotal += item.Price;});

            // double precision :/
            if (total.ToString("F2") != itemTotal.ToString("F2")) throw new InvalidReceipt($"Receipt total does not match item total");

            return new ValidatedTransaction(timestamp, receipt.Retailer, items, total);
        }

        // Unique ID for receipt; deterministic
        public string Id() {
            // TODO need to implement deterministic scheme
            return $"Spent ${Total} at {Retailer}";
        }

        public int Points() {
            var points = 0;

            rules.ForEach((rule) => {
                points += rule.Apply(this);
            });

            return points;
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
