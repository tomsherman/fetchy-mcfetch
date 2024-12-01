using System;

namespace FetchPoints.API.Exception {
    public class ReceiptAlreadyProcessed: ArgumentException {
        public ReceiptAlreadyProcessed(string message) : base(message) {}
    }
}
