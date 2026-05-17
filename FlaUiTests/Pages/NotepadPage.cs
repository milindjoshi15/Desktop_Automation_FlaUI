using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.UIA3;
using System;
using System.Diagnostics;
using System.Linq;

namespace FlaUiTests.Pages
{
    public class NotepadPage : IDisposable
    {
        private readonly Process? process;
        private Application application;
        private readonly UIA3Automation automation;
        private Window? mainWindow;

        public NotepadPage()
        {
            // Start or attach to Notepad in a resilient way
            try
            {
                var psi = new ProcessStartInfo("notepad.exe") { UseShellExecute = true };
                application = Application.AttachOrLaunch(psi);
                automation = new UIA3Automation();

                try
                {
                    mainWindow = application.GetMainWindow(automation, TimeSpan.FromSeconds(5));
                }
                catch
                {
                    // If the started process exited or main window wasn't found, try to attach to existing notepad process
                    try
                    {
                        var proc = Process.GetProcessesByName("notepad").FirstOrDefault();
                        if (proc != null)
                        {
                            application = Application.Attach(proc.Id);
                            mainWindow = application.GetMainWindow(automation, TimeSpan.FromSeconds(5));
                        }
                    }
                    catch { }
                }

                // Try to set process to the launched process if available
                try { process = Process.GetProcessById(application.ProcessId); } catch { process = null; }
            }
            catch
            {
                // In case of any failure, initialize automation so Dispose can run safely
                automation = new UIA3Automation();
                process = null;
                application = null!;
                mainWindow = null;
            }
        }

        public TextBox? GetEditor()
        {
            if (mainWindow == null) return null;
            // Notepad sometimes exposes the editor as Document or as Edit; try both
            var doc = mainWindow.FindFirstDescendant(cf => cf.ByControlType(FlaUI.Core.Definitions.ControlType.Document));
            if (doc != null) return doc.AsTextBox();
            var edit = mainWindow.FindFirstDescendant(cf => cf.ByControlType(FlaUI.Core.Definitions.ControlType.Edit));
            return edit?.AsTextBox();
        }

        public void EnterText(string text)
        {
            var edit = GetEditor();
            if (edit != null)
            {
                edit.Focus();
                edit.Enter(text);
            }
            else
            {
                // Fallback: send keyboard input
                FlaUI.Core.Input.Keyboard.Type(text);
            }
        }

        public string Text
        {
            get { return GetEditor()?.Text ?? string.Empty; }
            set { var e = GetEditor(); if (e != null) e.Text = value; }
        }

        public void CloseDontSave()
        {
            try
            {
                if (mainWindow != null)
                {
                    mainWindow.Close();
                    var saveDialog = mainWindow.ModalWindows.FirstOrDefault();
                    if (saveDialog != null)
                    {
                        // Try common button names first
                        var dontSave = saveDialog.FindFirstDescendant(cf => cf.ByName("Don't Save"))
                                     ?? saveDialog.FindFirstDescendant(cf => cf.ByName("Don’t Save"))
                                     ?? saveDialog.FindFirstDescendant(cf => cf.ByName("Don't save"));

                        if (dontSave == null)
                        {
                            // Fallback: find any button with Don't in the name
                            dontSave = saveDialog.FindAllDescendants(cf => cf.ByControlType(FlaUI.Core.Definitions.ControlType.Button))
                                                .FirstOrDefault(b => b.Name?.Contains("Don't") == true || b.Name?.Contains("Don’t") == true || b.Name?.Contains("Don") == true);
                        }

                        dontSave?.AsButton()?.Invoke();
                    }
                }
            }
            catch { }
        }

        public void Dispose()
        {
            try { automation?.Dispose(); } catch { }
            try
            {
                if (application != null && !application.HasExited)
                {
                    application.Close();
                }
            }
            catch
            {
                try { application?.Kill(); } catch { }
            }

            try { if (process != null && !process.HasExited) process.Kill(); } catch { }
        }
    }
}
