using Bismuthum.Core.Implementations;
using Bismuthum.Core.Interfaces;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tdms.Ui.Test.Components.Extensions;
using Tdms.Ui.Test.Components.Implementations;
using Tdms.Ui.Test.Components.Implementations.Abstractions;
using Tdms.Ui.Test.Components.Implementations.Application;
using Tdms.Ui.Test.Components.Implementations.ContextMenu;
using Tdms.Ui.Test.Components.Implementations.ObjectDescription;
using Tdms.Ui.Test.Components.Implementations.Table;
using Tdms.Ui.Test.Components.Implementations.TreeView;

namespace Tdms.Ui.Test.Components.Implementations.Application
{
    public static class ApplicationHeaderExtensions
    {
        private static readonly Description _logoButtonDescription = new(Selector.Css("*[id='logo-container']"), "Logo Button");

        private static readonly Description _chatButtonDescription = new(Selector.Css("*[id='chat-tab']"), "Chat Button");

        private static readonly Description _helpButtonDescription = new(Selector.Css("*[id='help-tab']"), "Help Button");

        private static readonly Description _administratorButtonDescription = new(Selector.Css("*[id='administrator-tab']"), "Administrator Button");

        public static ButtonComponent GetLogoButton(this ApplicationHeaderComponent header)
        {
            return header.GetComponent<ButtonComponent>().WithDescription(_logoButtonDescription).Build();
        }

        public static ButtonComponent GetChatButton(this ApplicationHeaderComponent header)
        {
            return header.GetComponent<ButtonComponent>().WithDescription(_chatButtonDescription).Build();
        }

        public static ButtonComponent GetHelpButton(this ApplicationHeaderComponent header)
        {
            return header.GetComponent<ButtonComponent>().WithDescription(_helpButtonDescription).Build();
        }

        public static ButtonComponent GetAdministratorButton(this ApplicationHeaderComponent header)
        {
            return header.GetComponent<ButtonComponent>().WithDescription(_administratorButtonDescription).Build();
        }
    }

    public static class ApplicationToolbarComponent_TDM365
    {
        public static ButtonComponent GetAdminButton(this ApplicationToolbarComponent toolbar)
        {
            var Button = toolbar.GetComponent<ButtonComponent>().BySystemIdentifier("SUB_SYSADMIN").Build();
            if (!Button.IsAvailable(TimeSpan.FromMilliseconds(500)))
            {
                var ExtButton = toolbar.GetComponent<ButtonComponent>().ByReference("global-commands-popup").Build();
                if (ExtButton.IsAvailable(TimeSpan.FromMilliseconds(500)))
                {
                    ExtButton.Click();
                    var contextMenu = ExtButton.GetComponent<ContextMenuComponent>().Build();
                    if (contextMenu.IsAvailable(TimeSpan.FromMilliseconds(500)))
                        Button = contextMenu.GetComponent<ButtonComponent>().BySystemIdentifier("SUB_SYSADMIN").Build();
                }
            }
            
            return Button;
        }
    }

}
