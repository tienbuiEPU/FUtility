using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using RestSharp;
using System.Configuration;
using Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web.SessionState;
using System.Threading.Tasks;
using FUtility.Models;

namespace FUtility.Controllers
{
    [RoutePrefix("api/app")]
    public class AppController : ApiController
    {
        [HttpPost]
        [Route("createWatermark")]
        [ActionName("createWatermark")]
        public IHttpActionResult createWatermark(UrlImage url)
        {
            RestClient client = new RestClient(ConfigurationManager.AppSettings["baseApiUrl"].ToString());
            var request = new RestRequest("api/watermark/createWatermark", Method.POST);
            request.AddObject(url);
            IRestResponse response = client.Execute(request);
            var content = response.Content;
            JObject json = JObject.Parse(content);
            return Ok(json);
        }
    }
}
