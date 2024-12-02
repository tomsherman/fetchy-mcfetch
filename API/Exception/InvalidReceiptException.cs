namespace FetchPoints.API.Exception {
    public class InvalidReceiptException: System.Exception {
        public InvalidReceiptException(string message) : base($"Invalid receipt. Validation error: {message}") {}
    }
}
