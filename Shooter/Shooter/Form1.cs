using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Engine;
using Engine.Input;
using Tao.OpenGl;
using Tao.DevIl;

namespace Shooter
{
    public partial class Form1 : Form
    {
        bool _fullscreen = false;
        FastLoop _fastLoop;
        StateSystem _system = new StateSystem();
        Input _input = new Input();
        TextureManager _textureManager = new TextureManager();
        SoundManager _soundManager = new SoundManager();
        Engine.Font _generalFont;
        Engine.Font _titleFont;

        public Form1()
        {
            InitializeComponent();
            simpleOpenGlControl1.InitializeContexts();

            _input.Mouse = new Mouse(this, simpleOpenGlControl1);
            _input.Keyboard = new Keyboard(simpleOpenGlControl1);

            InitializeDisplay();
            InitializeSounds();
            InitializeTextures();
            InitializeGameData();
            InitializeFonts();
            InitializeGameState();


            _fastLoop = new FastLoop(GameLoop);
        }

        PersistantGameData _persistantGameData = new PersistantGameData();
        public void InitializeGameData()
        {
            LevelDescription level1 = new LevelDescription();
            level1.Time = 25;
            level1.LevelFile = "Level1.txt";
            level1.LevelName = "Level 1";
            _persistantGameData.CurrentLevel = level1;
            
            LevelDescription level2 = new LevelDescription();
            level2.Time = 30;
            level2.LevelFile = "Level2.txt";
            level2.LevelName = "Level 2";
            _persistantGameData.Levels = new List<LevelDescription>();
            _persistantGameData.Levels.Add(level2);
        }


        private void InitializeTextures()
        {
            // Init DevIl
            Il.ilInit();
            Ilu.iluInit();
            Ilut.ilutInit();
            Ilut.ilutRenderer(Ilut.ILUT_OPENGL);

            // Textures are loaded here.
            _textureManager.LoadTexture("destroybonus", "destroybonus.jpg");
            _textureManager.LoadTexture("scoregift", "scoregift.jpg");
            _textureManager.LoadTexture("player_ship", "spaceship.tga");
            _textureManager.LoadTexture("enemy_ship", "spaceship2.tga");
            _textureManager.LoadTexture("title_font", "title_font.tga");
            _textureManager.LoadTexture("general_font", "general_font.tga");
            _textureManager.LoadTexture("background", "background.jpg");
            _textureManager.LoadTexture("background_layer_1", "background2.jpg"); 
            _textureManager.LoadTexture("bullet", "bullet.tga");
            _textureManager.LoadTexture("explosion", "ani1.jpg");
        }


        private void InitializeFonts()
        {
            
            // Fonts are loaded here.

            _titleFont = FontParser.CreateFont("title_font.fnt", _textureManager);
            _generalFont = FontParser.CreateFont("general_font.fnt", _textureManager);
        }

        private void InitializeSounds()
        {
            // Sounds are loaded here.
            _soundManager.LoadSound("intro", "intro.wav");
            _soundManager.LoadSound("shoot", "shoot.wav");
            _soundManager.LoadSound("enemyshoot", "enemyshoot.wav");
            _soundManager.LoadSound("explosion", "explosion.wav");
            _soundManager.LoadSound("select", "select.wav");
        }

        private void InitializeGameState()
        {
            // Game states are loaded here
            _system.AddState("start_menu", new StartMenuState(this, _titleFont, _generalFont, _input, _system, _soundManager));
            _system.AddState("inner_game", new InnerGameState(_system, _input, _textureManager, _persistantGameData, _generalFont, ClientSize, false, _soundManager));
            _system.AddState("inner_game2", new InnerGameState(_system, _input, _textureManager, _persistantGameData, _generalFont, ClientSize, true, _soundManager));
            _system.AddState("game_over", new GameOverState(_persistantGameData, _system, _input, _generalFont, _titleFont));
            _system.ChangeState("start_menu");
        }

        private void UpdateInput(double elapsedTime)
        {
            _input.Update(elapsedTime);
        }

        private void GameLoop(double elapsedTime)
        {
            UpdateInput(elapsedTime);
            _system.Update(elapsedTime);
            _system.Render();
            simpleOpenGlControl1.Refresh();
        }

        private void InitializeDisplay()
        {
            if (_fullscreen)
            {
                FormBorderStyle = FormBorderStyle.None;
                WindowState = FormWindowState.Maximized;
            }
            else
            {
                ClientSize = new Size(1024, 600);
                this.CenterToScreen();
            }
            Setup2DGraphics(ClientSize.Width, ClientSize.Height);
        }

        protected override void OnClientSizeChanged(EventArgs e)
        {
            base.OnClientSizeChanged(e);
            Gl.glViewport(0, 0, this.ClientSize.Width, this.ClientSize.Height);
            Setup2DGraphics(ClientSize.Width, ClientSize.Height);
        }

        private void Setup2DGraphics(double width, double height)
        {
            double halfWidth = width / 2;
            double halfHeight = height / 2;
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();
            Gl.glOrtho(-halfWidth, halfWidth, -halfHeight, halfHeight, -100, 100);
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadIdentity();
        }

    }
}