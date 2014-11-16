using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using System.IO;

namespace Shooter
{

    class EnemyManager
    {
        List<Enemy> _enemies = new List<Enemy>();
        TextureManager _textureManager;
        EffectsManager _effectsManager;
        int _leftBound;
        List<EnemyDef> _upComingEnemies = new List<EnemyDef>();
        WorldManager _bulletManager;
        List<PlayerCharacter> _players;
        SoundManager _soundManager;
        Level _level;

        public List<Enemy> EnemyList
        {
            get
            {
                return _enemies;
            }
        }

        private void initializeEnemies(string levelFile)
        {
            string[] enemyLines = File.ReadAllLines(levelFile);

            foreach (string line in enemyLines)
            {
                string enemyType = line.Split(',')[0].Trim();
                double launchTime = double.Parse(line.Split(',')[1].Trim());
                //Console.WriteLine("Adding Enemy: " + enemyType + ", " + launchTime);
                _upComingEnemies.Add(new EnemyDef(enemyType, launchTime));
            }
        }

        public EnemyManager(Level level, TextureManager textureManager, EffectsManager effectsManager, WorldManager bulletManager, List<PlayerCharacter> players, int leftBound, SoundManager soundManager, string levelFile)
        {
            _level = level;
            _players = players;
            _bulletManager = bulletManager;
            _textureManager = textureManager;
            _effectsManager = effectsManager;
            _soundManager = soundManager;
            _leftBound = leftBound;
            initializeEnemies(levelFile);


            //_upComingEnemies.Add(new EnemyDef("cannon_fodder", 30));
            //_upComingEnemies.Add(new EnemyDef("cannon_fodder", 29.5));
            //_upComingEnemies.Add(new EnemyDef("cannon_fodder", 29));
            //_upComingEnemies.Add(new EnemyDef("cannon_fodder", 28.5));

            //_upComingEnemies.Add(new EnemyDef("cannon_fodder_low", 30));
            //_upComingEnemies.Add(new EnemyDef("cannon_fodder_low", 29.5));
            //_upComingEnemies.Add(new EnemyDef("cannon_fodder_low", 29));
            //_upComingEnemies.Add(new EnemyDef("cannon_fodder_low", 28.5));

            //_upComingEnemies.Add(new EnemyDef("cannon_fodder", 25));
            //_upComingEnemies.Add(new EnemyDef("cannon_fodder", 24.5));
            //_upComingEnemies.Add(new EnemyDef("cannon_fodder", 24));
            //_upComingEnemies.Add(new EnemyDef("cannon_fodder", 23.5));

            //_upComingEnemies.Add(new EnemyDef("cannon_fodder_low", 20));
            //_upComingEnemies.Add(new EnemyDef("cannon_fodder_low", 19.5));
            //_upComingEnemies.Add(new EnemyDef("cannon_fodder_low", 19));
            //_upComingEnemies.Add(new EnemyDef("cannon_fodder_low", 18.5));

            //_upComingEnemies.Add(new EnemyDef("cannon_fodder_straight", 16));
            //_upComingEnemies.Add(new EnemyDef("cannon_fodder_straight", 15.8));
            //_upComingEnemies.Add(new EnemyDef("cannon_fodder_straight", 15.6));
            //_upComingEnemies.Add(new EnemyDef("cannon_fodder_straight", 15.4));


            //_upComingEnemies.Add(new EnemyDef("up_l", 10));
            //_upComingEnemies.Add(new EnemyDef("down_l", 9));
            //_upComingEnemies.Add(new EnemyDef("up_l", 8));
            //_upComingEnemies.Add(new EnemyDef("down_l", 7));
            //_upComingEnemies.Add(new EnemyDef("up_l", 6));

            //foreach (EnemyDef def in _upComingEnemies)
            //{
            //    Console.WriteLine(def.EnemyType + ", " + def.LaunchTime);
            //}

            // Sort enemies so the greater launch time appears first.
            _upComingEnemies.Sort(delegate(EnemyDef firstEnemy, EnemyDef secondEnemy)
            {
                return firstEnemy.LaunchTime.CompareTo(secondEnemy.LaunchTime);

            });

        }

