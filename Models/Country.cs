using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Infusive_back.Models
{
  [Index(nameof(Name), IsUnique = true)]
  public class Country : Base
  {
    [MaxLength(255)]
    public required string Name { get; set; }
    public required string Code { get; set; }
    public string? FlagUrl { get; set; } //  Country Flag Image (URL or relative path)

    public required string DialCode { get; set; }
    public virtual ICollection<State>? States { get; set; }
  }
}
