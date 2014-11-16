using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Shooter
{
    class ScoreBonus : Bonus
    {
        private Level _level;

        public ScoreBonus(Level ownerLevel, TextureManager textureManager, Vector position)
        {
            _level = ownerLevel;
            _sprite.SetPosition(position);
            _sprite.Texture = textureManager.Get("scoregift");
            _sprite.SetScale(0.8, 0.8);
        }

        public override void Execute()
        {
            _level.AddToScore(50);
            Dead = true;
        }
    }
}
