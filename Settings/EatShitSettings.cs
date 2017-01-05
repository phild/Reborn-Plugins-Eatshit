using System.ComponentModel;
using System.Configuration;
using ff14bot.Helpers;
using Newtonsoft.Json;

namespace EatShit.Settings
{
    public class EatShitSettings : JsonSettings
    {   
        public EatShitSettings() : base(CharacterSettingsDirectory + "/EatShit/EatShit.json")
        {
        }

        [JsonIgnore]
        private static EatShitSettings _instance;
        public static EatShitSettings Instance
        {
            get { return _instance ?? (_instance = new EatShitSettings()); }
        }

        [Setting]
        [DefaultValue("")]

        public string FoodName { get; set; }
    }

}