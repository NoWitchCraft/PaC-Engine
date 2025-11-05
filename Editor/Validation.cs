using System;
using System.Collections.Generic;

namespace Editor 
{
    public enum IssueSeverity { Info, Warning, Error }

    public sealed class ValidationIssue
    {
        public IssueSeverity Severity { get; }
        public string Code { get; } //z.b. "SCENE_ID_EMPTY"
        public string Message { get; } // Menschlich lesbar
        public string? Path { get; } // z.B. "Scene.Id oder "Hotspots[2].Rect.Width

        public ValidationIssue(IssueSeverity severity, string code, string message, string? path = null)
        {
            Severity = severity;
            Code = code;
            Message = message;
            Path = path;
        }

        public override string ToString()
            => $"{Severity}: {Message} + (Path is null ? "" : $" ({Path})");
    }

    public sealed class ValidationResult
    {
        public List<ValidationIssue> Issues { get; } = new();
        public bool HasErrors => Issues.Exists (i => i.Severity == IssueSeverity.Error);
        public bool HasWarnings => Issues.Exists (i => i.Severity == IssueSeverity.Warning);
        public void Add(IssueSeverity s, string code, string msg, string? path = null)
            => Issues.Add(new ValidationIssue(s, code, msg, path));
    }
}