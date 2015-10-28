using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Model;
using System.Linq;
using System.Web;

namespace GoldInventory.Models
{
    public class ItemReview
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string CategoryName { get; set; }
    }
}