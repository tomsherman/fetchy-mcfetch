using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;

using FetchPoints.API.Request;
using FetchPoints.API.Response;
using FetchPoints.API.Exception;
using FetchPoints.Entity;
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
        [SwaggerResponse(409, "The receipt has already been processed")]
        public Result Post([FromBody] Receipt request)
        {
            Result result;

            try {
                var transaction = ValidatedTransaction.Create(request);
                if (ValidatedTransactionsStore.has(transaction)) throw new ReceiptAlreadyProcessed("Receipt has already been processed");
                ValidatedTransactionsStore.add(transaction);
                result = Result.CreateSuccessfulResponse(transaction);

            } catch(ReceiptAlreadyProcessed ex) {
                result = Result.CreateErrorResponse(ex.Message);
                HttpContext.Response.StatusCode = 409;

            } catch(InvalidReceipt ex) {
                result = Result.CreateErrorResponse(ex.Message);
                HttpContext.Response.StatusCode = 400;

            } catch (Exception ex) {
                result = Result.CreateErrorResponse(ex.Message);
                HttpContext.Response.StatusCode = 400;
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

        [HttpDelete("reset")]
        [SwaggerOperation("Clears in-memory store of validated transactions")]
        public void Clear() {
            ValidatedTransactionsStore.clearData();
        }
    }
}
