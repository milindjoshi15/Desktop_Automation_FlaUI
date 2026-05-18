using FlaUI.Core;
using FlaUI.Core.Conditions;
using FlaUI.UIA3;
using System;
using System.Configuration;

namespace FlaUiTests.Pages
{
    public class MainWindowPage : IDisposable
    {
        public Application Application { get; }
        public UIA3Automation Automation { get; }
        public FlaUI.Core.AutomationElements.Window MainWindow { get; }
        public ConditionFactory Cf { get; }

        public MainWindowPage(string exePath)
        {
            Application = Application.Launch(exePath);
            Automation = new UIA3Automation();
            MainWindow = Application.GetMainWindow(Automation);
            Cf = new ConditionFactory(new UIA3PropertyLibrary());
        }

        // Default constructor reads exe path from configuration (BankSystemExePath)
        public MainWindowPage()
            : this(ConfigurationManager.AppSettings["BankSystemExePath"] ?? @"C:\\Users\\Admin\\Downloads\\FlaUIPractice-master\\FlaUIPractice-master\\FlaUIPractice\\BankSystem\\bin\\Release\\BankSystem.exe")
        {
        }

        public RegistrationPage OpenRegistration()
        {
            MainWindow.FindFirstDescendant(Cf.ByName("Registration")).AsButton().Click();
            return new RegistrationPage(MainWindow, Cf);
        }

        public bool RegisterNewUser(UserData user)
        {
            var reg = OpenRegistration();
            return reg.RegisterUser(user);
        }

        public void ExitAndConfirm()
        {
            MainWindow.FindFirstDescendant(Cf.ByName("Exit")).AsButton().Click();
            var exitWindow = MainWindow.FindFirstDescendant(Cf.ByName("Exit")).AsWindow();
            exitWindow.FindFirstDescendant(Cf.ByName("Yes")).AsButton().Click();
        }

        public void Dispose()
        {
            try
            {
                Automation?.Dispose();
            }
            catch { }
            try
            {
                Application?.Close();
            }
            catch { }
        }
    }
}
