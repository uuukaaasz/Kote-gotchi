using SQLitePCL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Tamagotchi.Content_Dialogs;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace Tamagotchi
{
    public sealed partial class MainPage : Page
    {
        int CheckSoundIs = 0;               //zmienna kontroli dźwięku
        int CheckSaving = 0;                //zmienna kontroli zapisu
        int[] Atrybuty = new int[7];        //tablica dla wartości atrybutów
        string WykonywanaCzynnosc = " ";    //zmienna do przechowywania nazwy wykonywanej aktualnie czynności
        double hours = 0;                   //zmienna pomocnicza dla sprawdzania czasu bez logowania
        double Level = 1;                   //level zależny od doświadczenia
        
        public MainPage()
        {
            this.InitializeComponent();
            Loaded += MainPage_Loaded;      //wczytaj stronę logowania
        }

        private async void CoSieStalo(double hours, double odz, double hig, double spr, double zdr)
        {
            var dialog = new MessageDialog("Nie było Cię przez " + hours + " godzin. W tym czasie:\n" +
                " - Odżywianie spadło o " + odz + " punktów.\n - Higiena spadła o " + hig + " punktów.\n" +
                " - Sprawność spadła o " + spr + " punktów.\n - Zdrowie spadło o " + zdr + " punktów.\n");
            await dialog.ShowAsync();
        }

        private void Nowy_level() //funkcja levelowania
        {
            switch (Level)
            {
                case 2:
                    Koteł.Source = new BitmapImage(new Uri("ms-appx:///Images/Kotełgotchi2.png"));
                    break;
                case 3:
                    Koteł.Source = new BitmapImage(new Uri("ms-appx:///Images/Kotełgotchi3.png"));
                    break;
                case 4:
                    Koteł.Source = new BitmapImage(new Uri("ms-appx:///Images/Kotełgotchi4.png"));
                    break;
                default:
                    break;
            }
        }

        #region --- LOGOWANIE I WPROWADZENIE WARTOŚCI Z BAZY ---
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if(DaneLogowania.Login == "user")
            {
                Save.Background = new SolidColorBrush(Colors.DeepPink);
            }

            else if (string.IsNullOrWhiteSpace(DaneLogowania.Login))
                Frame.Navigate(typeof(LogIn_SignUp));
            else
            {
                try
                {
                    #region -> ILE CZASU UŻYTKOWNIK BYŁ NIEAKTYWNY <-
                    hours = Math.Floor((DateTime.Now - DaneLogowania.data).TotalHours);
                    if (hours > 0)
                    {
                        CheckSaving = 1;

                        double hOdz = Convert.ToDouble(DaneLogowania.Attribiutes[1]);       //odżywianie  
                        double hHig = Convert.ToDouble(DaneLogowania.Attribiutes[2]);       //higiena
                        double hSpr = Convert.ToDouble(DaneLogowania.Attribiutes[3]);       //sprawność
                        double hZdr = Convert.ToDouble(DaneLogowania.Attribiutes[6]);       //zdrowie
                        double zOdz = 3 * hours;
                        double zHig = 2 * hours;
                        double zSpr = 1 * hours;
                        double zZdr = 1 * hours;

                        hOdz -= zOdz;
                        hHig -= zHig;
                        hSpr -= zSpr;
                        hZdr -= zZdr;

                        DaneLogowania.Attribiutes[1] = hOdz.ToString();
                        DaneLogowania.Attribiutes[2] = hHig.ToString();
                        DaneLogowania.Attribiutes[3] = hSpr.ToString();
                        DaneLogowania.Attribiutes[6] = hZdr.ToString();

                        CoSieStalo(hours, zOdz, zHig, zSpr, zZdr);

                        Save_Click(null, new RoutedEventArgs());        //zapis
                        CheckSaving = 0;                                //wyzerowanie kontroli komunikatu zapisu
                    }
                    #endregion

                    #region -> WCZYTAJ POZIOM <-
                    int exp = Convert.ToInt32(DaneLogowania.Attribiutes[5]);
                    if (exp >= 250 && exp < 500)
                    {
                        Level = 2;
                        Koteł.Source = new BitmapImage(new Uri("ms-appx:///Images/Kotełgotchi2.png"));
                    }
                    else if (exp >= 500 && exp < 750)
                    {
                        Level = 3;
                        Koteł.Source = new BitmapImage(new Uri("ms-appx:///Images/Kotełgotchi3.png"));
                    }
                    else if (exp >= 750 && exp < 1000)
                    {
                        Level = 4;
                        Koteł.Source = new BitmapImage(new Uri("ms-appx:///Images/Kotełgotchi4.png"));
                    } 
                    #endregion

                    poziomEnergii.Text = DaneLogowania.Attribiutes[0];
                    poziomOdzywienia.Text = DaneLogowania.Attribiutes[1];
                    poziomHigieny.Text = DaneLogowania.Attribiutes[2];
                    poziomSprawnosci.Text = DaneLogowania.Attribiutes[3];
                    poziomPieniedzy.Text = DaneLogowania.Attribiutes[4];
                    poziomDoswiadczenia.Text = DaneLogowania.Attribiutes[5];
                    poziomZdrowia.Text = DaneLogowania.Attribiutes[6];
                    Imie.Text = DaneLogowania.Koteł_Imie;
                }
                catch
                {
                    Frame.Navigate(typeof(LogIn_SignUp));
                }
            }
        }
        #endregion

        #region --- WCZYTYWANIE ATRYBUTÓW Z TEXTBLOCKÓW ---
        public void Przypisz()
        {
            Atrybuty[0] = Convert.ToInt32(poziomEnergii.Text);
            Atrybuty[1] = Convert.ToInt32(poziomOdzywienia.Text);
            Atrybuty[2] = Convert.ToInt32(poziomHigieny.Text);
            Atrybuty[3] = Convert.ToInt32(poziomSprawnosci.Text);
            Atrybuty[4] = Convert.ToInt32(poziomPieniedzy.Text);
            Atrybuty[5] = Convert.ToInt32(poziomDoswiadczenia.Text);
            Atrybuty[6] = Convert.ToInt32(poziomZdrowia.Text);
        }
        #endregion

        #region --- SPRAWDZANIE WIDEŁEK ATRYBUTÓW PO ZMIANACH ---
        private async void Sprawdz_czy_zyjesz()
        {
            if (Atrybuty[0] == 0)                           //energia spadła do 0
            {
                var dialog = new MessageDialog("Poziom energii spadł do 0! Koteł odczuje zmęczenie na zdrowiu.");
                await dialog.ShowAsync();
                Atrybuty[6] -= 20;                          //odejmujemy 20 punktów zdrowia
            }
            if (Atrybuty[0] <= 15 && Atrybuty[0] != 0)      //energia poniżej 15
            {
                var dialog = new MessageDialog("Poziom energii jest zbyt niski, musisz iść spać na minimum 3h!");
                await dialog.ShowAsync();
            }
            if (Atrybuty[1] == 0)                           //odżywianie spadło do 0
            {
                var dialog = new MessageDialog("Poziom odżywiania spadł do 0, Koteł umarł z głodu. Zacznij grę od początku!");
                await dialog.ShowAsync();
                Frame.Navigate(typeof(LogIn_SignUp));       //powrót do ekranu logowania
            }
            if (Atrybuty[1] <= 10 && Atrybuty[1] != 0)      //odżywianie poniżej 10
            {
                var dialog = new MessageDialog("Jestem głodny, zjedzmy coś!");
                await dialog.ShowAsync();
                Zjedz();                                    //uruchomienie funkcji jedz
            }
            if (Atrybuty[2] == 0)                           //poziom higieny spadł do 0
            {
                var dialog = new MessageDialog("Poziom higieny spadł do 0! Koteł odczuje zmęczenie na zdrowiu.");
                await dialog.ShowAsync();
                Atrybuty[6] -= 20;                          //odejmujemy 20 punktów zdrowia
            }
            if (Atrybuty[2] <= 15 && Atrybuty[2] != 0)      //poziom higieny poniżej 15
            {
                var dialog = new MessageDialog("Ależ cuchnie! Umyj go!");
                await dialog.ShowAsync();
            }
            if (Atrybuty[4] <= 10)                          //poziom pieniędzy poniżej 50
            {
                var dialog = new MessageDialog("Musisz iść do pracy, Twoje oszczędności się kończą!");
                await dialog.ShowAsync();
            }
            if (Atrybuty[5] >= 250 && Level == 1)           //osiągnięto 250 punktów doświadczenia
            {
                var dialog = new MessageDialog("Udało Ci się osiągnąć kolejny poziom rozwoju!");
                await dialog.ShowAsync();
                Level = 2;
                Nowy_level();
                Save_Click(null, new RoutedEventArgs());    //zapisywanie postępu konta
            }
            if (Atrybuty[5] >= 500 && Level == 2)           //osiągnięto 250 punktów doświadczenia
            {
                var dialog = new MessageDialog("Udało Ci się osiągnąć kolejny poziom rozwoju!");
                await dialog.ShowAsync();
                Level = 3;
                Nowy_level();
                Save_Click(null, new RoutedEventArgs());    //zapisywanie postępu konta
            }
            if (Atrybuty[5] >= 750 && Level == 3)           //osiągnięto 250 punktów doświadczenia
            {
                var dialog = new MessageDialog("Udało Ci się osiągnąć kolejny poziom rozwoju!");
                await dialog.ShowAsync();
                Level = 4;
                Nowy_level();
                Save_Click(null, new RoutedEventArgs());    //zapisywanie postępu konta
            }
            if (Atrybuty[5] == 999)                         //osiągnięto 999 punktów doświadczenia
            {
                var dialog = new MessageDialog("ZWYCIĘSTWO!!!\n Twój zwierzak jest tak doświadczony, że pora wyruszyć w świat!");
                await dialog.ShowAsync();
                Save_Click(null, new RoutedEventArgs());    //zapisywanie postępu konta
                Frame.Navigate(typeof(LogIn_SignUp));       //powrót do ekranu logowania
            }
            if (Atrybuty[6] <= 15 && Atrybuty[6] != 0)      //poziom zdrowia poniżej 15
            {
                var dialog = new MessageDialog("Musisz iść do lekarza Twój koteł jest chory!");
                await dialog.ShowAsync();
            }
            if (Atrybuty[6] == 0)                           //poziom zdrowia równy 0, PORAŻKA
            {
                var dialog = new MessageDialog("PORAŻKA!\nJak dbasz - tak masz. Poziom Zdrowia spadł do 0. Zacznij grę od początku!.");
                await dialog.ShowAsync();
                Frame.Navigate(typeof(LogIn_SignUp));       //powrót do ekranu logowania
            }
        }
#endregion

        #region --- WYKONANIE CZYNNOŚCI (COMBOBOX) ---
        private void Choose_Attribute_Click(object sender, RoutedEventArgs e)
        {
            #region -> WYBÓR CZYNNOŚCI (FUNKCJA) <-
            WykonywanaCzynnosc = NazwaCzynnosci.Text;
            switch(WykonywanaCzynnosc) 
            {
                case "WYBIERZ CZYNNOŚĆ:":
                    break;
                case "JEDZ:":
                    Zjedz();
                    break;
                case "PIJ:":
                    Wypij();
                    break;
                case "ŚPIJ:":
                    Wyspij_sie();
                    break;
                case "WYKĄP SIĘ:":
                    Wykap_sie();
                    break;
                case "IDŹ DO LEKARZA:":
                    Idz_do_lekarza();
                    break;
                case "PRACUJ:":
                    Praca();
                    break;
                case "BAW SIĘ:":
                    Zabawa();
                    break;
                case "RELAKS:":
                    Relaksuj_sie();
                    break;
                default:
                    break;
            }
            #endregion

            #region -> WYRÓWNANIE STATYSTYK DO WIDEŁEK <-
            if (Atrybuty[0] < 0) Atrybuty[0] = 0;
            if (Atrybuty[0] > 100) Atrybuty[0] = 100;
            if (Atrybuty[1] < 0) Atrybuty[1] = 0;
            if (Atrybuty[1] > 100) Atrybuty[1] = 100;
            if (Atrybuty[2] < 0) Atrybuty[2] = 0;
            if (Atrybuty[2] > 100) Atrybuty[2] = 100;
            if (Atrybuty[3] < 0) Atrybuty[3] = 0;
            if (Atrybuty[3] > 999) Atrybuty[3] = 999;
            if (Atrybuty[4] < 0) Atrybuty[4] = 0;
            if (Atrybuty[4] > 999) Atrybuty[4] = 999;
            if (Atrybuty[5] < 0) Atrybuty[5] = 0;
            if (Atrybuty[5] > 999) Atrybuty[5] = 999;
            if (Atrybuty[6] < 0) Atrybuty[6] = 0;
            if (Atrybuty[6] > 100) Atrybuty[6] = 100;
            #endregion

            #region -> AKTUALIZACJA ATRYBUTÓW <-
            poziomEnergii.Text = Convert.ToString(Atrybuty[0]);
            poziomOdzywienia.Text = Convert.ToString(Atrybuty[1]);
            poziomHigieny.Text = Convert.ToString(Atrybuty[2]);
            poziomSprawnosci.Text = Convert.ToString(Atrybuty[3]);
            poziomPieniedzy.Text = Convert.ToString(Atrybuty[4]);
            poziomDoswiadczenia.Text = Convert.ToString(Atrybuty[5]);
            poziomZdrowia.Text = Convert.ToString(Atrybuty[6]);
            #endregion

            #region -> CZYSZCZENIE WYGLĄDU COMBOBOXA CZYNNOŚCI <-
            Cmb.Items.Clear();
            Cmb.Items.Insert(0, "--- Wybierz czynność ---");
            Cmb.SelectedIndex = 0;
            NazwaCzynnosci.Text = "WYBIERZ CZYNNOŚĆ:";
            Czynnosci.Background = new SolidColorBrush(Colors.WhiteSmoke);
            #endregion
            
            Sprawdz_czy_zyjesz();       //FUNKCJA SPRAWDZAJĄCA ATRYBUTY
        }
        #endregion

        #region <<< PRZYCISKI CZYNNOŚCI >>>
        #region --- JEDZ ---
        private void Jedz_Click(object sender, RoutedEventArgs e)
        {
            Przypisz();
            Cmb.Items.Clear();
            Cmb.Items.Insert(0, "KURCZACZEK W SOSIE");
            Cmb.Items.Insert(1, "SUCHA KARMA");
            Cmb.Items.Insert(2, "LUDZKIE JEDZENIE");
            NazwaCzynnosci.Text = "JEDZ:";
            Czynnosci.Background = new SolidColorBrush(Color.FromArgb(255, 238, 132, 122));
        }

        private async void Zjedz()
        {
            int item = 3;
            var dialog = new MessageDialog("Nic nie zjadł :(");
            try
            {
                item = Cmb.SelectedIndex;
            }
            catch
            {
                await dialog.ShowAsync();
            }
            switch (item)
            {
                case 0:
                    Atrybuty[0] += 10;      //energia
                    Atrybuty[1] += 25;     //odżywianie  
                    Atrybuty[2] -= 12;     //higiena
                    Atrybuty[3] += 9;     //sprawność
                    Atrybuty[4] -= 15;     //pieniądze
                    Atrybuty[5] += 12;      //doświadczenie
                    Atrybuty[6] += 10;      //zdrowie
                    break;
                case 1:
                    Atrybuty[0] += 9;      //energia
                    Atrybuty[1] += 15;     //odżywianie  
                    Atrybuty[2] -= 2;     //higiena
                    Atrybuty[3] += 3;     //sprawność
                    Atrybuty[4] -= 11;     //pieniądze
                    Atrybuty[5] += 8;      //doświadczenie
                    Atrybuty[6] += 8;      //zdrowie
                    break;
                case 2:
                    Atrybuty[0] += 5;      //energia
                    Atrybuty[1] += 8;     //odżywianie  
                    Atrybuty[2] -= 8;     //higiena
                    Atrybuty[3] += 7;     //sprawność
                    Atrybuty[4] -= 8;     //pieniądze
                    Atrybuty[5] += 18;      //doświadczenie
                    Atrybuty[6] -= 3;      //zdrowie
                    break;
                default:
                    break;
            }
        }
        #endregion
        #region --- PIJ ---
        private void Pij_Click(object sender, RoutedEventArgs e)
        {
            Przypisz();
            Cmb.Items.Clear();
            Cmb.Items.Insert(0, "WODA");
            Cmb.Items.Insert(1, "HERBATA");
            Cmb.Items.Insert(2, "MLEKO");
            NazwaCzynnosci.Text = "PIJ:";
            Czynnosci.Background = new SolidColorBrush(Color.FromArgb(255, 173, 127, 255));
        }
        private async void Wypij()
        {
            int item = 3;
            var dialog = new MessageDialog("Nic nie wypił :(");
            try
            {
                item = Cmb.SelectedIndex;
            }
            catch
            {
                await dialog.ShowAsync();
            }
            switch (item)
            {
                case 0:
                    Atrybuty[0] += 8;      //energia
                    Atrybuty[1] += 10;     //odżywianie  
                                           // Atrybuty[2] -= 12;     //higiena	
                    Atrybuty[3] += 2;     //sprawność
                    Atrybuty[4] -= 5;     //pieniądze
                    Atrybuty[5] += 2;      //doświadczenie
                    Atrybuty[6] += 5;      //zdrowie
                    break;
                case 1:
                    Atrybuty[0] += 3;      //energia
                    Atrybuty[1] += 5;     //odżywianie  
                    Atrybuty[2] -= 5;     //higiena
                    Atrybuty[3] += 2;     //sprawność
                    Atrybuty[4] -= 3;     //pieniądze
                    Atrybuty[5] += 2;      //doświadczenie
                    Atrybuty[6] -= 2;      //zdrowie
                    break;
                case 2:
                    Atrybuty[0] += 5;      //energia
                    Atrybuty[1] += 15;     //odżywianie  
                    Atrybuty[2] -= 2;     //higiena
                    Atrybuty[3] += 2;     //sprawność
                    Atrybuty[4] -= 9;     //pieniądze
                    Atrybuty[5] += 2;      //doświadczenie
                    Atrybuty[6] -= 5;      //zdrowie

                    break;
                default:
                    break;
            }
        }
        #endregion
        #region --- ŚPIJ ---
        private void Spij_Click(object sender, RoutedEventArgs e)
        {
            Przypisz();
            Cmb.Items.Clear();
            Cmb.Items.Insert(0, "3 GODZINY SNU – na dywanie");
            Cmb.Items.Insert(1, "6 GODZIN SNU – na fotelu ");
            Cmb.Items.Insert(2, "9 GODZIN SNU - na kanapie");
            NazwaCzynnosci.Text = "ŚPIJ:";
            Czynnosci.Background = new SolidColorBrush(Color.FromArgb(255, 122, 206, 253));
        }

        private async void Wyspij_sie()
        {
            int item = 3;
            var dialog = new MessageDialog("Nie zasnął :(");
            try
            {
                item = Cmb.SelectedIndex;
            }
            catch
            {
                await dialog.ShowAsync();
            }
            switch (item)
            {
                case 0:
                    Atrybuty[0] += 15;      //energia
                    Atrybuty[1] -= 10;     //odżywianie  
                    Atrybuty[2] -= 3;     //higiena
                    Atrybuty[3] += 3;     //sprawność
                    Atrybuty[5] += 5;      //doświadczenie
                    Atrybuty[6] += 8;      //zdrowie

                    break;
                case 1:
                    Atrybuty[0] += 30;      //energia
                    Atrybuty[1] -= 20;     //odżywianie  
                    Atrybuty[2] -= 6;     //higiena
                    Atrybuty[3] += 5;     //sprawność
                    Atrybuty[5] += 5;      //doświadczenie
                    Atrybuty[6] += 16;      //zdrowie
                    break;
                case 2:
                    Atrybuty[0] += 45;      //energia
                    Atrybuty[1] -= 30;     //odżywianie  
                    Atrybuty[2] -= 9;     //higiena
                    Atrybuty[3] += 7;     //sprawność
                    Atrybuty[5] += 5;      //doświadczenie
                    Atrybuty[6] += 24;      //zdrowie
                    break;
                default:
                    break;
            }
        }
        #endregion
        #region --- WYKĄP SIĘ ---
        private void Wykap_Click(object sender, RoutedEventArgs e)
        {
            Przypisz();
            Cmb.Items.Clear();
            Cmb.Items.Insert(0, "KORZYSTAJ Z KUWETY");
            Cmb.Items.Insert(1, "UMYJ SIĘ");
            Cmb.Items.Insert(2, "PORPOŚ O KĄPIEL");
            NazwaCzynnosci.Text = "WYKĄP SIĘ:";
            Czynnosci.Background = new SolidColorBrush(Color.FromArgb(255, 210, 145, 253));
        }

        private async void Wykap_sie()
        {
            int item = 3;
            var dialog = new MessageDialog("Nie umył się :(");
            try
            {
                item = Cmb.SelectedIndex;
            }
            catch
            {
                await dialog.ShowAsync();
            }
            switch (item)
            {
                case 0:
                    Atrybuty[0] += 10;      //energia
                    Atrybuty[1] -= 18;     //odżywianie  
                    Atrybuty[2] += 10;     //higiena
                    Atrybuty[3] += 2;     //sprawność
                    Atrybuty[4] -= 5;     //pieniądze
                    Atrybuty[5] += 6;      //doświadczenie
                    Atrybuty[6] += 10;      //zdrowie
                    break;
                case 1:
                    Atrybuty[0] -= 3;      //energia
                    Atrybuty[1] -= 10;     //odżywianie  
                    Atrybuty[2] += 20;     //higiena
                    Atrybuty[3] += 9;     //sprawność
                    Atrybuty[5] += 12;      //doświadczenie
                    Atrybuty[6] += 7;      //zdrowie
                    break;
                case 2:
                    Atrybuty[0] -= 10;      //energia
                    Atrybuty[1] -= 6;     //odżywianie  
                    Atrybuty[2] += 40;     //higiena
                    Atrybuty[3] += 17;     //sprawność
                    Atrybuty[4] -= 5;     //pieniądze
                    Atrybuty[5] += 2;      //doświadczenie
                    Atrybuty[6] -= 15;      //zdrowie
                    break;
                default:
                    break;
            }
        }
        #endregion
        #region --- IDŹ DO LEKARZA ---
        private void Lekarz_Click(object sender, RoutedEventArgs e)
        {
            Przypisz();
            Cmb.Items.Clear();
            Cmb.Items.Insert(0, "OBCIĘCIE PAZURÓW");
            Cmb.Items.Insert(1, "TABLETKA NA ODROBACZENIE");
            Cmb.Items.Insert(2, "SZCZEPIENIE");
            NazwaCzynnosci.Text = "IDŹ DO LEKARZA:";
            Czynnosci.Background = new SolidColorBrush(Color.FromArgb(255, 255, 198, 107));
        }

        private async void Idz_do_lekarza()
        {
            int item = 3;
            var dialog = new MessageDialog("Nie poszedł do lekarza :(");
            try
            {
                item = Cmb.SelectedIndex;
            }
            catch
            {
                await dialog.ShowAsync();
            }
            switch (item)
            {
                case 0:
                    Atrybuty[0] -= 15;      //energia
                    Atrybuty[1] -= 12;     //odżywianie  
                    Atrybuty[2] += 10;     //higiena
                    Atrybuty[3] += 13;     //sprawność
                    Atrybuty[4] -= 14;     //pieniądze
                    Atrybuty[5] += 26;      //doświadczenie
                    Atrybuty[6] += 10;      //zdrowie


                    break;
                case 1:
                    Atrybuty[0] -= 5;      //energia
                    Atrybuty[1] -= 23;     //odżywianie  
                    Atrybuty[2] += 5;     //higiena
                    Atrybuty[3] += 17;     //sprawność
                    Atrybuty[4] -= 8;     //pieniądze
                    Atrybuty[5] += 13;      //doświadczenie
                    Atrybuty[6] += 17;      //zdrowie
                    break;
                case 2:
                    Atrybuty[0] -= 5;      //energia
                    Atrybuty[1] -= 5;     //odżywianie  
                    Atrybuty[3] += 10;     //sprawność
                    Atrybuty[4] -= 20;     //pieniądze
                    Atrybuty[5] += 5;      //doświadczenie
                    Atrybuty[6] += 25;      //zdrowie
                    break;
                default:
                    break;
            }
        }
        #endregion
        #region --- PRACUJ ---
        private void Pracuj_Click(object sender, RoutedEventArgs e)
        {
            Przypisz();
            Cmb.Items.Clear();
            Cmb.Items.Insert(0, "GOŃ MUCHY");
            Cmb.Items.Insert(1, "GRZEJ CZŁOWIEKA");
            Cmb.Items.Insert(2, "PILNUJ DOMU");
            NazwaCzynnosci.Text = "PRACUJ:";
            Czynnosci.Background = new SolidColorBrush(Color.FromArgb(255, 129, 158, 255));
        }

        private async void Praca()
        {
            int item = 3;
            var dialog = new MessageDialog("Nie pracuje :(");
            try
            {
                item = Cmb.SelectedIndex;
            }
            catch
            {
                await dialog.ShowAsync();
            }
            switch (item)
            {
                case 0:
                    Atrybuty[0] -= 25;      //energia
                    Atrybuty[1] -= 10;     //odżywianie  
                    Atrybuty[2] -= 20;     //higiena
                    Atrybuty[3] += 25;     //sprawność
                    Atrybuty[4] += 20;     //pieniądze
                    Atrybuty[5] += 15;      //doświadczenie
                    Atrybuty[6] -= 20;      //zdrowie
                    break;
                case 1:
                    Atrybuty[0] -= 15;      //energia
                    Atrybuty[1] -= 7;     //odżywianie  
                    Atrybuty[2] -= 6;     //higiena
                    Atrybuty[3] += 8;     //sprawność
                    Atrybuty[4] += 10;     //pieniądze
                    Atrybuty[5] += 20;      //doświadczenie
                    Atrybuty[6] -= 8;      //zdrowie

                    break;
                case 2:
                    Atrybuty[0] -= 19;      //energia
                    Atrybuty[1] -= 18;     //odżywianie  
                    Atrybuty[2] -= 6;     //higiena
                    Atrybuty[3] += 14;     //sprawność
                    Atrybuty[4] += 30;     //pieniądze
                    Atrybuty[5] += 25;      //doświadczenie
                    Atrybuty[6] -= 12;      //zdrowie

                    break;
                default:
                    break;
            }
        }
        #endregion
        #region --- BAW SIĘ ---
        private void BawSie_Click(object sender, RoutedEventArgs e)
        {
            Przypisz();
            Cmb.Items.Clear();
            Cmb.Items.Insert(0, "BAW SIĘ Z CZŁOWIEKIEM");
            Cmb.Items.Insert(1, "BAW SIĘ PLUSZOWĄ MYSZKĄ ");
            Cmb.Items.Insert(2, "DRAP KANAPĘ");
            NazwaCzynnosci.Text = "BAW SIĘ:";
            Czynnosci.Background = new SolidColorBrush(Color.FromArgb(255, 163, 247, 166));
        }

        private async void Zabawa()
        {
            int item = 3;
            var dialog = new MessageDialog("Nie bawi się :(");
            try
            {
                item = Cmb.SelectedIndex;
            }
            catch
            {
                await dialog.ShowAsync();
            }
            switch (item)
            {
                case 0:
                    Atrybuty[0] -= 25;      //energia
                    Atrybuty[1] -= 7;     //odżywianie  
                    Atrybuty[2] -= 22;     //higiena
                    Atrybuty[3] += 16;     //sprawność
                    Atrybuty[5] += 30;      //doświadczenie
                    Atrybuty[6] -= 12;      //zdrowie

                    break;
                case 1:
                    Atrybuty[0] -= 40;      //energia
                    Atrybuty[1] -= 10;     //odżywianie  
                    Atrybuty[2] -= 30;     //higiena
                    Atrybuty[3] += 23;     //sprawność
                    Atrybuty[4] -= 12;     //pieniądze
                    Atrybuty[5] += 18;      //doświadczenie
                    Atrybuty[6] -= 8;      //zdrowie
                    break;
                case 2:
                    Atrybuty[0] -= 20;      //energia
                    Atrybuty[1] -= 3;     //odżywianie  
                    Atrybuty[2] -= 16;     //higiena
                    Atrybuty[3] += 9;     //sprawność
                    Atrybuty[4] -= 40;     //pieniądze
                    Atrybuty[5] += 15;      //doświadczenie
                    Atrybuty[6] -= 6;      //zdrowie

                    break;
                default:
                    break;
            }
        }
        #endregion
        #region ---RELAKS ---
        private void Relaks_Click(object sender, RoutedEventArgs e)
        {
            Przypisz();
            Cmb.Items.Clear();
            Cmb.Items.Insert(0, "WSKOCZ DO KARTONU");
            Cmb.Items.Insert(1, "DAJ SIĘ GŁASKAĆ");
            Cmb.Items.Insert(2, "OGLĄDAJ PTAKI PRZEZ OKNO");
            NazwaCzynnosci.Text = "RELAKS:";
            Czynnosci.Background = new SolidColorBrush(Color.FromArgb(255, 242, 221, 81));
        }

        private async void Relaksuj_sie()
        {
            int item = 3;
            var dialog = new MessageDialog("Nic nie zrobił:(");
            try
            {
                item = Cmb.SelectedIndex;
            }
            catch
            {
                await dialog.ShowAsync();
            }
            switch (item)
            {
                case 0:
                    Atrybuty[0] += 5;      //energia
                    Atrybuty[1] -= 5;     //odżywianie  
                    Atrybuty[2] -= 7;     //higiena
                    Atrybuty[3] += 6;     //sprawność
                    Atrybuty[5] += 10;      //doświadczenie
                    Atrybuty[6] -= 12;      //zdrowie
                    break;
                case 1:
                    Atrybuty[0] += 8;      //energia
                    Atrybuty[1] -= 3;     //odżywianie  
                    Atrybuty[2] -= 5;     //higiena
                    Atrybuty[3] += 10;     //sprawność
                    Atrybuty[5] += 15;      //doświadczenie
                    Atrybuty[6] -= 3;      //zdrowie
                    break;
                case 2:
                    Atrybuty[0] += 7;      //energia
                    Atrybuty[1] -= 5;     //odżywianie       
                    Atrybuty[5] += 20;      //doświadczenie
                    Atrybuty[6] -= 7;      //zdrowie
                    break;
                default:
                    break;
            }
        }
        #endregion

        #endregion

        #region <<< PRZYCISKI FUNKCYJNE >>>
        #region -> BUTTON SAVE <-
        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            if(DaneLogowania.Login == "user")
            {
                var dialog = new MessageDialog("Zaloguj się aby móc korzystać z funkcji zapisu");
                await dialog.ShowAsync();
            }
            else
            {
                SQLiteConnection dbConnection = new SQLiteConnection("Baza.db");

                DateTime date = DateTime.Now;
                string email = DaneLogowania.Login;
                string password = DaneLogowania.Hasło;
                string imie = DaneLogowania.Koteł_Imie;
                string energia = poziomEnergii.Text;
                string odzywianie = poziomOdzywienia.Text;
                string higiena = poziomHigieny.Text;
                string sprawnosc = poziomSprawnosci.Text;
                string pieniadze = poziomPieniedzy.Text;
                string doswiadczenie = poziomDoswiadczenia.Text;
                string zdrowie = poziomZdrowia.Text;
                string sSQL = @"UPDATE [Users]
                        SET [ENERGIA]=" + energia + ", [ODZYWIANIE]=" + odzywianie + ",[HIGIENA]=" + higiena +
                            ",[SPRAWNOSC]=" + sprawnosc + ",[PIENIADZE]=" + pieniadze + ",[DOSWIADCZENIE]=" +
                            doswiadczenie + ",[ZDROWIE]=" + zdrowie + ",[DATA]='" + date + 
                            "' WHERE [MAIL]='" + email + "';";

                dbConnection.Prepare(sSQL).Step();
                if(CheckSaving==0)
                { 
                    var dialog = new MessageDialog("Zapisano pomyślnie!");
                    await dialog.ShowAsync();
                }
            }
        }
        #endregion

        #region -> BUTTON EXIT <-
        private async void Exit_Click(object sender, RoutedEventArgs e)
        {
            Exit exit = new Exit();
            await exit.ShowAsync();
            Frame.Navigate(typeof(LogIn_SignUp));
        }
        #endregion

        #region -> BUTTON HELP <-
        private async void Help_Click(object sender, RoutedEventArgs e)
        {
            Help help = new Help();
            await help.ShowAsync();
        }
        #endregion
        #endregion

        #region <<< CHECKBOX SOUND >>>
        private void CheckSound_Checked(object sender, RoutedEventArgs e)
        {
            if (CheckSoundIs == 1)
            {
                PlaySound.IsMuted = false;
            }
        }

        private void CheckSound_Unchecked(object sender, RoutedEventArgs e)
        {
            PlaySound.IsMuted = true;
            CheckSoundIs = 1;
        }
        #endregion

        #region <<< COMBOBOX TŁA >>>
        private void ComboTlo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBoxItem = e.AddedItems[0] as ComboBoxItem;
            if (comboBoxItem == null) return;
            var content = comboBoxItem.Content as string;
            if (content != null)
            {
                switch (content)
                {
                    case "Różowy":
                        main.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(BaseUri, "Images/Background2.jpg")), Stretch = Stretch.UniformToFill };
                        Title.Foreground = new SolidColorBrush(Color.FromArgb(255, 121, 20, 91));
                        break;
                    case "Żółty":
                        main.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(BaseUri, "Images/Background.png")), Stretch = Stretch.UniformToFill };
                        Title.Foreground = new SolidColorBrush(Color.FromArgb(255, 172, 89, 16));
                        break;
                    case "Czerwony":
                        main.Background = new SolidColorBrush(Color.FromArgb(255, 219, 75, 75));
                        Title.Foreground = new SolidColorBrush(Color.FromArgb(255, 112, 8, 8));
                        break;
                    case "Niebieski":
                        main.Background = new SolidColorBrush(Colors.CornflowerBlue);
                        Title.Foreground = new SolidColorBrush(Color.FromArgb(255, 26, 32, 90));
                        break;
                    case "XP":
                        main.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(BaseUri, "Images/Background4.jpg")), Stretch = Stretch.UniformToFill };
                        Title.Foreground = new SolidColorBrush(Color.FromArgb(255, 26, 32, 90));
                        break;
                    default:
                        break;
                }
                
            }
        }

        #endregion
    }
}
