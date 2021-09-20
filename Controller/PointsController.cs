using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FetchPoints.DataClass;
using FetchPoints.Retriever;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FetchPoints.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class PointsController : ControllerBase
    {
        // GET: api/<ValuesController>
        [HttpGet]
        public IEnumerable<PointsEntry> Get()
        {
            return DataRetriever.getEntries();
        }

        //// GET api/<ValuesController>/5
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        // todo try/catch

        // POST api/<ValuesController>
        [HttpPost]
        public List<PointsEntry> Post([FromBody] int points) // todo needs to be {"points":500}
        {
            // glossing over all user auth here
            var userPoints = UserPoints.create();
            return userPoints.spend(points);
        }

        [HttpPut]
        public void Put()
        {
            DataRetriever.populateFakeData();
        }

        //// PUT api/<ValuesController>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE api/<ValuesController>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
