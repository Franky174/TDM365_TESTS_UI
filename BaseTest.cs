using System;
using Bismuthum.Core.Implementations;
using Bismuthum.Core.Interfaces;
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
            var driver = new ChromeDriver();

            driver.Manage().Window.Maximize();
            driver.Navigate().GoToUrl(PageAddress);

            return driver;
        }

        private static void SetDefaultConfiguration()
        {
            ISearchContext.DefaultTimeout = TimeSpan.FromSeconds(16);
            ISearchContext.DefaultDuration = TimeSpan.FromMilliseconds(250);
        }
    }
}
