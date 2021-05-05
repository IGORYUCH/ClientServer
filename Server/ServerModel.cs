using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Net;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Documents;
using System.Windows;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Server
{
    class ServerModel: INotifyPropertyChanged
    {
        public ObservableCollection<ClientModel> Clients;

        public ObservableCollection<string> Messages;

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

        private string address = "0.0.0.0";

        public string Address
        {
            get { return address; }
            set
            {
                address = value;
                OnPropertyChanged("Address");
            }
        }

        private Socket socket;

        public ServerModel()
        {
            Messages = new ObservableCollection<string>();
            Clients = new ObservableCollection<ClientModel>();
        }

        public bool startServer()
        {
            try
            {
                IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(Address), Port);
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Bind(ipPoint);
                socket.Listen(10);

                Thread receiveConnectionsThread = new Thread(receiveConnections);
                receiveConnectionsThread.IsBackground = true;
                receiveConnectionsThread.Start();

                return true;
            } catch (SocketException exception)
            {
                if (exception.ErrorCode == 10048)
                {
                    MessageBox.Show($"Требуемый порт занят", "Ошибка запуска сервера", MessageBoxButton.OK, MessageBoxImage.Error);
                } else
                {
                    MessageBox.Show($"{exception.Message}", "Ошибка запуска сервера", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                return false;
            }
            catch (ArgumentOutOfRangeException)
            {
                MessageBox.Show($"Порт вне допустимого диапазона (1-65535)", "Ошибка запуска сервера", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public void stopServer()
        {
            socket.Close();
        }

        private void ClientMessagesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            foreach (object item in e.NewItems)
            {
                string message = (string)item;
                Messages.Add(message);
            }
        }

        private void receiveConnections()
        {
            try
            {
                while (true)
                {
                    Socket handler = socket.Accept();
                    ClientModel client = new ClientModel(handler);

                    client.Messages.CollectionChanged += ClientMessagesCollectionChanged;
                    client.PropertyChanged += ClientPropertyChanged;

                    Clients.Add(client);
                    Messages.Add($"Event: {client.Address}:{client.Port} подключился");
                }
            }
            catch (SocketException exception)
            {
                Messages.Add($"Event: {exception.Message}");
            }
        }

        private void ClientPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Disconnected")
            {
                ClientModel client = (ClientModel)sender;
                if (client.Disconnected)
                {
                    Clients.Remove(client);
                }
            }
        }

        public void ViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {

            ViewModel vm = (ViewModel)sender;
            if (e.PropertyName == "Port")
            {
                Port = vm.Port;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
