using Allure.Net.Commons;
using Allure.Xunit.Attributes;
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
using Tdms.Automatic.Ui.Tests;
using Tdms.Ui.Test.Components.Extensions;
using Tdms.Ui.Test.Components.Implementations;
using Tdms.Ui.Test.Components.Implementations.Application;
using Tdms.Ui.Test.Components.Implementations.ContextMenu;
using Tdms.Ui.Test.Components.Implementations.Dialog;
using Tdms.Ui.Test.Components.Implementations.ObjectDescription;
using Tdms.Ui.Test.Components.Implementations.Table;
using Tdms.Ui.Test.Components.Implementations.TreeView;
using Xunit;
using static System.Collections.Specialized.BitVector32;

namespace TDM365.UI//Tdms.Automatic.Ui.Tests.Cases
{
    [AllureParentSuite("Web interface TDM365")]
    public class Tests : BaseTest
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
        /// Проверка поля формы на заполнение
        /// </summary>
        /// <param name="DialogName">Наименование диалога</param>
        /// <param name="SysName">Системное имя поля</param>
        /// <param name="index">Номер шага</param>
        [AllureStep("{index}. Проверка поля {SysName}")]
        public string FieldEmptyCheck(string DialogName, string SysName, int index = 0)
        {
            string Value = "";
            var Dialog = Context.GetComponent<DialogComponent>().ByText(DialogName).Build();
            if (!Dialog.IsAvailable(TimeSpan.FromMilliseconds(500)))
                Assert.True(false, $"Не найден диалог {DialogName}");
            var Field0 = Dialog.Body.GetComponent<InputComponent>().BySystemIdentifier(SysName).Build();
            if (Field0.IsAvailable(TimeSpan.FromMilliseconds(100)))
                Value = Field0.GetValue(TimeSpan.FromMilliseconds(100));
            else
            {
                var Field1 = Dialog.Body.GetComponent<TextAreaComponent>().BySystemIdentifier(SysName).Build();
                if (Field1.IsAvailable(TimeSpan.FromMilliseconds(100)))
                    Value = Field1.GetValue(TimeSpan.FromMilliseconds(100));
            }

            if (Value == "")
                Assert.True(false, $"Не заполнено поле {SysName}");
            return Value;
        }

        /// <summary>
        /// Установка значения поля
        /// </summary>
        /// <param name="DialogName">Наименование диалога</param>
        /// <param name="SysName">Системное имя поля</param>
        /// <param name="Value">Новое значение</param>
        /// <param name="index">Номер шага</param>
        [AllureStep("{index}. Установка значения поля {SysName}")]
        public void SetFieldValue(string DialogName, string SysName, string Value, int index = 0)
        {
            var Dialog = Context.GetComponent<DialogComponent>().ByText(DialogName).Build();
            if (!Dialog.IsAvailable(TimeSpan.FromMilliseconds(500)))
                Assert.True(false, $"Не найден диалог {DialogName}");
            var Field0 = Dialog.Body.GetComponent<InputComponent>().BySystemIdentifier(SysName).Build();
            if (Field0.IsAvailable(TimeSpan.FromMilliseconds(100)))
                Field0.SetValue(Value, TimeSpan.FromMilliseconds(100));
            else
            {
                var Field1 = Dialog.Body.GetComponent<TextAreaComponent>().BySystemIdentifier(SysName).Build();
                if (Field1.IsAvailable(TimeSpan.FromMilliseconds(100)))
                    Field1.SetValue(Value, TimeSpan.FromMilliseconds(100));
            }
        }

