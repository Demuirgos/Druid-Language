using System;
using System.Collections.Generic;
using System.Xml;
using System.Text.RegularExpressions;
using System.Text;

namespace YetAnotherScriptingLanguage
{
    public class KeyWords : Dictionary<String, String>
    {
        XmlDocument configurationFile = new XmlDocument();
        string chosenLanguage = "slang";
        private Dictionary<string, Windows.UI.Color> colorMapValue;
        public Dictionary<string, Windows.UI.Color> ColorMapValue => colorMapValue;
        public KeyWords() : base()
        {
            configurationFile.Load("configurations.xml");
            String SettingConfig = configurationFile.SelectSingleNode("//Settings/Current/configuration").Attributes.GetNamedItem("value").Value;
            chosenLanguage = configurationFile.SelectSingleNode("//Settings/"+ SettingConfig + "/language").Attributes.GetNamedItem("Name").Value;
            XmlNodeList keywordSet = configurationFile.SelectNodes("//Settings//Localizations/lang[@Name='" + chosenLanguage + "'] | //Settings/symbols | //Settings/operators");
            foreach (XmlNode WordMap in keywordSet)
            {
                foreach (var keyword in WordMap)
                {
                    var KeywordNode = (XmlNode)keyword;
                    var key = KeywordNode.Attributes.GetNamedItem("key").Value;
                    var map = KeywordNode.Attributes.GetNamedItem("map").Value;
                    this.Add(key, map);
                }
            }
            colorMapValue = GetColorMap();
        }

        Dictionary<string, Windows.UI.Color> GetColorMap()
        {
            Dictionary<string, Windows.UI.Color> ColoringMap = new Dictionary<string, Windows.UI.Color>();
            var keywordSet = configurationFile.SelectSingleNode("//Settings//Localizations/lang[@Name='" + chosenLanguage + "']");
            var OperatorsSet = configurationFile.SelectSingleNode("//Settings/operators");
            var symbolsSet = configurationFile.SelectSingleNode("//Settings/symbols");
            foreach (XmlNode keyword in keywordSet)
            {
                var key = keyword.Attributes.GetNamedItem("key").Value.ToLower();
                var map = keyword.Attributes.GetNamedItem("map").Value.ToLower();
                var type = keyword.Attributes.GetNamedItem("type").Value.ToLower();
                if (type == "block")
                {
                    ColoringMap.Add(key, Windows.UI.Colors.MediumVioletRed);
                }
                else if (type=="type")
                {
                    ColoringMap.Add(key, Windows.UI.Colors.Cyan);
                }
                else if(type=="accessory")
                {
                    ColoringMap.Add(key, Windows.UI.Colors.LightYellow);
                }
                else if(type=="function")
                {
                    ColoringMap.Add(key, Windows.UI.Colors.LimeGreen);
                }
                else if (type == "keyword")
                {
                    ColoringMap.Add(key, Windows.UI.Colors.BlueViolet);
                }
                else if (type == "value")
                {
                    ColoringMap.Add(key, Windows.UI.Colors.OrangeRed);
                }
                else
                {
                    ColoringMap.Add(key, Windows.UI.Colors.Wheat);
                }
            }
            return ColoringMap;
        }

        public void UpdateColorMap()
        {
            foreach (var function in Interpreter.Functions.Keys)
            {
                var key = function;
                if (!colorMapValue.ContainsKey(key))
                    colorMapValue.Add(key, Windows.UI.Colors.LimeGreen);
            }
        }

        public String this[string keyword] => this.GetValueOrDefault(keyword, keyword);
    }
}
