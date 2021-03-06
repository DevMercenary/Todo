using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using HandyControl.Controls;
using Microsoft.AspNetCore.SignalR.Client;
using Prism.Commands;
using TodoNotes.Data;
using TodoShared;

namespace TodoNotes.ViewModels
{
    public class TextChatViewModel : INotifyPropertyChanged
    {
        public TextChatViewModel()
        {
            SettingXml xmlWorker = new SettingXml();
            ServerList = xmlWorker.ReadDocument();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private HubConnection _connection;

        public event Action MessageCollectionChanged;

        #region Props

        private ObservableCollection<Message> _messages;

        public ObservableCollection<Message> Messages
        {
            get => _messages;
            set
            {
                if (_messages != null)
                    _messages.CollectionChanged -= OnMessageCollectionChanged;

                _messages = value;
                _messages.CollectionChanged += OnMessageCollectionChanged;
                OnPropertyChanged(nameof(Messages));
            }
        }

        private void OnMessageCollectionChanged(object obj, NotifyCollectionChangedEventArgs e)
        {
            MessageCollectionChanged?.Invoke();
        }

        private ObservableCollection<string> _users;

        public ObservableCollection<string> Users
        {
            get => _users;
            set
            {
                _users = value;
                OnPropertyChanged(nameof(Users));
            }
        }

        public string Nickname { get; set; }

        private string _text = "";

        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                OnPropertyChanged(nameof(Text));
            }
        }

        private bool _isOffline = true;

        public bool IsOffline
        {
            get => _isOffline;
            set
            {
                _isOffline = value;
                OnPropertyChanged(nameof(IsOffline));
                OnPropertyChanged(nameof(IsOnline));
            }
        }

        public bool IsOnline
        {
            get => !_isOffline;
        }

        private string _status = "Offline";

        public string Status
        {
            get => _status;
            set
            {
                _status = value;
                OnPropertyChanged(nameof(Status));
            }
        }

        private Brush _statusColor = Brushes.Black;

        public Brush StatusColor
        {
            get => _statusColor;
            set
            {
                _statusColor = value;
                OnPropertyChanged(nameof(StatusColor));
            }
        }

        private string _connectionButtonText = "Подключиться";

        public string ConnectionButtonText
        {
            get => _connectionButtonText;
            private set
            {
                _connectionButtonText = value;
                OnPropertyChanged(nameof(ConnectionButtonText));
            }
        }

        private List<string> _serverList;

        public List<string> ServerList
        {
            get => _serverList;
            set
            {
                _serverList = value;
                OnPropertyChanged(nameof(ServerList));
            }
        }

        public string SelectedServer { get; set; }

        #endregion

        #region Commands

        public ICommand ConnectionManager
        {
            get
            {
                return new DelegateCommand<Task>(async o =>
                {
                    if (IsOnline)
                        await DisconnectAsync();
                    else
                        await ConnectAsync();
                });
            }
        }

        public ICommand SendMessageCommand => new DelegateCommand<Task>(async o => await SendMessageAsync());

        #endregion

        private async Task SendMessageAsync()
        {
            try
            {
                var txt = string.Copy(Text);
                await _connection.SendCoreAsync(Api.SendMessage, new object[] {Nickname, txt});
                Text = "";
            }
            catch (Exception e)
            {
                DisplayServerMessage(e.Message);
            }
        }

        private async Task ConnectAsync()
        {
            if (string.IsNullOrEmpty(Nickname))
            {
                MessageBox.Show("Имя не может быть пустым");
                return;
            }

            if (string.IsNullOrEmpty(SelectedServer))
            {
                MessageBox.Show("Нужно выбрать сервер");
                return;
            }
            //https://localhost:44389/chatHub
            _connection = new HubConnectionBuilder().WithUrl(SelectedServer).Build();
            _connection.Closed += async (error) =>
            {
                if (error != null)
                {
                    ConnectionButtonText = "Подключиться";
                    IsOffline = true;
                    Status = "Соединение потеряно";
                    StatusColor = Brushes.Red;
                }
            };

            _connection.On(nameof(IChatHub.Connected), OnConnected);
            _connection.On<string>(nameof(IChatHub.TransferMessage), OnReceiveMessage);
            _connection.On<List<string>>(nameof(IChatHub.SendAllUsers), OnGetUsers);
            _connection.On<bool>(nameof(IChatHub.SetNameResult), OnSetName);
            _connection.On<string>(nameof(IChatHub.UserConnected), OnUserConnected);
            _connection.On<string>(nameof(IChatHub.UserDisconnected), OnUserDisconnected);
            _connection.On<string>(nameof(IChatHub.SendLastMessages), FillLastMessages);

            try
            {
                await _connection.StartAsync();
            }
            catch (Exception e)
            {
                DisplayServerMessage(e.Message + "\n" + e.InnerException?.Message);
            }
        }

        private void FillLastMessages(string messagesJson)
        {
            var messages = JsonSerializer.Deserialize<List<Message>>(messagesJson);
            Messages = new ObservableCollection<Message>(messages);
        }

        private void OnUserDisconnected(string name)
        {
            Users.Remove(name);
            DisplayServerMessage($"{name} тут больше нет :(");
        }

        private void OnUserConnected(string name)
        {
            Users.Add(name);
            DisplayServerMessage($"{name} теперь с нами!");
        }

        private async Task DisconnectAsync()
        {
            ConnectionButtonText = "Подключиться";
            IsOffline = true;
            Status = "Offline";
            StatusColor = Brushes.Black;

            Messages.Clear();
            Users.Clear();

            try
            {
                await _connection.DisposeAsync();
            }
            catch (Exception e)
            {
                DisplayServerMessage(e.Message);
            }

        }

        private void OnReceiveMessage(string messageJson)
        {
            var message = JsonSerializer.Deserialize<Message>(messageJson);
            message.Time = DateTime.Now;

            Messages.Add(message);
        }

        private void OnGetUsers(List<string> users)
        {
            Users = new ObservableCollection<string>(users);
        }

        private async void OnSetName(bool isSet)
        {
            if (isSet)
            {
                Status = "Online";
                StatusColor = Brushes.LimeGreen;
                IsOffline = false;
            }
            else
            {
                Status = "Человек с таким именем уже есть!";
                StatusColor = Brushes.Red;
                IsOffline = true;
                ConnectionButtonText = "Подключиться";

                try
                {
                    await _connection.DisposeAsync();
                }
                catch (Exception e)
                {
                    DisplayServerMessage(e.Message);
                }
            }
        }

        private async void OnConnected()
        {
            ConnectionButtonText = "Отключиться";
            try
            {
                await _connection.InvokeCoreAsync<string>(Api.SetName, new[] {Nickname});
            }
            catch (Exception e)
            {
                DisplayServerMessage(e.Message);
            }
        }

        private void DisplayServerMessage(string message)
        {
            Messages.Add(new Message(Constants.ServerName, message));
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
