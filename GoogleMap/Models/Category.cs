//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GoogleMap.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Category
    {
        public Category()
        {
            this.SubCategories = new HashSet<SubCategory>();
        }
    
        public System.Guid Id { get; set; }
        public string Name { get; set; }
        public string CategoryIconType { get; set; }
        public System.Guid LayerId { get; set; }
    
        public virtual Layer Layer { get; set; }
        public virtual ICollection<SubCategory> SubCategories { get; set; }
    }
}