        /// <summary>
        /// Функция возвращает диалог после нажатия на кнопку формы
        /// </summary>
        /// <param name="DialogMainName">Наименование диалога с кнопкой</param>
        /// <param name="SysName">Системное имя кнопки</param>
        /// <param name="DialogName">Наименование нового диалога</param>
        /// <param name="index">Номер шага</param>
        [AllureStep("{index}. Запрос диалога по нажатию кнопки {SysName}")]
        public DialogComponent? GetDialogFromButton(string DialogMainName, string SysName, string DialogName, bool bClose, int index = 0)
        {
            //string DialogNameDefault = "TDMS";
            //Параметр из отчета скрывается, но в файл записывается...
            //AllureLifecycle.Instance.UpdateStep(stepResult => stepResult.parameters.ForEach(x => x.mode = ParameterMode.Hidden));
            var Dialog = Context.GetComponent<DialogComponent>().ByText(DialogMainName).Build();
            if (!Dialog.IsAvailable(TimeSpan.FromMilliseconds(500)))
                Assert.True(false, $"Не найден диалог {DialogMainName}");
            var Btn = Dialog.Body.GetComponent<ButtonComponent>().BySystemIdentifier(SysName).Build();
            if (!Btn.IsAvailable(TimeSpan.FromMilliseconds(500)))
                Assert.True(false, $"Не доступна кнопка {SysName}");
            Btn.Click();
            if (DialogName != "")
            {
                DialogComponent? BtnDialog = Context.GetComponent<DialogComponent>().ByText(DialogName).Build();

                int i = 0;
                bool bDialog = false;
                while (i < 5 && !bDialog)
                {
                    BtnDialog = Context.GetComponent<DialogComponent>().ByText(DialogName).Build();
                    if (BtnDialog.IsAvailable(TimeSpan.FromMilliseconds(500)))
                    {
                        if (bClose) BtnDialog.Close(TimeSpan.FromMilliseconds(500));
                        bDialog = true;
                    }
                    i++;
                    Thread.Sleep(500);
                }
                if (!bDialog) Assert.True(false, $"Не отобразился диалог кнопки {SysName}");
                //if (!BtnDialog.IsAvailable(TimeSpan.FromMilliseconds(1000)))
                //    Assert.True(false, $"Не отобразился диалог кнопки {SysName}");
                //if (bClose)
                //    BtnDialog.Close(TimeSpan.FromMilliseconds(500));
                return BtnDialog;
            }
            
            return null;
        }

        /// <summary>
        /// Функция переключения на вкладку диалога
        /// </summary>
        /// <param name="DialogMainName">Наименование диалога с кнопкой</param>
        /// <param name="TabName">Системное имя вкладки</param>
        /// <param name="DialogName">Наименование нового диалога</param>
        /// <param name="index">Номер шага</param>
        //[AllureStep("{index}. Запрос диалога по нажатию кнопки {SysName}")]
        //public TabComponent TabSwitch(object Dialog, string TabName, int index = 0)
        //{
        //    //string DialogNameDefault = "TDMS";
        //    //Параметр из отчета скрывается, но в файл записывается...
        //    //AllureLifecycle.Instance.UpdateStep(stepResult => stepResult.parameters.ForEach(x => x.mode = ParameterMode.Hidden));

        //    DialogComponent? ObjDlg = null;
        //    if (Dialog is DialogComponent)
        //        ObjDlg = Dialog as DialogComponent;
        //    else if (Dialog is string)
        //    {
        //        string sDialog = Dialog?.ToString() ?? "";
        //        ObjDlg = Context.GetComponent<DialogComponent>().ByText(sDialog).Build();
        //    }
        //    if (!ObjDlg.IsAvailable(TimeSpan.FromMilliseconds(500)))
        //        Assert.True(false, $"Не найден диалог {Dialog?.ToString() ?? ""}");

        //    TabComponent Tab = ObjDlg.Body.GetComponent<TabComponent>().BySystemIdentifier("FORM_MAIL_SETTINGS").Build();
        //    Tab.Click();

            
        //    var Btn = Dialog.Body.GetComponent<ButtonComponent>().BySystemIdentifier(SysName).Build();
        //    if (!Btn.IsAvailable(TimeSpan.FromMilliseconds(500)))
        //        Assert.True(false, $"Не доступна кнопка {SysName}");
        //    Btn.Click();
        //    if (DialogName != "")
        //    {
        //        DialogComponent? BtnDialog = Context.GetComponent<DialogComponent>().ByText(DialogName).Build();

        //        int i = 0;
        //        bool bDialog = false;
        //        while (i < 5 && !bDialog)
        //        {
        //            BtnDialog = Context.GetComponent<DialogComponent>().ByText(DialogName).Build();
        //            if (BtnDialog.IsAvailable(TimeSpan.FromMilliseconds(500)))
        //            {
        //                if (bClose) BtnDialog.Close(TimeSpan.FromMilliseconds(500));
        //                bDialog = true;
        //            }
        //            i++;
        //            Thread.Sleep(500);
        //        }
        //        if (!bDialog) Assert.True(false, $"Не отобразился диалог кнопки {SysName}");
        //        //if (!BtnDialog.IsAvailable(TimeSpan.FromMilliseconds(1000)))
        //        //    Assert.True(false, $"Не отобразился диалог кнопки {SysName}");
        //        //if (bClose)
        //        //    BtnDialog.Close(TimeSpan.FromMilliseconds(500));
        //        return Tab;
        //    }

