using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CampaignsManagement.Services.Api.Models
{
    public class AccountModel
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Department { get; set; }
        public int RoleId { get; set; }
        public int GroupId { get; set; }
        public bool Active { get; set; }
        public string LoggedOn { get; set; }
    }
}