using System.Text.Json.Serialization;

namespace Zadatak17
{
    public sealed class CocktailResponse
    {
        [JsonPropertyName("drinks")]
        public List<Drink>? Drinks { get; set; }
    }

    public sealed class Drink
    {
        public required string strDrink { get; set; }
        public string? strInstructions { get; set; }
    }
}
