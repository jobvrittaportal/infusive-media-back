using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Infusive_back.Models
{
  public class City : Base
  {

    [MaxLength(255)]
    public required string Name { get; set; }
    public required int StateId { get; set; }
    [ForeignKey(nameof(StateId))]
    public virtual State State { get; set; }
    public string? Description { get; set; }
  }
}
