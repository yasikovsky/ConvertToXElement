using System;
using System.Linq.Expressions;
using JetBrains.Application.Settings;
using JetBrains.Application.UI.Options;
using JetBrains.Application.UI.Options.Options.ThemedIcons;
using JetBrains.Application.UI.Options.OptionsDialog;
using JetBrains.DataFlow;
using JetBrains.IDE.UI.Extensions;
using JetBrains.IDE.UI.Options;
using JetBrains.Lifetimes;
using JetBrains.ReSharper.Feature.Services.Daemon.OptionPages;
using JetBrains.Rider.Model.UIAutomation;

namespace ReSharperPlugin.ConvertToXElement.Options
{
    [OptionsPage(Id, PageTitle, typeof(OptionsThemedIcons.EnvironmentGeneral),
        ParentId = CodeInspectionPage.PID
//        NestingType = OptionPageNestingType.Inline,
//        IsAlignedWithParent = true,
//        Sequence = 0.1d
    )]
    public class ConvertToXElementOptionsPage : BeSimpleOptionsPage
    {
        private const string Id = nameof(ConvertToXElementOptionsPage);
        private const string PageTitle = "ConvertToXElement options";

        private readonly Lifetime _lifetime;

        public ConvertToXElementOptionsPage(
            Lifetime lifetime,
            OptionsPageContext optionsPageContext,
            OptionsSettingsSmartContext optionsSettingsSmartContext)
            : base(lifetime, optionsPageContext, optionsSettingsSmartContext)
        {
            _lifetime = lifetime;

            AddCheckbox((PluginSettings x) => x.IncludeComments, "Include comments");
        }

        private BeCheckbox AddCheckbox<TKeyClass>(Expression<Func<TKeyClass, bool>> lambdaExpression,
            string description)
        {
            var property = new Property<bool>(description);
            OptionsSettingsSmartContext.SetBinding(_lifetime, lambdaExpression, property);
            var control = property.GetBeCheckBox(_lifetime, description);
            AddControl(control.WithDescription(description, _lifetime));
            return control;
        }
        
        private BeTextBox AddTextBox<TKeyClass>(Expression<Func<TKeyClass, string>> lambdaExpression, string description)
        {
            var property = new Property<string>(description);
            OptionsSettingsSmartContext.SetBinding(_lifetime, lambdaExpression, property);
            var control = property.GetBeTextBox(_lifetime);
            AddControl(control.WithDescription(description, _lifetime));
            return control;
        }
    }
}