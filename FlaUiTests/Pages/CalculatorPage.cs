using FlaUI.Core;
using FlaUI.UIA2;
using FlaUI.Core.AutomationElements;
using System;

namespace FlaUiTests.Pages
{
    public class CalculatorPage : IDisposable
    {
        private readonly Application _application;
        private readonly UIA2Automation _automation;
        private readonly Window _mainWindow;

        public CalculatorPage()
        {
            _application = Application.Launch("calc.exe");
            _automation = new UIA2Automation();
            _mainWindow = _application.GetMainWindow(_automation);
        }

        public void PressByAutomationId(string automationId)
        {
            _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId(automationId))
                       ?.AsButton()
                       .Invoke();
        }

        public string GetResultText()
        {
            var resultElement = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("CalculatorResults"));
            return resultElement?.AsLabel().Text;
        }

        public void Close()
        {
            try { _mainWindow.Close(); } catch { }
        }

        public void Dispose()
        {
            try { _automation?.Dispose(); } catch { }
            try { _application?.Close(); } catch { }
        }
    }
}
