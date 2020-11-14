using System;
using System.Collections.Generic;
using System.Xml;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Storage;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace App1.Prompts
{
    public sealed partial class LanguageSettingsPrompt : ContentDialog
    {
        XmlDocument configurationFile = new XmlDocument();
        string chosenLanguage = "English";
        Dictionary<string, string> MapKeywords = new Dictionary<string, string>();
        Dictionary<string, string> MapTypeKeywords = new Dictionary<string, string>();
        Dictionary<string, string> MapOperations = new Dictionary<string, string>();
        public LanguageSettingsPrompt()
        {
            this.InitializeComponent();
            configurationFile.Load(ApplicationData.Current.LocalFolder.Path + "\\Configurations.xml");
            chosenLanguage = configurationFile.SelectSingleNode("//Settings/Current/configuration").Attributes.GetNamedItem("value").Value;
            FillUI();
        }

        void FillUI()
        {
            SelectedPreset.Items.Clear();
            var languageSet = configurationFile.SelectSingleNode("//Settings//Localizations");
            int i = 0; bool found = false;
            foreach(XmlNode lang in languageSet)
            {
                string langName = lang.Attributes.GetNamedItem("Name").Value;
                StackPanel holder = new StackPanel { Orientation = Orientation.Horizontal };
                TextBlock Label = new TextBlock { Text = langName, Width = 190, Height = 25 };
                Button button = new Button { Name= langName, Content = "-", Width = 25, Height = 25 };
                button.Click += RemoveLanguageClicked;
                holder.Children.Add(Label);
                holder.Children.Add(button);
                SelectedPreset.Items.Add(new ComboBoxItem { Name = langName, Content = holder });
                found = found || langName == chosenLanguage;
                if(!found)
                    i++;
            }
            SelectedPreset.SelectedIndex = found?i:i-1;
        }
        private void RemoveLanguageClicked(object sender, RoutedEventArgs e)
        {
            var node = configurationFile.SelectSingleNode("//Settings//Localizations").SelectSingleNode("lang[@Name='"+ (sender as Button).Name + "']");
            if (node.ParentNode.ChildNodes.Count > 1)
            {
                configurationFile.SelectSingleNode("//Settings//Localizations").RemoveChild(node);
            }
            FillUI();
        }

        void UpdateUI()
        {
            FunctionHolder.Items.Clear();
            OperatorHolder.Items.Clear();
            KeywordHolder.Items.Clear();
            var keywordSet = configurationFile.SelectSingleNode("//Settings//Localizations/lang[@Name='" + chosenLanguage + "']/keywords");
            var operationSet = configurationFile.SelectSingleNode("//Settings//Localizations/lang[@Name='" + chosenLanguage + "']/operators");
            foreach (var keyword in keywordSet)
            {
                var KeywordNode = (XmlNode)keyword;
                var key = KeywordNode.Attributes.GetNamedItem("key").Value;
                var map = KeywordNode.Attributes.GetNamedItem("map").Value;
                var type = KeywordNode.Attributes.GetNamedItem("type").Value.ToLower(); 
                if (!MapKeywords.ContainsKey(map))
                    MapKeywords.Add(map, key);
                else
                    MapKeywords[map] = key; 
                if (!MapTypeKeywords.ContainsKey(map))
                    MapTypeKeywords.Add(map, type);
                else
                    MapTypeKeywords[map] = type;
                StackPanel holder = new StackPanel { Orientation = Orientation.Horizontal };
                TextBlock Label = new TextBlock { Text = "KeyWord : ", Width = 100, Height = 35 };
                TextBlock Key = new TextBlock { Text = map, Width = 100, Height = 35 };
                TextBox Value1 = new TextBox { Name = map, Text = key, Width = 100, Height = 35, Margin = new Thickness(15, 0, 0, 0) };
                Value1.TextChanged += KeywordTextChanged;
                holder.Children.Add(Label);
                holder.Children.Add(Key);
                holder.Children.Add(Value1);
                if (type == "function" || type == "keyword") 
                    this.KeywordHolder.Items.Add(new ListBoxItem { Content = holder });
                else this.FunctionHolder.Items.Add(new ListBoxItem { Content = holder });
            }
            foreach (var keyword in operationSet)
            {
                var KeywordNode = (XmlNode)keyword;
                var key = KeywordNode.Attributes.GetNamedItem("key").Value;
                var map = KeywordNode.Attributes.GetNamedItem("map").Value;
                if (!MapOperations.ContainsKey(map))
                    MapOperations.Add(map, key);
                else
                    MapOperations[map] = key;
                StackPanel holder = new StackPanel { Orientation = Orientation.Horizontal };
                TextBlock Label = new TextBlock { Text = "Operator : ", Width = 100, Height = 35 };
                TextBlock Key = new TextBlock { Text = map, Width = 100, Height = 35 };
                TextBox Value1 = new TextBox { Name = map, Text = key, Width = 100, Height = 35, Margin = new Thickness(15, 0, 0, 0) };
                Value1.TextChanged += OperatorTextChanged;
                holder.Children.Add(Label);
                holder.Children.Add(Key);
                holder.Children.Add(Value1);
                this.OperatorHolder.Items.Add(new ListBoxItem { Content = holder });
            }
        }

        private void OperatorTextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox senderCasted = sender as TextBox;
            this.MapOperations[senderCasted.Name] = senderCasted.Text;
        }

        private void KeywordTextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox senderCasted = sender as TextBox;
            this.MapKeywords[senderCasted.Name] = senderCasted.Text;
        }

        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (String.IsNullOrWhiteSpace(chosenLanguage))
            {
                args.Cancel = true;
                return;
            }
            if (CustomPresetMode.IsOn)
            {
                Func<bool> check = () =>
                {
                    List<string> foundWords = new List<string>();
                    foreach (var word in MapKeywords)
                    {
                        if (foundWords.Contains(word.Value))
                        {
                            return false;
                        }
                        foundWords.Add(word.Value);
                    }
                    foreach (var word in MapOperations)
                    {
                        if (foundWords.Contains(word.Value))
                        {
                            return false;
                        }
                        foundWords.Add(word.Value);
                    }
                    foreach(XmlNode lan in configurationFile.SelectNodes("//lang"))
                    {
                        if (lan.Attributes.GetNamedItem("Name").Value == chosenLanguage) return false;
                    }
                    return true;
                };
                if (check())
                {
                    XmlElement resultNode = configurationFile.CreateElement("lang");
                    XmlAttribute resultNodeName = configurationFile.CreateAttribute("Name");
                    resultNode.SetAttribute("Name", chosenLanguage);
                    XmlElement KeywordsNode = configurationFile.CreateElement("keywords");
                    foreach (var keyword in MapKeywords)
                    {
                        XmlElement keywordNode = configurationFile.CreateElement("keyword");
                        XmlAttribute keywordNodeKey = configurationFile.CreateAttribute("key");
                        XmlAttribute keywordNodeMap = configurationFile.CreateAttribute("map");
                        XmlAttribute keywordNodeType = configurationFile.CreateAttribute("type");
                        keywordNode.SetAttribute("map", keyword.Key);
                        keywordNode.SetAttribute("key", keyword.Value);
                        keywordNode.SetAttribute("type", MapTypeKeywords[keyword.Key]);
                        KeywordsNode.AppendChild(keywordNode);
                    }
                    XmlElement OperatorsNode = configurationFile.CreateElement("operators");
                    foreach (var operation in MapOperations)
                    {
                        XmlElement operatorNode = configurationFile.CreateElement("symbol");
                        XmlAttribute operatorNodeKey = configurationFile.CreateAttribute("key");
                        XmlAttribute operatorNodeMap = configurationFile.CreateAttribute("map");
                        operatorNode.SetAttribute("map", operation.Key);
                        operatorNode.SetAttribute("key", operation.Value);
                        OperatorsNode.AppendChild(operatorNode);
                    }
                    resultNode.AppendChild(KeywordsNode);
                    resultNode.AppendChild(OperatorsNode);
                    configurationFile.SelectSingleNode("//Settings//Localizations").AppendChild(resultNode);
                }
                else
                {
                    args.Cancel = true;
                    return;
                }
            }
            configurationFile.SelectSingleNode("//Settings/Current/configuration").Attributes.GetNamedItem("value").Value = chosenLanguage;
            configurationFile.Save(ApplicationData.Current.LocalFolder.Path + "//Configurations.xml");
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void SelectedPreset_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ComboBox).Items.Count > 0)
            {
                int i = (sender as ComboBox).SelectedIndex;
                chosenLanguage = ((sender as ComboBox).Items[i] as ComboBoxItem).Name;
                UpdateUI();
            }
        }

        private void CustomPresetMode_Toggled(object sender, RoutedEventArgs e)
        {
            if (!CustomPresetMode.IsOn)
            {
                SelectedPreset.IsEnabled = true;
            }
            else
            {
                SelectedPreset.IsEnabled = false;
                chosenLanguage = CustomPresetNameBox.Text;
            }
        }

        private void CustomPresetNameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            chosenLanguage = (sender as TextBox).Text;
        }
    }
}
