using System.Text.Json.Serialization;

namespace FhModulplaner.Services.Client
{
    public class Grade
    {
        [JsonPropertyName("grade")]
        public string Semester { get; set; }
        public int Modified { get; set;}
    }
}
