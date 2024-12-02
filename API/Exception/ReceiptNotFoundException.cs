namespace FetchPoints.API.Exception {
    public class ReceiptNotFoundException: System.Exception {
        public ReceiptNotFoundException() : base("Receipt not found") {}
    }
}
