using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace Server
{
    public class ClientModel : INotifyPropertyChanged
    {
        private IPEndPoint ipPoint;

        private Thread receiveMessagesThread;

        public ObservableCollection<string> Messages;

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

        public string Address
        {
            get { return ipPoint.Address.ToString(); }
        }
        public string Port
        {
            get { return ipPoint.Port.ToString(); }
        }

        public ClientModel(Socket socket)
        {
            Messages = new ObservableCollection<string>();
            this.socket = socket;
            this.ipPoint = (IPEndPoint)socket.RemoteEndPoint;

            receiveMessagesThread = new Thread(receiveMessages);
            receiveMessagesThread.IsBackground = true;
            receiveMessagesThread.Start();
        }

        public int sendMessage(string message)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
            return socket.Send(data);
        }

        public void disconnect()
        {
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }

        private void receiveMessages()
        {
            try
            {
                while (true)
                {
                    {
                        StringBuilder builder = new StringBuilder();
                        int bytes = 0; 
                        byte[] data = new byte[256]; 

                        do
                        {
                            bytes = socket.Receive(data, 256, SocketFlags.Partial);
                            builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                        }
                        while (socket.Available > 0);
                        if (bytes == 0)
                        {
                            Messages.Add($"Event: {Address}:{Port} отключился");
                            break;
                        }
                        Messages.Add($"{Address}:{Port}: {builder.ToString()}");
                    }
                }
            } catch (SocketException exception)
            {
                Messages.Add($"Event: соединение потеряно с {Address}:{Port} ({exception.Message})");  
            }
            Disconnected = true;
        }

        private Socket socket { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            
        }
    }
}
