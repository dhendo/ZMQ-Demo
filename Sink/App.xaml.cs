using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace Sink
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string args;


        private void Application_Startup(object sender, StartupEventArgs e)
        {
            args = e.Args.Length > 0 ? e.Args[0] : "";
        }
    }
}
