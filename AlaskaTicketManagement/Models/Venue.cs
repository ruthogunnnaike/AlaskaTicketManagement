using System.ComponentModel.DataAnnotations;

namespace AlaskaTicketManagement.Models
{
    public class Venue
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public int Capacity { get; set; }

        [StringLength(250)]
        [Required]
        public string Location { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
    }
}
