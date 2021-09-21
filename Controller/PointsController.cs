using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using FetchPoints.DataClass;
using FetchPoints.Retriever;
using FetchPoints.ApiException;
using FetchPoints.Input;
using Swashbuckle.AspNetCore.Annotations;

namespace FetchPoints.Controller
{
    [Route("api/points")]
    [ApiController]
    public class PointsController : ControllerBase
    {
        // GET: api/points
        /// <summary>
        /// Current balances by payer.
        /// </summary>
        [HttpGet]
        [SwaggerOperation("Returns all positive balances by payer", "If no points available, returns empty object.")]
        public Dictionary<string, int> Get()
        {
            var userPoints = UserPoints.create();
            return userPoints.getPayerBalances();
        }

        // POST api/points
        [HttpPost]
        [SwaggerOperation("Spends points, using oldest accumulated points first")]
        [SwaggerResponse(412, "Insufficient points available")]
        [SwaggerResponse(500, "General error")]
        public List<PointsEntry> Post([FromBody] SpendRequest request)
        {
            try
            {
                // glossing over all user auth here
                var userPoints = UserPoints.create();
                return userPoints.spend(request.points);
            } catch(InsufficientPointsException)
            {
                Response.StatusCode = 412;
                return null;
            } catch(Exception)
            {
                Response.StatusCode = 500;
                return null;
            }
        }

        // PUT api/points
        [SwaggerOperation("Adds a credit to the account from the given payer", "Operation is idempotent. Credits of the same payer/amount/timestamp are added only once to the user's account.")]
        [HttpPut]
        public void Put([FromBody] CreditRequest request)
        {
            var credit = PointsEntry.CreateCreditFromRequest(request);
            DataRetriever.addCredit(credit);
        }

        // PUT api/points
        [HttpPut("fake-it")]
        [SwaggerOperation("Populates example data specified in points.pdf")]
        public void Put()
        {
            DataRetriever.populateFakeData();
        }

        // DELETE api/points
        [HttpDelete]
        [SwaggerOperation("Clears all points data")]
        [SwaggerResponse(205, "All data cleared")]
        public void Delete()
        {
            DataRetriever.clearData();
            Response.StatusCode = 205;
        }

    }
}
