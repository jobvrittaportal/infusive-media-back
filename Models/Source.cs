namespace Infusive_back.Models
{
    public class Source : Base
    {
        public required string SourceName { get; set; }
        public bool? Status { get; set; } = true;

    }
}
