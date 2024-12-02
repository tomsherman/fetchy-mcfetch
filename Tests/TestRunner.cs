using System;
using FetchPoints.API.Request;
using FetchPoints.Entity;

namespace FetchPoints.Tests {

    internal class TestRunner {
        public static TestResult RunExpectedFailure(string description, Receipt receipt) {
            var result = TestResult.CreateExpectedFailureTestResult(description, receipt);
            try {
                result.ActualPoints = ValidatedTransaction.Create(receipt).Points();
            } catch(Exception ex) {
                result.ErrorDetail = ex.Message;
                result.ExceptionType = ex.GetType().ToString();
            }
            return result;
        }

        public static TestResult RunExpectedSuccess(string description, Receipt receipt, int expectedPoints) {
            var result = TestResult.CreateExpectedSuccessTestResult(description, receipt, expectedPoints);
            try {
                result.ActualPoints = ValidatedTransaction.Create(receipt).Points();
            } catch(Exception ex) {
                result.ErrorDetail = ex.Message;
                result.ExceptionType = ex.GetType().ToString();
            }
            return result;
        }
    }

}


