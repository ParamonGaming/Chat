using Client.Core;
using Clinet.Core;
using Microsoft.AspNetCore.SignalR.Client;
using Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Client.MVVM.ViewModel
{
    class ChatViewModel : ObservableObject
    {
        HubConnection hubConnection;
        #region Properties
        public string UserName { get; set; }
        public string Message { get; set; }
        public ObservableCollection<MessageData> Messages { get; set; }
        public ObservableCollection<ChanelModel> Contacts { get; set; }
        private ChanelModel selectedContact;
        public ChanelModel SelectedContact
        {
            get { return selectedContact; }
            set { selectedContact = value; getMessages(); }
        }
        #endregion
        public RelayCommand SendMessageCommand { get; }
        async void getMessages()
        {
            if (SelectedContact != null)
            {
                HttpClient Client = new();
                var json = JsonConvert.SerializeObject(selectedContact.GroupName);
                HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await Client.PostAsync("http://localhost:5249/Account/GetMessages", content);
                json = await response.Content.ReadAsStringAsync();
                Messages = new(JsonConvert.DeserializeObject<List<MessageData>>(json));
                OnPropertyChanged("Messages");
            }
        }
        public ChatViewModel(HubConnection hub, string username)
        {
            UserName = username;
            hubConnection = hub;
            Messages = new ObservableCollection<MessageData>();
            SendMessageCommand = new RelayCommand(async o => await SendMessage());
            hubConnection.On<MessageData>("Receive", (message) =>
            {
                if (SelectedContact != null && SelectedContact.Id == message.ChanelID)
                    Messages.Add(message);
            });
            hubConnection.On<List<int>, List<string>>("GetContacts", (ids, names) =>
            {
                Contacts = new();
                for (int i = 0; i < ids.Count; i++)
                {
                    Contacts.Add(new ChanelModel { GroupName = names[i], Id = ids[i] });
                }
                OnPropertyChanged("Contacts");
            });
        }

        async Task SendMessage()
        {
            if (selectedContact != null)
                await hubConnection.InvokeAsync("Send", new MessageData { Message = Message, Time = DateTime.Now }, SelectedContact, UserName);
        }
    }
}

