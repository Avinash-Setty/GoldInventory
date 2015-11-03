using System;
using System.ComponentModel.DataAnnotations;

namespace GoldInventory.Model
{
    public class ItemCategory
    {
        public string Id { get; set; }

        [Required]
        [StringLength(512)]
        public string Name { get; set; }

        [Required]
        [StringLength(512)]
        public string Description { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public string CompanyId { get; set; }
    }
}