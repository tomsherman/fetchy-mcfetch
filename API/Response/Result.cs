using System.Text.Json.Serialization;

using FetchPoints.Entity;

namespace FetchPoints.API.Response {

    public class Result
    {

        [JsonPropertyName("id")]
        public string Id { get; internal set; }

        [JsonPropertyName("isValid")]
        public bool IsValid { get; internal set; }

        [JsonPropertyName("errorMessage")]
        public string ErrorMessage { get; internal set; }

        [JsonPropertyName("points")]
        public int Points { get; }

        internal Result(string id, bool isValid, string errorMessage, int points) {
            Id = id;
            IsValid = isValid;
            ErrorMessage = errorMessage;
            Points = points;
        }

        public static Result CreateErrorResponse(string errorMessage) {
            return new Result(null, false, errorMessage, 0);
        }

        public static Result CreateSuccessfulResponse(ValidatedTransaction transaction) {
            return new Result(transaction.Id(), true, null, transaction.Points());
        }
    }

}