using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.UIA3;
using System;
using System.Diagnostics;
using System.Linq;

namespace FlaUiTests.Pages
{
    public class CalculatorPage : IDisposable
    {
        private readonly Process? process;
        private readonly Application application;
        private readonly UIA3Automation automation;
        private readonly Window mainWindow;
        public CalculatorPage()
        {
            try
            {
                application = Application.Launch(@"C:\\Windows\\System32\\calc.exe");
                automation = new UIA3Automation();               
                // If not found, try to attach to any running Calculator process by common names
                if (mainWindow == null)
                {
                    var candidates = new[] { "Calculator", "CalculatorApp", "calc", "WindowsCalculator", "ApplicationFrameHost" };
                    foreach (var name in candidates)
                    {
                        try
                        {
                            var proc = Process.GetProcessesByName(name).FirstOrDefault();
                            if (proc != null && !proc.HasExited)
                            {
                                try
                                {
                                    application = Application.Attach(proc.Id);
                                    mainWindow = application.GetMainWindow(automation, TimeSpan.FromSeconds(5));
                                }
                                catch { }
                                if (mainWindow != null)
                                    break;
                            }
                        }
                        catch { }
                    }                    
                    if (mainWindow == null)
                    {
                        // Fail fast with a clear exception so tests don't get a null reference later
                        throw new InvalidOperationException("Unable to start or attach to Calculator main window.");
                    }
                }
            }
            catch
            {
                // In case of any failure, initialize automation so Dispose can run safely
                automation = new UIA3Automation();
                process = null;
                application = null!;
                mainWindow = null;
                throw;
            }
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
