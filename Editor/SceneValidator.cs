using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Data.Scene;
using Engine.Diagnostics;

namespace Editor
{
    public static class SceneValidator
    {
        public static ValidationResult Validate(SceneDTO scene)
        {
            Log.Debug(nameof(SceneValidator), $"Validating scene: {scene?.Id ?? "<null>"}");
            
            var vr = new ValidationResult();

            if (scene == null)
            {
                vr.Add(IssueSeverity.Error, "SCENE_NULL", "Scene object is null", "Scene");
                return vr;
            }

            // Scene-Basics
            if (string.IsNullOrWhiteSpace(scene.Id))
                vr.Add(IssueSeverity.Error, "SCENE_ID_EMPTY", "Scene Id cant be empty", "Scene.Id");

            if (string.IsNullOrWhiteSpace(scene.BackgroundPath))
                vr.Add(IssueSeverity.Error, "BG_MISSING", "Background path cant be empty", "Scene.BackgroundPath");

            //Hotspots
            var ids = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < scene.Hotspots.Count; i++)
            {
                var hs = scene.Hotspots[i];
                string basePath = $"Hotspots[{i}]";

                //Id
                if (string.IsNullOrWhiteSpace(hs.Id))
                    vr.Add(IssueSeverity.Error, "HS_ID_EMPTY", "Hotspot Id cant be empty", $"{basePath}.Id");
                else
                {
                    if (!ids.Add(hs.Id))
                        vr.Add(IssueSeverity.Error, "HS_ID_DUP", $"Hotspot-Id doppelt: '{hs.Id}'.", $"{basePath}.Id");
                }

                // Rect
                if (hs.Rect.IsEmpty)
                {
                    vr.Add(IssueSeverity.Warning, "HS_RECT_EMPTY", "Hotspot Rect ist leer (Width oder Height ist 0).", $"{basePath}.Rect");
                }
                else
                {
                    if (hs.Rect.Width <= 0)
                        vr.Add(IssueSeverity.Error, "HS_W_LEQ_0", "Rect.Width muss > 0 sein.", $"{basePath}.Rect.Width");
                    if (hs.Rect.Height <= 0)
                        vr.Add(IssueSeverity.Error, "HS_H_LEQ_0", "Rect.Height muss > 0 sein.", $"{basePath}.Rect.Height");
                    if (hs.Rect.X < 0 || hs.Rect.Y < 0)
                        vr.Add(IssueSeverity.Warning, "HS_POS_NEG", "Rect.X/Y sind negativ – prüfen, ob gewünscht.", $"{basePath}.Rect.(X|Y)");
                }

                // LabelKey Hinweis (optional)
                if (string.IsNullOrWhiteSpace(hs.LabelKey))
                    vr.Add(IssueSeverity.Warning, "HS_LABEL_EMPTY", "LabelKey ist leer (nur Hinweis).", $"{basePath}.LabelKey");

            }

            if (vr.HasErrors)
                Log.Warn(nameof(SceneValidator), $"Scene validation completed with {vr.Issues.Count(i => i.Severity == IssueSeverity.Error)} error(s) and {vr.Issues.Count(i => i.Severity == IssueSeverity.Warning)} warning(s)");
            else if (vr.HasWarnings)
                Log.Info(nameof(SceneValidator), $"Scene validation completed with {vr.Issues.Count(i => i.Severity == IssueSeverity.Warning)} warning(s)");
            else
                Log.Info(nameof(SceneValidator), "Scene validation completed successfully");

            return vr;
        }
    }
}