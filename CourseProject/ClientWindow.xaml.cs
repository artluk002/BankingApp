using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CourseProject
{
    /// <summary>
    /// Логика взаимодействия для ClientWindow.xaml
    /// </summary>
    public partial class ClientWindow : Window
    {
        private GeneralWindow backUp;
        public ClientWindow()
        {
            InitializeComponent();
        }
        public ClientWindow(GeneralWindow generalWindow)
        {
            InitializeComponent();
            this.backUp = generalWindow;
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
            backUp.Show();
        }

        private void EnLangBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SignOutBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ChangePasswordBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ChangeMailBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ChangePhoneNumberBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void RuLangBtn_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
