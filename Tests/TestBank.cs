using System;
using System.Collections.Generic;
using FetchPoints.API.Request;
using FetchPoints.Entity;

namespace FetchPoints.Tests {
    public class TestBank {
        public static List<TestResult> RunTests() {
            var results = new List<TestResult>
            {
                // invalid receipt input failures
                // test parse failures
                invalidTime(),
                invalidDate(),
                noItems(),
                futureDate(),
                mismatchedTotal(),

                // test points rules
                baseCase(),
                givenExample1(),
                givenExample2(),
                roundNumberTotal(),
                multipleOfDot25Total(),
                receiptWith8Items(),
                receiptWith9Items(),
                oddNumberedDay(),
                afternoonPurchase(),
                multipleOf3itemStringLength(),
            };

            return results;
        }

# region "Test cases: Expected parse failures"

        private static TestResult noItems() {
            var receipt = baseline10pointReceipt();
            receipt.Items.Clear(); // no items on receipt
            return TestRunner.RunExpectedFailure("No items present", receipt);
        }

        private static TestResult mismatchedTotal() {
            var receipt = baseline10pointReceipt();
            receipt.Total = "892323423.22";
            return TestRunner.RunExpectedFailure("Item total does not match overall total", receipt);
        }

        private static TestResult futureDate() {
            var receipt = baseline10pointReceipt();
            receipt.PurchaseDate = "2050-01-01";
            return TestRunner.RunExpectedFailure("Receipt date in the future", receipt);
        }

        private static TestResult invalidTime() {
            var receipt = baseline10pointReceipt();
            receipt.PurchaseTime = "1:00 PM";
            return TestRunner.RunExpectedFailure("Invalid time", receipt);
        }

        private static TestResult invalidDate() {
            var receipt = baseline10pointReceipt();
            receipt.PurchaseDate = "2024-02-30";
            return TestRunner.RunExpectedFailure("Invalid date", receipt);
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

        private static TestResult baseCase() {
            var receipt = baseline10pointReceipt();
            TestResult result = TestResult.CreateExpectedSuccessTestResult("Baseline 10-point receipt", receipt, 10);
            try {
                result.ActualPoints = ValidatedTransaction.Create(receipt).Points();
            } catch(Exception ex) {
                result.ErrorDetail = ex.Message;
            }
            return result;
        }

        private static TestResult roundNumberTotal() {
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
            var receipt = baseCaseReceiptNoBonuses10points(items);

            // 10 points baseline
            // 50 points for whole number total
            // 25 points for being a multiple of 0.25
            // 5 points because there are two items on the receipt
            return TestRunner.RunExpectedSuccess("Round dollar amount total", receipt, 90);
        }

        private static TestResult multipleOfDot25Total() {
            var items = new List<Item>() {
                new() {
                    ShortDescription = "y",
                    Price = "0.25",
                }
            };
            var receipt = baseCaseReceiptNoBonuses10points(items);

            // 10 points baseline
            // 25 points for being a multiple of 0.25
            return TestRunner.RunExpectedSuccess("Total is multiple of 0.25", receipt, 35);
        }

        private static TestResult receiptWith8Items() {
            var items = new List<Item>();
            for (var i=0; i<8; i++) {
                items.Add(
                    new() {
                        ShortDescription = "x",
                        Price = "1.01",
                });
            }

            var receipt = baseCaseReceiptNoBonuses10points(items);

            // 10 points baseline
            // 5 points for every two items on the receipt
            return TestRunner.RunExpectedSuccess("8-item receipt", receipt, 30);
        }

        private static TestResult receiptWith9Items() {
            var items = new List<Item>();
            for (var i=0; i<9; i++) {
                items.Add(
                    new() {
                        ShortDescription = "x",
                        Price = "1.01",
                });
            }

            var receipt = baseCaseReceiptNoBonuses10points(items);

            // 10 points baseline
            // 5 points for every two items on the receipt
            return TestRunner.RunExpectedSuccess("9-item receipt", receipt, 30);
        }

        private static TestResult oddNumberedDay() {
            var receipt = baseline10pointReceipt();
            receipt.PurchaseDate = "2024-12-01";

            // 10 points baseline
            // 6 points if the day in the purchase date is odd.
            return TestRunner.RunExpectedSuccess("Transaction date with odd-numbered day", receipt, 16);
        }

        private static TestResult afternoonPurchase() {
            var receipt = baseline10pointReceipt();
            receipt.PurchaseTime = "15:30";

            // 10 points baseline
            // 10 points if the time of purchase is after 2:00pm and before 4:00pm.
            return TestRunner.RunExpectedSuccess("Transaction date with afternoon purchase getting bonus", receipt, 20);
        }

        // If the trimmed length of the item description is a multiple of 3, multiply the price by 0.2 and round up to the nearest integer. The result is the number of points earned.
        private static TestResult multipleOf3itemStringLength() {
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

            var receipt = baseCaseReceiptNoBonuses10points(items);

            // 10 points baseline
            // 5 points for every two items on the receipt.
            // If the trimmed length of the item description is a multiple of 3, multiply the price by 0.2 and round up to the nearest integer. The result is the number of points earned.
            return TestRunner.RunExpectedSuccess("3-item transaction with 2 of the items getting string length bonuses", receipt, 19);
        }

        private static TestResult givenExample1() {
            var receipt = new Receipt() {
                Retailer = "Target",
                PurchaseDate = "2022-01-01",
                PurchaseTime = "13:01",
                Items = [
                    new() {
                        ShortDescription = "Mountain Dew 12PK",
                        Price = "6.49"
                    },
                    new() {
                        ShortDescription = "Emils Cheese Pizza",
                        Price = "12.25"
                    },
                    new() {
                        ShortDescription = "Knorr Creamy Chicken",
                        Price = "1.26"
                    },
                    new() {
                        ShortDescription = "Doritos Nacho Cheese",
                        Price = "3.35"
                    },
                    new() {
                        ShortDescription = "   Klarbrunn 12-PK 12 FL OZ  ",
                        Price = "12.00"
                    },
                ],
                Total = "35.35"
            };

            return TestRunner.RunExpectedSuccess("First provided example (\"Food from Target\")", receipt, 28);
        }

        private static TestResult givenExample2() {
            var receipt = new Receipt() {
                Retailer = "M&M Corner Market",
                PurchaseDate = "2022-03-20",
                PurchaseTime = "14:33",
                Items = [
                    new() {
                        ShortDescription = "Gatorade",
                        Price = "2.25"
                    },
                    new() {
                        ShortDescription = "Gatorade",
                        Price = "2.25"
                    },
                    new() {
                        ShortDescription = "Gatorade",
                        Price = "2.25"
                    },
                    new() {
                        ShortDescription = "Gatorade",
                        Price = "2.25"
                    },
                ],
                Total = "9.00"
            };

            return TestRunner.RunExpectedSuccess("Second provided example (\"Gatorade from M&M Corner Market\")", receipt, 109);
        }

#endregion

        private static Receipt baseline10pointReceipt() {
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

        private static Receipt baseCaseReceiptNoBonuses10points(List<Item> items) {
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

}