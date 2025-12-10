namespace Infusive_back.Models
{
    public class Designation : Base
    {
        public required string DesignationName { get; set; }
        public bool? Status { get; set; } = true;
    }
}
