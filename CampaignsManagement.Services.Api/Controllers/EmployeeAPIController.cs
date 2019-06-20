using System;
using System.Linq;
using System.Web.Http;
using CampaignsManagement.Services.Api.DAL;

namespace CampaignsManagement.Services.Api.Controllers
{
    [RoutePrefix("Api/Employee")]
    public class EmployeeAPIController : ApiController
    {
        CampaignEntities objEntity = new CampaignEntities();
        
        [HttpGet]
        [Route("AllEmployeeDetails")]
        public IQueryable<CampaignUser> GetEmaployee()
        {
            try
            {
                return objEntity.CampaignUsers;
            }
            catch(Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("GetEmployeeDetailsById/{employeeId}")]
        public IHttpActionResult GetEmaployeeById(string employeeId)
        {
            var objEmp = new CampaignUser();
            int ID = Convert.ToInt32(employeeId);
            try
            {
                 objEmp = objEntity.CampaignUsers.Find(ID);
                if (objEmp == null)
                {
                    return NotFound();
                }

            }
            catch (Exception)
            {
                throw;
            }
           
            return Ok(objEmp);
        }

        [HttpPost]
        [Route("InsertEmployeeDetails")]
        public IHttpActionResult PostEmaployee(CampaignUser data)
        {
            
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                objEntity.CampaignUsers.Add(data);
                objEntity.SaveChanges();
            }
            catch(Exception)
            {
                throw;
            }



            return Ok(data);
        }
        
        [HttpPut]
        [Route("UpdateEmployeeDetails")]
        public IHttpActionResult PutEmaployeeMaster(CampaignUser employee)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var objEmp = new CampaignUser();
                objEmp = objEntity.CampaignUsers.Find(employee.UserId);
                if (objEmp != null)
                {
                    //objEmp.EmpName = employee.EmpName;
                    //objEmp.Address = employee.Address;
                    //objEmp.EmailId = employee.EmailId;
                    //objEmp.DateOfBirth = employee.DateOfBirth;
                    //objEmp.Gender = employee.Gender;
                    //objEmp.PinCode = employee.PinCode;
                }
                int i = this.objEntity.SaveChanges();

            }
            catch(Exception)
            {
                throw;
            }
            return Ok(employee);
        }
        [HttpDelete]
        [Route("DeleteEmployeeDetails")]
        public IHttpActionResult DeleteEmaployeeDelete(int id)
        {
            //int empId = Convert.ToInt32(id);
            var emaployee = objEntity.CampaignUsers.Find(id);
            if (emaployee == null)
            {
                return NotFound();
            }

            objEntity.CampaignUsers.Remove(emaployee);
            objEntity.SaveChanges();

            return Ok(emaployee);
        }
    }
}
