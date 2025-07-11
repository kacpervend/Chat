using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace API.Hubs
{
    [Authorize]
    public class MessageHub : Hub
    {
        private readonly UserConnectionService _userConnectionService;
        private string _username => _userConnectionService.GetClaimValue(Context.User, ClaimTypes.Name);
        private string _userId => _userConnectionService.GetClaimValue(Context.User, ClaimTypes.NameIdentifier);
        private const string _chatName = "Global";
        private readonly IChatService _chatService;

        public MessageHub(UserConnectionService userConnectionService, IChatService chatService)
        {
            _userConnectionService = userConnectionService;
            _chatService = chatService;
        }

        public override async Task OnConnectedAsync()
        {
            _userConnectionService.AddConnection(_username, Context.ConnectionId);
            await JoinChat(_chatService.GetPaginatedChat(_chatName, 1, 1).Result.Id.ToString());

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _userConnectionService.RemoveConnection(_username);
            await LeaveChat(_chatService.GetPaginatedChat(_chatName, 1, 1).Result.Id.ToString());

            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessageToChat(string chatId, string message)
        {
            await Clients.Group(chatId).SendAsync("ReceiveMessage", _username, message);
  
            await _chatService.SaveMessage(new Application.DTO.MessageDTO()
            {
                MessageId = Guid.NewGuid(),
                ChatId = Guid.Parse(chatId),
                SenderId = Guid.Parse(_userId),
                Sender = _username,
                MessageText = message,
                CreatedAt = DateTime.Now
            });
        }

        private async Task JoinChat(string chatName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, chatName);
        }

        private async Task LeaveChat(string chatName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatName);
        }
    }
}
