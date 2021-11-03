using System;
using System.Linq;
using System.Xml.Linq;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl;
using JetBrains.Util;
using System.Text.RegularExpressions;
using JetBrains.Application.Settings;
using JetBrains.Lifetimes;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Transactions;
using JetBrains.Application.UI.UIAutomation;

namespace ReSharperPlugin.ConvertToXElement
{
    [ContextAction(Name = "ConvertToXElement", Description = "Convert to XElement", Group = "C#", Disabled = false, Priority = 1)]
    public class ConvertToXElement : ContextActionBase
    {
        public bool IncludeComments { get; set; }
        
        private readonly IVariableDeclaration _variableDeclaration;

        public ConvertToXElement(LanguageIndependentContextActionDataProvider dataProvider)
        {
            _variableDeclaration = dataProvider.GetSelectedElement<IVariableDeclaration>();
            // var option = settingsStore.BindToContextLive(lifetime, ContextRange.ApplicationWide)
            //     .GetValueProperty(lifetime, (PluginSettings key) => key.IncludeComments);

            // IncludeComments = option.Value;
        }

        public override string Text => "Convert to XElement";

        protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            return control =>
            {
                var xml = XDocument.Parse(GetExpressionString(_variableDeclaration));
                var newText = NodeToXElement(xml.Root);
                
                var initializer = _variableDeclaration.Children<IExpressionInitializer>().FirstOrDefault();
                var psiServices = solution.GetPsiServices();

                using (new PsiTransactionCookie(psiServices, DefaultAction.Commit, "Insert arguments"))
                {
                    var factory = CSharpElementFactory.GetInstance(initializer);
                    var newExpression = factory.CreateExpressionAsIs(newText, false);
                    initializer.Value.ReplaceBy(newExpression);
                }
            };
        }

        public string GetExpressionString(IVariableDeclaration variableDeclaration)
        {
            try
            {
                var initializer = variableDeclaration.Children<IExpressionInitializer>().FirstOrDefault();
                var valueText = Regex.Unescape(initializer.Value.GetText().Trim('\"'));
                return valueText;
            }
            catch
            {
                return null;
            }
        }
        
        
        public override bool IsAvailable(IUserDataHolder cache)
        {
            try
            {
                XDocument.Parse(GetExpressionString(_variableDeclaration));
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        public string NodeToXElement(XElement node, int level = 0, XNamespace ns = null)
        {
            var xString = "";
            
            
            xString = $"new XElement(";

            if (ns != null)
            {
                xString += "ns + ";
            }

            xString += $"\"{node.Name}\"";

            if (!node.HasAttributes && !node.HasElements && string.IsNullOrEmpty(node.Value))
            {
                xString += ")";
                return xString;
            }

            foreach (var xText in node.Nodes().OfType<XText>())
            {
                xString += $", \"{xText.Value}\"";
            }

            
            
            if (node.HasAttributes)
            {
                foreach (var attr in node.Attributes())
                {
                    xString += $",\n{GetTabs(level+1)}new XAttribute(\"{attr.Name}\", \"{attr.Value}\")";
                }
            }
            
            var comments = "";
            
            foreach (var xComment in node.Nodes().OfType<XComment>())
            {
                var escapedValue = xComment.Value;
                
                if (comments == "")
                {
                    comments += $"  //{escapedValue}";
                }
                else comments += $";{escapedValue}";
            }

            if (node.HasElements)
            {
                foreach (var child in node.Elements())
                {
                    xString += $",\n{GetTabs(level+1)}{NodeToXElement(child, level + 1, ns)}";
                }                
            }

            xString += ")";
            
            var stringLines = xString.Split('\n');
            if (IncludeComments)
            {
                stringLines[0] = stringLines[0] += comments;
            }

            return string.Join("\n", stringLines);
        }

        public string GetTabs(int level)
        {
            var tabStr = "";
            
            for (var i = 0; i < level; i++)
            {
                tabStr += "\t";
            }

            return tabStr;
        }
    }
}