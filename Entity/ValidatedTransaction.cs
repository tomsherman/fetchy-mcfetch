using System;
using System.Globalization;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

using FetchPoints.API.Request;
using FetchPoints.Entity.PointsRules;
using FetchPoints.API.Exception;
using Microsoft.VisualBasic;

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
            var timestamp = getValidatedTimestamp(receipt);
            var items = getValidatedItems(receipt);

            double total;
            var validTotal = Double.TryParse(receipt.Total, out total);
            if (!validTotal) throw new InvalidReceiptException("Invalid total");

            double itemTotal = 0;
            items.ForEach(item => { itemTotal += item.Price;});

            // double precision :/
            if (total.ToString("F2") != itemTotal.ToString("F2")) throw new InvalidReceiptException($"Receipt total does not match item total");

            return new ValidatedTransaction(timestamp, receipt.Retailer, items, total);
        }

        // Unique ID for receipt; deterministic
        public string Id() {
            return sha256_hash(this.ToString());
        }

        public override string ToString() {
            var builder = new StringBuilder();

            builder.Append("Retailer: " + Retailer);
            builder.Append(ControlChars.NewLine);
            builder.Append("Timestamp: " + Timestamp);
            builder.Append(ControlChars.NewLine);
            builder.Append("Total: " + Total);
            builder.Append(ControlChars.NewLine);

            Items.ForEach(item => {
                builder.Append("  " + item.ToString()); 
                builder.Append(ControlChars.NewLine);
            });

            return builder.ToString();
        }

        public int Points() {
            var points = 0;

            rules.ForEach((rule) => {
                points += rule.Apply(this);
            });

            return points;
        }

        private static string sha256_hash(string value)
        {
            StringBuilder builder = new StringBuilder();

            using (var hash = SHA256.Create())
            {
                Encoding enc = Encoding.UTF8;
                byte[] result = hash.ComputeHash(enc.GetBytes(value));

                foreach (byte b in result) builder.Append(b.ToString("x2"));
            }

            return builder.ToString();
        }

        private static DateTime getValidatedTimestamp(Receipt receipt) {
            // Example:
            // "purchaseDate": "2022-01-01"
            // "purchaseTime": "13:01"
            DateTime timestamp;
            var dateTimeText = receipt.PurchaseDate + " " + receipt.PurchaseTime;
            var dateTimeFormat = "yyyy-MM-dd HH:mm";

            // assume local time 
            var validDate = DateTime.TryParseExact(dateTimeText, dateTimeFormat, new CultureInfo("en-US"), DateTimeStyles.AssumeLocal, out timestamp);
            if (!validDate) {
                throw new InvalidReceiptException("Invalid date or time");
            }

            if (timestamp > DateTime.Now) {
                throw new InvalidReceiptException("Date cannot be in the future");
            }

            return timestamp;
        }

        private static List<ValidatedTransactionItem> getValidatedItems(Receipt receipt) {
            var items = new List<ValidatedTransactionItem>();
            foreach (var item in receipt.Items) {
                string desc = item.ShortDescription.Trim();
                if (desc.Length == 0) {
                    throw new InvalidReceiptException("Invalid item description. Description must contain at leats one alphanumeric character.");
                }
                double price;
                var validPrice = Double.TryParse(item.Price, out price);
                if (!validPrice) throw new InvalidReceiptException($"Invalid item price: {item.Price}");

                items.Add(new ValidatedTransactionItem(desc, price));
            }

            return items;
        }

    }

    public class ValidatedTransactionItem {
        public string ShortDescription { get; }
        public double Price { get; }

        public ValidatedTransactionItem(string shortDescription, double price) {
            ShortDescription = shortDescription;
            Price = price;
        }

        public override string ToString() {
            return ShortDescription + ": " + Price;
        }
    }

}
