using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Tester.Database;

namespace Tester
{
    public class Program
    {
        public IEnumerable<CourseEntity> m_CourseEntities;
        public IEnumerable<HoleEntity> m_HoleEntities;
        public IEnumerable<ScoreEntity> m_ScoreEntities;
        public IEnumerable<Gamehole> m_GameHoles;
        public IEnumerable<PlayerEntity> m_PlayerEntities;
        public IEnumerable<RoundEntity> m_RoundEntities;
        public IEnumerable<SetEntity> m_SetEntities;

        public Dictionary<string, Course> m_CourseDbMap;
        public Dictionary<string, Player> m_PlayerDbMap;
        public Dictionary<string, Round> m_RoundDbMap;
        public Dictionary<string, Set> m_SetDbMap;
        public Dictionary<string, Hole> m_HoleDbMap;
        public List<HoleSet> m_HoleSetDbList;

        public string PathToSaveFile { get; set; }


        public static void Main(string[] args)
        {
            var program = new Program();

            Console.WriteLine("Enter the path to save file in. Start bundling by press enter." + Environment.NewLine);

            var path = Console.ReadLine();
            if (string.IsNullOrEmpty(path))
            {
                throw new Exception("Can't be empty.");
            }

            program.PathToSaveFile = path;

            Console.WriteLine("Start parsing files ..." + Environment.NewLine);

            program.ReadCoursesFile();
            Console.WriteLine("courses.json parsed ...");

            program.ReadHolesFile();
            Console.WriteLine("holes.json parsed ...");

            program.ReadHoleSetFile();
            Console.WriteLine("holesets.json parsed ...");

            program.ReadPlayersFile();
            Console.WriteLine("players.json parsed ...");

            program.ReadRoundsFile();
            Console.WriteLine("rounds.json parsed ...");

            program.ReadSetFile();
            Console.WriteLine("sets.json parsed ...");

            Console.WriteLine(Environment.NewLine + "Start map up relations ..." + Environment.NewLine);
            program.CanMapUp();

            Console.WriteLine("Will create archive ...");
            program.CreateArchive();
            Console.WriteLine("Archive created! Press any key to exit.");

            Console.ReadLine();
        }

        #region Read content from files

        private void ReadSetFile()
        {
            var fileContent = File.ReadAllText(Path.Combine("files", "sets.json"));
            m_SetEntities = JsonConvert.DeserializeObject<SetEntity[]>(fileContent);
        }

        private void ReadRoundsFile()
        {
            var fileContent = File.ReadAllText(Path.Combine("files", "rounds.json"));
            m_RoundEntities = JsonConvert.DeserializeObject<RoundEntity[]>(fileContent);
        }

        private void ReadPlayersFile()
        {
            var fileContent = File.ReadAllText(Path.Combine("files", "players.json"));
            m_PlayerEntities = JsonConvert.DeserializeObject<PlayerEntity[]>(fileContent);
        }

        private void ReadHolesFile()
        {
            var fileContent = File.ReadAllText(Path.Combine("files", "holes.json"));
            m_HoleEntities = JsonConvert.DeserializeObject<HoleEntity[]>(fileContent);
        }

        private void ReadCoursesFile()
        {
            var fileContent = File.ReadAllText(Path.Combine("files", "courses.json"));
            m_CourseEntities = JsonConvert.DeserializeObject<CourseEntity[]>(fileContent);
        }

        private void ReadHoleSetFile()
        {

            var fileContent = File.ReadAllText(Path.Combine("files", "holesets.json"));
            var serializedContent = JsonConvert.DeserializeObject<HoleSetEntities>(fileContent);
            m_GameHoles = serializedContent.GameHoles;
            m_ScoreEntities = serializedContent.Scores;

            //var holeSets = new List<HoleSet>();

            //foreach (var holeSetScores in serializedContent.Scores.GroupBy(g => g.SetId))
            //{
            //    foreach (var hole in holeSetScores)
            //    {
            //        var gameHole = serializedContent.GameHoles.SingleOrDefault(s => s.Uuid == hole.GameHoleId);

            //        holeSets.Add(new HoleSet()
            //        {
            //            HoleId = gameHole.HoleId,
            //            StrokeOffset = hole.Strokes - gameHole.Par,
            //            Strokes = hole.Strokes,
            //            SetId = hole.SetId,
            //            Par = gameHole.Par
            //        });
            //    }
            //}


            //// Unessecary? Should just save all holesets
            //var sets = holeSets.GroupBy(g => g.SetId);

        }

        #endregion

        #region Mapping functionality

        private void CanMapUp()
        {
            /*
             1. Courses
                - Create in Db to get Id
             2. Players
                - Create in Db to get Id
             3. Holes

             4. Rounds
             5. Set
             6. Holeset
             */

            m_CourseDbMap = new Dictionary<string, Course>();
            m_PlayerDbMap = new Dictionary<string, Player>();
            m_HoleDbMap = new Dictionary<string, Hole>();
            m_RoundDbMap = new Dictionary<string, Round>();
            m_SetDbMap = new Dictionary<string, Set>();
            m_HoleSetDbList = new List<HoleSet>();

            MapCourses();
            MapPlayers();
            MapHoles();
            MapRounds();
            MapSets();
            MapHoleSets();
        }

        private void MapCourses()
        {
            int count = 0;

            foreach (var courseItem in m_CourseEntities)
            {
                var nrOfHoles = m_HoleEntities.Count(w => w.courseUuid == courseItem.uuid);

                var course = new Course()
                {
                    Id = ++count, // Get from DB
                    Name = courseItem.name,
                    NrOfHoles = nrOfHoles
                };

                m_CourseDbMap.Add(courseItem.uuid, course);
            }
        }

