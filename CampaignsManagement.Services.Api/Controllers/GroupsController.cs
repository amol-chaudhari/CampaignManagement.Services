using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.Description;
using CampaignsManagement.Services.Api.DAL;
using Newtonsoft.Json;

namespace CampaignsManagement.Services.Api.Controllers
{
    [RoutePrefix("api/Groups")]
    public class GroupsController : ApiController
    {
        protected HttpResponseMessage ToJson(dynamic obj)
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");
            return response;
        }

        private CampaignEntities db = new CampaignEntities();

        // GET: api/Campaigns
        [HttpGet]
        [Route("")]
        public HttpResponseMessage GetGroups()
        {
            return ToJson(db.Groups.AsEnumerable());
        }

        [HttpPut]
        [Route("update/{id}")]
        [ResponseType(typeof(void))]
        public HttpResponseMessage UpdateGroup(int id, [FromBody]Group group)
        {
            if (id <= 0) return ToJson(false);
            //var cRecord = db.Groups.Find(id);
            //if (cRecord != null)
            //{
            //    cRecord.GroupName = group.GroupName;
            //    cRecord.Active = group.Active;
            //}
            db.Entry(group).State = EntityState.Modified;//need to test if this updates the data properly
            return ToJson(db.SaveChanges());
        }

        [HttpPost]
        [Route("insert")]
        [ResponseType(typeof(Campaign))]
        public HttpResponseMessage PostGroup([FromBody]Group group)
        {
            if (group.GroupId == 0)
            {
                db.Groups.Add(group);
                var outcome = db.SaveChanges();
                return ToJson(outcome);
            }
            return ToJson(false);
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