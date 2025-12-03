using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infusive_back.Models
{
    public class Permission : Base
    {
        public int PageId { get; set; }
        [ForeignKey(nameof(PageId))]
        public virtual Page? Page { get; set; }
        public int RoleId { get; set; }
        [ForeignKey(nameof(RoleId))]
        public virtual Role? Role { get; set; }
    }
}
