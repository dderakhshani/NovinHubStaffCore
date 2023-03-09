using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using NovinDevHubStaffCore.Core;
using NovinDevHubStaffCore.Models;
using NovinDevHubStaffCore.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NovinDevHubStaffCore.ViewModels
{
    class LoginDialogViewModel : ViewModelBase
    {
        private readonly IAuthService authService = Ioc.Default.GetRequiredService<IAuthService>();
        private readonly IAuthTokenStore authTokenStore = Ioc.Default.GetRequiredService<IAuthTokenStore>();

        private ICloseable window;


        public LoginDialogViewModel(ICloseable window)
        {
            LoginCommand = new AsyncRelayCommand(LoginHandler);
            this.window = window;
            this._userName = "mostafa";
        }

        #region Dependecy Properties

        private string _userName;
        public string Username
        {
            get => _userName;
            set => SetProperty(ref _userName, value);
        }

        public string Password { private get; set; }

        private bool _loginError;
        public bool LoginError
        {
            get => _loginError;
            set => SetProperty(ref _loginError, value);
        }


        #endregion



        public IAsyncRelayCommand LoginCommand { get; }

        public async Task<LoginResult> LoginHandler()
        {
            IsLoading = true;
            var response = await authService.Authenticate(new LoginRequest
            {
                client_id = "user-service",
                client_secret = Constants.CLIENT_SECRET,
                grant_type = "password",
                username = this.Username,
                password = this.Password,
            });
            IsLoading = false;
            LoginError = false;
            if (response.IsSuccessStatusCode)
            {
               
              
                var result = response.Content;

                ////It should be replace by IOC
                authTokenStore.SaveToken(result);

                this.window.Close(true);
                return result;
            }
            else
            {
                IsLoading = false;
                LoginError = true;
                return null;

            }
                
        }
    }
}
