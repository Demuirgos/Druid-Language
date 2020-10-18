using System;
using System.Collections.Generic;
using System.Xml;
using System.Text;

namespace YetAnotherScriptingLanguage
{
    class KeyWords : Dictionary<String, String>
    {
        public KeyWords() : base()
        {
            XmlDocument configurationFile = new XmlDocument();
            configurationFile.Load("configurations.xml");
            String SettingConfig = configurationFile.SelectSingleNode("//Settings/Current/configuration").Attributes.GetNamedItem("value").Value;
            String chosenLanguage = configurationFile.SelectSingleNode("//Settings/"+ SettingConfig + "/language").Attributes.GetNamedItem("Name").Value;
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
        }
        public String this[string keyword] => this.GetValueOrDefault(keyword, keyword);
    }
}
