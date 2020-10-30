using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using YetAnotherScriptingLanguage;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace App1
{
    public sealed partial class ConsoleBuild : UserControl
    {
        public ConsoleBuild()
        {
            this.InitializeComponent();
        }

        public void HookEvents()
        {
            ((PrintProcess)Interpreter.Functions["Print"]).PrintHandler += Interpreter_PrintHandler1;
            ((ReadProcess)Interpreter.Functions["Read"]).ReadHandler += Interpreter_ReadHandler;
        }

        public void addText(string s)
        {
            this.BuiltConsole.Text += s;
        }

        private YetAnotherScriptingLanguage.variables.Variable Interpreter_ReadHandler(ReadProcess sender, YetAnotherScriptingLanguage.variables.Variable Argument)
        {

            addText(Argument.Value + (Argument.Value == "" ? "" : " "));
            string before = BuiltConsole.Text;
            var input = BuiltConsole.Text.Substring(before.Length);
            return new YetAnotherScriptingLanguage.variables.Variable(input);
        }



        private void Interpreter_PrintHandler1(PrintProcess sender, List<YetAnotherScriptingLanguage.variables.Variable> Arguments)
        {
            for (int i = 0; i < Arguments.Count; i++)
                addText(Arguments[i].Value + (i < Arguments.Count - 1 ? " " : Environment.NewLine));
        }


        private void TokenizingTextBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {

        }

        private void InputTokens_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {

        }

        private void InputTokens_TokenItemAdded(Microsoft.Toolkit.Uwp.UI.Controls.TokenizingTextBox sender, object args)
        {

        }

        private void InputTokens_TokenItemRemoved(Microsoft.Toolkit.Uwp.UI.Controls.TokenizingTextBox sender, object args)
        {

        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            this.BuiltConsole.Text = "**********************************************************************" +  Environment.NewLine +
                                     "** AYSL CLI Runtime                                                                       **" + Environment.NewLine +
                                     "** Where every bug is shroedinger's bug                                      **" + Environment.NewLine +
                                     "**********************************************************************" + Environment.NewLine +
                                     "Entry Point >>" + Environment.NewLine;
        }
    }
}
