using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Client
{
    class ClientModel : INotifyPropertyChanged
    {
        public ObservableCollection<string> ClientMessages;

        private Thread receiveMessagesThread;

        private Socket socket;

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

        private bool disconnected = false;

        public bool Disconnected
        {
            get { return disconnected; }
            set 
            { 
                disconnected = value;
                OnPropertyChanged("Disconnected");
            }
        }

        public int sendMessage(string message)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
            return socket.Send(data);
        }

        public ClientModel()
        {
            ClientMessages = new ObservableCollection<string>();
        }

        private void receiveMessages()
        {
            try
            {
                while (true)
                {
                    byte[] data = new byte[256]; 
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0; 

                    do
                    {
                        bytes = socket.Receive(data, data.Length, 0);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (socket.Available > 0);
                    if (bytes == 0)
                    {
                        ClientMessages.Add($"Event: сервер был остановлен");
                        break;
                    }
                    ClientMessages.Add($"Server: {builder.ToString()}");
                }
            }
            catch (SocketException exception)
            {
                ClientMessages.Add($"Event: отключен от сервера ({exception.Message})");
            }
            Disconnected = true;
        }

        public void ViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {

            ViewModel vm = (ViewModel)sender;
            if (e.PropertyName == "Port")
            {
                Port = vm.Port;
            } 
            else if (e.PropertyName == "Address")
            {
                Address = vm.Address;
            }
        }

        public bool Connect()
        {

            try
            {
                IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(Address), Port);
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(ipPoint);

                receiveMessagesThread = new Thread(receiveMessages);
                receiveMessagesThread.IsBackground = true;
                receiveMessagesThread.Start();

                return true;
            }
            catch (SocketException exception)
            {
                MessageBox.Show($"{exception.Message}", "Ошибка установки соединения", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            catch (ArgumentOutOfRangeException)
            {
                MessageBox.Show($"Порт вне допустимого диапазона (1-65535)", "Ошибка установки соединения", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            } 
            catch (FormatException)
            {
                MessageBox.Show($"Недопустимый ip-адрес", "Ошибка установки соединения", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public void Disconnect()
        {
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }

}
