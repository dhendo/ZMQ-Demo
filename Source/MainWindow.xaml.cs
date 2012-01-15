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
using System.Windows.Threading;
using ZMQ;
using System.Diagnostics;

namespace Source
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Context context;
        private Socket socket;
        private int messageCount = 0;
        private Timer _timer;
        private int sinkid = 1;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void StartListening()
        {
            startListening.IsEnabled = false;
            context = new Context(1);
            socket = context.Socket(SocketType.PUSH);
            socket.Bind("tcp://*:8400"); // Start listening on 8400
            //socket.HWM = 1000;
            UpdateTextBox("Listening on port 8400");
            startSending.IsEnabled = true;
        }

        private void startListening_Click(object sender, RoutedEventArgs e)
        {
            startSending.IsEnabled = false;
            StartListening();
        }

        private void UpdateTextBox(String message)
        {
            textBlock1.Dispatcher.Invoke(new Action(
                delegate()
                { textBlock1.Text = message + "\n" + textBlock1.Text; }), null);
        }


        private void SendMessage(object state)
        {
            string message = string.Format("Message: {0} - {1}", messageCount, DateTime.Now);
            try
            {
                socket.Send(message, Encoding.UTF8, SendRecvOpt.NOBLOCK);
                UpdateTextBox(message);
            }catch(ZMQ.Exception ex)
            {
                if (ex.Errno == (int)ZMQ.ERRNOS.EAGAIN)
                {
                    UpdateTextBox("No Clients Available " + DateTime.Now);
                }else
                {
                    UpdateTextBox(ex.ToString());
                }

            }
            messageCount++;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (socket != null)
                socket.Dispose();

            if (context != null)
                context.Dispose();
        }

        private void startSending_Click(object sender, RoutedEventArgs e)
        {
            _timer = new Timer(SendMessage, null, new TimeSpan(0), new TimeSpan(0, 0, 0, 1));
            startSending.IsEnabled = false;
            stopSending.IsEnabled = true;
        }

        private void stopSending_Click(object sender, RoutedEventArgs e)
        {
            _timer.Dispose();
            startSending.IsEnabled = true;
            stopSending.IsEnabled = false;
        }

        private void openSink_Click(object sender, RoutedEventArgs e)
        {
            Process p = new Process();

            var path = System.IO.Path.GetFullPath("../../../sink/bin/debug/sink.exe");
            p.StartInfo = new ProcessStartInfo(path, sinkid.ToString());
            sinkid++;
            p.Start();
        }

    
    }
}
