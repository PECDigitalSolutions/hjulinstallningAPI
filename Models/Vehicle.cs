using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HjulinstallningAPI.Models
{
    public class Vehicle
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Id", TypeName = "int")]
        public int Id { get; set; }

        public string? LicensePlate { get; set; }
        public string? Make { get; set; }
        public string? Model { get; set; }
        public int Year { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime LastUpdated { get; set; }
    }
}
