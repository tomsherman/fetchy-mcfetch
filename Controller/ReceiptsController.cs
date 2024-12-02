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
        [SwaggerResponse(500, "A general server error occurred")]
        public Result Post([FromBody] Receipt request)
        {
            Result result;

            try {
                var transaction = ValidatedTransaction.Create(request);
                if (ValidatedTransactionsStore.has(transaction)) throw new ReceiptAlreadyProcessedException();
                ValidatedTransactionsStore.add(transaction);
                result = Result.CreateSuccessfulResponse(transaction);

            } catch(ReceiptAlreadyProcessedException ex) {
                result = Result.CreateErrorResponse(ex.Message);
                HttpContext.Response.StatusCode = 409;

            } catch(InvalidReceiptException ex) {
                result = Result.CreateErrorResponse(ex.Message);
                HttpContext.Response.StatusCode = 400;

            } catch (Exception ex) {
                result = Result.CreateErrorResponse(ex.Message);
                HttpContext.Response.StatusCode = 500;
            }

            return result;
        }

        [HttpGet("list")]
        [SwaggerOperation("Lists valid and persisted submitted receipts")]
        [SwaggerResponse(200, "Returns an array of the persisted set of transactions")]
        [SwaggerResponse(500, "A general server error occurred")]
        public IEnumerable<Result> Get()
        {
            try {
                var results = new List<Result>();
                var transactions = ValidatedTransactionsStore.getAll();
                transactions.ToList().ForEach((transaction) => {
                    results.Add(Result.CreateSuccessfulResponse(transaction));
                });
                return results;
            } catch(Exception) {
                HttpContext.Response.StatusCode = 500;
                return null;
            }
        }

        [HttpGet("{id}/points")]
        [SwaggerOperation("Returns validated receipt, or null if not found")]
        [SwaggerResponse(404, "Receipt not found")]
        [SwaggerResponse(500, "A general server error occurred")]
        public Result Get(string id) {
            Result result;

            try {
                var transaction = ValidatedTransactionsStore.get(id) ?? 
                    throw new ReceiptNotFoundException();
                result = Result.CreateSuccessfulResponse(transaction);

            } catch(ReceiptNotFoundException) {
                HttpContext.Response.StatusCode = 404;
                result = Result.CreateErrorResponse("Receipt not found");

            } catch(Exception ex) {
                HttpContext.Response.StatusCode = 500;
                result = Result.CreateErrorResponse(ex.Message);
            }

            return result;
        }

        [HttpDelete("reset")]
        [SwaggerOperation("Clears in-memory store of validated transactions")]
        public void Clear() {
            ValidatedTransactionsStore.clearData();
        }
    }
}
