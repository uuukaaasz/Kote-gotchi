using SQLitePCL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Tamagotchi
{
    public sealed partial class Admin : Page
    {
        PołączenieSQLite połączenieSQLite = new PołączenieSQLite();

        public Admin()
        {
            this.InitializeComponent();
        }

        private void Show_Click(object sender, RoutedEventArgs e)
        {
            SQLiteConnection dbConnection = new SQLiteConnection("Baza.db");
            var get_items = new List<String>();

            string sSQL = @"SELECT [MAIL],[PASS],[IMIE],[ENERGIA],[ODZYWIANIE]," +
                "[HIGIENA],[SPRAWNOSC],[PIENIADZE],[DOSWIADCZENIE],[ZDROWIE] FROM Users";
            ISQLiteStatement dbState = dbConnection.Prepare(sSQL);

            while (dbState.Step() == SQLiteResult.ROW)
            {
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

                mail = mail.Replace("''", "'");
                pass = pass.Replace("''", "'");
                imie = imie.Replace("''", "'");
                ener = ener.Replace("''", "'");
                odzy = odzy.Replace("''", "'");
                higi = higi.Replace("''", "'");
                spra = spra.Replace("''", "'");
                pien = pien.Replace("''", "'");
                dosw = dosw.Replace("''", "'");
                zdro = zdro.Replace("''", "'");

                get_items.Add(mail + "; " + pass + "; " + imie + "; " + ener + "; " + odzy + "; " + higi +
                    "; " + spra + "; " + pien + "; " + dosw + "; " + zdro);
            }
            FillToShow.ItemsSource = get_items;
        }

        private void Go_Back_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(LogIn_SignUp));
        }
    }
}
