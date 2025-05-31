using Allure.Net.Commons;
using Allure.Xunit.Attributes.Steps;
using Bismuthum.Core.Implementations;
using System;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Tdms.Ui.Test.Components.Extensions;
using Tdms.Ui.Test.Components.Implementations;
using Tdms.Ui.Test.Components.Implementations.Application;
using Tdms.Ui.Test.Components.Implementations.ContextMenu;
using Tdms.Ui.Test.Components.Implementations.Dialog;
using Tdms.Ui.Test.Components.Implementations.Table;
using Tdms.Ui.Test.Components.Implementations.TreeView;
using Xunit;
using static System.Collections.Specialized.BitVector32;

namespace Tdms.Automatic.Ui.Tests.Cases
{
    public class SimpleTests : BaseTest
    {
        #region Methods
        //====================================================================================================

        /// <summary>
        /// Переход в указанный раздел
        /// </summary>
        /// <param name="Section">Наименование раздела</param>
        /// <param name="index">Номер шага</param>
        [AllureStep("{index}. Переход в раздел {Section}")]
        public void GoToSection(string Section, int index = 0)
        {
            if (string.Equals(Section, "Панели", StringComparison.CurrentCultureIgnoreCase))
                Application.Header.GetLogoButton().Click();
            else if (string.Equals(Section, "Рабочий стол", StringComparison.CurrentCultureIgnoreCase))
                Application.Header.DesktopButton.Click();
            else if (string.Equals(Section, "Объекты", StringComparison.CurrentCultureIgnoreCase))
                Application.Header.ObjectsButton.Click();
            else if (string.Equals(Section, "Почта", StringComparison.CurrentCultureIgnoreCase))
                Application.Header.MailButton.Click();
            else if (string.Equals(Section, "Совещания", StringComparison.CurrentCultureIgnoreCase))
                Application.Header.GetChatButton().Click();
            else if (string.Equals(Section, "Справка", StringComparison.CurrentCultureIgnoreCase))
                Application.Header.GetHelpButton().Click();
            else if (string.Equals(Section, "Администратор", StringComparison.CurrentCultureIgnoreCase))
                Application.Header.GetAdministratorButton().Click();
            else if (string.Equals(Section, "Разработчик", StringComparison.CurrentCultureIgnoreCase))
                Application.Header.DeveloperButton.Click();
        }

        /// <summary>
        /// Авторизация пользователя
        /// </summary>
        /// <param name="UserName">Логин пользователя</param>
        /// <param name="index">Номер шага</param>
        [AllureStep("{index}. Авторизация пользователя {UserName}")]
        public void Authorization(string UserName, int index = 0)
        {
            Login(UserName, "Tdm365");
        }

        /// <summary>
        /// Выход пользователяя из системы
        /// </summary>
        /// <param name="index">Номер шага</param>
        [AllureStep("{index}. Выход из системы")]
        public void UnAuthorization(int index = 0)
        {
            Application.Header.UserButton.Click();
            var contextMenu = Context.GetComponent<ContextMenuComponent>().Build();
            var contextMenuItem = contextMenu.GetItem().ByText("Выход").Build();
            contextMenuItem.Click();
            Context.GetComponent<DialogComponent>().Build().Yes();
        }

        /// <summary>
        /// Выполнение команды в меню пользователя
        /// </summary>
        /// <param name="Command">Имя команды</param>
        /// <param name="bClose">true - закрыть диалог после выполнения</param>
        /// <param name="index">Номер шага</param>
        /// <returns></returns>
        [AllureStep("{index}. Команда пользователя - {Command}")]
        public ContextMenuItemComponent UserCommand(string Command, bool bClose, int index = 0)
        {
            Application.Header.UserButton.Click();
            var contextMenu = Context.GetComponent<ContextMenuComponent>().Build();
            var contextMenuItem = contextMenu.GetItem().ByText(Command).Build();
            contextMenuItem.Click();
            if (bClose)
            {
                if (string.Equals(Command, "О программе", StringComparison.CurrentCultureIgnoreCase))
                    Context.GetComponent<DialogComponent>().Build().Ok();
                else
                    Context.GetComponent<DialogComponent>().Build().Cancel();
            }
            return contextMenuItem;
        }

