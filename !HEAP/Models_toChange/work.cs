//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GoMySql.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class work
    {
        public int id_work { get; set; }
        public Nullable<int> id_variant { get; set; }
        public Nullable<System.DateTime> date { get; set; }
        public string code { get; set; }
        public string report { get; set; }
        public Nullable<sbyte> test { get; set; }
        public Nullable<sbyte> result { get; set; }
        public Nullable<System.DateTime> date_arr { get; set; }
    }
}