using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GoldInventory.Model
{
    public class CustomAttribute
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public string CompanyId { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}