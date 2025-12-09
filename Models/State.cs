using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Infusive_back.Models
{
  public class State : Base
  {
    [MaxLength(255)]
    public required string Name { get; set; }
    public required int CountryId { get; set; }
    [ForeignKey(nameof(CountryId))]
    public virtual Country Country { get; set; }
    public virtual ICollection<City>? Cities { get; set; }
  }
}
