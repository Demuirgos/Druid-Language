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
            configurationFile.Load("configurationFile.xml");
            String SettingConfig = configurationFile.SelectSingleNode("//Settings/Current/configuration").Attributes.GetNamedItem("value").Value;
            String chosenLanguage = configurationFile.SelectSingleNode("//Settings/"+ SettingConfig + "/language").Attributes.GetNamedItem("Name").Value;
            var keywordSet = configurationFile.SelectSingleNode("//Settings//Localizations//language[@Name=['" + chosenLanguage + "'] | //Settings/symbols | //Settings/operators");
            foreach (var WordMap in keywordSet)
            {
                var key = keywordSet.Attributes.GetNamedItem("key").Value;
                var map = keywordSet.Attributes.GetNamedItem("map").Value;
                this.Add(key, map);
            } 
        }
        public String this[string keyword] => this.ContainsKey(keyword)?this[keyword]: keyword;
    }
}
