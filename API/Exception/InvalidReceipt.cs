using System;

namespace FetchPoints.API.Exception {
    public class InvalidReceipt: ArgumentException {
        public InvalidReceipt(string message) : base(message) {}
    }
}
