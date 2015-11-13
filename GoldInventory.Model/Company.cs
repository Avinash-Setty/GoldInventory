using System.ComponentModel.DataAnnotations;

namespace GoldInventory.Model
{
    public class Company
    {
        public string Id { get; set; }

        [Required]
        [StringLength(200, ErrorMessage = "Name should have minimum 4 Characters and maximum 200 charaters", MinimumLength = 4)]
        public string Name { get; set; }

        [Required]
        [StringLength(2000, ErrorMessage = "Description should have minimum 4 Characters and maximum 2000 charaters", MinimumLength = 4)]
        public string Description { get; set; }

        [Required]
        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string State { get; set; }

        [Required]
        public string Country { get; set; }
    }
}