        //    return Tab;
        //}

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
        /// Выполнение команды в меню администратора
        /// </summary>
        /// <param name="Command">Имя команды</param>
        /// <param name="bClose">true - закрыть диалог после выполнения</param>
        /// <param name="index">Номер шага</param>
        /// <returns></returns>
        [AllureStep("{index}. Команда администратора - {Command}")]
        public ContextMenuItemComponent AdminCommand(string Command, bool bClose, int index = 0)
        {
            string Err0 = $"Не найдено меню администратора перед выполнением команды {Command}";
            //Поиск Меню администратора
            var SysMenuBtn = Application.Toolbar.GetComponent<ButtonComponent>().BySystemIdentifier("SUB_SYSADMIN").Build();
            //Если не нашли меню администратора - ищем глобальную кнопку тулбара
            if (!SysMenuBtn.IsAvailable(TimeSpan.FromMilliseconds(1000)))
            {
                var GlobalBtn = Application.Toolbar.GlobalCommandsButton;
                if (!GlobalBtn.IsAvailable(TimeSpan.FromMilliseconds(500)))
                    Assert.True(false, Err0);
                else
                {
                    GlobalBtn.Click();
                    //КМ глобальной кнопки тулбара
                    var MenuGlobalBtn = Context.GetComponent<ContextMenuComponent>().Build();
                    if (!MenuGlobalBtn.IsAvailable(TimeSpan.FromMilliseconds(500)))
                        Assert.True(false, Err0);
                    //Ищем элемент КМ - меню администратора
                    var SysMenuBtn1 = MenuGlobalBtn.GetItem().ByReference("SUB_SYSADMIN").Build();
                    if (!SysMenuBtn1.IsAvailable(TimeSpan.FromMilliseconds(500)))
                        Assert.True(false, Err0);
                    SysMenuBtn1.Click();
                }
            }
            else
            {
                //Кнопка Меню администратора найдена - кликаем
                SysMenuBtn.Click();
            }

            //Открываем КМ администратора
            var MenuAdmin = Context.GetComponent<ContextMenuComponent>().Build();
            if (!MenuAdmin.IsAvailable(TimeSpan.FromMilliseconds(500)))
                Assert.True(false, Err0);

            //Ищем команду КМ администратора
            var contextMenuCommand = MenuAdmin.GetItem().ByText(Command).Build();
            if (!contextMenuCommand.IsAvailable(TimeSpan.FromMilliseconds(500)))
                Assert.True(false, $"Не найдена команда {Command}");
            contextMenuCommand.Click();
            int i = 0;
            bool bDialog = false;
            while(i < 5 && !bDialog)
            {
                var Dialog = Context.GetComponent<DialogComponent>().Build();
                if (Dialog.IsAvailable(TimeSpan.FromMilliseconds(500)))
                {
                    if (bClose)
                        Dialog.Close();
                    bDialog = true;
                }
                i++;
                Thread.Sleep(500);
            }
            if (!bDialog)
                Assert.True(false, $"Не открылся диалог команды {Command}");

            return contextMenuCommand;
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
                if (MenuCmd.IsAvailable(TimeSpan.FromMilliseconds(500)))
                    MenuCmd.Click();
                else
                    Assert.True(false, $"{PanelName} - Недоступна команда {Command}");
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
        public string CreateObjDev(int index = 0)
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
            //var CreatedObj = rootTreeItem.GetItem().ByText(TreeDescr).Build();
            //bool bObj = CreatedObj.IsAvailable(TimeSpan.FromMilliseconds(1000));
            //Assert.True(bObj, $"В дереве созданный объект \"{TreeDescr}\" не найден");
            //CreatedObj.ShouldAvailable(TimeSpan.FromMilliseconds(1000));
            //CreatedObj.DoubleClick();

            //Обновить элемент дерева
            //CreatedObj.ContextClick();
            //contextMenu = Context.GetComponent<ContextMenuComponent>().Build();
            //contextMenuItem = contextMenu.GetItem().ByText("Обновить").Build();
            //contextMenuItem.Click();

            //Проверка состава
            //Сопровождающие документы по объекту
            //var Content0 = CreatedObj.GetItem().ByText(sContent0).Build();
            //Content0.ShouldAvailable(TimeSpan.FromMilliseconds(500));

            ////Размещение документации
            //var Content1 = CreatedObj.GetItem().ByText(sContent1).Build();
            //Content1.ShouldAvailable(TimeSpan.FromMilliseconds(500));

            ////Проекты
            //var Content2 = CreatedObj.GetItem().ByText(sContent2).Build();
            //Content2.ShouldAvailable(TimeSpan.FromMilliseconds(500));

            ////Структура объекта
            //var Content3 = CreatedObj.GetItem().ByText(sContent3).Build();
            //Content3.ShouldAvailable(TimeSpan.FromMilliseconds(500));

            ////Техническая документация
            //var Content4 = CreatedObj.GetItem().ByText(sContent4).Build();
            //Content4.ShouldAvailable(TimeSpan.FromMilliseconds(500));

            return TreeDescr;
        }

