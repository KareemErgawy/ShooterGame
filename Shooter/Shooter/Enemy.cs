using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using System.Drawing;
using Tao.OpenGl;

namespace Shooter
{
    public class Enemy : Entity
    {
        static readonly double HitFlashTime = 0.25;
        double _scale = 0.3;
        public int Health { get; set; }
        double _hitFlashCountDown = 0;
        WorldManager _bulletManager;
        Texture _bulletTexture;
        EffectsManager _effectsManager;
        public Path Path { get; set; }

        public double MaxTimeToShoot { get; set; }
        public double MinTimeToShoot { get; set; }
        Random _random = new Random();
        double _shootCountDown;
        Random random = new Random();
        List<PlayerCharacter> _players;
        SoundManager _soundManager;
        TextureManager _textureManager;
        Level _level;
        EnemyManager _enemyManager;

        public void RestartShootCountDown()
        {
            _shootCountDown = MinTimeToShoot + (_random.NextDouble() * MaxTimeToShoot);
        }

        public bool IsDead
        {
            get { return Health == 0; }
        }

        internal Enemy(Level level, EnemyManager enemyManager, TextureManager textureManager, EffectsManager effectsManager, WorldManager bulletManager, List<PlayerCharacter> players, SoundManager soundManager)
        {
            _level = level;
            _textureManager = textureManager;
            _enemyManager = enemyManager;
            _players = players;
            _bulletManager = bulletManager;
            _bulletTexture = textureManager.Get("bullet");
            MaxTimeToShoot = 12;
            MinTimeToShoot = 1;
            RestartShootCountDown();

            _effectsManager = effectsManager;
            Health = 50; // default health value.
            _sprite.Texture = textureManager.Get("enemy_ship");
            _sprite.SetScale(_scale, _scale);
            _sprite.SetRotation(Math.PI); // make it face the player
            _sprite.SetPosition(200, 0); // put it somewhere easy to see
            _soundManager = soundManager;
        }

        private int getRandomPlayerIndex()
        {
            int index = random.Next(10, 10000) % _players.Count;
            //System.Console.WriteLine(index);
            return index;
        }

        public void Update(double elapsedTime)
        {
            _shootCountDown = _shootCountDown - elapsedTime;
            if (_shootCountDown <= 0)
            {
                Bullet bullet = new Bullet(_bulletTexture);
                Vector currentPosition = _sprite.GetPosition();
                int playerIndex = getRandomPlayerIndex();
                Vector bulletDir = _players[playerIndex].GetPosition() - currentPosition;
                bulletDir = Vector.Normalize(bulletDir);
                bullet.Direction = bulletDir;


                bullet.Speed = 350;
                //bullet.Direction = new Vector(-1, 0, 0);
                bullet.SetPosition(_sprite.GetPosition());
                bullet.SetColor(new Engine.Color(1, 0, 0, 1));
                _bulletManager.EnemyShoot(bullet);
                _soundManager.PlaySound("enemyshoot");
                RestartShootCountDown();
            }

            if (Path != null)
            {
                Path.UpdatePosition(elapsedTime, this);
            }
            if (_hitFlashCountDown != 0)
            {
                _hitFlashCountDown = Math.Max(0, _hitFlashCountDown - elapsedTime);
                double scaledTime = 1 - (_hitFlashCountDown / HitFlashTime);
                _sprite.SetColor(new Engine.Color(1, 1, (float)scaledTime, 1));
            }

        }

        public void Render(Renderer renderer)
        {
            renderer.DrawSprite(_sprite);
            // Render_Debug();
        }

        internal void OnCollision(PlayerCharacter player)
        {
            // Handle collision with player.
        }


        internal void OnCollision(Bullet bullet)
        {
            // If the ship is already dead then ignore any more bullets.
            if (Health == 0)
            {
                return;
            }

            Health = Math.Max(0, Health - 25);
            _hitFlashCountDown = HitFlashTime; // half
            _sprite.SetColor(new Engine.Color(1, 1, 0, 1));

            if (Health == 0)
            {
                OnDestroyed();
                int bonus = _random.Next(10, 10000) % 3;
                if (bonus == 1)
                {
                    Console.WriteLine("Opps no bonus will be thrown");
                }
                else if (bonus == 2)
                {
                    Console.WriteLine("Hahah! You will destroy all enemies");
                    _bulletManager.ThrowBonus(_enemyManager, _textureManager, _sprite.GetPosition());
                }
                else if (bonus == 0)
                {
                    Console.WriteLine("Congrats! Free Score");
                    _bulletManager.ThrowBonus(_level, _textureManager, _sprite.GetPosition());
                }
            }

        }

        public void OnDestroyed()
        {
            _effectsManager.AddExplosion(_sprite.GetPosition());
            _soundManager.PlaySound("explosion");
        }

        internal void SetPosition(Vector position)
        {
            _sprite.SetPosition(position);
        }
    }

}
