using System;

namespace Engine.Resources
{
    public interface IResourceManager
    {
        // Lädt oder liefert aus Cache. Wirft, wenn kein Loader registriert.
        T Get<T>(string relativePath) where T : class;

        // Versucht zu laden/finden, ohne Exceptions bei Nichtverfügbarkeit.
        bool TryGet<T>(string relativePath, out T? resource) where T : class;

        // Entlädt einen Eintrag (Dispose, wenn IDisposable).
        bool Unload(string relativePath);

        // Alles entladen.
        void UnloadAll();

        // Ist (genau dieser Pfad) geladen?
        bool IsLoaded(string relativePath);

        // Loader für einen Typ registrieren (Argument ist ABSOLUTER OS-Pfad).
        void RegisterLoader<T>(Func<string, T> loader) where T : class;

        // Bequeme Voreinstellungen (string/byte[]/…)
        void RegisterDefaultLoaders();
    }
}
