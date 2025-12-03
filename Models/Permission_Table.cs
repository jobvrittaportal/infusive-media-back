using System.ComponentModel.DataAnnotations.Schema;

namespace Infusive_back.Models
{
    public class Permission_Table : Base
    {
        public required int Roles_ID { get; set; }
        [ForeignKey(nameof(Roles_ID))]
        public virtual Role? Role_Mast { get; set; }
        public required string Permissions { get; set; }

    }
}