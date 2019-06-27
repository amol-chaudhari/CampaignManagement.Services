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
    [RoutePrefix("api/Surveys")]
    public class SurveysController : ApiController
    {
        protected HttpResponseMessage ToJson(dynamic obj)
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");
            return response;
        }

        private CampaignEntities db = new CampaignEntities();
                
        // GET: api/Surveys/campaign/5
        [HttpGet]
        [Route("campaign/{id}")]
        [ResponseType(typeof(Survey))]
        public IHttpActionResult GetCampaign(int id)
        {
            Survey survey = db.Surveys.FirstOrDefault(x => x.CampaignId == id);
            if (survey == null)
            {
                return NotFound();
            }

            return Ok(survey);
        }

        [HttpPost]
        [Route("insert")]
        [ResponseType(typeof(Survey))]
        public HttpResponseMessage PostSurvey([FromBody]Survey survey)
        {
            if (survey.SurveyId == 0)
                db.Surveys.Add(survey);
            var outcome = db.SaveChanges();
            return ToJson(outcome);
        }

        [HttpPost]
        [Route("question")]
        [ResponseType(typeof(SurveyQuestion))]
        public HttpResponseMessage PostQuestion([FromBody]SurveyQuestion question)
        {
            if (question.QuestionId == 0)
                db.SurveyQuestions.Add(question);
            else
            {
                var qRecord = db.SurveyQuestions.Find(question.QuestionId);
                if (qRecord != null)
                {
                    qRecord.Active = question.Active;
                    qRecord.Description = question.Description;
                    qRecord.Choice1 = question.Choice1;
                    qRecord.Choice2 = question.Choice2;
                    qRecord.Choice3 = question.Choice3;
                    qRecord.Choice4 = question.Choice4;
                    qRecord.Answer = question.Answer;
                }
            }
            var outcome = db.SaveChanges();
            var result = new { Succeeded = outcome, List = db.SurveyQuestions.Where(d => d.SurveyId == question.SurveyId).AsEnumerable() };
            return ToJson(result);
        }

        [HttpDelete]
        [Route("removequestion")]
        [ResponseType(typeof(SurveyQuestion))]
        public HttpResponseMessage RemoveQuestion(int id)
        {
            var question = db.SurveyQuestions.Find(id);
            if (question == null)
                return null;
            var outcome = question.Active == false ? 1 : 0;
            if (outcome == 0)
            {
                question.Active = false;
                outcome = db.SaveChanges();
            }
            var result = new { Succeeded = outcome, List = db.SurveyQuestions.Where(d => d.SurveyId == question.SurveyId).AsEnumerable() };
            return ToJson(result);
        }

        [HttpPost]
        [Route("submitsurvey")]
        [ResponseType(typeof(bool))]
        public HttpResponseMessage SubmitSurvey([FromBody]UserQuestionDetail[] userAnswers)
        {
            var ac = new AccountController();
            var question = new SurveyQuestion();
            var surveyId = 0;

            foreach (var answer in userAnswers)
            {
                question = db.SurveyQuestions.Find(answer.QuestionId);
                if(surveyId == 0) surveyId = question.SurveyId;

                db.UserQuestionDetails.Add(new UserQuestionDetail {
                    QuestionId = answer.QuestionId,
                    Skipped = answer.Skipped,
                    SelectedChoice = answer.SelectedChoice,
                    UserId = ac.LoggedInUser.UserId,
                    IsCorrect = question != null ? question.Answer.ToLower().Equals(answer.SelectedChoice?.ToLower()) : false
                });
            }

            //update user campaign table to mark it completed.
            var survey = db.Surveys.Find(surveyId);
            var campaignId = survey?.CampaignId;

            var userCampaign = db.UserCampaigns.FirstOrDefault(d => d.CampaignId == campaignId && d.UserId == ac.LoggedInUser.UserId);
            userCampaign.VideoDate = DateTime.Now;
            userCampaign.VideoWatched = true;
            userCampaign.CompletedDate = DateTime.Now;
            db.Entry(userCampaign).State = EntityState.Modified;

            var outcome = db.SaveChanges();
            return ToJson(outcome);
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