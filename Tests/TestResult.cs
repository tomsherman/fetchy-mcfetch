using System.Text.Json;
using FetchPoints.API.Request;

namespace FetchPoints.Tests {


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
        internal string ExceptionType { get; set; }

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
