namespace Engine.Content
{
    public interface IContentResolver
    {
        /// Absoluter ContentRoot (z. B. ".../bin/Debug/net8.0/Content")
        string ContentRootAbsolute { get; }

        /// Relativen Content-Pfad in absoluten OS-Pfad auflösen.
        string ResolveContentPath(string relative);

        /// Absoluten Pfad (falls unter ContentRoot) in einen relativen Content-Pfad umwandeln.
        /// Gibt andernfalls den Originalpfad zurück.
        string ToContentRelative(string absolute);
    }
}
