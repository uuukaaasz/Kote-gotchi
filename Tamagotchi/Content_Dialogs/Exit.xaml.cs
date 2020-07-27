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

//Szablon elementu Okno dialogowe zawartości jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=234238

namespace Tamagotchi.Content_Dialogs
{
    public sealed partial class Exit : ContentDialog
    {
        public Exit()
        {
            this.InitializeComponent();
        }

        private void ContentDialog_Exit(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Application.Current.Exit();
        }

        private void ContentDialog_Logout(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            DaneLogowania.Login = null;
            DaneLogowania.Hasło = null;
            DaneLogowania.Koteł_Imie = null;
            for (int i = 0; i < 7; i++)
                DaneLogowania.Attribiutes[i] = null;
        }
    }
}
