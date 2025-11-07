using System;

namespace Editor
{
    /// <summary>
    /// ViewModel für die Darstellung von ValidationIssues in der UI
    /// </summary>
    public class ValidationIssueViewModel
    {
        public IssueSeverity Severity { get; }
        public string Code { get; }
        public string Message { get; }
        public string? Path { get; }

        public string SeverityIcon
        {
            get
            {
                return Severity switch
                {
                    IssueSeverity.Error => "❌",
                    IssueSeverity.Warning => "⚠️",
                    IssueSeverity.Info => "ℹ️",
                    _ => "•"
                };
            }
        }

        public ValidationIssueViewModel(ValidationIssue issue)
        {
            Severity = issue.Severity;
            Code = issue.Code;
            Message = issue.Message;
            Path = issue.Path;
        }

        public override string ToString()
            => $"{SeverityIcon} {Message}" + (Path is null ? "" : $" ({Path})");
    }
}
