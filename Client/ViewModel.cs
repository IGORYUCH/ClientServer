using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class ViewModel : INotifyPropertyChanged
    {
        private ClientModel clientModel;

        public ObservableCollection<string> Messages;


        private bool portTextBoxEnabled = true;
        public bool PortTextBoxEnabled
        {
            get { return portTextBoxEnabled; }
            set
            {
                portTextBoxEnabled = value;
                OnPropertyChanged("PortTextBoxEnabled");
            }
        }

        private bool addressTextBoxEnabled = true;
        public bool AddressTextBoxEnabled
        {
            get { return addressTextBoxEnabled; }
            set
            {
                addressTextBoxEnabled = value;
                OnPropertyChanged("AddressTextBoxEnabled");
            }
        }

        private int port = 8005;

        public int Port
        {
            get { return port; }
            set
            {
                port = value;
                OnPropertyChanged("Port");
            }
        }

        private string address = "192.168.0.144";

        public string Address
        {
            get { return address; }
            set
            {
                address = value;
                OnPropertyChanged("Address");
            }
        }

        private bool sendMessageButtonEnabled = false;
        public bool SendMessageButtonEnabled
        {
            get { return sendMessageButtonEnabled; }
            set
            {
                sendMessageButtonEnabled = value;
                OnPropertyChanged("SendMessageButtonEnabled");
            }
        }

        private string connectionStatus = "Не подключен";
        public string ConnectionStatus
        {
            get { return connectionStatus; }
            set
            {
                connectionStatus = value;
                OnPropertyChanged("ConnectionStatus");
            }
        }

        private string connectDisconnectButtonStatus = "Подключиться";
        public string ConnectDisconnectButtonStatus
        {
            get { return connectDisconnectButtonStatus; }
            set
            {
                connectDisconnectButtonStatus = value;
                OnPropertyChanged("ConnectDisconnectButtonStatus");
            }
        }

        private string messagesText = "";

        public string MessagesText
        {
            get { return messagesText; }
            set
            {
                messagesText = value; OnPropertyChanged("MessagesText");
            }
        }

        private string messageText;

        public string MessageText
        {
            get { return messageText; }
            set
            {
                messageText = value;
                OnPropertyChanged("MessageText");
            }
        }

        private RelayCommand connectDisconnectButtonCommand;
        public RelayCommand ConnectDisconnectButtonCommand
        {
            get { return connectDisconnectButtonCommand; }
            set
            {
                connectDisconnectButtonCommand = value;
                OnPropertyChanged("ConnectDisconnectButtonCommand");
            }
        }

        private RelayCommand connectCommand;

        public RelayCommand ConnectCommand
        {
            get
            {
                return connectCommand ??
                  (connectCommand = new RelayCommand(obj =>
                  {
                      bool connected = clientModel.Connect();

                      if (connected)
                      {
                          SetViewModelConnectedState();
                      }
                  }));
            }
        }

        private RelayCommand disconnectCommand;

        public RelayCommand DisconnectCommand
        {
            get
            {
                return disconnectCommand ??
                  (disconnectCommand = new RelayCommand(obj =>
                  {
                      clientModel.Disconnect();

                      SetViewModelDisconnectedState();
                  }));
            }
        }

        private RelayCommand sendMessageCommand;

        public RelayCommand SendMessageCommand
        {
            get
            {
                return sendMessageCommand ??
                  (sendMessageCommand = new RelayCommand(obj =>
                  {
                      string message = (string)obj;
                      if (message != string.Empty)
                      {
                          clientModel.sendMessage(message);
                          Messages.Add($"Client: {message}");
                          MessageText = "";
                      }
                  }));
            }
        }

        public void SetViewModelConnectedState()
        {
            ConnectDisconnectButtonStatus = "Отключиться";
            ConnectionStatus = "Подключен";
            SendMessageButtonEnabled = true;
            PortTextBoxEnabled = false;
            AddressTextBoxEnabled = false;
            ConnectDisconnectButtonCommand = DisconnectCommand;
        }

        public void SetViewModelDisconnectedState()
        {
            ConnectDisconnectButtonStatus = "Подключиться";
            ConnectionStatus = "Не подключен";
            SendMessageButtonEnabled = false;
            PortTextBoxEnabled = true;
            AddressTextBoxEnabled = true;
            ConnectDisconnectButtonCommand = ConnectCommand;
        }

        private void ClientModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Disconnected") 
            { 
                if (clientModel.Disconnected)
                {
                    SetViewModelDisconnectedState();
                }
            }
        }

        private void ClientMessagesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (object item in e.NewItems)
                    {
                        string message = (string)item;
                        Messages.Add(message);
                    }
                    break;
            }
        }

        private void MessagesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            MessagesText = string.Join("\n", Messages);
        }

        public ViewModel()
        {
            Messages = new ObservableCollection<string>();
            clientModel = new ClientModel();

            clientModel.ClientMessages.CollectionChanged += ClientMessagesCollectionChanged;
            clientModel.PropertyChanged += ClientModelPropertyChanged;
            this.PropertyChanged += clientModel.ViewModelPropertyChanged;            
            Messages.CollectionChanged += MessagesCollectionChanged;

            ConnectDisconnectButtonCommand = ConnectCommand;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
