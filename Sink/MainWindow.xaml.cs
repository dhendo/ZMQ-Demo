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
using ZMQ;

namespace Sink
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Context context;
        private Socket socket;
        private Thread t;
        


        public MainWindow()
        {
            InitializeComponent();

            label1.Content = label1.Content + App.args;
        }

        private void StartListening()
        {
            context = new Context(1);
            socket = context.Socket(SocketType.PULL);
            socket.Connect("tcp://127.0.0.1:8400"); // Connect to 8400
            UpdateTextBox("Connected to 127.0.0.1:8400");
        }



        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

            if(t!= null)
            {
                t.Abort();
            }
            if (socket != null)
                socket.Dispose();

            if (context != null)
                context.Dispose();
        }


        private void UpdateTextBox(String message)
        {
            textBlock1.Dispatcher.Invoke(new Action(
                delegate()
                { textBlock1.Text = message + "\n" + textBlock1.Text; }), null);
        }

        private void Listen()
        {
            while (true)
            {
                String message = socket.Recv(Encoding.UTF8);
                string msg = string.Format("{0} -  Received {1}", message, DateTime.Now);
                UpdateTextBox(msg);
            }
        }

        private void connect_Click(object sender, RoutedEventArgs e)
        {


            connect.IsEnabled = false;
            StartListening();

            t = new Thread(new ThreadStart(Listen));
            t.Start();
        }
    }
}
