using System.Configuration;

namespace FlaUiTests.Pages
{
    public static class PageSettings
    {
        public static string BankSystemExePath => ConfigurationManager.AppSettings["BankSystemExePath"]
            ?? @"C:\\Users\\Admin\\Downloads\\FlaUIPractice-master\\FlaUIPractice-master\\FlaUIPractice\\BankSystem\\bin\\Release\\BankSystem.exe";

        public static string WinFormsAppExePath => ConfigurationManager.AppSettings["WinFormsAppExePath"]
            ?? @"C:\\Users\\Admin\\Downloads\\FlaUIPractice-master\\FlaUIPractice-master\\FlaUIPractice\\FlaUiTests\\Resources\\WinFormsApplication.exe";

        public static string PicturesFolder => ConfigurationManager.AppSettings["PicturesFolder"]
            ?? @"C:\\Users\\Admin\\Pictures\\Saved Pictures";
    }
}
