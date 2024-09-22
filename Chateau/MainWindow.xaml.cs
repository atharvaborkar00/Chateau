using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net.Sockets;


namespace Chateau
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TcpClient client;
        NetworkStream stream;
        public MainWindow()
        {
            InitializeComponent();

            //Connect to server on localhost and port 5002
            client = new TcpClient("127.0.0.1", 5002);
            stream = client.GetStream();

            //Start a new thread to listen for incoming messages
            Thread receiveThread = new Thread(ReceiveMessages);
            receiveThread.IsBackground = true;
            receiveThread.Start();
        }

        private void ReceiveMessages()
        {
            byte[] data = new byte[256];
            while (true)
            {
                try
                {
                    int bytes = stream.Read(data, 0, data.Length);
                    if (bytes == 0) continue;
                    ;
                    string message = Encoding.UTF8.GetString(data, 0, bytes);
                    Dispatcher.Invoke(() => ChatHistory.Items.Add(message));
                }
                catch
                {
                    Dispatcher.Invoke(() => ChatHistory.Items.Add("Connection closed"));
                    break;
                }
            }
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            string message = MessageInput.Text;
            byte[] data = Encoding.UTF8.GetBytes(message);
            stream.Write(data, 0, data.Length);
            MessageInput.Clear();
        }
    }
}
