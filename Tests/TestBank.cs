using System;
using System.Collections.Generic;
using System.Text.Json;
using FetchPoints.API.Request;
using FetchPoints.Entity;

namespace FetchPoints.Tests {
    public class TestBank {
        public static List<TestResult> RunTests() {
            var results = new List<TestResult>
            {
                getBaselineSuccessResult(),
                getRoundNumberTotalSuccessResult(),
                getPoint25MultipleTotalSuccessResult(),
                get8ItemReceiptSuccessResult(),
                get9ItemReceiptSuccessResult(),

                // invalid receipt input failures
                getInvalidTimeFailureResult(),
                getNoItemsParseFailureResult(),
                getFutureDateFailureResult(),
                getMismatchedTotalFailureResult(),
                getOddDayDateResult(),
                getAfternoonPurchaseTestresult(),
                getMultiplierOf3ItemDescriptionTestResult(),
            };

            return results;
        }

# region "Test cases: Expected parse failures"

        private static TestResult getNoItemsParseFailureResult() {
            var receipt = get10pointBaseReceipt();
            receipt.Items.Clear(); // no items on receipt
            TestResult result = TestResult.CreateExpectedFailureTestResult("No items present", receipt);
            try {
                result.ActualPoints = ValidatedTransaction.Create(receipt).Points();
            } catch(Exception ex) {
                result.ErrorDetail = ex.Message;
            }
            return result;
        }

        private static TestResult getMismatchedTotalFailureResult() {
            var receipt = get10pointBaseReceipt();
            receipt.Total = "892323423.22";
            TestResult result = TestResult.CreateExpectedFailureTestResult("Item total does not match overall total", receipt);
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

/*

    One point for every alphanumeric character in the retailer name.
    50 points if the total is a round dollar amount with no cents.
    25 points if the total is a multiple of 0.25.
    5 points for every two items on the receipt.
    If the trimmed length of the item description is a multiple of 3, multiply the price by 0.2 and round up to the nearest integer. The result is the number of points earned.
    6 points if the day in the purchase date is odd.
    10 points if the time of purchase is after 2:00pm and before 4:00pm.

*/


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

            // 10 points baseline
            // 50 points for whole number total
            // 25 points for being a multiple of 0.25
            // 5 points because there are two items on the receipt
            TestResult result = TestResult.CreateExpectedSuccessTestResult("Round dollar amount total", receipt, 90);
            try {
                result.ActualPoints = ValidatedTransaction.Create(receipt).Points();
            } catch(Exception ex) {
                result.ErrorDetail = ex.Message;
            }
            return result;
        }

        private static TestResult getPoint25MultipleTotalSuccessResult() {
            var items = new List<Item>() {
                new() {
                    ShortDescription = "y",
                    Price = "0.25",
                }
            };
            var receipt = getBaseReceiptNoBonuses(items);

            // 10 points baseline
            // 25 points for being a multiple of 0.25
            TestResult result = TestResult.CreateExpectedSuccessTestResult("Total is multiple of 0.25", receipt, 35);
            try {
                result.ActualPoints = ValidatedTransaction.Create(receipt).Points();
            } catch(Exception ex) {
                result.ErrorDetail = ex.Message;
            }
            return result;
        }

        private static TestResult get8ItemReceiptSuccessResult() {
            var items = new List<Item>();
            for (var i=0; i<8; i++) {
                items.Add(
                    new() {
                        ShortDescription = "x",
                        Price = "1.01",
                });
            }

            var receipt = getBaseReceiptNoBonuses(items);

            // 10 points baseline
            // 5 points for every two items on the receipt
            TestResult result = TestResult.CreateExpectedSuccessTestResult("8-item receipt", receipt, 30);
            try {
                result.ActualPoints = ValidatedTransaction.Create(receipt).Points();
            } catch(Exception ex) {
                result.ErrorDetail = ex.Message;
            }
            return result;
        }

        private static TestResult get9ItemReceiptSuccessResult() {
            var items = new List<Item>();
            for (var i=0; i<9; i++) {
                items.Add(
                    new() {
                        ShortDescription = "x",
                        Price = "1.01",
                });
            }

            var receipt = getBaseReceiptNoBonuses(items);

            // 10 points baseline
            // 5 points for every two items on the receipt
            TestResult result = TestResult.CreateExpectedSuccessTestResult("9-item receipt", receipt, 30);
            try {
                result.ActualPoints = ValidatedTransaction.Create(receipt).Points();
            } catch(Exception ex) {
                result.ErrorDetail = ex.Message;
            }
            return result;
        }

        private static TestResult getOddDayDateResult() {
            var receipt = get10pointBaseReceipt();
            receipt.PurchaseDate = "2024-12-01";


            // 10 points baseline
            // 6 points if the day in the purchase date is odd.
            TestResult result = TestResult.CreateExpectedSuccessTestResult("Transaction date with odd-numbered day", receipt, 16);
            try {
                result.ActualPoints = ValidatedTransaction.Create(receipt).Points();
            } catch(Exception ex) {
                result.ErrorDetail = ex.Message;
            }
            return result;
        }

        private static TestResult getAfternoonPurchaseTestresult() {
            var receipt = get10pointBaseReceipt();
            receipt.PurchaseTime = "15:30";

            // 10 points baseline
            // 10 points if the time of purchase is after 2:00pm and before 4:00pm.
            TestResult result = TestResult.CreateExpectedSuccessTestResult("Transaction date with afternoon purchase getting bonus", receipt, 20);
            try {
                result.ActualPoints = ValidatedTransaction.Create(receipt).Points();
            } catch(Exception ex) {
                result.ErrorDetail = ex.Message;
            }
            return result;
        }

        // If the trimmed length of the item description is a multiple of 3, multiply the price by 0.2 and round up to the nearest integer. The result is the number of points earned.
        private static TestResult getMultiplierOf3ItemDescriptionTestResult() {
            var items = new List<Item>
            {
                new Item()
                {
                    ShortDescription = "xxx",
                    Price = "1.1" // point result is 1
                },
                new Item()
                {
                    ShortDescription = "xxxYYY",
                    Price = "10.1" // point result is 3
                },
                new Item()
                {
                    ShortDescription = "hello",
                    Price = "5.73" // point result is 0; description length not a multiple of 3
                },
            };

            var receipt = getBaseReceiptNoBonuses(items);

            // 10 points baseline
            // 5 points for every two items on the receipt.
            // If the trimmed length of the item description is a multiple of 3, multiply the price by 0.2 and round up to the nearest integer. The result is the number of points earned.
            TestResult result = TestResult.CreateExpectedSuccessTestResult("3-item transaction with 2 of the items getting string length bonuses", receipt, 19);
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