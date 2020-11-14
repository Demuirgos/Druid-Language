using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.Storage;
using System.Text.RegularExpressions;
using YetAnotherScriptingLanguage;
// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace App1
{
    public sealed partial class TranslationUnitEdit : UserControl
    {
        StorageFile _local;
        string tabSize = "    ";
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
                             if (previousword == "function" || previousword == "variable" || Regex.Match(word,"[0-9]+([.][0-9]+)?").Success)
                             {
                                 Color = previousword == "variable" ? Windows.UI.Colors.DeepSkyBlue : previousword== "function" ? Windows.UI.Colors.LimeGreen : Windows.UI.Colors.OrangeRed;
                             }
                             EditZone.Document.Selection.SetRange(idx - word.Length, idx);
                             EditZone.Document.Selection.CharacterFormat.ForegroundColor = Color;
                             EditZone.Document.Selection.SetRange(idx,idx + 1);
                             EditZone.Document.Selection.CharacterFormat.ForegroundColor = Windows.UI.Colors.IndianRed; ;
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
            EditZoneWrapper.Height = Math.Max(10,this.ActualHeight - 75);
            EditZoneWrapper.Width = (this.ActualWidth -25);
        }

        private void BoldButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedText = EditZone.Document.Selection;
            if (!(selectedText is null))
            {
                var charFormatting = selectedText.CharacterFormat;
                charFormatting.Bold = Windows.UI.Text.FormatEffect.Toggle;
                selectedText.CharacterFormat = charFormatting;
            }
        }

        private void ItalicButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedText = EditZone.Document.Selection;
            if (!(selectedText is null))
            {
                var charFormatting = selectedText.CharacterFormat;
                charFormatting.Italic = Windows.UI.Text.FormatEffect.Toggle;
                selectedText.CharacterFormat = charFormatting;
            }
        }

        private void SubButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedText = EditZone.Document.Selection;
            if (!(selectedText is null))
            {
                var charFormatting = selectedText.CharacterFormat;
                charFormatting.Underline = charFormatting.Underline!= Windows.UI.Text.UnderlineType.None ? Windows.UI.Text.UnderlineType.None: Windows.UI.Text.UnderlineType.Single;
                selectedText.CharacterFormat = charFormatting;
            }
        }

        private void SizeIncButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedText = EditZone.Document.Selection;
            if (!(selectedText is null))
            {
                var charFormatting = selectedText.CharacterFormat;
                charFormatting.Size = Math.Max(charFormatting.Size + 2,2);
                selectedText.CharacterFormat = charFormatting;
            }
        }

        private void SizeDecButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedText = EditZone.Document.Selection;
            if (!(selectedText is null))
            {
                var charFormatting = selectedText.CharacterFormat;
                charFormatting.Size = charFormatting.Size-2<=0?1:charFormatting.Size - 2;
                selectedText.CharacterFormat = charFormatting;
            }
        }

        private void SearchRequest_Click(object sender, RoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            String selectedText; EditZone.Document.GetText(Windows.UI.Text.TextGetOptions.UseCrlf,out selectedText);
            if (!(selectedText is null))
            {
                //EditZone.Document.Selection.set
            }
        }

        private void EditZone_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if(e.Key == Windows.System.VirtualKey.Tab)
            {
                int start = EditZone.Document.Selection.StartPosition;
                EditZone.Document.Selection.Text = tabSize;
                EditZone.Document.Selection.SetRange(start + tabSize.Length, start + tabSize.Length);
                e.Handled = true;
                if (EditZone.Focus(FocusState.Programmatic))
                    return;
                else
                    EditZone.Focus(FocusState.Programmatic);
            }
        }

        private void TabSizeSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            tabSize = "";
            for (int i = 0; i < e.NewValue; i++) tabSize += " ";
        }
    }
}
