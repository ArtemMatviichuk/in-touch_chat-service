using ChatService.Common.Dtos.Chat;
using ChatService.Data.Entity;
using ChatService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ChatService.Controller
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpGet("My")]
        public async Task<IActionResult> GetChats()
        {
            int authId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var response = await _chatService.GetChats(authId);

            return Ok(response);
        }

        [HttpGet("{chatId}", Name = "GetChatById")]
        public async Task<IActionResult> GetChat(int chatId)
        {
            int authId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var response = await _chatService.GetChat(authId, chatId);

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateChat([FromForm] CreateChatDto dto)
        {
            int authId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var chat = await _chatService.CreateChat(authId, dto);

            return CreatedAtRoute("GetChatById", new { chatId = chat.Id }, chat);
        }

        [HttpPut("{chatId}")]
        public async Task<IActionResult> UpdateChat(int chatId, [FromForm] UpdateChatDto dto)
        {
            int authId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _chatService.UpdateChat(authId, chatId, dto);

            return NoContent();
        }

        [HttpDelete("{chatId}")]
        public async Task<IActionResult> DeleteChat(int chatId)
        {
            int authId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _chatService.DeleteChat(authId, chatId);

            return NoContent();
        }

        [HttpDelete("{chatId}/Leave")]
        public async Task<IActionResult> LeaveChat(int chatId)
        {
            int authId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _chatService.LeaveChat(authId, chatId);

            return NoContent();
        }

        [HttpGet("{chatId}/Image")]
        public async Task<IActionResult> GetChatImage(int chatId)
        {
            var file = await _chatService.GetChatImage(chatId);

            return File(file.Bytes!, file.ContentType, file.FileName);
        }
    }
}
