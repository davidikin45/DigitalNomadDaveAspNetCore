using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Net.Http;

namespace Solution.Base.Controllers.Api
{
    [Route("api/test")]
    public class TestController : BaseWebApiControllerAuthorize
    {
        [Route("checkid/{id}")]
        [HttpGet]
        public IActionResult CheckId(int id)
        {
            if (id > 100)
            {
                return ApiErrorMessage("We cannot use IDs greater than 100.");
            }
            return Ok(id);
        }

        [Route("unauthorized")]
        [HttpGet]
        public IActionResult Unauthorized_401()
        {
            return Unauthorized();
        }

        [Route("forbidden")]
        [HttpGet]
        public IActionResult Forbidden_403()
        {
            return Forbidden();
        }

        [Route("ok")]
        [HttpGet]
        public IActionResult OK_200()
        {
            return Ok();
        }

        [Route("not-found")]
        [HttpGet]
        public IActionResult NotFound_404()
        {
            return NotFound();
        }

        [Route("throw-exception")]
        [HttpGet]
        public IActionResult ThrowException()
        {
            throw new Exception("");
        }
    }
}
