using Bismuthum.Core.Implementations;
using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using Tdms.Automatic.Ui.Components.Extensions;
using Tdms.Automatic.Ui.Components.Implementations;
using Tdms.Automatic.Ui.Components.Implementations.Application;
using Tdms.Automatic.Ui.Components.Implementations.ContextMenu;
using Tdms.Automatic.Ui.Components.Implementations.Dialog;
using Tdms.Automatic.Ui.Components.Implementations.Table;
using Tdms.Automatic.Ui.Components.Implementations.TreeView;
using Xunit;

namespace Tdms.Automatic.Ui.Tests.Cases
{
    public class SimpleTests : BaseTest
    {
        /// <summary>
        /// Вход и выход в системе
        /// </summary>
        [Fact]
        public void LoginTest()
        {
            //тест
            Login("SYSADMIN", "Tdm365");
            Application.Header.UserButton.Click();
            var contextMenu = Context.GetComponent<ContextMenuComponent>().Build();
            var contextMenuItem = contextMenu.GetItem().ByText("Выход").Build();
            contextMenuItem.Click();
            Context.GetComponent<DialogComponent>().Build().Yes();
        }

        /// <summary>
        /// Прощелкать все разделы и все команды меню пользователя
        /// </summary>
        [Fact]
        public void HeadersTest()
        {
            Login("SYSADMIN", "Tdm365");

            Application.Header.GetLogoButton().Click();
            Application.Header.DesktopButton.Click();
            Application.Toolbar.RefreshButton.Click();
            Application.Header.ObjectsButton.Click();
            Application.Toolbar.RefreshButton.Click();
            Application.Header.MailButton.Click();
            Application.Header.GetChatButton().Click();
            Application.Header.GetHelpButton().Click();
            Application.Header.MessagesButton.Click();
            Context.GetComponent<DialogComponent>().Build().Close();
            Application.Header.UserButton.Click();
            var contextMenu = Context.GetComponent<ContextMenuComponent>().Build();
            var contextMenuItem = contextMenu.GetItem().ByText("О программе").Build();
            contextMenuItem.Click();
            Context.GetComponent<DialogComponent>().Build().Ok();
            Application.Header.UserButton.Click();
            contextMenu = Context.GetComponent<ContextMenuComponent>().Build();
            contextMenuItem = contextMenu.GetItem().ByText("Сменить пароль").Build();
            contextMenuItem.Click();
            Context.GetComponent<DialogComponent>().Build().Cancel();
            Application.Header.UserButton.Click();
            contextMenu = Context.GetComponent<ContextMenuComponent>().Build();
            contextMenuItem = contextMenu.GetItem().ByText("Настройки").Build();
            contextMenuItem.Click();
            Context.GetComponent<DialogComponent>().Build().Cancel();
            Application.Header.UserButton.Click();
            contextMenu = Context.GetComponent<ContextMenuComponent>().Build();
            contextMenuItem = contextMenu.GetItem().ByText("Интерфейс").Build();
            contextMenuItem.Click();
            Context.GetComponent<DialogComponent>().Build().Cancel();
        }

