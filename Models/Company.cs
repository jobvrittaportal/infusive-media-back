using System.ComponentModel.DataAnnotations.Schema;

namespace Infusive_back.Models
{
    public class Company : Base
    {
        public required string CompanyName { get; set; }
        public int IndustrytypeId { get; set; }
        [ForeignKey(nameof(IndustrytypeId))]
        public virtual IndustryType? IndustryType { get; set; }
        public required string PhoneCountryCode { get; set; }
        public required string CompanyPhone { get; set; }
        public required string CompanyEmail { get; set; }
        public required string WebsiteUrl { get; set; }
        public required string Feid { get; set; }
        public required string PostalZipCode { get; set; }
        public bool Status { get; set; }
        public int? CountryId { get; set; }
        [ForeignKey(nameof(CountryId))]
        public virtual Country? Country { get; set; }
        public int? StateId { get; set; }
        [ForeignKey(nameof(StateId))]
        public virtual State? State { get; set; }
        public int? CityId { get; set; }
        [ForeignKey(nameof(CityId))]
        public virtual City? City { get; set; }
    }
}
