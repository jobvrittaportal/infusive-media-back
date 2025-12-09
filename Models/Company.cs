using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Metrics;

namespace Infusive_back.Models
{
  public class Company : Base
  {
    public required string CompanyName { get; set; }
    public int IndustrytypeId { get; set; }
    [ForeignKey(nameof(Id))]
    public virtual IndustryType? IndustryType { get; set; }
    public required string PhoneCountryCode { get; set; }
    public required string CompanyPhone { get; set; }
    public required string CompanyEmail { get; set; }
    public required string WebsiteUrl { get; set; }
    public required string Feid { get; set; }
    public required string AddressLine { get; set; }
    public required string PostalZipCode { get; set; }
    public bool Status { get; set; }

  }
}
