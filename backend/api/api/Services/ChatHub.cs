using api.Models;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;



namespace api.Services
{
    public class ChatHub :Hub
    {
        static public readonly Dictionary<string,string> Users=new Dictionary<string,string>();
        private readonly IJwtToken _tokenService;
        public ChatHub(IJwtToken tokenService)
        {
            _tokenService=tokenService;
        }

        public override async Task OnConnectedAsync()
        {
            string token=Context.GetHttpContext().Request.Query["access_token"];
            if (token == null)
                return;
            string id=_tokenService.TokenToId(token);
            Users.Add(Context.ConnectionId,id);
            //await SendDirect(id, "poruka");
            //await Send(Context.ConnectionId);
            await base.OnConnectedAsync();

        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            Users.Remove(Context.ConnectionId);
        }
        public static bool CheckUser(string id)
        {
            var users=Users.Values;
            foreach (var user in users)
            {
                if(user==id)
                    return true;
            }
            return false;
        }
        public static List<string> getAllConnectionsOfUser(string id)
        {
            List<string> keys=new List<string>();
            foreach (var user in Users)
            {
                if(user.Value==id)
                    keys.Add(user.Key);
            }
            return keys;
        }
    }
    

}
