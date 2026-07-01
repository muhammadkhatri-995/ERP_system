using System.Text.Json.Serialization;

namespace ERP_sys.Models
{
    public class department
    {
        [JsonPropertyName("departmentId")]
        public int departmentId { get; set; }

        [JsonPropertyName("departmentName")]
        public string departmentName { get; set; } = string.Empty;
    }
}