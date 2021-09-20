using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using FetchPoints.DataClass;
using FetchPoints.Retriever;
using FetchPoints.ApiException;
using FetchPoints.Input;

namespace FetchPoints.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class PointsController : ControllerBase
    {
        // GET: api/points
        [HttpGet]
        public IEnumerable<PointsEntry> Get()
        {
            return DataRetriever.getEntries();
        }

        // POST api/points
        [HttpPost]
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

        // POST api/points
        [HttpPut]
        public void Put([FromBody] CreditRequest request)
        {
            var credit = PointsEntry.CreateCreditFromRequest(request);
            DataRetriever.addCredit(credit);
        }

        // PUT api/points
        [HttpPut]
        public void Put()
        {
            DataRetriever.populateFakeData();
        }

        // DELETE api/points
        [HttpDelete]
        public void Delete()
        {
            DataRetriever.clearData();
            Response.StatusCode = 205;
        }

    }
}
