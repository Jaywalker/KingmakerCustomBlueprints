﻿using System;
using System.Linq;
using JetBrains.Annotations;
using Kingmaker.Blueprints;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CustomRaces
{
    public class BlueprintAssetIdConverter : JsonConverter
    {
        [UsedImplicitly]
        public bool Enabled { get; set; }

        private BlueprintAssetIdConverter() { }

        public BlueprintAssetIdConverter(bool enabled)
        {
            Enabled = enabled;
        }

        public override void WriteJson(JsonWriter w, object o, JsonSerializer szr)
        {
            var bp = (BlueprintScriptableObject)o;
            w.WriteValue(string.Format($"Blueprint:{bp.AssetGuid}:{bp.name}"));
        }

        public override object ReadJson(
          JsonReader reader,
          Type objectType,
          object existingValue,
          JsonSerializer serializer
        )
        {
            string text = (string)reader.Value;
            Main.DebugLog($"Reading blueprint: {text}");
            if (text == null || text == "null")
            {
                return null;
            }
            if (text.StartsWith("Blueprint"))
            {
                var parts = text.Split(':');
                BlueprintScriptableObject blueprintScriptableObject = ResourcesLibrary.TryGetBlueprint(parts[1]);
                if (blueprintScriptableObject == null)
                {
                    throw new JsonSerializationException(string.Format("Failed to load blueprint by guid {0}", text));
                }
                return blueprintScriptableObject;
            }
            if (text.StartsWith("File"))
            {
                var parts = text.Split(':');
                return JsonBlueprints.Load<BlueprintScriptableObject>($"customraces/mods/data/{parts[1]}");
            }
            throw new JsonSerializationException(string.Format("Invalid blueprint format {0}", text));
        }

        // ReSharper disable once IdentifierTypo
        private static readonly Type _tBlueprintScriptableObject = typeof(BlueprintScriptableObject);

        public override bool CanConvert(Type type) => Enabled
          && _tBlueprintScriptableObject.IsAssignableFrom(type);
    }
}