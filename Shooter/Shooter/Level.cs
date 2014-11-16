using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Input;
using System.Windows.Forms;
using System.Drawing;

namespace Shooter
{
    class Level
    {
        WorldManager _bulletManager;
        Input _input;
        PersistantGameData _gameData;
        PlayerCharacter _playerCharacter;
        bool _twoPlayerMode = false;
        PlayerCharacter _player2;
        TextureManager _textureManager;
        EffectsManager _effectsManager;

        ScrollingBackground _background;
        ScrollingBackground _backgroundLayer;
        int _levelScore;
        Engine.Font _generalFont;
        Size _clientSize;

        // List<Enemy> _enemyList = new List<Enemy>(); <- Removed
        EnemyManager _enemyManager;

        public Level(Input input, TextureManager textureManager, PersistantGameData gameData, Size clientSize, bool twoPlayerMode, Engine.Font scoreFont, SoundManager soundManager)
        {
            _bulletManager = new WorldManager(new RectangleF(-1300 / 2, -750 / 2, 1300, 750));
            _input = input;
            _gameData = gameData;
            _textureManager = textureManager;
            _effectsManager = new EffectsManager(_textureManager);
            List<PlayerCharacter> players = new List<PlayerCharacter>();
            _playerCharacter = new PlayerCharacter(_textureManager, "player_ship", _bulletManager, _effectsManager, soundManager);
            players.Add(_playerCharacter);
            _twoPlayerMode = twoPlayerMode;

            if (_twoPlayerMode)
            {
                _playerCharacter.SetPosition(new Vector(0, 100, 0));
                _player2 = new PlayerCharacter(_textureManager, "player_ship", _bulletManager, new Vector(0, -100, 0), _effectsManager, soundManager);
                players.Add(_player2);
            }

            // -1300 is bad for two reasons
            // 1. It's a magic number in the middle of the code
            // 2. It's based on the size of the form but doesn't directly reference the size of the form
            // this means duplication and two places to edit the code if the form size changes.
            // The form size and the enemy manager play area size should both get their value
            // from one central place.
            _clientSize = clientSize;
            _enemyManager = new EnemyManager(this, _textureManager, _effectsManager, _bulletManager, players, -(clientSize.Width / 2), soundManager, gameData.CurrentLevel.LevelFile);

            _background = new ScrollingBackground(textureManager.Get("background"));
            _background.SetScale(3, 3);
            _background.Speed = 0.15f;

            _backgroundLayer = new ScrollingBackground(textureManager.Get("background_layer_1"));
            _backgroundLayer.Speed = 0.1f;
            _backgroundLayer.SetScale(3.0, 3.0);

            _generalFont = scoreFont;
        }

        private void UpdateCollisions()
        {

            UpdatePlayerCollisions(_playerCharacter);

            if (_twoPlayerMode)
            {
                UpdatePlayerCollisions(_player2);
            }

            //_bulletManager.UpdatePlayerCollision(_playerCharacter);
            foreach (Enemy enemy in _enemyManager.EnemyList)
            {
                //if (enemy.GetBoundingBox().IntersectsWith(_playerCharacter.GetBoundingBox()))
                //{
                //    enemy.OnCollision(_playerCharacter);
                //    _playerCharacter.OnCollision(enemy);
                //}

                _bulletManager.UpdateEnemyCollisions(enemy);

                if (enemy.IsDead)
                {
                    _levelScore += 10;
                    Console.WriteLine("Score: {0}", _levelScore);
                }

            }

        }

        private void UpdatePlayerCollisions(PlayerCharacter player)
        {
            if (!player.IsDead)
            {
                _bulletManager.UpdatePlayerCollision(player);
                foreach (Enemy enemy in _enemyManager.EnemyList)
                {
                    if (enemy.GetBoundingBox().IntersectsWith(player.GetBoundingBox()))
                    {
                        enemy.OnCollision(player);
                        player.OnCollision(enemy);
                    }

                    //_bulletManager.UpdateEnemyCollisions(enemy);
                }
            }
        }

