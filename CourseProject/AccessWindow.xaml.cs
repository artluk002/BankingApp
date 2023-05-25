using CourseProject.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Логика взаимодействия для AccessWindow.xaml
    /// </summary>
    public partial class AccessWindow : Window
    {
        private int attempts;
        public AccessWindow()
        {
            InitializeComponent();
            attempts = 3;
        }

        private void PinCodeTB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Регулярное выражение, разрешающее только цифры и ограничение на 4 символа
            Regex regex = new Regex("^[0-9]{0,4}$");

            // Проверяем, что вводимый текст соответствует регулярному выражению
            if (!regex.IsMatch(((TextBox)sender).Text + e.Text))
            {
                e.Handled = true; // Предотвращаем ввод символа
            }
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OKBtn_Click(object sender, RoutedEventArgs e)
        {
            string pinCode = PinCodeTB.Text;
            
            if(DataStorage.card.CheckPinCode(pinCode))
            {
                MessageBox.Show("Operation confirmed", "OK", MessageBoxButton.OK, MessageBoxImage.Information);
                DataStorage.attempts = attempts;
                Close();
            }
            else
            {
                MessageBox.Show("Invalid pin code", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                if(attempts > 0)
                    attempts--;
                else
                {
                    DataStorage.attempts = attempts;
                    MessageBox.Show("You lost all attempts", "Ups", MessageBoxButton.OK, MessageBoxImage.Warning);
                    Close();
                }
            }
        }
    }
}
