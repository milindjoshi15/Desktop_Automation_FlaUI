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
            var exe = @"C:\Users\Admin\Downloads\FlaUIPractice-master\FlaUIPractice-master\FlaUIPractice\BankSystem\bin\Release\BankSystem.exe";
            using (var main = new Pages.MainWindowPage(exe))
            {
                var reg = main.OpenRegistration();
                reg.FillForm(
                    firstName: "Joao",
                    lastName: "Santos",
                    age: "34",
                    country: "Romania",
                    phone: "8444941223",
                    email: "sds@gmail.com",
                    pass: "17745",
                    card: "3216547773216547",
                    vip: true);

                var success = reg.SubmitAndHandleResult();
                if (success)
                {
                    Console.WriteLine("User registered successfully.");
                }
                else
                {
                    Console.WriteLine("User already exists. Please try again with different credentials.");
                }

                main.ExitAndConfirm();
            }
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
            using (var calc = new Pages.CalculatorPage())
            {
                calc.PressByAutomationId("num5Button");
                calc.PressByAutomationId("plusButton");
                calc.PressByAutomationId("num3Button");
                calc.PressByAutomationId("equalButton");

                string actualResult = calc.GetResultText();
                Assert.IsTrue(actualResult.Contains("8"),
                    $"Expected result to contain 8 but got: {actualResult}");
                calc.Close();
            }
        }

        [TestMethod]
        public void NotepadTest()
        {
            using (var note = new Pages.NotepadPage())
            {
                string initialText = "Hello this is FlaUI automation";
                note.EnterText(initialText);
                Thread.Sleep(1000);
                Assert.AreEqual(initialText, note.Text);

                string updatedText = initialText + " - Updated Successfully";
                note.Text = updatedText;
                Thread.Sleep(1000);
                Assert.AreEqual(updatedText, note.Text);
                Console.WriteLine("Text updated successfully");

                note.CloseDontSave();
            }
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