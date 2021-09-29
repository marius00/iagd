namespace StatTranslator {
    public interface ILocalizedLanguage {
        string GetTag(string tag);

        string GetTag(string tag, params object[] args);

        string TranslateName(string prefix, string quality, string style, string name, string suffix);

        bool WarnIfMissing { get; }
    }
}