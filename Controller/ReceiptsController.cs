using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using FetchPoints.DataClass;
using FetchPoints.Input;
using Swashbuckle.AspNetCore.Annotations;
using FetchPoints.Response;
using FetchPoints.PersistedData;

namespace FetchPoints.Controller
{
    [Route("api/receipts")]
    [ApiController]
    public class ReceiptsController : ControllerBase
    {

        [HttpPost("process")]
        [SwaggerOperation("Submits a receipt for processing")]
        [SwaggerResponse(200, "Returns the ID assigned to the receipt")]
        [SwaggerResponse(400, "The receipt is invalid")]
        public ApiResponse Post([FromBody] Receipt request)
        {
            ApiResponse response;
            try {
                var transaction = ValidatedTransaction.Create(request);
                ValidatedTransactionsStore.add(transaction);
                response = ApiResponse.CreateSuccessfulResponse(transaction.Id(), transaction.Points());
            } catch(Exception ex) {
                response = ApiResponse.CreateErrorResponse(ex.Message);
            }
            return response;
        }

        [HttpGet("list")]
        [SwaggerOperation("Lists valid and persisted submitted receipts")]
        public IEnumerable<ValidatedTransaction> Get()
        {
            return  ValidatedTransactionsStore.getAll();
        }

        [HttpGet("{id}/points")]
        [SwaggerOperation("Returns validated receipt, or null if not found")]
        public ApiResponse Get(string id) {
            var transaction = ValidatedTransactionsStore.get(id);
            if (transaction != null) {
                return ApiResponse.CreateSuccessfulResponse(transaction.Id(), transaction.Points());
            } else {
                return ApiResponse.CreateErrorResponse("Receipt not found");
            }
        }
    }
}
