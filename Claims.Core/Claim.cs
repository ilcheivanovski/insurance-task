using Newtonsoft.Json;

namespace Claims.Core
{
    public class Claim
    {
        public Claim()
        {
            Id = Guid.NewGuid().ToString();
            Created = DateTime.UtcNow;
        }
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string CoverId { get; set; }
        public DateTime Created { get; private set; }
        public string Name { get; set; }
        public ClaimType Type { get; set; }
        public decimal DamageCost { get; set; }
    }

    public enum ClaimType
    {
        Collision = 0,
        Grounding = 1,
        BadWeather = 2,
        Fire = 3
    }
}
