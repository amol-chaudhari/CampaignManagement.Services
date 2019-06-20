//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CampaignsManagement.Services.Api.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class CampaignUser
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CampaignUser()
        {
            this.PublishedCampaigns = new HashSet<PublishedCampaign>();
            this.UserQuestionDetails = new HashSet<UserQuestionDetail>();
        }
    
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Department { get; set; }
        public Nullable<int> EmployeeId { get; set; }
        public string Phone { get; set; }
        public int GroupId { get; set; }
        public int RoleId { get; set; }
        public bool Active { get; set; }
    
        public virtual CampaignRole CampaignRole { get; set; }
        public virtual Group Group { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PublishedCampaign> PublishedCampaigns { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserQuestionDetail> UserQuestionDetails { get; set; }
    }
}