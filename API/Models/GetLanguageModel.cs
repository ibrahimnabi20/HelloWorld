namespace API.Models;

public class GetLanguageModel
{
    public class Response
    {
        public string DefaultLanguage { get; set; }
        public string[] Languages { get; set; }
    }
}