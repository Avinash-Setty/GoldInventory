﻿using System;
using System.ComponentModel.DataAnnotations;

namespace GoldInventory.Model
{
    public class CustomAttribute
    {
        public string Id { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Type")]
        public string Type { get; set; }

        public string CompanyId { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}