using System;
using System.IO;
using Bismuthum.Core.Implementations;
using Bismuthum.Core.Interfaces;
using OpenQA.Selenium.BiDi.Modules.Session;
using OpenQA.Selenium.Chrome;
using Tdms.Ui.Test.Components.Implementations.Abstractions;
using Tdms.Ui.Test.Components.Implementations.Application;
using IWebDriver = OpenQA.Selenium.IWebDriver;

namespace Tdms.Automatic.Ui.Tests
{
    public abstract class BaseTest : IDisposable
    {
        protected ApplicationComponent Application { get; }//TDM365

        protected ISearchContext Context { get; }

        protected IWebDriver Driver => Context.Driver;

        protected virtual Uri PageAddress => new("http://localhost/client/");

        protected BaseTest()
        {
            Context = new SearchContext(GetDriver());
            SetDefaultConfiguration();
            Application = Context.GetComponent<ApplicationComponent>().Build();
        }

        public void Login(string username, string password = "")
        {
            var fields = Context.GetComponents<BaseWebComponent>().WithDescription(new Description(Selector.Css("*[data-signature='input-wrapper']"), "Поле")).Build();
            fields[0].Actions.SendKeys(username);
            fields[1].Actions.SendKeys(password);

            Context.GetComponent<BaseWebComponent>().WithDescription(new Description(Selector.Css("*[data-signature='button-wrapper']"), "Кнопка")).Build().Actions.Click();
        }

        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Driver.Dispose();
            }
        }

        protected virtual IWebDriver GetDriver()
        {
            ChromeOptions options = new();

            // Настройка пути к профилю текущего пользователя
            //string userDataDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            //string chromeUserDataDir = Path.Combine(userDataDir, @"AppData\Local\Google\Chrome\User Data");
            //options.AddArguments($"user-data-dir={chromeUserDataDir}");
            //options.AddArguments("profile-directory=Default");
            //options.AddArgument("--no-sandbox");

            options.AddArguments("--disable-notifications");
            options.AddExcludedArgument("enable-automation");
            options.AddArguments("--disable-infobars");
            options.AddArguments("--incognito");

            var driver = new ChromeDriver(options);
            driver.Manage().Window.Maximize();
            driver.Navigate().GoToUrl(PageAddress);
            //Изменение масштаба страницы 
            //((OpenQA.Selenium.IJavaScriptExecutor)driver).ExecuteScript("document.body.style.zoom='80%'");

            return driver;
        }

        private static void SetDefaultConfiguration()
        {
            ISearchContext.DefaultTimeout = TimeSpan.FromSeconds(16);
            ISearchContext.DefaultDuration = TimeSpan.FromMilliseconds(250);
        }
    }
}
