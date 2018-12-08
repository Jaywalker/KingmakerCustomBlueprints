﻿using Kingmaker.Blueprints;
using Kingmaker.Blueprints.CharGen;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Blueprints.Items.Weapons;
using Kingmaker.View;
using Kingmaker.Visual.CharacterSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomRaces
{
    class AssetsDump
    {
        public static void DumpBlueprints()
        {
            var seen = new HashSet<Type>();
            
            var blueprints = ResourcesLibrary.GetBlueprints<BlueprintScriptableObject>();
            foreach (var blueprint in blueprints)
            {
                if (!seen.Contains(blueprint.GetType()))
                {
                    seen.Add(blueprint.GetType());
                    JsonBlueprints.Dump(blueprint);
                }
            }
        }
        public static void DumpQuick()
        {
            var types = new Type[]
            {
                            typeof(BlueprintCharacterClass),
                            typeof(BlueprintRaceVisualPreset),
                            typeof(BlueprintRace),
                            typeof(BlueprintArchetype),
                            typeof(BlueprintProgression),
                            typeof(BlueprintStatProgression),
                            typeof(BlueprintFeature),
                            typeof(BlueprintFeatureSelection),
                            typeof(BlueprintSpellbook),
                            typeof(BlueprintSpellList),
                            typeof(BlueprintSpellsTable),
                            typeof(BlueprintItemWeapon),
            };
            foreach (var type in types)
            {
                string assetId;
                RaceUtil.FallbackTable.TryGetValue(type, out assetId);
                if (assetId == null)
                {
                    Main.DebugLog($"No Default {type}");
                    continue;
                }
                JsonBlueprints.Dump(ResourcesLibrary.TryGetBlueprint<BlueprintScriptableObject>(assetId));
            }
        }
        public static void DumpAllBlueprints()
        {
            var blueprints = ResourcesLibrary.GetBlueprints<BlueprintScriptableObject>();
            foreach (var blueprint in blueprints)
            {
                JsonBlueprints.Dump(blueprint);
            }
        }
        public static void DumpEquipmentEntities()
        {
            foreach (var kv in ResourcesLibrary.LibraryObject.ResourcePathsByAssetId)
            {
                var resource = ResourcesLibrary.TryGetResource<EquipmentEntity>(kv.Key);
                if (resource == null) continue;
                JsonBlueprints.Dump(resource, kv.Key);
            }
        }
        public static void DumpUnitViews()
        {
            foreach (var kv in ResourcesLibrary.LibraryObject.ResourcePathsByAssetId)
            {
                var resource = ResourcesLibrary.TryGetResource<UnitEntityView>(kv.Key);
                if (resource == null) continue;
                JsonBlueprints.Dump(resource, kv.Key);
            }
        }
        public static void DumpList()
        {
            Directory.CreateDirectory($"Blueprints/");
            var blueprints = ResourcesLibrary.GetBlueprints<BlueprintScriptableObject>().ToList();
            var blueprintsByAssetID = ResourcesLibrary.LibraryObject.BlueprintsByAssetId;
            Main.DebugLog($"BlueprintsByAssetId contains  {blueprintsByAssetID.Count} blueprints");
            Main.DebugLog($"Dumping {blueprints.Count} blueprints");
            using (var file = new StreamWriter("Blueprints/Blueprints.txt"))
            {
                foreach (var blueprint in blueprints)
                {
                    file.WriteLine($"{blueprint.name}\t{blueprint.AssetGuid}\t{blueprint.GetType()}\t");
                }
            }
            var resourcePathsByAssetId = ResourcesLibrary.LibraryObject.ResourcePathsByAssetId;
            Main.DebugLog($"ResourcePathsByAssetId contains  {blueprintsByAssetID.Count} resources");
            using (var file = new StreamWriter("Blueprints/Resources.txt"))
            {
                foreach (var kv in ResourcesLibrary.LibraryObject.ResourcePathsByAssetId)
                {
                    var resource = ResourcesLibrary.TryGetResource<UnityEngine.Object>(kv.Key);
                    file.WriteLine($"{resource?.name ?? "NULL"}\t{kv.Key}\t{resource?.GetType()?.Name ?? "NULL"}\t{kv.Value}");
                }
            }
        }
    }
}
