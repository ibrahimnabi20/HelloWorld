namespace Messages;

public class LanguageResponse
{
    public string DefaultLanguage { get; } = "en";
    public string[] Languages { get; set; }
}