//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ShoePOSProject.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class NewOption
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Price { get; set; }
        public Nullable<int> InvId { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<int> IsActive { get; set; }
        public Nullable<System.DateTime> CreatedAt { get; set; }
        public Nullable<int> Items { get; set; }
    
        public virtual User User { get; set; }
    }
}
