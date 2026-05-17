using FlaUI.Core;
using FlaUI.UIA3;
using FlaUI.Core.AutomationElements;
using System;
using System.Diagnostics;
using System.Linq;

namespace FlaUiTests.Pages
{
    public class CalculatorPage : IDisposable
    {
        private readonly Application _application;
        private readonly UIA3Automation _automation;
        private readonly Window _mainWindow;

        public CalculatorPage()
        {
            // Launch or attach to Calculator in a way that works for both classic and Store Calculator
            var psi = new ProcessStartInfo("calc.exe") { UseShellExecute = true };
            _application = Application.AttachOrLaunch(psi);
            _automation = new UIA3Automation();

            try
            {
                // Wait up to 5 seconds for the main window to appear
                _mainWindow = _application.GetMainWindow(_automation, TimeSpan.FromSeconds(5));
            }
            catch
            {
                // If the launched process exited (store app behavior), try to attach to an existing Calculator process
                try
                {
                    var procs = Process.GetProcessesByName("Calculator");
                    var proc = procs.FirstOrDefault() ?? Process.GetProcessesByName("CalculatorApp").FirstOrDefault();
                    if (proc != null)
                    {
                        _application = Application.Attach(proc.Id);
                        _mainWindow = _application.GetMainWindow(_automation, TimeSpan.FromSeconds(5));
                    }
                }
                catch
                {
                    // ignore - _mainWindow may remain null and actions will be null-safe
                }
            }
        }

        public void PressByAutomationId(string automationId)
        {
            var button = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId(automationId))?.AsButton();
            button?.Invoke();
        }

        public string GetResultText()
        {
            var resultElement = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("CalculatorResults"));
            // Some Calculator versions expose the result as the Name property rather than a Label control
            if (resultElement == null) return string.Empty;
            var name = resultElement.Name;
            if (!string.IsNullOrEmpty(name)) return name;
            return resultElement.AsLabel()?.Text ?? string.Empty;
        }

        public void Close()
        {
            try { _mainWindow.Close(); } catch { }
        }

        public void Dispose()
        {
            try { _automation?.Dispose(); } catch { }
            try
            {
                if (_application != null && !_application.HasExited)
                {
                    _application.Close();
                }
            }
            catch
            {
                try { _application?.Kill(); } catch { }
            }
        }
    }
}
