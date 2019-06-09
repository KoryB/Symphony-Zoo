using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Symphony_Zoo_New.Models;

namespace Symphony_Zoo_New.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GraphDebugController : ControllerBase
    {
        //GET /api/graph
        [HttpGet]
        public ActionResult<string> Get()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(Graph.Instance.GetAllMeasures().ToArray());
        }
    }
}