        /// <summary>
        /// Проверка объекта структуры
        /// </summary>
        /// <param name="sObjDev">Описание объекта разработки</param>
        /// <param name="index">Номер шага</param>
        /// <returns></returns>
        [AllureStep("{index}. Проверка Объекта разработки")]
        public string CheckObjDev(string sObjDev, int index = 0)
        {
            string sContent0 = "Сопровождающие документы по объекту";
            string sContent1 = "Размещение документации";
            string sContent2 = "Проекты";
            //string sContent3 = $"{sCode} Структура объекта \"{sName}\"";
            string sContent3 = "Структура объекта";
            //string sContent4 = $"{sCode}.ТД Техническая документация на \"{sName}\"";
            string sContent4 = "ТД Техническая документация на";

            var rootTreeItem = Application.TreeView.GetComponent<TreeViewItemComponent>().ByText("TDM365").Build();

            //Проверка Объекта разработки
            var TreeObj = rootTreeItem.GetItem().ByText(sObjDev).Build();
            bool bObj = TreeObj.IsAvailable(TimeSpan.FromMilliseconds(1000));
            Assert.True(bObj, $"В дереве Объект разработки \"{sObjDev}\" не найден");
            //CreatedObj.ShouldAvailable(TimeSpan.FromMilliseconds(1000));
            if (!TreeObj.GetItems().Build().Any()) TreeObj.DoubleClick();

            //Обновить элемент дерева
            //CreatedObj.ContextClick();
            //contextMenu = Context.GetComponent<ContextMenuComponent>().Build();
            //contextMenuItem = contextMenu.GetItem().ByText("Обновить").Build();
            //contextMenuItem.Click();

            //Проверка состава
            //Сопровождающие документы по объекту
            var Content0 = TreeObj.GetItem().ByText(sContent0).Build();
            Content0.ShouldAvailable(TimeSpan.FromMilliseconds(500));

            //Размещение документации
            var Content1 = TreeObj.GetItem().ByText(sContent1).Build();
            Content1.ShouldAvailable(TimeSpan.FromMilliseconds(500));

            //Проекты
            var Content2 = TreeObj.GetItem().ByText(sContent2).Build();
            Content2.ShouldAvailable(TimeSpan.FromMilliseconds(500));

            //Структура объекта
            var Content3 = TreeObj.GetItem().ByText(sContent3).Build();
            Content3.ShouldAvailable(TimeSpan.FromMilliseconds(500));

            //Техническая документация
            var Content4 = TreeObj.GetItem().ByText(sContent4).Build();
            Content4.ShouldAvailable(TimeSpan.FromMilliseconds(500));

            return sObjDev;
        }

