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
using Windows.Storage;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using YetAnotherScriptingLanguage;
// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace App1
{
    public sealed partial class TranslationUnitEdit : UserControl
    {
        StorageFile _local;
        public string code;
        public string Path => _local.Path;
        public String Code { 
            get
            {
                return code;
            }
        }
        public TranslationUnitEdit(StorageFile local)
        {
            _local = local;
            this.InitializeComponent();
            setupAsync();
        }

        async void setupAsync()
        {
            code = await FileIO.ReadTextAsync(_local);
            EditZone.Document.SetText( Windows.UI.Text.TextSetOptions.None, code);
        }


        private void EditZone_TextChanged(object sender, RoutedEventArgs e)
        {
            var txt = "";
            int start = EditZone.Document.Selection.StartPosition;
            int idx = start;
            EditZone.Document.GetText(Windows.UI.Text.TextGetOptions.None, out txt);
            if (!String.IsNullOrWhiteSpace(txt))
            {
                int count = 2;
                while (0 < idx && count>0)
                {
                    count -= Convert.ToInt32(Tokenizer.Separators.ToList().Contains(txt[idx]));
                    idx--;
                }
                idx = idx==0?0:idx+1;
                Action<Dictionary<string, Windows.UI.Color>, string> ColorProcess = (Dictionary<string, Windows.UI.Color> map, string text) =>
                 {
                     var word = "";
                     var previousword = "";
                     for (; idx < text.Length; idx++)
                     {
                         if (Tokenizer.Separators.ToList().Contains(text[idx]))
                         {
                             var Color = map.ContainsKey(word.ToLower()) ? map[word.ToLower()] : Windows.UI.Colors.White;
                             if (previousword == "function" || previousword == "variable")
                             {
                                 Color = previousword == "variable" ? Windows.UI.Colors.DeepSkyBlue : Windows.UI.Colors.LimeGreen;
                             }
                             if (text[idx] == '\'')
                             {
                                 do
                                 {
                                     word += text[idx];
                                     idx++;
                                 } while (idx < code.Length && text[idx] != '\'');
                                 Color = Windows.UI.Colors.IndianRed;
                             }
                             EditZone.Document.Selection.SetRange(idx - word.Length, idx);
                             EditZone.Document.Selection.CharacterFormat.ForegroundColor = Color;
                             previousword = word.ToLower();
                             word = "";
                         }
                         else
                         {
                             word += text[idx];
                         }
                     }
                     EditZone.Document.Selection.SetRange(start, start);
                 };
                if (txt != code)
                {
                    code = txt;
                    ColorProcess(Interpreter.Keywords.ColorMapValue, txt);
                }
            }
        }

        private void RootGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            EditZone.MinHeight = Math.Max(10,this.ActualHeight - 75);
        }

        private void BoldButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ItalicButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SubButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SpellCkeck_Toggled(object sender, RoutedEventArgs e)
        {

        }
    }
}
