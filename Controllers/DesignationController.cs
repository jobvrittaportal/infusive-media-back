using Infusive_back.EntityData;
using Infusive_back.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using static Infusive_back.Controllers.IndustryTypeController;

namespace Infusive_back.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DesignationController(MyDbContext db) : ControllerBase
    {
        readonly private MyDbContext db = db;

        [HttpGet]
        public IActionResult GetDesignation([FromQuery] int? skip, [FromQuery] int? limit, [FromQuery] string? search)
        {
            try
            {
                var query = (from ityp in db.Designation
                             select new GetDesignationDTO
                             {
                                 Id = ityp.Id,
                                 DesignationName = ityp.DesignationName ?? "",
                                 Status = ityp.Status ?? false,
                             });

                var totalCount = query.Count();

                var designations = query.OrderBy(u => u.Id).Skip(skip ?? 0).Take(limit ?? 10).ToList();

                return Ok(new { totalCount = totalCount, designations = designations });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpPost]
        [Authorize]
        public IActionResult AddDesignation([FromBody] PostEditDesignation pi)
        {
            try
            {
                int userId = Int32.Parse(User.FindFirst("userId")?.Value);

                Designation? alreadyExist = db.Designation.AsEnumerable().FirstOrDefault(f => Regex.Replace(f.DesignationName, @"\s+", "").ToLower() == Regex.Replace(pi.DesignationName, @"\s+", "").ToLower());

                if (alreadyExist != null)
                {
                    return BadRequest("Already Exist");
                }

                Designation newDesignation = new()
                {
                    DesignationName = pi.DesignationName,
                    CreatedAt = DateTime.Now,
                    CreatedBy = userId,
                };
                db.Designation.Add(newDesignation);
                db.SaveChanges();

                GetDesignationDTO response = new()
                {
                    Id = newDesignation.Id,
                    DesignationName = newDesignation.DesignationName,
                    Status = newDesignation.Status ?? false,
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Authorize]
        public IActionResult UpdateDesignation([FromBody] PostEditDesignation ei)
        {
            try
            {
                int userId = Int32.Parse(User.FindFirst("userId")?.Value);

                Designation? designationToUpdate = db.Designation.Find(ei.Id);
                if (designationToUpdate == null)
                    return NotFound("Designation not found");

                bool nameExists = db.Designation.AsEnumerable()
                    .Any(f => Regex.Replace(f.DesignationName, @"\s+", "").ToLower() == Regex.Replace(ei.DesignationName, @"\s+", "").ToLower() && f.Id != ei.Id);
                if (nameExists)
                    return BadRequest("Designation name already exists");

                designationToUpdate.DesignationName = ei.DesignationName;
                designationToUpdate.Status = ei.Status;
                designationToUpdate.UpdatedAt = DateTime.Now;
                designationToUpdate.UpdatedBy = userId;

                db.Designation.Update(designationToUpdate);
                db.SaveChanges();

                return Ok(new { Message = "Updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPut("toggleStatus/{id}")]
        [Authorize]
        public IActionResult ToggleIndustryTypeStatus(int id, [FromBody] ToggleStatus ts)
        {
            try
            {

                if (id != ts.Id) return BadRequest("Invalid Record Id");

                Designation? Existingdesignation = db.Designation.FirstOrDefault(f => f.Id == id);

                if (Existingdesignation == null) return NotFound("Record Not Found");

                Existingdesignation.Status = ts.Status;

                db.Designation.Update(Existingdesignation);
                db.SaveChanges();

                return Ok(new { Message = "Status Changed successfully" });

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public class GetDesignationDTO
        {
            public required int Id { get; set; }
            public required string DesignationName { get; set; }
            public required bool Status { get; set; }
        }
        public class PostEditDesignation
        {
            public int? Id { get; set; }
            public required string DesignationName { get; set; }
            public bool? Status { get; set; } = true;
        }
    }
}
