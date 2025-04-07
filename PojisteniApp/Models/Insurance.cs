using System.ComponentModel.DataAnnotations;

namespace PojisteniApp.Models;

public class Insurance
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Název pojištění je povinný.")]
    [StringLength(100)]
    public string Name { get; set; }

    [Required(ErrorMessage = "Název předmětu pojištění je povinný.")]
    [StringLength(50)]
    public string InsuranceObject { get; set; }

    [Required(ErrorMessage = "Musíš zadat částku.")]
    [Range(0, double.MaxValue, ErrorMessage = "Částka musí být kladná.")]
    public double Price { get; set; }

    [DataType(DataType.Date)]
    public DateTime EffectiveDate { get; set; } = DateTime.Today;

    [DataType(DataType.Date)]
    public DateTime ExpirationDate { get; set; } = DateTime.Today.AddYears(1);
    [Required]
    public int PolicyholderId { get; set; }
    public Policyholder? Policyholder { get; set; }
}
