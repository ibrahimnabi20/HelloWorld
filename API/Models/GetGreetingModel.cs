namespace API.Models
{
    public class GetGreetingModel
    {
        public class Response
        {
            public string Greeting { get; set; }
            public string Planet { get; set; }
        }
    }
}