using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Shooter
{
    class DestroyEnemiesBonus : Bonus
    {
        private EnemyManager _enemyManager;

        public DestroyEnemiesBonus(EnemyManager enemyManager, TextureManager textureManager, Vector position)
        {
            _enemyManager = enemyManager;
            _sprite.SetPosition(position);
            _sprite.Texture = textureManager.Get("destroybonus");
            //_sprite.SetScale(0.2, 0.2);
        }

        public override void Execute()
        {
            _enemyManager.KillNearEnemies();
            Dead = true;
        }
    }
}
