using FlaUI.Core.AutomationElements;
using FlaUI.Core.Conditions;
using System;

namespace FlaUiTests.Pages
{
    public class RegistrationPage
    {
        private readonly Window _mainWindow;
        private readonly ConditionFactory _cf;

        public RegistrationPage(Window mainWindow, ConditionFactory cf)
        {
            _mainWindow = mainWindow;
            _cf = cf;
        }

        public void FillForm(string firstName, string lastName, string age, string country, string phone, string email, string pass, string card, bool vip)
        {
            _mainWindow.FindFirstDescendant(_cf.ByAutomationId("InFName")).AsTextBox().Enter(firstName);
            _mainWindow.FindFirstDescendant(_cf.ByAutomationId("InLName")).AsTextBox().Enter(lastName);

            var ageBox = _mainWindow.FindFirstDescendant(_cf.ByAutomationId("InAge")).AsComboBox();
            if (ageBox != null)
            {
                ageBox.Patterns.ExpandCollapse.Pattern.Expand();
                ageBox.Select(age);
            }

            var countryBox = _mainWindow.FindFirstDescendant(_cf.ByAutomationId("InCountry")).AsComboBox();
            if (countryBox != null)
            {
                countryBox.Patterns.ExpandCollapse.Pattern.Expand();
                countryBox.Select(country);
            }

            _mainWindow.FindFirstDescendant(_cf.ByAutomationId("InPhone")).AsTextBox().Enter(phone);
            _mainWindow.FindFirstDescendant(_cf.ByAutomationId("InEmail")).AsTextBox().Enter(email);
            _mainWindow.FindFirstDescendant(_cf.ByAutomationId("InPass")).AsTextBox().Enter(pass);
            _mainWindow.FindFirstDescendant(_cf.ByAutomationId("InCard")).AsTextBox().Enter(card);
            if (vip)
            {
                _mainWindow.FindFirstDescendant(_cf.ByAutomationId("VipCheck")).AsCheckBox().Click();
            }
        }

        public bool SubmitAndHandleResult()
        {
            _mainWindow.FindFirstDescendant(_cf.ByName("Ok")).AsButton().Click();
            var congrats = _mainWindow.FindFirstDescendant(_cf.ByName("Congratulations"));
            if (congrats != null)
            {
                congrats.AsWindow().FindFirstDescendant(_cf.ByName("OK")).AsButton().Click();
                return true;
            }
            var error = _mainWindow.FindFirstDescendant(_cf.ByName("Error"));
            if (error != null)
            {
                error.AsWindow().FindFirstDescendant(_cf.ByName("OK")).AsButton().Click();
                _mainWindow.FindFirstDescendant(_cf.ByName("Cancel")).AsButton().Click();
                return false;
            }
            return false;
        }
    }
}