        /// <summary>
        /// Открытие и закрытие всех панелей
        /// </summary>
        [Fact]
        public void PanelTest()
        {
            Login("SYSADMIN", "Tdm365");
            Application.Header.GetLogoButton().Click();

            var panel = Context.GetComponent<WebComponent>().WithDescription(new Description(Selector.Css("*[title^='Положение панели']"), "Интересная панель")).Build();

            //var size = panel.Properties.GetSize();
            //if (Context.GetComponent<ContextMenuComponent>().Build().GetItem().ByText("Закрыть панель").Build().IsAvailable(TimeSpan.FromMilliseconds(1000)))
            //    Context.GetComponent<ContextMenuComponent>().Build().GetItem().ByText("Закрыть панель").Build().Click();

            new ActionBuilder(Driver).MoveToComponent(panel).MoveByOffset(new Point(panel.Properties.GetSize().Width / 2 - 10, 10)).ContextClick().Perform();
            if (Context.GetComponent<ContextMenuComponent>().Build().GetItem().ByText("TDM365").Build().IsAvailable(TimeSpan.FromMilliseconds(500)))
                Context.GetComponent<ContextMenuComponent>().Build().GetItem().ByText("TDM365").Build().Click();

            new ActionBuilder(Driver).MoveToComponent(panel).MoveByOffset(new Point(panel.Properties.GetSize().Width / 2 - 10, 10)).ContextClick().Perform();
            if (Context.GetComponent<ContextMenuComponent>().Build().GetItem().ByText("Задачи").Build().IsAvailable(TimeSpan.FromMilliseconds(500)))
                Context.GetComponent<ContextMenuComponent>().Build().GetItem().ByText("Задачи").Build().Click();

            new ActionBuilder(Driver).MoveToComponent(panel).MoveByOffset(new Point(panel.Properties.GetSize().Width / 2 - 10, 10)).ContextClick().Perform();
            if (Context.GetComponent<ContextMenuComponent>().Build().GetItem().ByText("Журнал сервера").Build().IsAvailable(TimeSpan.FromMilliseconds(500)))
                Context.GetComponent<ContextMenuComponent>().Build().GetItem().ByText("Журнал сервера").Build().Click();

            new ActionBuilder(Driver).MoveToComponent(panel).MoveByOffset(new Point(panel.Properties.GetSize().Width / 2 - 10, 10)).ContextClick().Perform();
            if (Context.GetComponent<ContextMenuComponent>().Build().GetItem().ByText("Журнал событий").Build().IsAvailable(TimeSpan.FromMilliseconds(500)))
                Context.GetComponent<ContextMenuComponent>().Build().GetItem().ByText("Журнал событий").Build().Click();

            new ActionBuilder(Driver).MoveToComponent(panel).MoveByOffset(new Point(panel.Properties.GetSize().Width / 2 - 10, 10)).ContextClick().Perform();
            if (Context.GetComponent<ContextMenuComponent>().Build().GetItem().ByText("Журнал уведомлений").Build().IsAvailable(TimeSpan.FromMilliseconds(500)))
                Context.GetComponent<ContextMenuComponent>().Build().GetItem().ByText("Журнал уведомлений").Build().Click();

            new ActionBuilder(Driver).MoveToComponent(panel).MoveByOffset(new Point(panel.Properties.GetSize().Width / 2 - 10, 10)).ContextClick().Perform();
            if (Context.GetComponent<ContextMenuComponent>().Build().GetItem().ByText("Проекты").Build().IsAvailable(TimeSpan.FromMilliseconds(5000)))
                Context.GetComponent<ContextMenuComponent>().Build().GetItem().ByText("Проекты").Build().Click();

            for (int i = 0; i < 6; i++)
            {
                new ActionBuilder(Driver).MoveToComponent(panel).MoveByOffset(new Point(panel.Properties.GetSize().Width / 2 - 10, 10)).ContextClick().Perform();
                if (Context.GetComponent<ContextMenuComponent>().Build().GetItem().ByText("Закрыть панель").Build().IsAvailable(TimeSpan.FromMilliseconds(500)))
                    Context.GetComponent<ContextMenuComponent>().Build().GetItem().ByText("Закрыть панель").Build().Click();
            }
        }

        /// <summary>
        /// Создание Объекта разработки
        /// </summary>
        [Fact]
        public void ObjDevCreateTest()
        {
            Login("SYSADMIN", "Tdm365");

            string sCode = "xunit";
            string sName = $"{DateTime.Now} Автотестовый объект";
            string sDescr = $"Создано автотестом";
            string TreeDescr = $"{sCode} {sName}";

            Application.Header.ObjectsButton.Click();
            var rootTreeItem = Application.TreeView.GetComponent<TreeViewItemComponent>().ByText("TDM365").Build();
            rootTreeItem.ContextClick();

            var contextMenu = Context.GetComponent<ContextMenuComponent>().Build();
            var contextMenuItem = contextMenu.GetItem().ByText("Создать объект разработки").Build();
            contextMenuItem.Click();

            var createDlg = Context.GetComponent<DialogComponent>().Build();
            var sectionEdit = createDlg.Body.GetComponent<InputComponent>().BySystemIdentifier("ATTR_OCC_CODE").Build();
            sectionEdit.SetValue(sCode);

            var infoEdit = createDlg.Body.GetComponent<TextAreaComponent>().BySystemIdentifier("ATTR_OCC_NAME").Build();
            infoEdit.SetValue(sName);

            var DescrEdit = createDlg.Body.GetComponent<TextAreaComponent>().BySystemIdentifier("ATTR_DESCRIPTION").Build();
            DescrEdit.SetValue(sDescr);
            createDlg.Ok();

            var CreatedObj = rootTreeItem.GetItem().ByText(TreeDescr).Build();
            bool bCreatedObj = CreatedObj.IsAvailable(TimeSpan.FromMilliseconds(1000));
            Assert.True(bCreatedObj, $"В дереве созданный объект \"{TreeDescr}\" не найден");
            CreatedObj.ShouldAvailable(TimeSpan.FromMilliseconds(1000));

            var navigationList = Application.Table;
            navigationList.ShouldAvailable(TimeSpan.FromMilliseconds(1000));

            var createdFolderItem = navigationList.GetCell().ByText(TreeDescr).Build();
            createdFolderItem.ShouldAvailable(TimeSpan.FromMilliseconds(1000));
        }

        
    }
}
