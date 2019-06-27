using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.Description;
using CampaignsManagement.Services.Api.DAL;
using Newtonsoft.Json;
using CampaignsManagement.Services.Api.Models;
using System.Web;

namespace CampaignsManagement.Services.Api.Controllers
{
    [RoutePrefix("api/Videos")]
    public class VideosController : ApiController
    {
        protected HttpResponseMessage ToJson(dynamic obj)
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");
            return response;
        }

        private CampaignEntities db = new CampaignEntities();
        
        [HttpPut]
        [Route("update/{id}")]
        [ResponseType(typeof(void))]
        public HttpResponseMessage UpdateVideo(int id, [FromBody]Video video)
        {
            if (id <= 0) return ToJson(false);
            db.Entry(video).State = EntityState.Modified; //need to test if this updates the data properly
            return ToJson(db.SaveChanges());
        }

        [HttpPost]
        [Route("insert")]
        [ResponseType(typeof(Video))]
        public HttpResponseMessage PostVideo([FromBody]Video video)
        {
            if (video.VideoId == 0)
            {
                db.Videos.Add(video);
                var outcome = db.SaveChanges();
                return ToJson(outcome);
            }
            return ToJson(false);
        }
        
        [HttpGet]
        [Route("")]
        public HttpResponseMessage GetVideos()
        {
            return ToJson(db.Videos.AsEnumerable());
        }

        [HttpPost]
        [Route("uploadfile")]
        public HttpResponseMessage UploadFile()
        {
            var httpRequest = HttpContext.Current.Request;
            if (httpRequest.Files.Count < 1)
            {
                return ToJson(false);
            }

            //foreach (string file in httpRequest.Files)
            //{
                var postedFile = httpRequest.Files[0];
                var filePath = HttpContext.Current.Server.MapPath("~/" + postedFile.FileName); //need to provide path for angular app projects folder assests,somehow
                postedFile.SaveAs(filePath);
            //}

            return ToJson(new { Response = true, FileName = postedFile.FileName});
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CampaignExists(int id)
        {
            return db.Campaigns.Count(e => e.CampaignId == id) > 0;
        }
    }
}