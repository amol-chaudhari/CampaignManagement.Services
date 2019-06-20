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
    
    public partial class Campaign
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Campaign()
        {
            this.Surveys = new HashSet<Survey>();
        }
    
        public int CampaignId { get; set; }
        public string CampaignName { get; set; }
        public int ReferenceId { get; set; }
        public string Category { get; set; }
        public string Level { get; set; }
        public string Description { get; set; }
        public Nullable<int> VideoLink { get; set; }
        public string VideoDetails { get; set; }
        public bool Active { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Survey> Surveys { get; set; }
        public virtual Video Video { get; set; }
    }
}