        private void UpdateEnemySpawns(double gameTime)
        {
            // If no upcoming enemies then there's nothing to spawn.
            if (_upComingEnemies.Count == 0)
            {
                return;
            }

            EnemyDef lastElement = _upComingEnemies[_upComingEnemies.Count - 1];
            if (gameTime < lastElement.LaunchTime)
            {
                _upComingEnemies.RemoveAt(_upComingEnemies.Count - 1);
                _enemies.Add(CreateEnemyFromDef(lastElement));
            }
        }

        private Enemy CreateEnemyFromDef(EnemyDef definition)
        {
            Enemy enemy = new Enemy(_level, this, _textureManager, _effectsManager, _bulletManager, _players, _soundManager);

            if (definition.EnemyType == "cannon_fodder")
            {
                List<Vector> _pathPoints = new List<Vector>();
                _pathPoints.Add(new Vector(1400, 0, 0));
                _pathPoints.Add(new Vector(0, 250, 0));
                _pathPoints.Add(new Vector(-1400, 0, 0));

                enemy.Path = new Path(_pathPoints, 10);
            }
            else if (definition.EnemyType == "cannon_fodder_low")
            {
                List<Vector> _pathPoints = new List<Vector>();
                _pathPoints.Add(new Vector(1400, 0, 0));
                _pathPoints.Add(new Vector(0, -250, 0));
                _pathPoints.Add(new Vector(-1400, 0, 0));

                enemy.Path = new Path(_pathPoints, 10);
            }
            else if (definition.EnemyType == "cannon_fodder_straight")
            {
                List<Vector> _pathPoints = new List<Vector>();
                _pathPoints.Add(new Vector(1400, 0, 0));
                _pathPoints.Add(new Vector(-1400, 0, 0));

                enemy.Path = new Path(_pathPoints, 14);
            }
            else if (definition.EnemyType == "up_l")
            {
                List<Vector> _pathPoints = new List<Vector>();
                _pathPoints.Add(new Vector(500, -375, 0));
                _pathPoints.Add(new Vector(500, 0, 0));
                _pathPoints.Add(new Vector(500, 0, 0));
                _pathPoints.Add(new Vector(-1400, 200, 0));

                enemy.Path = new Path(_pathPoints, 10);
            }
            else if (definition.EnemyType == "down_l")
            {
                List<Vector> _pathPoints = new List<Vector>();
                _pathPoints.Add(new Vector(500, 375, 0));
                _pathPoints.Add(new Vector(500, 0, 0));
                _pathPoints.Add(new Vector(500, 0, 0));
                _pathPoints.Add(new Vector(-1400, -200, 0));

                enemy.Path = new Path(_pathPoints, 10);
            }
            else
            {
                System.Diagnostics.Debug.Assert(false, "Unknown enemy type.");
            }

            return enemy;
        }

        public void Update(double elapsedTime, double gameTime)
        {
            UpdateEnemySpawns(gameTime);
            _enemies.ForEach(x => x.Update(elapsedTime));
            CheckForOutOfBounds();
            RemoveDeadEnemies();
        }

        public void KillNearEnemies()
        {
            foreach (Enemy enemy in _enemies)
            {
                //if (enemy.GetBoundingBox().Left < 0)
                //{
                enemy.Health = 0;
                enemy.OnDestroyed();
                //}
            }
        }

        private void CheckForOutOfBounds()
        {
            foreach (Enemy enemy in _enemies)
            {
                if (enemy.GetBoundingBox().Right < _leftBound)
                {
                    enemy.Health = 0; // kill the enemy off
                }
            }
        }

        public void Render(Renderer renderer)
        {
            _enemies.ForEach(x => x.Render(renderer));
        }

        private void RemoveDeadEnemies()
        {
            for (int i = _enemies.Count - 1; i >= 0; i--)
            {
                if (_enemies[i].IsDead)
                {
                    _enemies.RemoveAt(i);
                }
            }
        }
    }

}
