using FlaUI.Core;
using FlaUI.UIA3;
using FlaUI.Core.AutomationElements;
using System;

namespace FlaUiTests.Pages
{
    public class CalculatorPage : IDisposable
    {
        private readonly Application application;
        private readonly UIA3Automation automation;
        private readonly Window mainWindow;

        public CalculatorPage()
        {
            application = Application.Launch("calc.exe");
            automation = new UIA3Automation();
            mainWindow = application.GetMainWindow(automation);
        }

        public void PressByAutomationId(string automationId)
        {
            var button = mainWindow.FindFirstDescendant(cf => cf.ByAutomationId(automationId))?.AsButton();
            button?.Invoke();
        }

        public string GetResultText()
        {
            var resultElement = mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("CalculatorResults"));
            return resultElement?.AsLabel()?.Text ?? string.Empty;
        }

        public void Close()
        {
            try { mainWindow.Close(); } catch { }
        }

        public void Dispose()
        {
            try { automation?.Dispose(); } catch { }
            try { application?.Close(); } catch { }
        }
    }
}
