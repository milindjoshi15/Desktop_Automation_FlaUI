using FlaUI.Core.AutomationElements;
using FlaUI.Core.Conditions;

namespace FlaUiTests.Pages
{
    public class RegistrationPage
    {
        private readonly Window _mainWindow;
        private readonly ConditionFactory _cf;
        // Element references (locators)
        private readonly TextBox firstNameBox;
        private readonly TextBox lastNameBox;
        private readonly ComboBox ageCombo;
        private readonly ComboBox countryCombo;
        private readonly TextBox phoneBox;
        private readonly TextBox emailBox;
        private readonly TextBox passBox;
        private readonly TextBox cardBox;
        private readonly CheckBox vipCheck;
        private readonly Button okButton;
        private readonly Button cancelButton;
        
        public RegistrationPage(Window mainWindow, ConditionFactory cf)
        {
            _mainWindow = mainWindow;
            _cf = cf;

            // Initialize element references
            firstNameBox = _mainWindow.FindFirstDescendant(_cf.ByAutomationId("InFName"))?.AsTextBox();
            lastNameBox = _mainWindow.FindFirstDescendant(_cf.ByAutomationId("InLName"))?.AsTextBox();
            ageCombo = _mainWindow.FindFirstDescendant(_cf.ByAutomationId("InAge"))?.AsComboBox();
            countryCombo = _mainWindow.FindFirstDescendant(_cf.ByAutomationId("InCountry"))?.AsComboBox();
            phoneBox = _mainWindow.FindFirstDescendant(_cf.ByAutomationId("InPhone"))?.AsTextBox();
            emailBox = _mainWindow.FindFirstDescendant(_cf.ByAutomationId("InEmail"))?.AsTextBox();
            passBox = _mainWindow.FindFirstDescendant(_cf.ByAutomationId("InPass"))?.AsTextBox();
            cardBox = _mainWindow.FindFirstDescendant(_cf.ByAutomationId("InCard"))?.AsTextBox();
            vipCheck = _mainWindow.FindFirstDescendant(_cf.ByAutomationId("VipCheck"))?.AsCheckBox();
            okButton = _mainWindow.FindFirstDescendant(_cf.ByName("Ok"))?.AsButton();
            cancelButton = _mainWindow.FindFirstDescendant(_cf.ByName("Cancel"))?.AsButton();
        }

        public bool RegisterUser(UserData user)
        {
            FillForm(user.FirstName, user.LastName, user.Age, user.Country, user.Phone, user.Email, user.Password, user.Card, user.Vip);
            return SubmitAndHandleResult();
        }

        public void FillForm(string firstName, string lastName, string age, string country, string phone, string email, string pass, string card, bool vip)
        {
            firstNameBox?.Enter(firstName);
            lastNameBox?.Enter(lastName);

            if (ageCombo != null)
            {
                ageCombo.Patterns.ExpandCollapse.Pattern.Expand();
                ageCombo.Select(age);
            }

            if (countryCombo != null)
            {
                countryCombo.Patterns.ExpandCollapse.Pattern.Expand();
                countryCombo.Select(country);
            }

            phoneBox?.Enter(phone);
            emailBox?.Enter(email);
            passBox?.Enter(pass);
            cardBox?.Enter(card);
            if (vip)
            {
                vipCheck?.Click();
            }
        }

        public bool SubmitAndHandleResult()
        {
            okButton?.Click();
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
                cancelButton?.Click();
                return false;
            }
            return false;
        }
    }
}