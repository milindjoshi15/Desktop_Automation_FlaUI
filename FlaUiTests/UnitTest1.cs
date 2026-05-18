using FlaUI.Core;
using FlaUI.Core.Conditions;
using FlaUI.Core.Input;
using FlaUI.Core.Shapes;
using FlaUI.UIA3;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Configuration;
using System.Threading;
using NUnit.Framework;
using Allure.NUnit;
using Allure.NUnit.Attributes;
using Allure.Net.Commons;
using Assert = NUnit.Framework.Assert;

namespace FlaUiTests
{
    [TestFixture]
    [AllureNUnit]

    [TestClass]
    public class UnitTest1
    {
        [AllureTag("Desktop Automation")]
        [AllureSeverity(SeverityLevel.critical)]
       
        [Test]
        public void newUserRegistration()
        {
            // Arrange test data
            var user = new Pages.UserData
            {
                FirstName = "Joao",
                LastName = "Santos",
                Age = "34",
                Country = "Romania",
                Phone = "8444941223",
                Email = "sds@gmail.com",
                Password = "17745",
                Card = "3216547773216547",
                Vip = true
            };

            // Act using POM style - MainWindowPage will use configured exe path by default
            using (var main = new Pages.MainWindowPage(Pages.PageSettings.BankSystemExePath))
            {
                var success = main.RegisterNewUser(user);
                // Assert/Log
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

        [Test]
        public void TestFindMethods()
        {
            var bankExe = Pages.PageSettings.BankSystemExePath;
            var application = FlaUI.Core.Application.Launch(bankExe);
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

        [Test]
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

        [Test]
        public void NotepadTest()
        {
            using (var note = new Pages.NotepadPage())
            {
                string initialText = "Hello this is FlaUI automation";
                note.EnterText(initialText);
                Thread.Sleep(1000);
                Assert.AreEqual(initialText, note.Text);

                string updatedText = initialText + " - Updated Successfully";
                Thread.Sleep(1000);
                note.Text = updatedText;
                Thread.Sleep(1000);
                Assert.AreEqual(updatedText, note.Text);
                Console.WriteLine("Text updated successfully");

                note.CloseDontSave();
            }
        }
               
        [Test]
        public void TestMenuControls()
        {
            var winFormsExe = Pages.PageSettings.WinFormsAppExePath;
            var application = FlaUI.Core.Application.Launch(winFormsExe);
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

        [Test]
        public void TestMouseActions()
        {
            Point point = new Point(2298, 82);
            Mouse.MoveTo(1500, 100);
            Mouse.MoveBy(100, 100);
            Mouse.LeftClick();
            Mouse.Click(MouseButton.Left, point);
        }
     
        [Test]
        public void TestCaptureMethod()
        {
            var picturesFolder = Pages.PageSettings.PicturesFolder;
            //Full screen
            var fullscreenImg = Capture.Screen();
            fullscreenImg.ToFile(System.IO.Path.Combine(picturesFolder, "FullScreen.png"));

            //only one automation element
            var bankExe = Pages.PageSettings.BankSystemExePath;
            var application = FlaUI.Core.Application.Launch(bankExe);
            var automation = new UIA3Automation();
            var mainWindow = application.GetMainWindow(automation);
            ConditionFactory cf = new ConditionFactory(new UIA3PropertyLibrary());
            var loginBtn = mainWindow.FindFirstDescendant(cf.ByName("Log In"));
            var loginImg = Capture.Element(loginBtn);
            loginImg.ToFile(System.IO.Path.Combine(picturesFolder, "Login Button.png"));

            //user defined rectangle area 
            var rectangleImg = Capture.Rectangle(new Rectangle(500, 500, 100, 150));
            rectangleImg.ToFile(System.IO.Path.Combine(picturesFolder, "Rectangle Img.png"));
        }       
    }
}