        /// <summary>
        /// Открытие диалога уведомлений
        /// </summary>
        /// <param name="bClose">true - закрыть диалог после выполнения</param>
        /// <param name="index">Номер шага</param>
        /// <returns></returns>
        [AllureStep("{index}. Уведомления")]
        public DialogComponent Notifications(bool bClose, int index = 0)
        {
            Application.Header.MessagesButton.Click();
            var Dialog = Context.GetComponent<DialogComponent>().Build();
            if (bClose) Dialog.Close();
            return Dialog;
        }

        /// <summary>
        /// Выполнение команды в указанной панели
        /// </summary>
        /// <param name="Command">Описание команды</param>
        /// <param name="PanelName">Системное имя формы или имя iFrame</param>
        /// <param name="index">Номер шага</param>
        [AllureStep("{index}. Команда контекстного меню в панели {PanelName} - {Command}")]
        public void PanelCommand(string Command, string PanelName, int index = 0)
        {
            WebComponent panel;
            var mains = Context.GetComponents<WebComponent>().WithDescription(new Description(Selector.Css("*[class^='Panel_panel']"), "класс панели")).Build();
            foreach (var main in mains)
            {
                panel = main.GetComponent<WebComponent>().WithDescription(new Description(Selector.Css($"*[data-sysid^='{PanelName}']"), PanelName)).Build();
                if (!panel.IsAvailable(TimeSpan.FromMilliseconds(100)))
                {
                    panel = main.GetComponent<WebComponent>().WithDescription(new Description(Selector.Css($"*[name^='{PanelName}']"), PanelName)).Build();
                    if (!panel.IsAvailable(TimeSpan.FromMilliseconds(100)))
                        continue;
                }
                panel = main.GetComponent<WebComponent>().WithDescription(new Description(Selector.Css("*[title^='Положение панели']"), "Интересная панель")).Build();
                new ActionBuilder(Driver).MoveToComponent(panel).MoveByOffset(new Point(panel.Properties.GetSize().Width / 2 - 10, 10)).ContextClick().Perform();
                var MenuCmd = Context.GetComponent<ContextMenuComponent>().Build().GetItem().ByText(Command).Build();
                if (MenuCmd.IsAvailable(TimeSpan.FromMilliseconds(100)))
                    MenuCmd.Click();
                else
                    Assert.True(false, $"Недоступна команда {Command}");
                break;
            }
            return;
        }

        /// <summary>
        /// Создание объекта структуры
        /// </summary>
        /// <param name="index">Номер шага</param>
        /// <returns></returns>
        [AllureStep("{index}. Создание Объекта разработки")]
        public TreeViewItemComponent CreateObjDev(int index = 0)
        {
            string sCode = "xunit";
            string sName = $"{DateTime.Now} Автотестовый объект";
            string sDescr = $"Создано автотестом";
            string TreeDescr = $"{sCode} {sName}";
            string sContent0 = "Сопровождающие документы по объекту";
            string sContent1 = "Размещение документации";
            string sContent2 = "Проекты";
            string sContent3 = $"{sCode} Структура объекта \"{sName}\"";
            string sContent4 = $"{sCode}.ТД Техническая документация на \"{sName}\"";

            var rootTreeItem = Application.TreeView.GetComponent<TreeViewItemComponent>().ByText("TDM365").Build();
            rootTreeItem.ContextClick();
            var contextMenu = Context.GetComponent<ContextMenuComponent>().Build();
            var contextMenuItem = contextMenu.GetItem().ByText("Создать объект разработки").Build();
            contextMenuItem.Click();

            //Заполнение атрибутов
            var createDlg = Context.GetComponent<DialogComponent>().Build();
            var sectionEdit = createDlg.Body.GetComponent<InputComponent>().BySystemIdentifier("ATTR_OCC_CODE").Build();
            sectionEdit.SetValue(sCode);

            var infoEdit = createDlg.Body.GetComponent<TextAreaComponent>().BySystemIdentifier("ATTR_OCC_NAME").Build();
            infoEdit.SetValue(sName);

            var DescrEdit = createDlg.Body.GetComponent<TextAreaComponent>().BySystemIdentifier("ATTR_DESCRIPTION").Build();
            DescrEdit.SetValue(sDescr);
            createDlg.Ok();

            //Проверка Объекта разработки
            var CreatedObj = rootTreeItem.GetItem().ByText(TreeDescr).Build();
            bool bObj = CreatedObj.IsAvailable(TimeSpan.FromMilliseconds(1000));
            Assert.True(bObj, $"В дереве созданный объект \"{TreeDescr}\" не найден");
            //CreatedObj.ShouldAvailable(TimeSpan.FromMilliseconds(1000));
            CreatedObj.DoubleClick();

            //Обновить элемент дерева
            //CreatedObj.ContextClick();
            //contextMenu = Context.GetComponent<ContextMenuComponent>().Build();
            //contextMenuItem = contextMenu.GetItem().ByText("Обновить").Build();
            //contextMenuItem.Click();

            //Проверка состава
            //Сопровождающие документы по объекту
            var Content0 = CreatedObj.GetItem().ByText(sContent0).Build();
            Content0.ShouldAvailable(TimeSpan.FromMilliseconds(1000));

            //Размещение документации
            var Content1 = CreatedObj.GetItem().ByText(sContent1).Build();
            Content1.ShouldAvailable(TimeSpan.FromMilliseconds(1000));

            //Проекты
            var Content2 = CreatedObj.GetItem().ByText(sContent2).Build();
            Content2.ShouldAvailable(TimeSpan.FromMilliseconds(1000));

            //Структура объекта
            var Content3 = CreatedObj.GetItem().ByText(sContent3).Build();
            Content3.ShouldAvailable(TimeSpan.FromMilliseconds(1000));

            //Техническая документация
            var Content4 = CreatedObj.GetItem().ByText(sContent4).Build();
            Content4.ShouldAvailable(TimeSpan.FromMilliseconds(1000));

            return CreatedObj;
        }

