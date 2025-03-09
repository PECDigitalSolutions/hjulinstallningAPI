using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HjulinstallningAPI.Models
{
    public class WowCharacter
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Id", TypeName = "int")]
        public int Id { get; set; }

        [Required]
        public string? Name { get; set; }

        [Required]
        public string? Race { get; set; }  // Example: "Orc", "Human"

        [Required]
        public string? Class { get; set; }  // Example: "Warrior", "Mage"

        [Required]
        public string? ArmorType { get; set; }  // Example: "Plate", "Leather", "Mail", "Cloth"

        [Required]
        public int Level { get; set; }  // Character Level

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
