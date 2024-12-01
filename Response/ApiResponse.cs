using System.Text.Json.Serialization;

namespace FetchPoints.Response {

    public class ApiResponse
    {

        [JsonPropertyName("id")]
        public string Id { get; internal set; }

        [JsonPropertyName("isValid")]
        public bool IsValid { get; internal set; }

        [JsonPropertyName("errorMessage")]
        public string ErrorMessage { get; internal set; }

        [JsonPropertyName("points")]
        public int Points { get; }

        internal ApiResponse(string id, bool isValid, string errorMessage, int points) {
            Id = id;
            IsValid = isValid;
            ErrorMessage = errorMessage;
            Points = points;
        }

        public static ApiResponse CreateErrorResponse(string errorMessage) {
            return new ApiResponse(null, false, errorMessage, 0);
        }

        public static ApiResponse CreateSuccessfulResponse(string id, int points) {
            return new ApiResponse(id, true, null, points);
        }
    }

}