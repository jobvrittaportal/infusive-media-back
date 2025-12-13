using System.ComponentModel.DataAnnotations.Schema;

namespace Infusive_back.Models
{
    public class Lead : Base
    {
        public int? PocId { get; set; }
        [ForeignKey(nameof(PocId))]
        public virtual CompanyPoc? POC { get; set; }
        public int? AssignedToUserId { get; set; }
        [ForeignKey(nameof(AssignedToUserId))]
        public virtual User_Details? AssignedTo { get; set; }
        public required int SourceId { get; set; }
        [ForeignKey(nameof(SourceId))]
        public virtual Source Source { get; set; }
        public required int StatusId { get; set; }
        [ForeignKey(nameof(StatusId))]
        public virtual Status Status { get; set; }
        public int? InternalUserId { get; set; }
        [ForeignKey(nameof(InternalUserId))]
        public virtual User_Details? InternalUser { get; set; }

        public int? TransferFrom { get; set; }
        public DateTime? TransferDate { get; set; }
        //public virtual List<LeadStatusUpdateHistory>? UpdateHistories { get; set; }
    }
}
