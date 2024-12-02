using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text.Json;
using FetchPoints.API.Request;
using FetchPoints.Entity;
using Microsoft.AspNetCore.Http.Features;

namespace FetchPoints.Tests {
    public class TestBank {
        public static List<TestResult> RunTests() {
            var results = new List<TestResult>
            {
                getBaselineSuccessResult(),
                getRoundNumberTotalSuccessResult(),
                getInvalidTimeFailureResult(),
                getNoItemsParseFailureResult(),
                getFutureDateFailureResult(),
            };

            return results;
        }

# region "Test cases: Expected parse failures"

        private static TestResult getNoItemsParseFailureResult() {
            var receipt = get10pointBaseReceipt();
            receipt.Items.Clear();
            TestResult result = TestResult.CreateExpectedFailureTestResult("No items present", receipt);
            try {
                result.ActualPoints = ValidatedTransaction.Create(receipt).Points();
            } catch(Exception ex) {
                result.ErrorDetail = ex.Message;
            }
            return result;
        }


        private static TestResult getFutureDateFailureResult() {
            var receipt = get10pointBaseReceipt();
            receipt.PurchaseDate = "2050-01-01";
            TestResult result = TestResult.CreateExpectedFailureTestResult("Receipt date in the future", receipt);
            try {
                result.ActualPoints = ValidatedTransaction.Create(receipt).Points();
            } catch(Exception ex) {
                result.ErrorDetail = ex.Message;
            }
            return result;
        }

        private static TestResult getInvalidTimeFailureResult() {
            var receipt = get10pointBaseReceipt();
            receipt.PurchaseTime = "1:00 PM";
            TestResult result = TestResult.CreateExpectedFailureTestResult("Invalid time", receipt);
            try {
                result.ActualPoints = ValidatedTransaction.Create(receipt).Points();
            } catch(Exception ex) {
                result.ErrorDetail = ex.Message;
            }
            return result;
        }

#endregion

#region "Test cases: Expected successful receipt parsing"

        private static TestResult getBaselineSuccessResult() {
            var receipt = get10pointBaseReceipt();
            TestResult result = TestResult.CreateExpectedSuccessTestResult("Baseline 10-point receipt", receipt, 10);
            try {
                result.ActualPoints = ValidatedTransaction.Create(receipt).Points();
            } catch(Exception ex) {
                result.ErrorDetail = ex.Message;
            }
            return result;
        }

        private static TestResult getRoundNumberTotalSuccessResult() {
            var items = new List<Item>() {
                new() {
                    ShortDescription = "x",
                    Price = "0.52",
                },
                new() {
                    ShortDescription = "y",
                    Price = "0.48",
                }
            };
            var receipt = getBaseReceiptNoBonuses(items);

            TestResult result = TestResult.CreateExpectedSuccessTestResult("Round dollar amount total", receipt, 60);
            try {
                result.ActualPoints = ValidatedTransaction.Create(receipt).Points();
            } catch(Exception ex) {
                result.ErrorDetail = ex.Message;
            }
            return result;
        }

#endregion

        private static Receipt get10pointBaseReceipt() {
            var baseItem = new Item() {
                ShortDescription = "blah",
                Price = "1.01",
            };
            var baseItems = new List<Item>([baseItem]);
            var baseReceipt = new Receipt() {
                Retailer = "TenDigitsX", // 10 points
                Total = "1.01", // no bonuses
                PurchaseDate = "2024-02-02",
                PurchaseTime = "12:00", // no bonuses
            };
            baseReceipt.Items = baseItems;
            return baseReceipt;
        }

        private static Receipt getBaseReceiptNoBonuses(List<Item> items) {
            var baseReceipt = new Receipt() {
                Retailer = "TenDigitsX", // 10 points
                PurchaseDate = "2024-02-02",
                PurchaseTime = "12:00", // no bonuses
            };
            baseReceipt.Items = items;

            double total = 0;
            items.ForEach((item) => {
                total += Convert.ToDouble(item.Price);
            });
            baseReceipt.Total = total.ToString();

            return baseReceipt;
        }
    }

    public class TestResult {
        public string Description { get; }
        public bool Success {
            get {
                if (ExpectedToSucceed && ActualPoints == ExpectedPoints) return true;
                if (!ExpectedToSucceed && !string.IsNullOrEmpty(ErrorDetail)) return true;
                return false;
            }
        }

        internal string InputReceiptJSON { get; }
        internal string ErrorDetail { get; set; }

        internal int ExpectedPoints { get; }
        internal bool ExpectedToSucceed { get; }
        internal int ActualPoints { get; set; }

        // Constructor for expected success
        private TestResult(string description, Receipt receipt, int expectedPoints) {
            Description = description;
            ExpectedPoints = expectedPoints;
            InputReceiptJSON = JsonSerializer.Serialize(receipt);
            ExpectedToSucceed = true;
        }

        // Comstructor for expected failure
        private TestResult(string description, Receipt receipt) {
            Description = description;
            ExpectedPoints = int.MinValue;
            InputReceiptJSON = JsonSerializer.Serialize(receipt);
            ExpectedToSucceed = false;
        }

        internal static TestResult CreateExpectedSuccessTestResult(string description, Receipt receipt, int expectedPoints) {
            return new TestResult(description, receipt, expectedPoints);
        }

        internal static TestResult CreateExpectedFailureTestResult(string description, Receipt receipt) {
            return new TestResult(description, receipt);
        }
    }
}