        /// <summary>
        /// Создание проекта
        /// </summary>
        /// <param name="sObjDev">Описание объекта разработки, в составе которого создание Проекта</param>
        /// <param name="index">Номер шага</param>
        /// <returns></returns>
        [AllureStep("{index}. Создание Проекта")]
        public string CreateProject(string sObjDev, int index = 0)
        {
            string sCode = $"{DateTime.Now.Ticks}";
            string sName = "Автотестовый проект";
            string sFullName = $"Создано автотестом";
            var Date = DateTime.Now;
            string TreeDescr = $"{sCode} {sName}";
            string sContent0 = $"{sCode} Размещение документации";
            string sContent1 = $"{sCode} План работ";
            string sContent2 = "Сопровождающая документация";
            string sContent3 = "Техническая документация проекта";
            string sContent4 = "Пакеты документов";

            //Создание проекта
            var rootTreeItem = Application.TreeView.GetComponent<TreeViewItemComponent>().ByText(sObjDev).Build();
            var ProjectsTreeItem = rootTreeItem.GetComponent<TreeViewItemComponent>().ByText("Проекты").Build();
            ProjectsTreeItem.ContextClick();
            var contextMenu = Context.GetComponent<ContextMenuComponent>().Build();
            var contextMenuItem = contextMenu.GetItem().ByText("Создать проект").Build();
            contextMenuItem.Click();
            var createDlg = Context.GetComponent<DialogComponent>().Build();

            //Переключение на вкладку Проект
            var Tab = createDlg.Body.GetComponent<TabComponent>().ByText("Проект").Build();
            Tab.Click();
            if (Tab.IsAvailable(TimeSpan.FromMilliseconds(100))) Tab.Click(); //С первого раза почему-то не срабатывает

            //Заполнение атрибутов
            var CodeEdit = createDlg.Body.GetComponent<InputComponent>().BySystemIdentifier("ATTR_Project_Code").Build();
            CodeEdit.SetValue(sCode);
            var NameEdit = createDlg.Body.GetComponent<InputComponent>().BySystemIdentifier("ATTR_Project_Name").Build();
            NameEdit.SetValue(sName);
            var FullNameEdit = createDlg.Body.GetComponent<TextAreaComponent>().BySystemIdentifier("ATTR_Project_Full_Name").Build();
            FullNameEdit.SetValue(sFullName);
            var DateEdit = createDlg.Body.GetComponent<InputComponent>().BySystemIdentifier("ATTR_PROJECT_DATE_FINISH_PLAN").Build();
            DateEdit.SetValue(Date.ToString());

            //Указание групп
            //Назначить права
            var Btn = createDlg.Body.GetComponent<ButtonComponent>().ByReference("BUTTON_ROLES_SET").Build();
            Btn.Click();
            Btn.Click();
            var SelectDlg = Context.GetComponents<DialogComponent>().ByText("Определение").Build().LastOrDefault();
            if (SelectDlg != null)
            {
                //Участники проекта
                Btn = SelectDlg.Body.GetComponent<ButtonComponent>().ByReference("BUTTON_PROJECT_TEAM").Build();
                Btn.Click();
                var Select0Dlg = Context.GetComponents<DialogComponent>().ByText("Выбор").Build().LastOrDefault();
                if (Select0Dlg != null)
                {
                    var Cell = Select0Dlg.Body.GetComponent<TableComponent>().Build().GetCell().ByText("Все пользователи").Build();
                    if (Cell.IsAvailable(TimeSpan.FromMilliseconds(1000)))
                        Cell.Click();
                    Select0Dlg.Ok();
                }
                //Приемка документации
                Btn = SelectDlg.Body.GetComponent<ButtonComponent>().ByReference("BUTTON_PROJECT_CHECKING").Build();
                Btn.Click();
                Select0Dlg = Context.GetComponents<DialogComponent>().ByText("Выбор").Build().LastOrDefault();
                if (Select0Dlg != null)
                {
                    var Cell = Select0Dlg.Body.GetComponent<TableComponent>().Build().GetCell().ByText("Все пользователи").Build();
                    if (Cell.IsAvailable(TimeSpan.FromMilliseconds(1000)))
                        Cell.Click();
                    Select0Dlg.Ok();
                }
                //Размещение документации
                Btn = SelectDlg.Body.GetComponent<ButtonComponent>().ByReference("BUTTON_PROJECT_CREATING").Build();
                Btn.Click();
                Select0Dlg = Context.GetComponents<DialogComponent>().ByText("Выбор").Build().LastOrDefault();
                if (Select0Dlg != null)
                {
                    var Cell = Select0Dlg.Body.GetComponent<TableComponent>().Build().GetCell().ByText("Все пользователи").Build();
                    if (Cell.IsAvailable(TimeSpan.FromMilliseconds(1000)))
                        Cell.Click();
                    Select0Dlg.Ok();
                }
                SelectDlg.Ok();
                if (SelectDlg.IsAvailable(TimeSpan.FromMilliseconds(100))) SelectDlg.Ok();
            }

            //Переключение на вкладку Регламент
            Tab = createDlg.Body.GetComponent<TabComponent>().ByText("Регламент").Build();
            Tab.Click();
            if (Tab.IsAvailable(TimeSpan.FromMilliseconds(100))) Tab.Click(); //С первого раза почему-то не срабатывает

            //Указание производственного календаря
            Btn = createDlg.Body.GetComponent<ButtonComponent>().ByReference("BUTTON_PROD_CALENDAR").Build();
            Btn.Click();
            SelectDlg = Context.GetComponents<DialogComponent>().ByText("Выбор").Build().LastOrDefault();
            if (SelectDlg != null)
            {
                var Cell = SelectDlg.Body.GetComponent<TableComponent>().Build().GetCell().ByText("календарь").Build();
                if (Cell.IsAvailable(TimeSpan.FromMilliseconds(500)))
                    Cell.Click();
                SelectDlg.Ok();
                if (SelectDlg.IsAvailable(TimeSpan.FromMilliseconds(100))) SelectDlg.Ok();
            }

            //Сохранение проекта
            createDlg.Ok();
            //if (createDlg.IsAvailable(TimeSpan.FromMilliseconds(100))) createDlg.Ok();

            //Проверка Проекта
            ProjectsTreeItem.DoubleClick();
            var CreatedObj = ProjectsTreeItem.GetItem().ByText(TreeDescr).Build();
            bool bObj = CreatedObj.IsAvailable(TimeSpan.FromMilliseconds(1000));
            Assert.True(bObj, $"В дереве созданный объект \"{TreeDescr}\" не найден");
            //CreatedObj.ShouldAvailable(TimeSpan.FromMilliseconds(1000));
            CreatedObj.DoubleClick();

            //Запуск проекта на выполнение
            //CreatedObj.ContextClick();
            //contextMenu = Context.GetComponent<ContextMenuComponent>().Build();
            //contextMenuItem = contextMenu.GetItem().ByText("Начать выполнение").Build();
            //contextMenuItem.Click();
            ////Запрос запуска проекта
            //var ExecuteDialog = Context.GetComponent<DialogComponent>().ByText("TDM365").Build();
            //if (!ExecuteDialog.IsAvailable(TimeSpan.FromMilliseconds(3000)))
            //    Assert.True(false, $"Не отобразился диалог команды \"Начать выполнение\"");
            //ExecuteDialog.Yes(TimeSpan.FromMilliseconds(500));

            ////Проверка состава
            ////Размещение документации
            //var Content0 = CreatedObj.GetItem().ByText(sContent0).Build();
            //Content0.ShouldAvailable(TimeSpan.FromMilliseconds(1000));
            ////План работ
            //var Content1 = CreatedObj.GetItem().ByText(sContent1).Build();
            //Content1.ShouldAvailable(TimeSpan.FromMilliseconds(1000));
            ////Сопровождающая документация
            //var Content2 = CreatedObj.GetItem().ByText(sContent2).Build();
            //Content2.ShouldAvailable(TimeSpan.FromMilliseconds(1000));
            ////Техническая документация проекта
            //var Content3 = CreatedObj.GetItem().ByText(sContent3).Build();
            //Content3.ShouldAvailable(TimeSpan.FromMilliseconds(1000));
            ////Пакеты документов
            //var Content4 = CreatedObj.GetItem().ByText(sContent4).Build();
            //Content4.ShouldAvailable(TimeSpan.FromMilliseconds(1000));

            return TreeDescr;
        }

