using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shooter
{
    class PersistantGameData
    {
        public bool JustWon { get; set; }
        public List<LevelDescription> Levels { get; set; }
        public LevelDescription CurrentLevel { get; set; }
        public PersistantGameData()
        {
            JustWon = false;
        }

        public void AdvanceToNextLevel()
        {
            CurrentLevel = Levels[0];
            Levels.RemoveAt(0);
        }
    }

}
