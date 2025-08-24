using Microsoft.AspNetCore.Mvc;
using Setur.Contacts.MessageBus.Services;

namespace Setur.Contacts.ReportApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class KafkaController : ControllerBase
{
    private readonly KafkaAdminService _kafkaAdminService;

    public KafkaController(KafkaAdminService kafkaAdminService)
    {
        _kafkaAdminService = kafkaAdminService;
    }

    // GET: api/kafka/topics
    [HttpGet("topics")]
    public async Task<IActionResult> GetTopics()
    {
        try
        {
            var topics = await _kafkaAdminService.ListTopicsAsync();
            return Ok(new { Success = true, Data = topics });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Success = false, Message = ex.Message });
        }
    }

    // POST: api/kafka/topics/ensure
    [HttpPost("topics/ensure")]
    public async Task<IActionResult> EnsureTopic()
    {
        try
        {
            await _kafkaAdminService.EnsureTopicExistsAsync();
            return Ok(new { Success = true, Message = "Topic başarıyla oluşturuldu/doğrulandı" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Success = false, Message = ex.Message });
        }
    }

    // DELETE: api/kafka/topics/{topicName}
    [HttpDelete("topics/{topicName}")]
    public async Task<IActionResult> DeleteTopic(string topicName)
    {
        try
        {
            await _kafkaAdminService.DeleteTopicAsync(topicName);
            return Ok(new { Success = true, Message = $"Topic '{topicName}' başarıyla silindi" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Success = false, Message = ex.Message });
        }
    }
}
