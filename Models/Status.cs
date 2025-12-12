namespace Infusive_back.Models
{
    public class Status : Base
    {
        public required string StatusName { get; set; }
        public bool? IsActive { get; set; } = true;

    }
}
