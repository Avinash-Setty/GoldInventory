using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GoldInventory.Model
{
    public class ItemAttribute
    {
        public string Id { get; set; }

        public string ItemId { get; set; }

        public string AttributeId { get; set; }

        public string Value { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}