        /// <summary>
        /// Запуск проекта на выполнение
        /// </summary>
        /// <param name="sProject">Описание объекта</param>
        /// <param name="index">Номер шага</param>
        /// <returns></returns>
        [AllureStep("{index}. Запуск Проекта")]
        public string ExecuteProject(string sProject, int index = 0)
        {
            //Поиск проекта в дереве
            var ProjectTreeItem = Application.TreeView.GetComponent<TreeViewItemComponent>().ByText(sProject).Build();
            Assert.True(ProjectTreeItem.IsAvailable(TimeSpan.FromMilliseconds(500)), $"В дереве Проект \"{sProject}\" не найден");

            //Просмотр проекта для чтения кода проекта
            ProjectTreeItem.ContextClick();
            var contextMenu = Context.GetComponent<ContextMenuComponent>().Build();
            var contextMenuItem = contextMenu.GetItem().ByText("Просмотреть").Build();
            contextMenuItem.Click();
            var ObjDlg = Context.GetComponent<DialogComponent>().Build();
            Assert.True(ObjDlg.IsAvailable(TimeSpan.FromMilliseconds(500)), $"Диалог просмотра проекта не найден");
            var Tab = ObjDlg.Body.GetComponent<TabComponent>().ByText("Проект").Build();
            Tab.Click();
            var CodeEdit = ObjDlg.Body.GetComponent<InputComponent>().BySystemIdentifier("ATTR_Project_Code").Build();
            string sCode = CodeEdit.GetValue();
            ObjDlg.Close();

            string sContent0 = $"{sCode} Размещение документации";
            string sContent1 = $"{sCode} План работ";
            string sContent2 = "Сопровождающая документация";
            string sContent3 = "Техническая документация проекта";
            string sContent4 = "Пакеты документов";

            //Запуск проекта на выполнение
            ProjectTreeItem.ContextClick();
            contextMenu = Context.GetComponent<ContextMenuComponent>().Build();
            contextMenuItem = contextMenu.GetItem().ByText("Начать выполнение").Build();
            contextMenuItem.Click();
            //Запрос запуска проекта
            var ExecuteDialog = Context.GetComponent<DialogComponent>().ByText("TDM365").Build();
            if (!ExecuteDialog.IsAvailable(TimeSpan.FromMilliseconds(1000)))
                Assert.True(false, $"Не отобразился диалог команды \"Начать выполнение\"");
            ExecuteDialog.Yes(TimeSpan.FromMilliseconds(500));

            //Проверка состава
            //Размещение документации
            var Content0 = ProjectTreeItem.GetItem().ByText(sContent0).Build();
            Content0.ShouldAvailable(TimeSpan.FromMilliseconds(500));

            //План работ
            var Content1 = ProjectTreeItem.GetItem().ByText(sContent1).Build();
            Content1.ShouldAvailable(TimeSpan.FromMilliseconds(500));

            //Сопровождающая документация
            var Content2 = ProjectTreeItem.GetItem().ByText(sContent2).Build();
            Content2.ShouldAvailable(TimeSpan.FromMilliseconds(500));

            //Техническая документация проекта
            var Content3 = ProjectTreeItem.GetItem().ByText(sContent3).Build();
            Content3.ShouldAvailable(TimeSpan.FromMilliseconds(500));

            //Пакеты документов
            var Content4 = ProjectTreeItem.GetItem().ByText(sContent4).Build();
            Content4.ShouldAvailable(TimeSpan.FromMilliseconds(500));
            return sProject;
        }

