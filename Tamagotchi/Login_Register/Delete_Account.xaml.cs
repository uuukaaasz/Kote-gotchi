using SQLitePCL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
    public sealed partial class Delete_Account : Page
    {
        public Delete_Account()
        {
            this.InitializeComponent();
        }

        private async void Delete__Click(object sender, RoutedEventArgs e)
        {
            SQLiteConnection dbConnection = new SQLiteConnection("Baza.db");

            string email = UsernameTextBox.Text;
            string password = PasswordTextBox.Password.ToString();

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                var dialog1 = new MessageDialog("Podaj login i hasło!");
                await dialog1.ShowAsync();
                Frame.Navigate(typeof(Delete_Account));
            }
            else
            {
                string sSQL = $@"DELETE FROM [Users] WHERE MAIL='{email}';";
                string sSQLselect = $@"SELECT PASS FROM [Users] WHERE MAIL='{email}';";

                ISQLiteStatement dbState = dbConnection.Prepare(sSQLselect);

                if (dbState.Step() == SQLiteResult.ROW)
                {
                    string pass = dbState["PASS"] as string;
                    if (pass == password)
                    {
                        dbConnection.Prepare(sSQL).Step();
                        var dialog2 = new MessageDialog("Konto '" + email + "' pomyślnie usunięte.");
                        await dialog2.ShowAsync();
                        Frame.Navigate(typeof(LogIn_SignUp));
                    }
                }
                else
                {
                    var dialog3 = new MessageDialog("Niepoprawny login lub hasło.");
                    await dialog3.ShowAsync();
                    Frame.Navigate(typeof(Delete_Account));
                }
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(LogIn_SignUp));
        }
    }
}
