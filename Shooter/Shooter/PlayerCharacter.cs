using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Shooter
{

    public class PlayerCharacter : Entity
    {
        bool _dead = false;
        WorldManager _bulletManager;
        Texture _bulletTexture;
        EffectsManager _effectManager;
        SoundManager _soundManager;

        public bool IsDead
        {
            get
            {
                return _dead;
            }
        }

        double _speed = 512; // pixels per second

        public void Move(Vector amount)
        {
            amount *= _speed;
            _sprite.SetPosition(_sprite.GetPosition() + amount);
        }

        public PlayerCharacter(TextureManager textureManager, string textureId, WorldManager bulletManager, EffectsManager effectManager, SoundManager soundManager)
        {
            _bulletManager = bulletManager;
            _bulletTexture = textureManager.Get("bullet");

            _sprite.Texture = textureManager.Get(textureId);
            _sprite.SetScale(0.5, 0.5); // spaceship is quite big, scale it down.
            _effectManager = effectManager;
            _soundManager = soundManager;
        }

        public PlayerCharacter(TextureManager textureManager, string textureId, WorldManager bulletManager, Vector initialPosition, EffectsManager effectManager, SoundManager soundManager)
            : this(textureManager, textureId, bulletManager, effectManager, soundManager)
        {
            _sprite.SetPosition(initialPosition);
        }

        public void Render(Renderer renderer)
        {
            // Render_Debug();

            if (!_dead)
                renderer.DrawSprite(_sprite);
        }

        Vector _gunOffset = new Vector(55, 0, 0);
        static readonly double FireRecovery = 0.25;
        double _fireRecoveryTime = FireRecovery;
        public void Update(double elapsedTime)
        {
            if (!_dead)
                _fireRecoveryTime = Math.Max(0, (_fireRecoveryTime - elapsedTime));
        }

        public void Fire()
        {
            if (_fireRecoveryTime > 0)
            {
                return;
            }
            else
            {
                _fireRecoveryTime = FireRecovery;
            }

            Bullet bullet = new Bullet(_bulletTexture);
            bullet.SetColor(new Color(0, 1, 0, 1));
            bullet.SetPosition(_sprite.GetPosition() + _gunOffset);
            _bulletManager.Shoot(bullet);
            _soundManager.PlaySound("shoot");
        }

        internal void OnCollision(Enemy enemy)
        {
            killPlayer();
        }

        internal void OnCollision(Bullet bullet)
        {
            killPlayer();
        }

        private void killPlayer()
        {
            _effectManager.AddExplosion(_sprite.GetPosition());
            _soundManager.PlaySound("explosion");
            _dead = true;
        }

        internal void SetPosition(Vector position)
        {
            _sprite.SetPosition(position);
        }

        internal Vector GetPosition()
        {
            return _sprite.GetPosition();
        }
    }
}

