using FlaUI.Core;
using FlaUI.Core.Conditions;
using FlaUI.Core.Definitions;
using FlaUI.Core.Input;
using FlaUI.Core.Shapes;
using FlaUI.Core.WindowsAPI;
using FlaUI.UIA2;
using FlaUI.UIA3;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Threading;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace FlaUiTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void newUserRegistration()
        {
            var application = FlaUI.Core.Application.Launch(@"C:\Users\Admin\Downloads\FlaUIPractice-master\FlaUIPractice-master\FlaUIPractice\BankSystem\bin\Release\BankSystem.exe");
            var automation = new UIA3Automation();
            var mainWindow = application.GetMainWindow(automation);
            ConditionFactory cf = new ConditionFactory(new UIA3PropertyLibrary());

            mainWindow.FindFirstDescendant(cf.ByName("Registration")).AsButton().Click();
            mainWindow.FindFirstDescendant(cf.ByAutomationId("InFName")).AsTextBox().Enter("Joao");
            mainWindow.FindFirstDescendant(cf.ByAutomationId("InLName")).AsTextBox().Enter("Santos");
            //Thread.Sleep(500);
            var age = mainWindow.FindFirstDescendant(cf.ByAutomationId("InAge")).AsComboBox();
            if (age != null)
            {
                age.Patterns.ExpandCollapse.Pattern.Expand();
                age.Select("34");
            }
            var country = mainWindow.FindFirstDescendant(cf.ByAutomationId("InCountry")).AsComboBox();
            if(country != null)
            {
                country.Patterns.ExpandCollapse.Pattern.Expand();                 
                country.Select("Romania");
            }
            mainWindow.FindFirstDescendant(cf.ByAutomationId("InPhone")).AsTextBox().Enter("8444941223");
            mainWindow.FindFirstDescendant(cf.ByAutomationId("InEmail")).AsTextBox().Enter("sds@gmail.com");
            mainWindow.FindFirstDescendant(cf.ByAutomationId("InPass")).AsTextBox().Enter("17745");
            mainWindow.FindFirstDescendant(cf.ByAutomationId("InCard")).AsTextBox().Enter("3216547773216547");
            mainWindow.FindFirstDescendant(cf.ByAutomationId("VipCheck")).AsCheckBox().Click();
            mainWindow.FindFirstDescendant(cf.ByName("Ok")).AsButton().Click();
            if(mainWindow.FindFirstDescendant(cf.ByName("Congratulations")) != null) {
                var congratulationsWindow = mainWindow.FindFirstDescendant(cf.ByName("Congratulations")).AsWindow();
                congratulationsWindow.FindFirstDescendant(cf.ByName("OK")).AsButton().Click();
                Console.WriteLine("User registered successfully.");
            }
            else
            {
                var existingUserWindow = mainWindow.FindFirstDescendant(cf.ByName("Error")).AsWindow();
                existingUserWindow.FindFirstDescendant(cf.ByName("OK")).AsButton().Click();
                mainWindow.FindFirstDescendant(cf.ByName("Cancel")).AsButton().Click();
                Console.WriteLine("User already exists. Please try again with different credentials.");
            }
            mainWindow.FindFirstDescendant(cf.ByName("Exit")).AsButton().Click();
            var exitWindow = mainWindow.FindFirstDescendant(cf.ByName("Exit")).AsWindow();
            exitWindow.FindFirstDescendant(cf.ByName("Yes")).AsButton().Click();
        }

        [TestMethod]
        public void TestFindMethods()
        {
            var application = FlaUI.Core.Application.Launch(@"C:\Users\Admin\Downloads\FlaUIPractice-master\FlaUIPractice-master\FlaUIPractice\BankSystem\bin\Release\BankSystem.exe");
            var automation = new UIA3Automation();
            var mainWindow = application.GetMainWindow(automation);
            ConditionFactory cf = new ConditionFactory(new UIA3PropertyLibrary());

            var elements = mainWindow.FindAll(FlaUI.Core.Definitions.TreeScope.Children,
                new PropertyCondition(automation.PropertyLibrary.Element.ControlType, FlaUI.Core.Definitions.ControlType.Edit));
            foreach (var item in elements)
            {
                item.DrawHighlight();
                Thread.Sleep(500);
            }
        }

        [TestMethod]
        public void CalcTest()
        {
            var application = Application.Launch("calc.exe");
            var automation = new UIA2Automation();
            var mainWindow = application.GetMainWindow(automation);
            mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("num5Button"))
                          ?.AsButton()
                          .Invoke();
            mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("plusButton"))
                          ?.AsButton()
                          .Invoke();
            mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("num3Button"))
                         ?.AsButton()
                         .Invoke();
            mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("equalButton"))
                         ?.AsButton()
                         .Invoke();
            var resultElement = mainWindow.FindFirstDescendant(
                    cf => cf.ByAutomationId("CalculatorResults"));

            string actualResult = resultElement?.AsLabel().Text;
            Assert.IsTrue(actualResult.Contains("8"),
                    $"Expected result to contain 8 but got: {actualResult}");
            mainWindow.Close();
        }

        [TestMethod]
        public void NotepadTest()
        {
            var process = Process.Start("notepad.exe");
            //var application = FlaUI.Core.Application.Launch("calc.exe");
            var automation = new UIA3Automation();
            var app = FlaUI.Core.Application.Attach(process);
            var mainWindow = app.GetMainWindow(automation);
            //ConditionFactory cf = new ConditionFactory(new UIA3PropertyLibrary());
            var edit = mainWindow.FindFirstDescendant(cf => cf.ByControlType(FlaUI.Core.Definitions.ControlType.Document))?.AsTextBox();
            //Assert.IsNotNull(edit, "Text editor not found");
            string initialText = "Hello this is FlaUI automation";
            edit.Enter(initialText);
            Thread.Sleep(1000);
            Assert.AreEqual(initialText, edit.Text);
            string updatedText = initialText + " - Updated Successfully";
            edit.Text = updatedText;
            Thread.Sleep(1000);
            // Step 3 - Validate updated text
            Assert.AreEqual(updatedText, edit.Text);
            Console.WriteLine("Text updated successfully");
            mainWindow.Close();
            var saveDialog = mainWindow.ModalWindows[0];
            saveDialog.FindFirstDescendant(cf => cf.ByName("Don't Save")).AsButton().Click();
        }
               
        [TestMethod]
        public void TestMenuControls()
        {
            var application = FlaUI.Core.Application.Launch(@"C:\Users\Admin\Downloads\FlaUIPractice-master\FlaUIPractice-master\FlaUIPractice\FlaUiTests\Resources\WinFormsApplication.exe");
            var automation = new UIA3Automation();
            var mainWindow = application.GetMainWindow(automation);
            ConditionFactory cf = new ConditionFactory(new UIA3PropertyLibrary());
            var menu = mainWindow.FindFirstDescendant(cf.Menu()).AsMenu();
            menu.DrawHighlight();

            menu.Items["File"].Invoke();
            menu.Items["Edit"].Invoke();
            mainWindow.FindFirstDescendant(cf.ByName("ContextMenu")).AsButton().RightClick();
            var contextMenu = mainWindow.ContextMenu;
            contextMenu.DrawHighlight();
            contextMenu.Items[0].DrawHighlight();
            mainWindow.FindFirstDescendant(cf.ByName("Close")).AsButton().Click();
        }

        [TestMethod]
        public void TestMouseActions()
        {
            Point point = new Point(2298, 82);
            Mouse.MoveTo(1500, 100);
            Mouse.MoveBy(100, 100);
            Mouse.LeftClick();
            Mouse.Click(MouseButton.Left, point);
        }
     
        [TestMethod]
        public void TestCaptureMethod()
        {
            //Full screen
            var fullscreenImg = Capture.Screen();
            fullscreenImg.ToFile(@"C:\Users\Admin\Pictures\Saved Pictures\FullScreen.png");

            //only one automation element
            var application = FlaUI.Core.Application.Launch(@"C:\Users\Admin\Downloads\FlaUIPractice-master\FlaUIPractice-master\FlaUIPractice\BankSystem\bin\Release\BankSystem.exe");
            var automation = new UIA3Automation();
            var mainWindow = application.GetMainWindow(automation);
            ConditionFactory cf = new ConditionFactory(new UIA3PropertyLibrary());
            var loginBtn = mainWindow.FindFirstDescendant(cf.ByName("Log In"));
            var loginImg = Capture.Element(loginBtn);
            loginImg.ToFile(@"C:\Users\Admin\Pictures\Saved Pictures\Login Button.png");

            //user defined rectangle area 
            var rectangleImg = Capture.Rectangle(new Rectangle(500, 500, 100, 150));
            rectangleImg.ToFile(@"C:\Users\Admin\Pictures\Saved Pictures\Rectangle Img.png");
        }       
    }
}