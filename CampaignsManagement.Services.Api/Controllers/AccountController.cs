using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using CampaignsManagement.Services.Api.Models;
using Newtonsoft.Json;
using System.Text;
using CampaignsManagement.Services.Api.DAL;
using System.Web.Http.Description;
using System.Data.Entity;

namespace CampaignsManagement.Services.Api.Controllers
{
    public class AccountController : ApiController
    {
        public AccountController()
            : this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
        {
        }

        public AccountController(UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
        }

        protected HttpResponseMessage ToJson(dynamic obj)
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");
            return response;
        }

        internal AccountModel LoggedInUser
        {
            get
            {
                var identityClaims = (ClaimsIdentity)User.Identity;
                IEnumerable<Claim> claims = identityClaims.Claims;
                var email = identityClaims.FindFirst("Email").Value;
                var user = db.CampaignUsers.FirstOrDefault(x => x.Email.ToLower().Equals(email.ToLower()));

                return new AccountModel
                {
                    Email = email,
                    UserId = user == null ? 0 : user.UserId,
                    RoleId = user == null ? 0 : user.RoleId,
                    GroupId = user == null ? 0 : user.GroupId,
                    Department = user?.Department,
                    Active = user == null ? false : user.Active,
                    FirstName = user?.FirstName,
                    LastName = user?.LastName
                };
            }
        }

        internal CampaignUser ToEntity(AccountModel model)
        {
            return new CampaignUser
            {
                UserId = model.UserId,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Department = model.Department,
                RoleId = model.RoleId,
                GroupId = model.GroupId,
                Active = model.Active
            };
        }

        private CampaignEntities db = new CampaignEntities();

        public UserManager<ApplicationUser> UserManager { get; private set; }

        [Route("api/User/Register")]
        [HttpPost]
        [AllowAnonymous]
        public IdentityResult Register(AccountModel model)
        {
            var user = new ApplicationUser() { UserName = model.UserName, Email = model.Email, FirstName = model.FirstName, LastName = model.LastName };
            var result = UserManager.Create(user, model.Password);
            if (result.Succeeded)//means user creation was successful
            {
                CampaignUser cUser = new CampaignUser
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Department = model.Department,
                    RoleId = model.RoleId,
                    Active = model.Active
                };
                db.CampaignUsers.Add(cUser);
                db.SaveChanges();
            }
            return result;
        }

        [HttpGet]
        [Route("api/GetUserClaims")]
        public AccountModel GetUserClaims()
        {
            var identityClaims = (ClaimsIdentity)User.Identity;
            IEnumerable<Claim> claims = identityClaims.Claims;
            AccountModel model = new AccountModel()
            {
                Email = identityClaims.FindFirst("Email").Value,
                FirstName = identityClaims.FindFirst("FirstName").Value,
                LastName = identityClaims.FindFirst("LastName").Value,
                LoggedOn = identityClaims.FindFirst("LoggedOn").Value//,
                                                                     //   Active = Convert.ToBoolean(identityClaims.FindFirst("Active").Value)
            };

            //return model.Active ? model : null;
            return model;
        }

        [HttpGet]
        [Route("api/GetUsers")]
        public HttpResponseMessage GetUsers()
        {
            //var identityClaims = (ClaimsIdentity)User.Identity;
            //IEnumerable<Claim> claims = identityClaims.Claims;
            //AccountModel model = new AccountModel()
            //{
            //    UserName = identityClaims.FindFirst("Username").Value,
            //    Email = identityClaims.FindFirst("Email").Value,
            //    FirstName = identityClaims.FindFirst("FirstName").Value,
            //    LastName = identityClaims.FindFirst("LastName").Value,
            //    LoggedOn = identityClaims.FindFirst("LoggedOn").Value,
            //    Active = Convert.ToBoolean(identityClaims.FindFirst("Active").Value)
            //};
            var users = new List<AccountModel>();
            foreach (var user in db.CampaignUsers.AsEnumerable())
            {
                var identity = UserManager.FindByEmail(user.Email);
                users.Add(new AccountModel
                {
                    UserId = user.UserId,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    UserName = identity.UserName,
                    Email = user.Email,
                    Department = user.Department,
                    RoleId = user.RoleId,
                    Active = user.Active
                });
            }
            return ToJson(users);
        }

        [HttpPut]
        [Route("update/{id}")]
        [ResponseType(typeof(void))]
        public HttpResponseMessage UpdateUser(int id, [FromBody]AccountModel model)
        {
            if (id <= 0) return ToJson(false);
            var user = ToEntity(model);
            db.Entry(user).State = EntityState.Modified;
            return ToJson(db.SaveChanges());
        }

    }
}