using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Client.ViewModels
{
    public static class Settings
    {
        public static Regex RegForName = new Regex(@"[^a-zA-Z]");
        public static Regex RegForPassword = new Regex(@"[\W]");
        public static int MinLengthPassword = 8;
        public static int MaxSizeScreen = 2097152;
        public static int MaxSizeFile = 209715200;
    }
}
