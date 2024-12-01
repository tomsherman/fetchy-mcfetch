using System.Text.Json.Serialization;

namespace FetchPoints.Response {

    public class ReceiptSubmissionResponse
    {

        [JsonPropertyName("id")]
        public string Id { get; }

        [JsonPropertyName("isValid")]
        public bool IsValid { get; }

        [JsonPropertyName("errorMessage")]
        public string ErrorMessage { get; }

        private ReceiptSubmissionResponse(string id, bool isValid, string errorMessage) {
            Id = id;
            IsValid = isValid;
            ErrorMessage = errorMessage;
        }

        public static ReceiptSubmissionResponse CreateErrorResponse(string errorMessage) {
            return new ReceiptSubmissionResponse(null, false, errorMessage);
        }

        public static ReceiptSubmissionResponse CreateSuccessfulResponse(string id) {
            return new ReceiptSubmissionResponse(id, true, null);
        }
    }

}