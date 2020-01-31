using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tester
{
    class Entitites
    {
    }
    public class HoleSetEntities
    {
        [JsonProperty("gameHoles")]
        public Gamehole[] GameHoles { get; set; }
        [JsonProperty("scores")]
        public ScoreEntity[] Scores { get; set; }
    }

    public class Gamehole
    {
        [JsonProperty("roundId")]
        public string RoundId { get; set; }

        [JsonProperty("holeNr")]
        public int HoleNr { get; set; }

        [JsonProperty("holeId")]
        public string HoleId { get; set; }

        [JsonProperty("par")]
        public int Par { get; set; }

        [JsonProperty("uuid")]
        public string Uuid { get; set; }
    }

    public class ScoreEntity
    {
        [JsonProperty("gameHoleUuid")]
        public string GameHoleId { get; set; }
        [JsonProperty("setId")]
        public string SetId { get; set; }
        [JsonProperty("roundId")]
        public string RoundId { get; set; }
        [JsonProperty("holeId")]
        public string HoleId { get; set; }

        [JsonProperty("score")]
        public int Strokes { get; set; }
        [JsonProperty("uuid")]
        public string Uuid { get; set; }
    }

    public class CourseEntity
    {
        public string name { get; set; }
        public string uuid { get; set; }
    }

    public class PlayerEntity
    {
        public string created { get; set; }

        public string id { get; set; }
        public string name { get; set; }
        public int owner { get; set; }
        public string profileImageFilename { get; set; }
        public string updatedAt { get; set; }
        public string uuid { get; set; }
    }

    public class HoleEntity
    {

        public string courseUuid { get; set; }

        public string createdAt { get; set; }

        public int hole { get; set; }

        public string id { get; set; }

        public int par { get; set; }

        public string uuid { get; set; }

        public string updatedAt { get; set; }
    }

    public class RoundEntity
    {
        public string courseUuid { get; set; }
        public string createdAt { get; set; }
        public string endedAt { get; set; }
        public string id { get; set; }
        public string startedAt { get; set; }
        public string updatedAt { get; set; }
        public string uuid { get; set; }
    }

    public class SetEntity
    {
        public string createdAt { get; set; }
        public string roundId { get; set; }
        public string id { get; set; }
        public string playerId { get; set; }
        public string uuid { get; set; }
    }

}
