using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;

using FetchPoints.API.Request;
using FetchPoints.API.Response;
using FetchPoints.Entity;
using FetchPoints.PersistedData;
using System.Linq;

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
        public Result Post([FromBody] Receipt request)
        {
            Result result;
            try {
                var transaction = ValidatedTransaction.Create(request);
                ValidatedTransactionsStore.add(transaction);
                result = Result.CreateSuccessfulResponse(transaction);
            } catch(Exception ex) {
                result = Result.CreateErrorResponse(ex.Message);
            }
            return result;
        }

        [HttpGet("list")]
        [SwaggerOperation("Lists valid and persisted submitted receipts")]
        public IEnumerable<Result> Get()
        {
            var results = new List<Result>();
            var transactions = ValidatedTransactionsStore.getAll();
            transactions.ToList().ForEach((transaction) => {
                results.Add(Result.CreateSuccessfulResponse(transaction));
            });
            return results;
        }

        [HttpGet("{id}/points")]
        [SwaggerOperation("Returns validated receipt, or null if not found")]
        public Result Get(string id) {
            var transaction = ValidatedTransactionsStore.get(id);
            if (transaction != null) {
                return Result.CreateSuccessfulResponse(transaction);
            } else {
                return Result.CreateErrorResponse("Receipt not found");
            }
        }
    }
}
