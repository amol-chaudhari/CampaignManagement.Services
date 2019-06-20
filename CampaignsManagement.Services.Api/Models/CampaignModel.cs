using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CampaignsManagement.Services.Api.Models
{
    public class CampaignModel
    {
        public int CampaignId { get; set; }
        public string CampaignName { get; set; }
        public int ReferenceId { get; set; }
        public string Category { get; set; }
        public string Level { get; set; }
        public string Description { get; set; }
        public int? VideoLink { get; set; }
        public string VideoDetails { get; set; }
        public bool Active { get; set; }
        public string PublishedDate { get; set; }
        public string PublishedBy { get; set; }
    }

    public class SurveyModel
    {
        public int CampaignId { get; set; }
        public int SurveyId { get; set; }
        public string SurveyName { get; set; }
        public string Description { get; set; }
        public int Sequence { get; set; }
        public bool Active { get; set; }

        public List<SurveyQuestionModel> Questions { get; set; }
    }

    public class SurveyQuestionModel
    {
        public int QuestionId { get; set; }
        public string Description { get; set; }
        public int Sequence { get; set; }
        public string Choice1 { get; set; }
        public string Choice2 { get; set; }
        public string Choice3 { get; set; }
        public string Choice4 { get; set; }
        public string Answer { get; set; }
        public bool Active { get; set; }
    }
}