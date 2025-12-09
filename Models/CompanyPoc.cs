using System.ComponentModel.DataAnnotations.Schema;

namespace Infusive_back.Models
{
  public class CompanyPoc : Base
  {
    public required string Name { get; set; }
    public int CompanyId { get; set; }
    [ForeignKey(nameof(Id))]
    public virtual Company? Company { get; set; }
    public string? Email { get; set; }
    public string? PhoneCountryCode { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Whatsapp { get; set; }
    public string? Designation { get; set; }
    public string? LinkedinUrl { get; set; }
  }
}
