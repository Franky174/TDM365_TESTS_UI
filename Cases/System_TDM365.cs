using Bismuthum.Core.Implementations;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tdms.Automatic.Ui.Components.Extensions;
using Tdms.Automatic.Ui.Components.Implementations;
using Tdms.Automatic.Ui.Components.Implementations.Abstractions;
using Tdms.Automatic.Ui.Components.Implementations.Application;
using Tdms.Automatic.Ui.Components.Implementations.ObjectDescription;
using Tdms.Automatic.Ui.Components.Implementations.Table;
using Tdms.Automatic.Ui.Components.Implementations.TreeView;

namespace Tdms.Automatic.Ui.Components.Implementations.Application
{
    public static class ApplicationHeaderExtensions
    {
        private static readonly Description _logoButtonDescription = new(Selector.Css("*[id='logo-container']"), "Logo Button");

        private static readonly Description _chatButtonDescription = new(Selector.Css("*[id='chat-tab']"), "Chat Button");

        private static readonly Description _helpButtonDescription = new(Selector.Css("*[id='help-tab']"), "Help Button");

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
    }
}
