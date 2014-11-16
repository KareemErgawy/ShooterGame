using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Shooter
{
    public abstract class Bonus : Entity
    {
        protected double _speed = 64;
        protected Vector _direction = new Vector(-1, -1, 0);
        public bool Dead { set; get; }

        public abstract void Execute();

        public void Update(double elapsedTime)
        {
            Vector moveAmount = _direction * (elapsedTime * _speed);
            _sprite.SetPosition(_sprite.GetPosition() + moveAmount);
        }

        public void Render(Renderer renderer)
        {
            if (!Dead)
            {
                renderer.DrawSprite(_sprite);
            }
        }
    }
}
