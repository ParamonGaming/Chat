using Client.Core;
using Client.MVVM.View;
using Clinet.Core;
using Microsoft.AspNetCore.SignalR.Client;
using Models;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Client.MVVM.ViewModel
{
    class LoginViewModel : ObservableObject
    {
        #region Fields
        private string _username;
        private string _password;
        private bool _isViewVisible = true;
        #endregion
        #region Properties
        public string Username
        {
            get { return _username; }
            set
            {
                _username = value;
                OnPropertyChanged();
            }
        }
        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }


        public bool IsViewVisible
        {
            get { return _isViewVisible; }
            set
            {
                _isViewVisible = value;
                OnPropertyChanged();
            }
        }
        #endregion
        string myAccessToken = "";
        HubConnection hubConnection;
        public RelayCommand LoginCommand { get; }
        public LoginViewModel()
        {
            hubConnection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5249/chat", o =>
                {

                    o.AccessTokenProvider = () => Task.FromResult(myAccessToken);
                })
                .Build();
            LoginCommand = new RelayCommand(async o => await Connect());
        }
        async Task Connect()
        {
            try
            {
                HttpClient Client = new();
                UserModel newUser = new UserModel {UserName = Username, Password = this.Password };
                var json = JsonConvert.SerializeObject(newUser);
                HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await Client.PostAsync("http://localhost:5249/Account/Token", content);
                json = await response.Content.ReadAsStringAsync();
                myAccessToken = JsonConvert.DeserializeObject<string>(json);
                await hubConnection.StartAsync();
                if (hubConnection.State == HubConnectionState.Connected)
                {
                    IsViewVisible = false;
                    ChatView cv = new(hubConnection, newUser.UserName);
                    cv.Show();
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}
