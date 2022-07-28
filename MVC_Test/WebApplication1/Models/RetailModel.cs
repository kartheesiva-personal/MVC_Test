using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class RetailModel
    {
        public Int64 Id { get; set; }
        public Guid UniqueId { get; set; }
        public string ECS { get; set; }
        public string BCat { get; set; }
        public string Series { get; set; }
        public int Count { get; set; }

        public int Jan { get; set; }
        public int Feb { get; set; }
        public int Mar { get; set; }
        public int Apr { get; set; }
        public int May { get; set; }
        public int Jun { get; set; }
        public int Jul { get; set; }
        public int Aug { get; set; }
        public int Sep { get; set; }
        public int Oct { get; set; }
        public int Nov { get; set; }
        public int Dec { get; set; }
        public int Total { get; set; }
        public DateTime CreatedDate { get; set; }
        public string SystemIp { get; set; }
    }
}