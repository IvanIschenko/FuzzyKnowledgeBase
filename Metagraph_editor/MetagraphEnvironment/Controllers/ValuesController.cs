using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApplication1;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApplication1.FuzzyLogicBase;
//using System.Web.Mvc;
using Newtonsoft.Json;
using System.Runtime.Serialization;
//using System.Web.Helpers;
using System.Drawing;
using PagedList;

using C__04._07._2015.Algorithms.Clustering;// progr_V.cs
using L.DataStructures.Matrix;// multidim..cs
using L.Algorithms.Clustering;// k_means.cs
using L.DataStructures.Geometry; // cluster.cs
using System.Web.Script.Serialization;

namespace Service.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public string Get(int id)
        {
            return (WebApplication1.WebForm1.Getjsonstring());
            //return "{\"id\":\"test1\",\"name\":\"Package1\",\"items\":[{\"id\":1434312038217,\"title\":\"Entity1\",\"name\":\"Entity1\",\"description\":\"Entity1\",\"fields\":[],\"positionX\":365,\"positionY\":252},{\"id\":1434312049142,\"title\":\"Entity2\",\"name\":\"Entity2\",\"description\":\"Entity2\",\"fields\":[{\"name\":\"Connection1\",\"title\":\"Conneciton1\",\"type\":\"Connection\",\"typeTitle\":\"Connection\",\"relatedItem\":\"Entity1\"}],\"positionX\":94,\"positionY\":231}]}";
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}