using Infusive_back.EntityData;
using Infusive_back.Models;

namespace Infusive_back.JwtAuth
{
    public class CheckPermission
    {
        private readonly MyDbContext _db;

        public CheckPermission(MyDbContext db)
        {
            _db = db;
        }


        /// Check if user has permission for a page or feature
        public bool Role_Granted(int loginId, string? pageName, string? featureName = null)
        {
            if (pageName == null) return false;

            // Get user roles
            var roles = _db.UserRole
                .Where(x => x.UserId == loginId)
                .Select(x => x.RoleId)
                .ToList();

            if (!roles.Any()) return false;

            // Find Page Id
            int pageId = _db.Page
                .Where(p => !p.IsFeature && p.Name == pageName)
                .Select(p => p.Id)
                .SingleOrDefault();

            // If feature check is requested → find child page
            if (featureName != null)
            {
                pageId = _db.Page
                    .Where(p => p.ParentId == pageId && p.IsFeature && p.Name == featureName)
                    .Select(p => p.Id)
                    .SingleOrDefault();
            }

            if (pageId <= 0) return false;

            // Check permission
            return _db.Permission
                .Where(p => roles.Contains(p.RoleId) && p.PageId == pageId)
                .Any();
        }



        /// Get all permissions for a user as object/list
        public object Permission_Granted(int loginId)
        {
            // Get role IDs of user
            var roles = _db.UserRole
                .Where(x => x.UserId == loginId)
                .Select(x => x.RoleId)
                .ToList();

            if (!roles.Any()) return new { };

            // Return list of pages & permissions
            var allowedPermissions = _db.Permission
                .Where(p => roles.Contains(p.RoleId))
                .Select(p => new
                {
                    p.PageId,
                    PageName = p.Page!.Name,
                    p.RoleId
                })
                .ToList();

            return allowedPermissions;
        }
    }
}