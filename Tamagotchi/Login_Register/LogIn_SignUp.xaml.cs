using SQLitePCL;
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
    public sealed partial class LogIn_SignUp : Page
    {
        PołączenieSQLite PołączenieSQLite = new PołączenieSQLite();

        public LogIn_SignUp()
        {
            this.InitializeComponent();
            Loaded += PołączenieSQLite.CreateDatabase;      //tworzenie bazy danych
        }

        #region --- SPRAWDZENIE DANYCH LOGOWANIA ---
        private async void SignInButton_Click(object sender, RoutedEventArgs e)
        {
            string email = UsernameTextBox.Text;
            string pass = PasswordTextBox.Password.ToString();

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(pass))
            {
                var dialog = new MessageDialog("Błędny email lub hasło!");
                await dialog.ShowAsync();
            }

            else
            {
                //logowanie na konto admin (MASTER)
                if (email == "admin" && pass == "admin")
                    Frame.Navigate(typeof(Admin));

                //logowanie na konto user (DEMO)
                else if(email == "user" && pass == "user")
                {
                    DaneLogowania.Login = email;
                    Frame.Navigate(typeof(MainPage));
                }

                //logowanie na konto
                else
                {
                    Validate(email, pass);      //logowanie
                    Frame.Navigate(typeof(MainPage));
                }
            }
        }
        #endregion

        #region --- FUNKCJA LOGOWANIA ---
        public async void Validate(string email, string passwd)
        {
            SQLiteConnection dbConnection = new SQLiteConnection("Baza.db");

            string sSQL = @"SELECT [MAIL],[PASS],[IMIE],[ENERGIA],[ODZYWIANIE]," +
                "[HIGIENA],[SPRAWNOSC],[PIENIADZE],[DOSWIADCZENIE],[ZDROWIE],[DATA] " +
                "FROM Users " +
                "WHERE [MAIL]='" + email + "' AND [PASS]='" + passwd + "';";
            ISQLiteStatement dbState = dbConnection.Prepare(sSQL);

            while (dbState.Step() == SQLiteResult.ROW)
            {
                #region pobranie wartości z bazy danych
                string mail = dbState["MAIL"] as string;
                string pass = dbState["PASS"] as string;
                string imie = dbState["IMIE"] as string;
                string ener = dbState["ENERGIA"] as string;
                string odzy = dbState["ODZYWIANIE"] as string;
                string higi = dbState["HIGIENA"] as string;
                string spra = dbState["SPRAWNOSC"] as string;
                string pien = dbState["PIENIADZE"] as string;
                string dosw = dbState["DOSWIADCZENIE"] as string;
                string zdro = dbState["ZDROWIE"] as string;
                string data = dbState["DATA"] as string;
                #endregion

                DaneLogowania.Attribiutes[0] = null;                //wyzerowanie kontrolki

                #region //przypisanie wartości z bazy danych
                DateTime myDate = DateTime.Parse(data);
                DaneLogowania.data = myDate;
                DaneLogowania.Login = mail;
                DaneLogowania.Koteł_Imie = imie;
                DaneLogowania.Attribiutes[0] = ener;
                DaneLogowania.Attribiutes[1] = odzy;
                DaneLogowania.Attribiutes[2] = higi;
                DaneLogowania.Attribiutes[3] = spra;
                DaneLogowania.Attribiutes[4] = pien;
                DaneLogowania.Attribiutes[5] = dosw;
                DaneLogowania.Attribiutes[6] = zdro;
                #endregion
            }

            //sprawdzenie poprawności (login, hasło)
            if (string.IsNullOrWhiteSpace(DaneLogowania.Attribiutes[0]))
            {
                var dialog = new MessageDialog("Błędny email lub hasło!");
                await dialog.ShowAsync();
                Frame.Navigate(typeof(LogIn_SignUp));
            }
        }
        #endregion

        //odnośnik do strony rejestracji
        private void SignUpButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Register));
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Delete_Account));
        }
    }
}
