using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Tao.OpenGl;
using Engine.Input;

namespace Shooter
{
    class StartMenuState : IGameObject
    {
        Engine.Font _generalFont;
        Input _input;
        VerticalMenu _menu;

        Renderer _renderer = new Renderer();
        Text _title;

        StateSystem _system;
        SoundManager _soundManager;
        Sound introSound;
        public StartMenuState(Form1 gameForm, Engine.Font titleFont, Engine.Font generalFont, Input input, StateSystem system, SoundManager soundManager)
        {
            _system = system;
            _generalFont = generalFont;
            _input = input;
            InitializeMenu(soundManager, gameForm);
            _title = new Text("Shooter", titleFont);
            _title.SetColor(new Color(0, 0, 0, 1));
            // Centerre on the x and place somewhere near the top
            _title.SetPosition(-_title.Width / 2, 300);
            _soundManager = soundManager;
        }

        private void InitializeMenu(SoundManager soundManager, Form1 gameform)
        {
            _menu = new VerticalMenu(0, 150, _input, soundManager);
            //Console.WriteLine("menu initialized");
            Button start1Player = new Button(
                delegate(object o, EventArgs e)
                {
                    //Console.WriteLine("1 player game");
                    _system.ChangeState("inner_game");
                    _soundManager.StopSound(introSound);
                    gameform.InitializeGameData();
                },
                new Text("1 Player", _generalFont));

            Button start2Player = new Button(
                delegate(object o, EventArgs e)
                {
                    //Console.WriteLine("2 player game");
                    _system.ChangeState("inner_game2");
                    _soundManager.StopSound(introSound);
                    gameform.InitializeGameData();
                },
                new Text("2 Players", _generalFont));

            Button exitGame = new Button(
                delegate(object o, EventArgs e)
                {
                    // Quit
                    System.Windows.Forms.Application.Exit();
                },
                new Text("Exit", _generalFont));

            _menu.AddButton(start1Player);
            _menu.AddButton(start2Player);
            _menu.AddButton(exitGame);
        }

        public void Update(double elapsedTime)
        {
            _menu.HandleInput();
        }

        public void Render()
        {
            Gl.glClearColor(1, 1, 1, 0);
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT);
            _renderer.DrawText(_title);
            _menu.Render(_renderer);
            _renderer.Render();
        }

        public void Activated()
        {
            introSound = _soundManager.PlaySound("intro", true);
        }
    }

}
