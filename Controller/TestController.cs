using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Text;

using FetchPoints.Tests;

namespace FetchPoints.Controller
{
    [Route("api/test")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet("run-all-tests")]
        [SwaggerOperation("Runs all tests")]
        public string Get()
        {
            var builder = new StringBuilder();
            try {
                var testResults = TestBank.RunTests();
                testResults.ForEach((TestResult result) => {
                    builder.AppendLine($"Test: {result.Description}");
                    builder.AppendLine($"  Passed? {result.Success}");
                    if (result.ExpectedToSucceed) {
                        builder.AppendLine($"  Expected points: {result.ExpectedPoints}, actual points: {result.ActualPoints}");
                    }
                    if (!string.IsNullOrEmpty(result.ErrorDetail)) builder.AppendLine($"  Error: {result.ErrorDetail}");
                    builder.AppendLine();
                });
            } catch(Exception) {
                HttpContext.Response.StatusCode = 500;
            }
            return builder.ToString();
        }

    }
}
