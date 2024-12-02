namespace FetchPoints.API.Exception {
    public class ReceiptAlreadyProcessedException: System.Exception {
        public ReceiptAlreadyProcessedException() : base("Receipt has already been processed") {}
    }
}