        private void handlePlayerInput(PlayerCharacter player, Keys fireKey, Keys leftKey, Keys rightKey, Keys upKey, Keys downKey, double elapsedTime)
        {
            if (_input.Keyboard.IsKeyPressed(fireKey))
            {
                player.Fire();
            }

            Vector controlInput = new Vector();

            if (_input.Keyboard.IsKeyHeld(leftKey))
            {
                controlInput.X = -1;
            }

            if (_input.Keyboard.IsKeyHeld(rightKey))
            {
                controlInput.X = 1;
            }

            if (_input.Keyboard.IsKeyHeld(upKey))
            {
                controlInput.Y = 1;
            }

            if (_input.Keyboard.IsKeyHeld(downKey))
            {
                controlInput.Y = -1;
            }


            player.Move(controlInput * elapsedTime);
        }

        private void UpdateInput(double elapsedTime)
        {
            handlePlayerInput(_playerCharacter, Keys.Space, Keys.Left, Keys.Right, Keys.Up, Keys.Down, elapsedTime);

            if (_twoPlayerMode)
            {
                handlePlayerInput(_player2, Keys.V, Keys.A, Keys.D, Keys.W, Keys.S, elapsedTime);
            }

            //if (_input.Keyboard.IsKeyPressed(Keys.NumPad0) || (_input.UsingController && _input.Controller.ButtonA.Pressed))
            //{
            //    _playerCharacter.Fire();
            //}
            // Get controls and apply to player character
            //double _x = 0;
            //double _y = 0;

            //if (_input.UsingController)
            //{
            //    _x = _input.Controller.LeftControlStick.X;
            //    _y = _input.Controller.LeftControlStick.Y * -1;
            //}

            //Vector controlInput = new Vector(_x, _y, 0);

            //if (Math.Abs(controlInput.Length()) < 0.0001)
            //{
            //    // If the input is very small, then the player may not be using 
            //    // a controller;, they might be using the keyboard.
            //    if (_input.Keyboard.IsKeyHeld(Keys.Left))
            //    {
            //        controlInput.X = -1;
            //    }

            //    if (_input.Keyboard.IsKeyHeld(Keys.Right))
            //    {
            //        controlInput.X = 1;
            //    }

            //    if (_input.Keyboard.IsKeyHeld(Keys.Up))
            //    {
            //        controlInput.Y = 1;
            //    }

            //    if (_input.Keyboard.IsKeyHeld(Keys.Down))
            //    {
            //        controlInput.Y = -1;
            //    }
            //}

            //_playerCharacter.Move(controlInput * elapsedTime);
        }

        public void Update(double elapsedTime, double gameTime)
        {
            _effectsManager.Update(elapsedTime);
            _playerCharacter.Update(elapsedTime);

            if (_twoPlayerMode)
            {
                _player2.Update(elapsedTime);
            }

            _background.Update((float)elapsedTime);
            _backgroundLayer.Update((float)elapsedTime);

            UpdateCollisions();
            //_enemyList.ForEach(x => x.Update(elapsedTime));
            _enemyManager.Update(elapsedTime, gameTime);
            _bulletManager.Update(elapsedTime);

            UpdateInput(elapsedTime);
        }

        public void Render(Renderer renderer)
        {
            _background.Render(renderer);
            _backgroundLayer.Render(renderer);

            // _enemyList.ForEach(x => x.Render(renderer));
            _enemyManager.Render(renderer);
            _playerCharacter.Render(renderer);

            if (_twoPlayerMode)
            {
                _player2.Render(renderer);
            }

            _bulletManager.Render(renderer);
            renderer.Render();
            _effectsManager.Render(renderer);
            renderer.Render();

            RenderTexts(renderer);
        }

        private void RenderTexts(Renderer renderer)
        {
            Text levelText = new Text(_gameData.CurrentLevel.LevelName, _generalFont);
            levelText.SetColor(new Engine.Color(1, 1, 1, 1));
            levelText.SetPosition(-(levelText.Width / 2), _clientSize.Height / 2);
            renderer.DrawText(levelText);

            Text scoreText = new Text("Score: " + _levelScore, _generalFont);
            scoreText.SetColor(new Engine.Color(1, 1, 1, 1));
            scoreText.SetPosition((_clientSize.Width / 2) - scoreText.Width - 10, _clientSize.Height / 2);
            renderer.DrawText(scoreText);
        }

        internal bool HavePlayersDied()
        {
            if (_twoPlayerMode)
            {
                return _playerCharacter.IsDead && _player2.IsDead;
            }

            return _playerCharacter.IsDead;
        }

        public void AddToScore(int value)
        {
            _levelScore += value;
        }
    }
}
