using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Collections.Specialized;
using System.Threading;

namespace Server
{
    class ViewModel : INotifyPropertyChanged
    {
        private MainWindow windowHandle;

        private string serverStatus = "Сервер не запущен";

        public string ServerStatus
        {
            get { return serverStatus; }
            set
            {
                serverStatus = value;
                OnPropertyChanged("ServerStatus");
            }
        }

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

        private string startStopButtonStatus = "Запустить";

        public string StartStopButtonStatus
        {
            get { return startStopButtonStatus; }
            set
            {
                startStopButtonStatus = value;
                OnPropertyChanged("StartStopButtonStatus");
            }
        }

        private RelayCommand startStopButtonCommand;

        public RelayCommand StartStopButtonCommand
        {
            get { return startStopButtonCommand; }
            set
            {
                startStopButtonCommand = value;
                OnPropertyChanged("StartStopButtonCommand");
            }
        }

        private ServerModel serverModel;

        public ObservableCollection<string> Messages { get; set; }

        private string messagesText;

        public string MessagesText {
            get { return messagesText; } 
            set 
            { 
                messagesText = value; 
                OnPropertyChanged("MessagesText"); 
            } 
        }

        private string messageText = "";
        public string MessageText
        {
            get { return messageText; }
            set
            {
                messageText = value;
                OnPropertyChanged("MessageText");
            }
        }

        private ObservableCollection<ClientModel> clients;

        public ObservableCollection<ClientModel> Clients
        {
            get { return clients; }
            set
            {
                clients = value;
                OnPropertyChanged("Clients");
            }
        }
        private ClientModel selectedClient;

        public ClientModel SelectedClient
        {
            get { return selectedClient; }
            set
            {
                selectedClient = value;
                OnPropertyChanged("SelectedClient");
            }
        }

        private RelayCommand stopServerCommand;

        public RelayCommand StopServerCommand
        {
            get
            {
                return stopServerCommand ??
                  (stopServerCommand = new RelayCommand(obj =>
                  {
                      serverModel.stopServer();

                      foreach (ClientModel client in Clients)
                      {
                          client.disconnect();
                      }

                      StartStopButtonStatus = "Запустить";
                      ServerStatus = "Сервер не запущен";
                      SendMessageButtonEnabled = false;
                      PortTextBoxEnabled = true;
                      StartStopButtonCommand = StartServerCommand;
                  }));
            }
        }

        private void MessagesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            MessagesText = string.Join("\n", Messages);
        }

        private void ServerMessagesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
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

        private void ServerClientsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (object item in e.NewItems)
                    {
                        ClientModel client = (ClientModel)item;
                        
                        windowHandle.Dispatcher.BeginInvoke(new ThreadStart(delegate
                        {
                            Clients.Add(client);
                        }));
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (object item in e.OldItems)
                    {
                        ClientModel client = (ClientModel)item;
                        windowHandle.Dispatcher.BeginInvoke(new ThreadStart(delegate
                        {
                            Clients.Remove(client);
                        }));
                    }
                    break;
            }
        }

        private RelayCommand startServerCommand;

        public RelayCommand StartServerCommand
        {
            get
            {
                return startServerCommand ??
                  (startServerCommand = new RelayCommand(obj =>
                  {
                      bool started = serverModel.startServer();
                      if (started)
                      {
                          StartStopButtonStatus = "Остановить";
                          ServerStatus = "Сервер запущен";
                          SendMessageButtonEnabled = true;
                          PortTextBoxEnabled = false;
                          StartStopButtonCommand = StopServerCommand;
                      }
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
                          if (SelectedClient != null)
                          {
                              SelectedClient.sendMessage(message);
                              Messages.Add($"Server to {SelectedClient.Address}:{SelectedClient.Port}: {message}");
                              MessageText = "";
                          } else
                          {
                              MessageBox.Show("Не выбран получатель!", "Ошибка отправки", MessageBoxButton.OK, MessageBoxImage.Error);
                          }
                      }
                  }));
            }
        }

        public ViewModel(MainWindow windowHandle)
        {
            this.windowHandle = windowHandle;
            StartStopButtonCommand = StartServerCommand;
            Messages = new ObservableCollection<string>();
            Clients = new ObservableCollection<ClientModel>();

            serverModel = new ServerModel();

            this.PropertyChanged += serverModel.ViewModelPropertyChanged;
            serverModel.Clients.CollectionChanged += ServerClientsCollectionChanged;
            serverModel.Messages.CollectionChanged += ServerMessagesCollectionChanged;
            Messages.CollectionChanged += MessagesCollectionChanged;
        }
       
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