        //====================================================================================================
        #endregion Methods

        /// <summary>
        /// Вход и выход в системе
        /// </summary>
        [Fact]
        public void LoginTest()
        {
            Authorization("SYSADMIN", 0);
            UnAuthorization(1);
        }

        /// <summary>
        /// Прощелкать все разделы и все команды меню пользователя
        /// </summary>
        [Fact]
        public void HeadersTest()
        {
            Authorization("SYSADMIN", 0);
            GoToSection("Панели", 1);
            GoToSection("Рабочий стол", 2);
            GoToSection("Объекты", 3);
            GoToSection("Почта", 4);
            GoToSection("Совещания", 5);
            GoToSection("Справка", 6);
            Notifications(true, 7);
            UserCommand("О программе", true, 8);
            UserCommand("Сменить пароль", true, 9);
            UserCommand("Настройки", true, 10);
            UserCommand("Интерфейс", true, 11);
        }

        /// <summary>
        /// Открытие и закрытие всех панелей
        /// </summary>
        [Fact]
        public void PanelTest()
        {
            Authorization("SYSADMIN", 0);
            GoToSection("Панели", 1);
            //Открытие панелей
            PanelCommand("TDM365", "TDM365", 2);
            PanelCommand("Задачи", "TDM365", 3);
            PanelCommand("Журнал сервера", "FORM_PANEL_SPD", 4);
            PanelCommand("Журнал событий", "FORM_SERVER_LOG", 5);
            PanelCommand("Журнал уведомлений", "FORM_EVENTS_LOG", 6);
            PanelCommand("Проекты", "FORM_NOTIFICATIONS_HISTORY", 7);
            PanelCommand("Планирование", "FORM_PANEL_PROJECTS", 8);
            //Закрытие панелей
            PanelCommand("Закрыть панель", "TDM365", 9);
            PanelCommand("Закрыть панель", "FORM_PANEL_SPD", 10);
            PanelCommand("Закрыть панель", "FORM_SERVER_LOG", 11);
            PanelCommand("Закрыть панель", "FORM_EVENTS_LOG", 12);
            PanelCommand("Закрыть панель", "FORM_NOTIFICATIONS_HISTORY", 13);
            PanelCommand("Закрыть панель", "FORM_PANEL_PROJECTS", 14);
            PanelCommand("Закрыть панель", "FORM_PANEL_PLANNING", 15);
        }

        /// <summary>
        /// Создание Объекта разработки
        /// </summary>
        [Fact]
        public void ObjDevCreateTest()
        {
            Authorization("SYSADMIN", 0);

            GoToSection("Объекты", 1);

            //Создание Объекта разработки
            var ObjDev = CreateObjDev(2);
        }

        
    }
}
