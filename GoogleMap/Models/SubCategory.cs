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
    
    public partial class SubCategory
    {
        public SubCategory()
        {
            this.Markers = new HashSet<Marker>();
        }
    
        public System.Guid Id { get; set; }
        public System.Guid CategoryId { get; set; }
        public string Name { get; set; }
        public string SubCategoryIconType { get; set; }
    
        public virtual Category Category { get; set; }
        public virtual ICollection<Marker> Markers { get; set; }
    }
}
