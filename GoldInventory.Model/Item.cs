using System;
using System.ComponentModel.DataAnnotations;

namespace GoldInventory.Models
{
    public class Item
    {
        public string Id { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        [Range(1,1000)]
        [Required]
        public int StoneWeight { get; set; }

        [Range(1, 1000)]
        [Required]
        public int ItemWeight { get; set; }

        [Required]
        [StringLength(512)]
        public string Name { get; set; }
    }
}