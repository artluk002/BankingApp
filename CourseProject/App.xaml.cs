using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace CourseProject
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            /*var langCode = CourseProject.Properties.Settings.Default.languageCode;*/
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
            base.OnStartup(e);
        }
    }
}
