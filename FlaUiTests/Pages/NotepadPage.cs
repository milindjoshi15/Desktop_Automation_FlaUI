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
        private readonly Process? _process;
        private Application _application;
        private readonly UIA3Automation _automation;
        private Window? _mainWindow;

        public NotepadPage()
        {
            // Start or attach to Notepad in a resilient way
            try
            {
                var psi = new ProcessStartInfo("notepad.exe") { UseShellExecute = true };
                _application = Application.AttachOrLaunch(psi);
                _automation = new UIA3Automation();

                try
                {
                    _mainWindow = _application.GetMainWindow(_automation, TimeSpan.FromSeconds(5));
                }
                catch
                {
                    // If the started process exited or main window wasn't found, try to attach to existing notepad process
                    try
                    {
                        var proc = Process.GetProcessesByName("notepad").FirstOrDefault();
                        if (proc != null)
                        {
                            _application = Application.Attach(proc.Id);
                            _mainWindow = _application.GetMainWindow(_automation, TimeSpan.FromSeconds(5));
                        }
                    }
                    catch { }
                }

                // Try to set _process to the launched process if available
                try { _process = Process.GetProcessById(_application.ProcessId); } catch { _process = null; }
            }
            catch
            {
                // In case of any failure, initialize automation so Dispose can run safely
                _automation = new UIA3Automation();
                _process = null;
                _application = null!;
                _mainWindow = null;
            }
        }

        public TextBox? GetEditor()
        {
            if (_mainWindow == null) return null;
            // Notepad sometimes exposes the editor as Document or as Edit; try both
            var doc = _mainWindow.FindFirstDescendant(cf => cf.ByControlType(FlaUI.Core.Definitions.ControlType.Document));
            if (doc != null) return doc.AsTextBox();
            var edit = _mainWindow.FindFirstDescendant(cf => cf.ByControlType(FlaUI.Core.Definitions.ControlType.Edit));
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
                if (_mainWindow != null)
                {
                    _mainWindow.Close();
                    var saveDialog = _mainWindow.ModalWindows.FirstOrDefault();
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

            try { if (_process != null && !_process.HasExited) _process.Kill(); } catch { }
        }
    }
}
