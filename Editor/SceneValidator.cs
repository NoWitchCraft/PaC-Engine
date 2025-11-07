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
            var vr = new ValidationResult();

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
                if (hs.Rect == null)
                {
                    vr.Add(IssueSeverity.Error, "HS_RECT_NULL", "Hotspot Rect fehlt.", $"{basePath}.Rect");
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

            return vr;
        }

        /// <summary>
        /// Versucht automatisch behebbare Probleme zu fixen
        /// </summary>
        public static int AutoFixIssues(SceneDTO scene, ValidationResult validationResult)
        {
            int fixedCount = 0;

            // Fix Hotspot IDs
            for (int i = 0; i < scene.Hotspots.Count; i++)
            {
                var hs = scene.Hotspots[i];
                string basePath = $"Hotspots[{i}]";

                // Fix empty IDs
                if (string.IsNullOrWhiteSpace(hs.Id))
                {
                    hs.Id = GenerateUniqueHotspotId(scene, $"hotspot_{i}");
                    fixedCount++;
                }

                // Fix empty LabelKey
                if (string.IsNullOrWhiteSpace(hs.LabelKey) &&
                    validationResult.Issues.Any(issue => issue.Path == $"{basePath}.LabelKey"))
                {
                    hs.LabelKey = hs.Id;
                    fixedCount++;
                }

                // Fix null Rect
                if (hs.Rect == null)
                {
                    hs.Rect = new Engine.Common.RectF { X = 64, Y = 64, Width = 160, Height = 80 };
                    fixedCount++;
                }
                else
                {
                    // Fix invalid dimensions
                    if (hs.Rect.Width <= 0)
                    {
                        hs.Rect.Width = 160;
                        fixedCount++;
                    }
                    if (hs.Rect.Height <= 0)
                    {
                        hs.Rect.Height = 80;
                        fixedCount++;
                    }
                    // Fix negative positions (set to 0)
                    if (hs.Rect.X < 0)
                    {
                        hs.Rect.X = 0;
                        fixedCount++;
                    }
                    if (hs.Rect.Y < 0)
                    {
                        hs.Rect.Y = 0;
                        fixedCount++;
                    }
                }
            }

            // Fix duplicate IDs
            var seenIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < scene.Hotspots.Count; i++)
            {
                var hs = scene.Hotspots[i];
                if (!seenIds.Add(hs.Id))
                {
                    hs.Id = GenerateUniqueHotspotId(scene, hs.Id);
                    fixedCount++;
                }
            }

            return fixedCount;
        }

        /// <summary>
        /// Generiert eine eindeutige Hotspot-ID
        /// </summary>
        private static string GenerateUniqueHotspotId(SceneDTO scene, string baseId)
        {
            if (!scene.Hotspots.Any(h => h.Id.Equals(baseId, StringComparison.OrdinalIgnoreCase)))
                return baseId;

            int n = 1;
            while (scene.Hotspots.Any(h => h.Id.Equals($"{baseId}_{n}", StringComparison.OrdinalIgnoreCase)))
                n++;
            return $"{baseId}_{n}";
        }
    }
}
