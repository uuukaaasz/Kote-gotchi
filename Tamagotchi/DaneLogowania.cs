using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using SQLitePCL;
using Windows.UI.Xaml;

namespace Tamagotchi
{
    class DaneLogowania
    {
        public static string Login = null;
        public static string Hasło = null;
        public static string Koteł_Imie = null;
        public static string[] Attribiutes = new string[7];
        public static DateTime data = DateTime.Now;
    }
}
