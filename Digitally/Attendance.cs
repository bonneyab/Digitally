//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Digitally
{
    using System;
    using System.Collections.Generic;
    
    public partial class Attendance
    {
        public int id { get; set; }
        public System.DateTime date { get; set; }
        public string type { get; set; }
        public int total { get; set; }
        public Nullable<int> group_id { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
    }
}