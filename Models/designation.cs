using System.Text.Json.Serialization;

namespace ERP_sys.Models
{
    public class designation
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("designationName")]
        public string designationName { get; set; } = string.Empty;
    }
}