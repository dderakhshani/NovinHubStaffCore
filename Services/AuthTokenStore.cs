
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NovinDevHubStaffCore.Services
{
    public interface IAuthTokenStore
    {
        Task<Models.LoginResult> GetToken();

        void SaveToken(Models.LoginResult token);

        void DeleteToken();
    }
    public class AuthTokenStore : IAuthTokenStore
    {
        public void DeleteToken()
        {
            Properties.Settings.Default.Token =null;
            Properties.Settings.Default.Save();
        }

        //public NovinDevHubStaffCore.Models.LoginResult Token;
        public async Task<Models.LoginResult> GetToken()
        {
            
            var token = JsonConvert.DeserializeObject<Models.LoginResult>(Properties.Settings.Default.Token);
            
            return await Task.FromResult(token);
        }

        public void SaveToken(Models.LoginResult token)
        {
            Properties.Settings.Default.Token = JsonConvert.SerializeObject(token);
            Properties.Settings.Default.Save();
        }
    }
}
