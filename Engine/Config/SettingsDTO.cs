namespace Engine.Config
{
    /// <summary>
    /// Reine Datenklasse für Projekt-/Spiel-Settings,
    /// wird später aus settings.json geladen (Game & Editor).
    /// </summary>
    public class SettingsDTO
    { 
        public string ContentRoot { get; set; } = "Content";
        public string StartSceneId { get; set; } = "";
        public int WindowWidth { get; set; } = 1280;
        public int WindowHeight { get; set; } = 720;
        public string Language { get; set; } = "de-DE";
        public float MasterVolume { get; set; } = 1.0f;
    }
}