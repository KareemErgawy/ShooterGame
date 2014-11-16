using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Input;
using Tao.OpenGl;

namespace Shooter
{
    class InnerGameState : IGameObject
    {
        Renderer _renderer = new Renderer();
        Input _input;
        StateSystem _system;
        PersistantGameData _gameData;
        Font _generalFont;
        Level _level;
        System.Drawing.Size clientSize;
        SoundManager _soundManager;

        double _gameTime;
        bool _twoPlayerMode;
        
        TextureManager _textureManager;
        public InnerGameState(StateSystem system, Input input, TextureManager textureManager,
                                PersistantGameData gameData, Font generalFont, System.Drawing.Size clientSize, bool twoPlayerMode, SoundManager soundManager)
        {
            _textureManager = textureManager;
            _input = input;
            _system = system;
            _gameData = gameData;
            _generalFont = generalFont;
            this.clientSize = clientSize;
            _twoPlayerMode = twoPlayerMode;
            _soundManager = soundManager;
            OnGameStart(twoPlayerMode);
        }

        public void OnGameStart(bool twoPlayerMode)
        {

            _level = new Level(_input, _textureManager, _gameData, clientSize, twoPlayerMode, _generalFont, _soundManager);
            _gameTime = _gameData.CurrentLevel.Time;

        }

        #region IGameObject Members

        public void Update(double elapsedTime)
        {
            _level.Update(elapsedTime, _gameTime);
            _gameTime -= elapsedTime;
            bool advanceToNextLevel = false;

            if (_gameTime <= 0)
            {
                if (_gameData.Levels == null || _gameData.Levels.Count == 0)
                {
                    OnGameStart(_twoPlayerMode);
                    _gameData.JustWon = true;
                    _system.ChangeState("game_over");
                }
                else
                {
                    advanceToNextLevel = true;
                }
            }

            if (_level.HavePlayersDied())
            {
                OnGameStart(_twoPlayerMode);
                _gameData.JustWon = false;
                _system.ChangeState("game_over");
            }

            if (advanceToNextLevel)
            {
                _gameData.AdvanceToNextLevel();
                OnGameStart(_twoPlayerMode);
            }

        }

        public void Render()
        {
            Gl.glClearColor(1, 0, 1, 0);
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT);
            _level.Render(_renderer);
            _renderer.Render();
        }

        public void Activated()
        {
        }

        #endregion
    }

}
