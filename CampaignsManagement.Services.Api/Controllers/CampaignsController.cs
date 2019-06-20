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

namespace CampaignsManagement.Services.Api.Controllers
{
    [RoutePrefix("api/Campaigns")]
    public class CampaignsController : ApiController
    {
        protected HttpResponseMessage ToJson(dynamic obj)
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");
            return response;
        }

        private CampaignEntities db = new CampaignEntities();

        private List<CampaignModel> GetCampaignModel(IEnumerable<Campaign> campaigns)
        {
            var ac = new AccountController();
            var list = new List<CampaignModel>();
            foreach (var item in campaigns)
            {
                var model = new CampaignModel
                {
                    CampaignId = item.CampaignId,
                    Active = item.Active,
                    CampaignName = item.CampaignName,
                    Category = item.Category,
                    Description = item.Description,
                    Level = item.Level,
                    ReferenceId = item.ReferenceId,
                    VideoDetails = item.VideoDetails,
                    VideoLink = item.VideoLink
                };

                var published = db.PublishedCampaigns.FirstOrDefault(d => d.CampaignId == model.CampaignId);
                if (published != null)
                {
                    model.PublishedDate = published.PublishedDate.Value.ToString("dd MMM yyyy");
                    model.PublishedBy = $"{published.CampaignUser.FirstName} { published.CampaignUser.LastName}";
                }

                //var completedCampaign = db.UserCampaigns.FirstOrDefault(d => d.CampaignId == model.CampaignId && d.UserId == ac.LoggedInUser.UserId);
                //model.CompletedDate = completedCampaign?.CompletedDate?.ToString("dd MMM yyyy");
                list.Add(model);
            }
            return list;
        }

        // GET: api/Campaigns
        [HttpGet]
        [Route("")]
        public HttpResponseMessage GetCampaigns()
        {
            var campaigns = GetCampaignModel(db.Campaigns.AsEnumerable());
            return ToJson(campaigns);
        }
        
        // GET: api/Campaigns/5
        [HttpGet]
        [ResponseType(typeof(Campaign))]
        public IHttpActionResult GetCampaign(int id)
        {
            Campaign campaign = db.Campaigns.Find(id);
            if (campaign == null)
            {
                return NotFound();
            }

            return Ok(campaign);
        }

        [HttpPut]
        [Route("update/{id}")]
        [ResponseType(typeof(void))]
        public HttpResponseMessage UpdateCampaign(int id, [FromBody]Campaign campaign)
        {
            if (id <= 0) return ToJson(false);
            db.Entry(campaign).State = EntityState.Modified; //need to test if this updates the data properly
            return ToJson(db.SaveChanges());
            //var cRecord = db.Campaigns.Find(id);
            //if (cRecord != null)
            //{
            //    cRecord.CampaignName = campaign.CampaignName;
            //    cRecord.Active = campaign.Active;
            //    cRecord.Category = campaign.Category;
            //    cRecord.Description = campaign.Description;
            //    cRecord.Level = campaign.Level;
            //    cRecord.ReferenceId = campaign.ReferenceId;
            //    cRecord.VideoLink = campaign.VideoLink;
            //    cRecord.VideoDetails = campaign.VideoDetails;
            //}
        }

        [HttpPost]
        [Route("insert")]
        [ResponseType(typeof(Campaign))]
        public HttpResponseMessage PostCampaign([FromBody]Campaign campaign)
        {
            if (campaign.CampaignId == 0)
            {
                db.Campaigns.Add(campaign);
                var outcome = db.SaveChanges();
                return ToJson(outcome);
            }
            return ToJson(false);
        }

        [HttpPost]
        [Route("publish/{id}")]
        [ResponseType(typeof(Campaign))]
        public HttpResponseMessage PublishCampaign(int id, [FromBody]Campaign campaign)
        {
            if(campaign.CampaignId > 0)
            {
                var ac = new AccountController();
                
                var publishCampaign = new PublishedCampaign
                {
                    CampaignId = campaign.CampaignId,
                    GroupId = id,
                    PublishedBy = ac.LoggedInUser.UserId,
                    PublishedDate = DateTime.Now
                };
                db.PublishedCampaigns.Add(publishCampaign);

                //insert into user inbox
                //get all the users for the group and insert record in usercampaign table, default will b null
                var campaignUsers = db.Groups.Find(id).UserCampaigns;
                foreach (var item in campaignUsers)
                {
                    db.UserCampaigns.Add(new UserCampaign {
                        UserId = item.UserId,
                        GroupId = item.GroupId,
                        CampaignId = campaign.CampaignId                        
                    });
                }
                
                if(!campaignUsers.Any(d =>d.UserId == ac.LoggedInUser.UserId))
                    db.UserCampaigns.Add(new UserCampaign
                    {
                        UserId = ac.LoggedInUser.UserId,
                        GroupId = id,
                        CampaignId = campaign.CampaignId
                    });

                var outcome = db.SaveChanges();
                return ToJson(outcome);
            }
            return ToJson(false);
        }

        [HttpGet]
        [Route("videos")]
        public HttpResponseMessage GetVideos()
        {
            return ToJson(db.Videos.AsEnumerable());
        }
        
        [HttpGet]
        [Route("inbox")]
        public HttpResponseMessage GetUserInbox()
        {
            var ac = new AccountController();
            return ToJson(db.UserCampaigns.Where(d => d.UserId == ac.LoggedInUser.UserId));
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