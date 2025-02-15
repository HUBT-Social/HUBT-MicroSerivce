using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUBT_Social_Base
{
    public class PingController : ControllerBase
    {
        [HttpGet("ping"), HttpHead("ping")]
        public IActionResult Ping()
        {
            return Ok(new { message = "Server is alive" });
        }
    }
}
