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
    
    public partial class MarkerComment
    {
        public System.Guid Id { get; set; }
        public string UserName { get; set; }
        public string CommentText { get; set; }
        public System.Guid MarkerId { get; set; }
        public System.DateTime CreatedDate { get; set; }
    
        public virtual Marker Marker { get; set; }
    }
}
