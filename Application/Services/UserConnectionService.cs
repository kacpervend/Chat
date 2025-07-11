using System.Collections.Concurrent;
using System.Security.Claims;

namespace Application.Services
{
    public class UserConnectionService
    {
        private readonly ConcurrentDictionary<string, string> _userConnections = new ConcurrentDictionary<string, string>();

        public void AddConnection(string username, string connectionId)
        {
            if(!string.IsNullOrEmpty(username))
            {
                _userConnections[username] = connectionId;
            }
        }

        public void RemoveConnection(string username)
        {
            if (!string.IsNullOrEmpty(username))
            {
                _userConnections.TryRemove(username, out _);
            }
        }

        public string GetClaimValue(ClaimsPrincipal? user, string claimType)
        {
            if (user == null)
                throw new ArgumentNullException("Empty user!");

            var claimValue = user.Claims.FirstOrDefault(x => x.Type == claimType)?.Value;

            if (string.IsNullOrEmpty(claimValue))
                throw new ArgumentException($"Empty claim '{claimType}'!");

            return claimValue;
        }
    }
}
