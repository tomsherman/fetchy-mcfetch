using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using FetchPoints.DataClass;
using FetchPoints.Retriever;
using FetchPoints.ApiException;
using FetchPoints.Input;
using Swashbuckle.AspNetCore.Annotations;
using FetchPoints.Response;

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
        public ReceiptSubmissionResponse Post([FromBody] Receipt request)
        {
            ReceiptSubmissionResponse response;
            try {
                var validatedTransaction = ValidatedTransaction.Create(request);
                response = ReceiptSubmissionResponse.CreateSuccessfulResponse(validatedTransaction.Id());
            } catch(Exception ex) {
                response = ReceiptSubmissionResponse.CreateErrorResponse(ex.Message);
            }
            return response;
        }
    }
}
