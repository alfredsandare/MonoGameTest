using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Transactions;

namespace MonoGameTest
{
    public class Game1 : Game
    {
        readonly string PATH = Directory.GetCurrentDirectory() + "\\Content\\";

        List<VisualObject> visualObjects;
        List<VisualObject> sortedVisualObjects = new List<VisualObject>();
        IDictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();
       
        float cameraXPos = 0;
        float cameraYPos = 0;

        char yDirection = 'n';
        char xDirection = 'e';

        Player player;

        string map;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;


        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            Random random = new Random();
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            Animation anAnimation = new Animation(new List<string>() { "greenbox", "blackbox" }, new List<float>() { 1, 2 });
            IDictionary<string, Animation> animationSet = new Dictionary<string, Animation>() { { "n", anAnimation} };

            Animation playerWalkRight = new Animation(new List<string>() { "player_walk_right_1", "player_stand_right", "player_walk_right_2", "player_stand_right" }, new List<float> { 0.2f, 0.4f, 0.6f, 0.8f });
            Animation playerWalkLeft = new Animation(new List<string>() { "player_walk_left_1", "player_stand_left", "player_walk_left_2", "player_stand_left" }, new List<float> { 0.2f, 0.4f, 0.6f, 0.8f});
            Animation playerStandRight = new Animation(new List<string>() { "player_stand_right" }, new List<float> { 1 });
            Animation playerStandLeft = new Animation(new List<string>() { "player_stand_left" }, new List<float> { 1 });

            Animation playerWalkUp = new Animation(new List<string>() { "player_walk_up_1", "player_walk_up_2" }, new List<float> { 0.2f, 0.4f });
            Animation playerWalkDown = new Animation(new List<string>() { "player_walk_down_1", "player_walk_down_2" }, new List<float> { 0.2f, 0.4f });
            Animation playerStandUp = new Animation(new List<string>() { "player_stand_up" }, new List<float> { 1 });
            Animation playerStandDown = new Animation(new List<string>() { "player_stand_down" }, new List<float> { 1 });

            IDictionary<string, Animation> playerAnimations = new Dictionary<string, Animation>() {
                { "player_stand_right", playerStandRight },
                { "player_stand_left", playerStandLeft },
                { "player_walk_right", playerWalkRight },
                { "player_walk_left", playerWalkLeft },
                { "player_stand_up", playerStandUp },
                { "player_stand_down", playerStandDown },
                { "player_walk_up", playerWalkUp },
                { "player_walk_down", playerWalkDown }
            };

            visualObjects = new List<VisualObject>();
            player = new Player(playerAnimations, 0, 0, 18, 36, 1, "player_stand_down");
            player.SetHitboxOffset(16, 6);

            string[] test = File.ReadAllLines(PATH+"map.txt");

            foreach(string s  in test)
            {
                if (s.ToCharArray()[0] == '/' && s.ToCharArray()[1] == '/') continue;
                string[] data = s.Split(" ");
                
                visualObjects.Add(new VisualObject(
                    null,
                    Convert.ToInt32(data[2]),
                    Convert.ToInt32(data[3]),
                    Convert.ToInt32(data[4]),
                    Convert.ToInt32(data[5]),
                    Convert.ToInt32(data[6]),
                    Convert.ToBoolean(Convert.ToInt32(data[7])),
                    data[1] != "None" ? data[1] : null
                    ));
            }
            
            sortedVisualObjects.Add(player);
            for (int i = 0; i < visualObjects.Count; i++)
            {
                bool added = false;
                for (int j = 0; j < sortedVisualObjects.Count; j++)
                {
                    if (visualObjects[i].layer <= sortedVisualObjects[j].layer)
                    {
                        sortedVisualObjects.Insert(j, visualObjects[i]);
                        added = true;
                        break;
                    }
                }
                if (!added) sortedVisualObjects.Add(visualObjects[i]);
            }
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            string[] files = Directory.GetFiles(PATH + "graphics\\");
            foreach (string file in files)
            {
                string fileName = file.Split("\\")[file.Split("\\").Length - 1];
                fileName = fileName.Substring(0, fileName.Length - 4);
                textures.Add(fileName, Content.Load<Texture2D>("graphics\\" + fileName));
            }

            string[] subfolders = new string[] { "tiles", "worker" };
            foreach (string subfolder in subfolders)
            {
                files = Directory.GetFiles(PATH + $"graphics\\{subfolder}\\");
                foreach (string file in files)
                {
                    Debug.WriteLine(file);
                    string fileName = subfolder + "/" + file.Split("\\")[file.Split("\\").Length - 1];
                    fileName = fileName.Substring(0, fileName.Length - 4);
                    textures.Add(fileName, Content.Load<Texture2D>("graphics\\" + fileName));
                }
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            var kstate = Keyboard.GetState();
            string xDirection = "";
            string yDirection = "";
            if (!kstate.IsKeyDown(Keys.S) && kstate.IsKeyDown(Keys.W)) yDirection = "n";
            else if (!kstate.IsKeyDown(Keys.W) && kstate.IsKeyDown(Keys.S)) yDirection = "s";
            if (!kstate.IsKeyDown(Keys.D) && kstate.IsKeyDown(Keys.A)) xDirection = "e";
            else if (!kstate.IsKeyDown(Keys.A) && kstate.IsKeyDown(Keys.D)) xDirection = "w";
            player.MovePlayer(xDirection, yDirection, player.speed, (double)gameTime.ElapsedGameTime.TotalSeconds, visualObjects);

            cameraXPos = player.xPos;
            cameraYPos = player.yPos;

            foreach (VisualObject visualObject in visualObjects)
            {
                visualObject.UpdateAnimation(gameTime.ElapsedGameTime.TotalSeconds);
            }
            player.UpdateAnimation(gameTime.ElapsedGameTime.TotalSeconds);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            int windowWidth = GraphicsDevice.PresentationParameters.BackBufferWidth;
            int windowHeight = GraphicsDevice.PresentationParameters.BackBufferHeight;

            foreach (VisualObject visualObject in sortedVisualObjects)
            {
                if (visualObject.currentSprite == null) continue;
                _spriteBatch.Draw(textures[visualObject.currentSprite],
                    new Vector2(visualObject.xPos - (int)cameraXPos + windowWidth / 2,
                    visualObject.yPos - (int)cameraYPos + windowHeight / 2),
                    Color.White);
            }
            _spriteBatch.End();

            base.Draw(gameTime);

            double framerate = (1 / gameTime.ElapsedGameTime.TotalSeconds);
            Debug.WriteLine(framerate);
        }
    }
}