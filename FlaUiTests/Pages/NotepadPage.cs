using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.UIA3;
using System;
using System.Diagnostics;

namespace FlaUiTests.Pages
{
    public class NotepadPage : IDisposable
    {
        private readonly Process _process;
        private readonly Application _application;
        private readonly UIA3Automation _automation;
        private readonly Window _mainWindow;

        public NotepadPage()
        {
            _process = Process.Start("notepad.exe");
            _automation = new UIA3Automation();
            _application = Application.Attach(_process);
            _mainWindow = _application.GetMainWindow(_automation);
        }

        public TextBox GetEditor()
        {
            return _mainWindow.FindFirstDescendant(cf => cf.ByControlType(FlaUI.Core.Definitions.ControlType.Document))?.AsTextBox();
        }

        public void EnterText(string text)
        {
            var edit = GetEditor();
            edit.Enter(text);
        }

        public string Text
        {
            get { return GetEditor()?.Text; }
            set { var e = GetEditor(); if (e != null) e.Text = value; }
        }

        public void CloseDontSave()
        {
            try
            {
                _mainWindow.Close();
                var saveDialog = _mainWindow.ModalWindows[0];
                saveDialog.FindFirstDescendant(cf => cf.ByName("Don't Save")).AsButton().Click();
            }
            catch { }
        }

        public void Dispose()
        {
            try { _automation?.Dispose(); } catch { }
            try { if (!_process.HasExited) _process.Kill(); } catch { }
        }
    }
}
