using Infusive_back.EntityData;
using Infusive_back.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Infusive_back.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeadController(MyDbContext db) : ControllerBase
    {
        readonly private MyDbContext db = db;

        [HttpGet]
        [Authorize]
        public IActionResult GetLead([FromQuery] int? skip, [FromQuery] int? limit, [FromQuery] string? search)
        {
            try
            {
                var query = from lead in db.Lead
                    join poc in db.CompanyPoc on lead.PocId equals poc.Id
                    join comp in db.Company on poc.CompanyId equals comp.Id
                    join st in db.Status on lead.StatusId equals st.Id
                    join so in db.Source on lead.SourceId equals so.Id
                    join desg in db.Designation on poc.DesignationId equals desg.Id
                    // Assigned To User
                    join assignedUser in db.User_Details on lead.AssignedToUserId equals assignedUser.Id into assignedJoin
                    from assignedUser in assignedJoin.DefaultIfEmpty()
                    // Created By User
                    join createdUser in db.User_Details on lead.CreatedBy equals createdUser.Id into createdJoin
                    from createdUser in createdJoin.DefaultIfEmpty()
                    select new
                    {
                        LeadId = lead.Id,
                        PocName = poc.Name,
                        PocId = lead.PocId,
                        CompanyName = comp.CompanyName,
                        CompanyId = comp.Id,
                        Source = so.SourceName,
                        SourceId = lead.SourceId,
                        Status = st.StatusName,
                        StatusId = lead.StatusId,
                        Designation = desg.DesignationName,
                        DesignationId = poc.DesignationId,

                        AssignedToUser = assignedUser != null ? assignedUser.Name : null,
                        AssignedToUserId = lead.AssignedToUserId,

                        CreatedBy = createdUser != null ? createdUser.Name : null,
                        CreatedById = lead.CreatedBy,

                        CountryCode = poc.PhoneCountryCode,
                        Contact = poc.PhoneNumber,
                        Email = poc.Email,
                        CreatedAt = lead.CreatedAt
                    };

                if (!string.IsNullOrEmpty(search))
                {
                    var lower = search.ToLower();
                    query = query.Where(u =>
                        u.CompanyName.ToLower().Contains(lower) ||
                        u.PocName.ToLower().Contains(lower)
                    );
                }

                var totalCount = query.Count();
                var leads = query.OrderBy(u => u.LeadId).Skip(skip ?? 0).Take(limit ?? 10).ToList();

                return Ok(new { totalCount, leads });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddLead([FromBody] AddLeadDTO leadDTO)
        {
            try
            {
                int userId = Int32.Parse(User.FindFirst("userId")?.Value);

                if (leadDTO.PocId == null || !leadDTO.PocId.Any())
                    return BadRequest("At least one POC Id is required");

                List<Lead> newLead = new();

                foreach (var poc in leadDTO.PocId)
                {
                    var lead = new Lead
                    {
                        PocId = poc,
                        SourceId = leadDTO.SourceId,
                        StatusId = leadDTO.StatusId,
                        AssignedToUserId = leadDTO.UserId,
                        CreatedAt = DateTime.Now,
                        CreatedBy = userId,
                    };
                    newLead.Add(lead);
                }

                await db.Lead.AddRangeAsync(newLead);
                await db.SaveChangesAsync();

                return Ok(new { Message = "Lead Generated Successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("MyLeads")]
        [Authorize]
        public IActionResult GetMyLead([FromQuery] int? skip, [FromQuery] int? limit, [FromQuery] string? search)
        {
            try
            {
                int userId = Int32.Parse(User.FindFirst("userId")?.Value);

                var query = from lead in db.Lead
                            join poc in db.CompanyPoc on lead.PocId equals poc.Id
                            join comp in db.Company on poc.CompanyId equals comp.Id
                            join st in db.Status on lead.StatusId equals st.Id
                            join so in db.Source on lead.SourceId equals so.Id
                            join desg in db.Designation on poc.DesignationId equals desg.Id
                            // Assigned To User
                            join assignedUser in db.User_Details on lead.AssignedToUserId equals assignedUser.Id into assignedJoin
                            from assignedUser in assignedJoin.DefaultIfEmpty()
                                // Created By User
                            join createdUser in db.User_Details on lead.CreatedBy equals createdUser.Id into createdJoin
                            from createdUser in createdJoin.DefaultIfEmpty()
                            select new
                            {
                                LeadId = lead.Id,
                                PocName = poc.Name,
                                PocId = lead.PocId,
                                CompanyName = comp.CompanyName,
                                CompanyId = comp.Id,
                                Source = so.SourceName,
                                SourceId = lead.SourceId,
                                Status = st.StatusName,
                                StatusId = lead.StatusId,
                                Designation = desg.DesignationName,
                                DesignationId = poc.DesignationId,

                                AssignedToUser = assignedUser != null ? assignedUser.Name : null,
                                AssignedToUserId = lead.AssignedToUserId,

                                CountryCode = poc.PhoneCountryCode,
                                CreatedBy = createdUser != null ? createdUser.Name : null,
                                CreatedById = lead.CreatedBy,

                                Contact = poc.PhoneNumber,
                                Email = poc.Email,
                                CreatedAt = lead.CreatedAt
                            };

                var MyLeads = query.Where(l => l.AssignedToUserId == userId);

                if (!string.IsNullOrEmpty(search))
                {
                    var lower = search.ToLower();
                    MyLeads = MyLeads.Where(u =>
                        u.CompanyName.ToLower().Contains(lower) ||
                        u.PocName.ToLower().Contains(lower)
                    );
                }

                var totalCount = MyLeads.Count();
                var Leads = MyLeads.OrderBy(u => u.LeadId).Skip(skip ?? 0).Take(limit ?? 10).ToList();

                return Ok(new { totalCount, Leads });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        public class AddLeadDTO
        {
            public required List<int> PocId { get; set; }
            public required int SourceId { get; set; }
            public required int StatusId { get; set; }
            public required int UserId { get; set; }
        }
    }
}
