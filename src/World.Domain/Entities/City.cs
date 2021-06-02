using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace company.world.Domain
{
    [Table("city")]
    public class City
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        public string Name { get; set; }
        [Required]
        public string CountryCode { get; set; }
        [Required]
        public string District { get; set; }
        [Required]
        public int? Population { get; set; }

        // jhipster-needle-entity-add-field - JHipster will add fields here, do not remove

        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            if (obj == null || GetType() != obj.GetType()) return false;
            var city = obj as City;
            if (city?.Id == null || city?.Id == 0 || Id == 0) return false;
            return EqualityComparer<long>.Default.Equals(Id, city.Id);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }

        public override string ToString()
        {
            return "City{" +
                    $"ID='{Id}'" +
                    $", Name='{Name}'" +
                    $", CountryCode='{CountryCode}'" +
                    $", District='{District}'" +
                    $", Population='{Population}'" +
                    "}";
        }
    }
}
