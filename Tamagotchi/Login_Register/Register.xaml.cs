using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Tamagotchi
{
    public sealed partial class Register : Page
    {
        public Register()
        {
            this.InitializeComponent();
        }

        PołączenieSQLite login = new PołączenieSQLite();

        #region --- PRZYCISK ZAREJESTRUJ ---
        private async void SignUp_Click(object sender, RoutedEventArgs e)
        {
            string email = MailTextBox.Text;
            string password = PasswordTextBox.Password.ToString();
            string returnPassword = ReturnPasswordTextBox.Password.ToString();
            string imie = KotełgotchiTextBox.Text;

            var hasNumber = new Regex(@"[0-9]+");
            var hasUpperChar = new Regex(@"[A-Z]+");
            var hasMinimum8Chars = new Regex(@".{8,}");

            //sprawdzenie czy email jest w bazie
            bool czy_istnieje = login.Czy_istnieje(email);

            //poprawność email
            if (string.IsNullOrWhiteSpace(email) || !Regex.IsMatch(email, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"))
            {
                var dialog = new MessageDialog("Niepoprawny email");
                await dialog.ShowAsync();
            }

            //sprawdzenie powtórzenia hasła
            else if(password != returnPassword)
            {
                var dialog = new MessageDialog("Niepoprawnie powtórzone hasło!");
                await dialog.ShowAsync();
            }

            //sprawdzenie poprawności (siły) hasła
            else if (string.IsNullOrWhiteSpace(password) || !hasNumber.IsMatch(password) || 
                !hasUpperChar.IsMatch(password) || !hasMinimum8Chars.IsMatch(password))
            {
                var dialog = new MessageDialog("Hasło musi: \n- zawierać minimum 8 znaków;\n- zawierać cyfry 0-9;" +
                    "\n- zawierać wielkie i małe litery\n- zawierać znaki specjalne");
                await dialog.ShowAsync();
            }

            //sprawdzenie poprawności imienia
            else if(string.IsNullOrWhiteSpace(email) || imie.Length < 2 || imie.Length > 15)
            {
                var dialog = new MessageDialog("Imię musi mieć przynajmniej 2 znaki, a maksymalnie 15 znaków!");
                await dialog.ShowAsync();
            }

            //sprawdzenie czy email jest w bazie
            else if(czy_istnieje == false)
            {
                var dialog = new MessageDialog("Podany email już istnieje!");
                await dialog.ShowAsync();
                Frame.Navigate(typeof(Register));
            }

            //wykonanie rejestracji
            else
            { 
                login.Write_To_Database(email, password, imie);
                var dialog = new MessageDialog("Udało się zarejestrować!");
                await dialog.ShowAsync();
                Frame.Navigate(typeof(MainPage));
            }
        }
        #endregion

        private void GoBack_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(LogIn_SignUp));
        }
    }
}
