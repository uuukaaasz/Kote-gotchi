using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLitePCL;
using Windows.UI.Xaml;

namespace Tamagotchi
{
    class PołączenieSQLite
    {
        //tworzenie bazy danych i tabeli Users
        public void CreateDatabase(object sender, RoutedEventArgs e)
        {
            SQLiteConnection dbConnection = new SQLiteConnection("Baza.db");
            string sSQL = @"CREATE TABLE IF NOT EXISTS Users 
                ( ID_USER INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                MAIL TEXT NOT NULL,
                PASS TEXT NOT NULL,
                IMIE TEXT NOT NULL,
                ENERGIA TEXT,
                ODZYWIANIE TEXT,
                HIGIENA TEXT,
                SPRAWNOSC TEXT,
                PIENIADZE TEXT,
                DOSWIADCZENIE TEXT,
                ZDROWIE TEXT,
                DATA TEXT);";
            ISQLiteStatement cnStatement = dbConnection.Prepare(sSQL);
            cnStatement.Step();
        }

        //sprawdzenie czy adres mail nie jest już w użyciu w bazie danych
        public bool Czy_istnieje(string email)
        {
            SQLiteConnection dbConnection = new SQLiteConnection("Baza.db");

            string sSQL = @"SELECT MAIL FROM Users";
            ISQLiteStatement dbState = dbConnection.Prepare(sSQL);
            while (dbState.Step() == SQLiteResult.ROW)
            {
                if (dbState[0].ToString() == email)
                    return false;
            }
            return true;
        }

        //tworzenie nowego użytkownika w bazie danych
        public void Write_To_Database(string email, string password, string imie)
        {
            SQLiteConnection dbConnection = new SQLiteConnection("Baza.db");

            DateTime date = DateTime.Now;
            string energia = "100";
            string odzywianie = "80";
            string higiena = "100";
            string sprawnosc = "10";
            string pieniadze = "50";
            string doswiadczenie = "1";
            string zdrowie = "100";
            string sSQL = @"INSERT INTO [Users]
                        ([MAIL],[PASS],[IMIE],[ENERGIA],[ODZYWIANIE],[HIGIENA],[SPRAWNOSC],[PIENIADZE],
                        [DOSWIADCZENIE],[ZDROWIE],[DATA])
                        VALUES
                        ('" + email + "','" + password + "','" + imie + "','" + energia + "','" 
                        + odzywianie + "','" + higiena + "','" + sprawnosc + "','" + pieniadze 
                        + "','" + doswiadczenie + "','" + zdrowie + "','" + date + "');";
            dbConnection.Prepare(sSQL).Step();
        }
    }
}