        //====================================================================================================
        #endregion Methods

        /// <summary>
        /// Вход и выход в системе
        /// </summary>
        [Fact]
        [AllureFeature("Авторизация")]
        [AllureDescription("Авторизация пользователя в системе и выход из системы")]
        [AllureTag("Авторизация", "Выход из системы")]
        [AllureOwner("Чернышов Дмитрий")]
        public void LoginTest()
        {
            Authorization("SYSADMIN", 0);
            UnAuthorization(1);
        }

        /// <summary>
        /// Прощелкать все разделы и все команды меню пользователя
        /// </summary>
        [Fact]
        [AllureFeature("Авторизация", "Разделы")]
        [AllureDescription("Открытие основных разделов TDM365 и выполнение каждой команды из меню пользователя")]
        [AllureTag("Авторизация", "Разделы", "Панели", "Рабочий стол", "Объекты", "Почта", "Совещания", "Справка", "О программе", "Сменить пароль",
            "Настройки", "Интерфейс", "Уведомления")]
        [AllureOwner("Чернышов Дмитрий")]
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
            UserCommand("Свойства пользователя", true, 10);
            UserCommand("Интерфейс", true, 11);
        }

        /// <summary>
        /// Открытие и закрытие всех панелей
        /// </summary>
        [Fact]
        [AllureFeature("Авторизация", "Панели")]
        [AllureDescription("В разделе панелей открываются все панели и закрываются")]
        [AllureTag("Авторизация", "Панели", "Стартовая страница", "Задачи", "Журнал сервера", "Журнал событий", "Журнал уведомлений", "Проекты",
            "Планирование", "Закрыть панель")]
        [AllureOwner("Чернышов Дмитрий")]
        public void PanelTest()
        {
            Authorization("SYSADMIN", 0);
            GoToSection("Панели", 1);
            //Открытие панелей
            PanelCommand("TDM365", "TDM365", 2);
            PanelCommand("Оперативная панель", "TDM365", 3);
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
            //PanelCommand("Закрыть панель", "FORM_PANEL_PLANNING", 15); //У последней панели нет команды на закрытие
        }

        #region Меню_администратора
        /// <summary>
        /// Команда меню администратора - Информация о системе
        /// </summary>
        [Fact]
        [AllureFeature("Авторизация", "Меню администратора")]
        [AllureDescription("Запуск команды меню администратора - Информация о системе")]
        [AllureTag("Авторизация", "Меню администратора", "Информация о системе")]
        [AllureOwner("Чернышов Дмитрий")]
        public void SystemInfo()
        {
            Authorization("SYSADMIN", 0);
            string DialogName = "Информация о системе";
            var Command = AdminCommand("Информация о системе", false, 1);
            
            //Нажатие на кнопки формы
            GetDialogFromButton(DialogName, "BUTTON_CHANGE_LOG_SERVER", "TDM365", true, 2);
            GetDialogFromButton(DialogName, "BUTTON_CHANGE_LOG_TDM", "TDM365", true, 3);

            //Проверка полей на форме
            //Версия сервера
            FieldEmptyCheck(DialogName, "VER_SERVER", 4);
            //Версия TDM365
            FieldEmptyCheck(DialogName, "VER_TDM365", 5);
            //База данных
            FieldEmptyCheck(DialogName, "BD_PATH", 6);
        }

        /// <summary>
        /// Команда меню администратора - Утилиты TDMS
        /// </summary>
        [Fact]
        [AllureFeature("Авторизация", "Меню администратора")]
        [AllureDescription("Запуск команды меню администратора - Утилиты TDMS")]
        [AllureTag("Авторизация", "Меню администратора", "Утилиты TDMS")]
        [AllureOwner("Чернышов Дмитрий")]
        public void TdmsUtilities()
        {
            Authorization("SYSADMIN", 0);
            string DialogName = "Утилиты TDMS";
            var Command = AdminCommand("Утилиты TDMS", false, 1);

            //Запись команды в поле
            SetFieldValue(DialogName, "ATTR_CODE", "info", 2);

            //Нажатие на кнопку - Отправить
            GetDialogFromButton(DialogName, "BUTTON_SEND", "", false, 3);

            //Проверка результата в поле
            FieldEmptyCheck(DialogName, "ATTR_NAME", 4);
        }

        /// <summary>
        /// Команда меню администратора - Системные атрибуты
        /// </summary>
        [Fact]
        [AllureFeature("Авторизация", "Меню администратора")]
        [AllureDescription("Запуск команды меню администратора - Системные атрибуты")]
        [AllureTag("Авторизация", "Меню администратора", "Системные атрибуты")]
        [AllureOwner("Чернышов Дмитрий")]
        public void SysAttrs()
        {
            Authorization("SYSADMIN", 0);
            string DialogName = "Системные атрибуты";
            var Command = AdminCommand("Системные атрибуты", false, 1);

            //Нажатие на кнопку - Строки переменной величины
            GetDialogFromButton(DialogName, "BUTTON_HEIGHT", "", true, 2);
        }

        /// <summary>
        /// Команда меню администратора - Импорт пользователей
        /// </summary>
        [Fact]
        [AllureFeature("Авторизация", "Меню администратора")]
        [AllureDescription("Запуск команды меню администратора - Импорт пользователей")]
        [AllureTag("Авторизация", "Меню администратора", "Импорт пользователей")]
        [AllureOwner("Чернышов Дмитрий")]
        public void ImportUsers()
        {
            Authorization("SYSADMIN", 0);
            string DialogName = "Импорт пользователей";
            var Command = AdminCommand("Импорт пользователей", false, 1);

            //Нажатие на кнопку - Строки переменной величины
            GetDialogFromButton(DialogName, "BUTTON_HEIGHT", "", true, 2);
        }

        /// <summary>
        /// Команда меню администратора - Импорт пользователей
        /// </summary>
        [Fact]
        [AllureFeature("Авторизация", "Меню администратора")]
        [AllureDescription("Запуск команды меню администратора - Делегирование прав")]
        [AllureTag("Авторизация", "Меню администратора", "Делегирование прав")]
        [AllureOwner("Чернышов Дмитрий")]
        public void DelegationUsers()
        {
            Authorization("SYSADMIN", 0);
            string DialogName = "Делегирование прав";
            var Command = AdminCommand("Делегирование прав", false, 1);

            //Нажатие на кнопку - Добавить запись
            GetDialogFromButton(DialogName, "BUTTON_ADD", "Замещение", true, 2);
        }

        /// <summary>
        /// Команда меню администратора - Настройка шаблона уведомлений
        /// </summary>
        [Fact]
        [AllureFeature("Авторизация", "Меню администратора")]
        [AllureDescription("Запуск команды меню администратора - Настройка шаблона уведомлений")]
        [AllureTag("Авторизация", "Меню администратора", "Настройка шаблона уведомлений")]
        [AllureOwner("Чернышов Дмитрий")]
        public void NotificationTemplate()
        {
            Authorization("SYSADMIN", 0);
            var Command = AdminCommand("Настройка шаблона уведомлений", false, 1);

            var ObjDlg = Context.GetComponent<DialogComponent>().Build();
            //Переключение на вкладку Проект
            var Tab = ObjDlg.Body.GetComponent<TabComponent>().BySystemIdentifier("FORM_NOTIFICATION_TEMPLATE").Build();
            Tab.Click();
            //if (Tab.IsAvailable(TimeSpan.FromMilliseconds(100))) Tab.Click(); //С первого раза почему-то не срабатывает

            //Нажатие на кнопку - Добавить уведомление
            GetDialogFromButton("Редактирование объекта", "BUTTON_ADD", "Настройка уведомления", true, 2);

            ObjDlg = Context.GetComponent<DialogComponent>().Build();
            Tab = ObjDlg.Body.GetComponent<TabComponent>().BySystemIdentifier("FORM_MAIL_SETTINGS").Build();
            Tab.Click();
            Tab = ObjDlg.Body.GetComponent<TabComponent>().BySystemIdentifier("FORM_NOTIFICATIONS_HISTORY").Build();
            Tab.Click();
            Thread.Sleep(200);

            ObjDlg.Cancel();
        }

        #endregion AdminMenu

        /// <summary>
        /// Создание структуры Объекта разработки
        /// </summary>
        [Fact]
        [AllureFeature("Авторизация", "Объекты")]
        [AllureDescription("Создание структуры Объекта разработки")]
        [AllureTag("Авторизация", "Объекты", "Объект разработки", "Проект")]
        [AllureOwner("Чернышов Дмитрий")]
        public void ObjDevCreateTest()
        {
            Authorization("SYSADMIN", 0);

            GoToSection("Объекты", 1);

            //Создание Объекта разработки
            string ObjDev = CreateObjDev(2);

            //Проверка Объекта разработки
            ObjDev = CheckObjDev(ObjDev, 3);

            //Создание Проекта
            string Project = CreateProject(ObjDev, 4);

            //Запуск проекта
            Project = ExecuteProject(Project, 5);
        }

        
    }
}
