﻿using Harmony12;
using JetBrains.Annotations;
using Kingmaker.Blueprints;
using Kingmaker.Localization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomRaces
{
    public class LocalizedStringConverter : JsonConverter
    {
        [UsedImplicitly]
        public bool Enabled { get; set; }

        private LocalizedStringConverter() { }

        public LocalizedStringConverter(bool enabled)
        {
            Enabled = enabled;
        }

        public override void WriteJson(JsonWriter w, object o, JsonSerializer szr)
        {
            var ls = (LocalizedString)o;
            for(int i = 0; i < 50 && ls.Shared != null; i++)
            {
                ls = ls.Shared.String;
            }
            int previewLength = 20;
            var text = ls.ToString();
            if (text.Length > previewLength + 1)
            {
                text = text.Remove(previewLength);
            }
            if (ls.ToString().Length > previewLength) text += "...";
            w.WriteValue($"LocalizedString:{ls.Key}:{text}");
        }

        public override object ReadJson(JsonReader reader, Type type, object existing, JsonSerializer serializer)
        {
            string text = (string)reader.Value;
            if (text == null || text == "null")
            {
                return null;
            }
            if(text.StartsWith("LocalizedString") || text.StartsWith("CustomString"))
            {
                var parts = text.Split(':');
                if (parts.Length < 2) return null;
                var localizedString = new LocalizedString();
                Traverse.Create(localizedString).Field("m_Key").SetValue(parts[1]);
                return localizedString;
            }
            return null;
        }

        // ReSharper disable once IdentifierTypo
        private static readonly Type _tBlueprintScriptableObject = typeof(LocalizedString);

        public override bool CanConvert(Type type) => Enabled
          && _tBlueprintScriptableObject.IsAssignableFrom(type);
    }
}