        private void MapPlayers()
        {
            int count = 0;

            foreach (var playerItem in m_PlayerEntities)
            {
                var player = new Player()
                {
                    Id = ++count,
                    Alias = playerItem.name.Substring(0, 3),
                    Created = new DateTime(1970, 1, 1).AddMilliseconds(double.Parse(playerItem.created)),
                    Name = playerItem.name
                };

                m_PlayerDbMap.Add(playerItem.uuid, player);
            }
        }

        private void MapHoles()
        {
            int count = 0;

            foreach (var holeItem in m_HoleEntities)
            {
                if (!m_CourseDbMap.TryGetValue(holeItem.courseUuid, out Course course))
                    continue;

                var hole = new Hole()
                {
                    Id = ++count, // Get from DB
                    Number = holeItem.hole,
                    Par = holeItem.par,
                    CourseId = course.Id
                };

                m_HoleDbMap.Add(holeItem.uuid, hole);
            }
        }

        private void MapRounds()
        {
            int count = 0;

            foreach (var roundItem in m_RoundEntities)
            {
                if (!m_CourseDbMap.TryGetValue(roundItem.courseUuid, out Course course))
                    continue;

                var round = new Round()
                {
                    Id = ++count, // Get from DB
                    CourseId = course.Id,
                    Start = new DateTime(1970, 1, 1).AddMilliseconds(double.Parse(roundItem.startedAt)),
                    End = new DateTime(1970, 1, 1).AddMilliseconds(double.Parse(roundItem.endedAt)),
                };

                m_RoundDbMap.Add(roundItem.uuid, round);
            }
        }

        private void MapSets()
        {
            int count = 0;

            foreach (var setItem in m_SetEntities)
            {
                if (!m_RoundDbMap.TryGetValue(setItem.roundId, out Round round))
                    continue;

                if (!m_PlayerDbMap.TryGetValue(setItem.playerId, out Player player))
                    continue;

                var holesWithScore = m_ScoreEntities.Where(s => s.SetId == setItem.uuid);
                var score = holesWithScore.Sum(s => s.Strokes);


                var course = m_CourseDbMap.Values.FirstOrDefault(f => f.Id == round.CourseId);

                if (course == null)
                    continue;

                var holes = m_HoleDbMap.Values.Where(f => f.CourseId == course.Id);

                if (!holes.Any())
                    continue;

                var totalParForSet = holes.Sum(s => s.Par);

                var scoreOffset = score - totalParForSet;

                var set = new Set()
                {
                    Id = ++count, // Get from DB
                    PlayerId = player.Id,
                    RoundId = round.Id,
                    Score = score,
                    ScoreOffset = scoreOffset
                };

                m_SetDbMap.Add(setItem.uuid, set);
            }
        }

        private void MapHoleSets()
        {
            foreach (var scoreEntity in m_ScoreEntities)
            {
                var gameHole = m_GameHoles.FirstOrDefault(f => f.Uuid == scoreEntity.GameHoleId);

                if (!m_RoundDbMap.TryGetValue(gameHole.RoundId, out Round round))
                    continue;

                var course = m_CourseDbMap.Values.FirstOrDefault(f => f.Id == round.CourseId);

                if (course == null)
                    continue;

                var hole = m_HoleDbMap.FirstOrDefault(f => f.Value.CourseId == course.Id && f.Value.Number == gameHole.HoleNr).Value;

                if (hole == null)
                    continue;

                if (!m_SetDbMap.TryGetValue(scoreEntity.SetId, out Set set))
                    continue;

                var strokeOffset = scoreEntity.Strokes - hole.Par;

                var holeSet = new HoleSet()
                {
                    SetId = set.Id,
                    HoleId = hole.Id,
                    Strokes = scoreEntity.Strokes,
                    StrokeOffset = strokeOffset
                };

                m_HoleSetDbList.Add(holeSet);
            }
        }

        #endregion

        private void CreateArchive()
        {
            using var memoryStream = new MemoryStream();
            using var archive = ZipFile.Open(PathToSaveFile, ZipArchiveMode.Create);

            var courses = JsonConvert.SerializeObject(m_CourseDbMap.Select(s => s.Value).Select(s => s));
            var players = JsonConvert.SerializeObject(m_PlayerDbMap.Select(s => s.Value).Select(s => s));
            var rounds = JsonConvert.SerializeObject(m_RoundDbMap.Select(s => s.Value).Select(s => s));
            var sets = JsonConvert.SerializeObject(m_SetDbMap.Select(s => s.Value).Select(s => s));
            var holes = JsonConvert.SerializeObject(m_HoleDbMap.Select(s => s.Value).Select(s => s));
            var holeSets = JsonConvert.SerializeObject(m_HoleSetDbList);

            CreateEntry(archive, courses, "courses.json");
            CreateEntry(archive, players, "players.json");
            CreateEntry(archive, rounds, "rounds.json");
            CreateEntry(archive, sets, "sets.json");
            CreateEntry(archive, holes, "holes.json");
            CreateEntry(archive, holeSets, "holeSets.json");

            memoryStream.Seek(0, SeekOrigin.Begin);
        }

        private void CreateEntry(ZipArchive archive, string content, string fileName)
        {
            var fileEntry = archive.CreateEntry(fileName);
            using var fileStream = fileEntry.Open();
            using var fileToCompressStream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            fileToCompressStream.CopyTo(fileStream);
        }
    }
}
