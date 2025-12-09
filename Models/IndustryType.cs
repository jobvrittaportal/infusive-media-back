namespace Infusive_back.Models
{
    public class IndustryType : Base
    {
        public required string IndustryName { get; set; }
        public bool? Status { get; set; } = true;

    }
}
