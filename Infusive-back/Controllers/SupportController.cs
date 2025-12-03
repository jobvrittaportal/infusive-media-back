using Infusive_back.EntityData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;
using static Infusive_back.Controllers.RoleController;

namespace Infusive_back.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupportController : ControllerBase
    {

        private async Task<string> GetHrlenseToken()
        {
            using var client = new HttpClient();

            var response = await client.PostAsync(
                $"https://hrms-demo.jobvritta.com/api/DropDown/generateToken?employee_Code=120",
                null
            );

            if (!response.IsSuccessStatusCode)
                throw new Exception("Failed to generate HRMS token");

            var json = await response.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
            return json.GetProperty("token").GetString();
        }


        [HttpGet("ApplicationGroup")]
        [Authorize]
        public async Task<IActionResult> ApplicationGroupDropDown()
        {
            try
            {
                string token = await GetHrlenseToken();

                using var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                var response = await client.GetAsync(
                    "https://hrms-demo.jobvritta.com/api/DropDown/ApplicationGroup"
                );

                var result = await response.Content.ReadAsStringAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        [Route("supportAssignGroup")]
        [Authorize]
        public async Task<IActionResult> SupportGroup()
        {
            try
            {
                string token = await GetHrlenseToken();

                using var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                var response = await client.GetAsync(
                    "https://hrms-demo.jobvritta.com/api/DropDown/supportAssignGroup"
                );

                var result = await response.Content.ReadAsStringAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPost("CreateTicket")]
        [Authorize]
        public async Task<IActionResult> CreateTicket([FromForm] CreateSupportTicketDto dto)
        {
            try
            {
                string token = await GetHrlenseToken();

                using var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                var formData = new MultipartFormDataContent
                {
                    { new StringContent(dto.Application_Id.ToString()), "Application_Id" },
                    { new StringContent(dto.Priority), "Priority" },
                    { new StringContent(dto.Impact), "Impact" },
                    { new StringContent(dto.Assign_Group_Id.ToString()), "Assign_Group_Id" },
                    { new StringContent(dto.Subject), "Subject" },
                    { new StringContent(dto.Desc), "Desc" }
                };

                if (dto.Screenshot != null)
                {
                    var stream = dto.Screenshot.OpenReadStream();
                    formData.Add(new StreamContent(stream), "Screenshot", dto.Screenshot.FileName);
                }

                var response = await client.PostAsync(
                    "https://hrms-demo.jobvritta.com/api/Support/CreateTicket",
                    formData
                );

                var result = await response.Content.ReadAsStringAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("getTicket")]
        [Authorize]
        public async Task<IActionResult> GetTickets([FromQuery] string? filter, [FromQuery] string? lazyParams)
        {
            try
            {
                string token = await GetHrlenseToken();

                using var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                string url =
                    $"https://hrms-demo.jobvritta.com/api/Support/getTicket" +
                    $"?filter={Uri.EscapeDataString(filter ?? "")}" +
                    $"&lazyParams={Uri.EscapeDataString(lazyParams ?? "")}";

                var response = await client.GetStringAsync(url);

                var json = JsonDocument.Parse(response).RootElement;

                return Ok(new
                {
                    totalCount = json.GetProperty("totalCount").GetInt32(),
                    result = json.GetProperty("result")
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("getTicketHistory")]
        [Authorize]
        public async Task<IActionResult> GetTicketHistory(int ticket_Id)
        {
            try
            {
                string token = await GetHrlenseToken();

                using var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                string url = $"https://hrms-demo.jobvritta.com/api/Support/getTicketHistory?ticket_Id={ticket_Id}";

                var response = await client.GetAsync(url);
                string jsonString = await response.Content.ReadAsStringAsync();

                var json = JsonDocument.Parse(jsonString).RootElement;
                return Ok(json);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }



        public class CreateSupportTicketDto
        {
            public int Application_Id { get; set; }
            public string Priority { get; set; }
            public string Impact { get; set; }
            public int Assign_Group_Id { get; set; }
            public string Subject { get; set; }
            public string Desc { get; set; }
            public IFormFile? Screenshot { get; set; }
        }
    }
}
