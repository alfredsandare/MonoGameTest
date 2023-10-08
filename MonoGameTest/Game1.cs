using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace MonoGameTest
{
    public class Game1 : Game
    {

        //readonly string string PATH = "C:\\Users\\alfre\\Source\\repos\\alfredsandare\\MonoGameTest\\MonoGameTest\\Content\\";
        //readonly string PATH = "C:\\users\\04alsa25\\source\\repos\\MonoGameTest\\MonoGameTest\\Content\\";
        readonly string PATH = Directory.GetCurrentDirectory() + "\\Content\\";

        List<VisualObject> visualObjects;
        IDictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();
       
        float cameraXPos = 0;
        float cameraYPos = 0;

        double playerSpeed = 150f;

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
            player = new Player(new List<SpriteComponent> { new SpriteComponent("player_stand_right", 0, 0, playerAnimations) }, -60, 100, 27, 54, 1, true);
            player.SetHitboxOffset(24, 9);

            string[] test;

            /*
            using (var stream = TitleContainer.OpenStream("Content\\map.txt"))
            {
                using (var reader = new StreamReader(stream))
                {
                    test = reader.ReadToEnd().Split("\r\n");
                }
            }
            */


            test = File.ReadAllLines(PATH+"map.txt");

            foreach(string s  in test)
            {
                if (s.ToCharArray()[0] == '/' && s.ToCharArray()[1] == '/') continue;
                string[] data = s.Split(" ");
                int[] pos = new int[2];
                for (int i=0; i<2; i++)
                {
                    if (data[i+2].Contains("*"))
                    {
                        string[] nums = data[i + 2].Split("*");
                        pos[i] = Convert.ToInt32(nums[0]) * Convert.ToInt32(nums[1]);
                    } else
                    {
                        pos[i] = Convert.ToInt32(data[i + 2]);
                    }
                }

                Debug.WriteLine(Convert.ToBoolean(Convert.ToInt32(data[7])));

                visualObjects.Add(new VisualObject(new List<SpriteComponent> { new SpriteComponent(data[1], 0, 0, null) }, pos[0], pos[1], Convert.ToInt32(data[4]), Convert.ToInt32(data[5]), Convert.ToInt32(data[6]), Convert.ToBoolean(Convert.ToInt32(data[7]))));
            }

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            //foreach (string s in graphics) textures.Add(s, Content.Load<Texture2D>(s));

            string[] files = Directory.GetFiles(PATH + "graphics\\");
            foreach (string file in files)
            {
                string fileName = file.Split("\\")[file.Split("\\").Length - 1];
                fileName = fileName.Substring(0, fileName.Length - 4);
                textures.Add(fileName, Content.Load<Texture2D>("graphics\\" + fileName));
            }

            files = Directory.GetFiles(PATH + "graphics\\tiles\\");
            foreach (string file in files)
            {
                string fileName = "tiles/" + file.Split("\\")[file.Split("\\").Length - 1];
                fileName = fileName.Substring(0, fileName.Length - 4);
                textures.Add(fileName, Content.Load<Texture2D>("graphics\\" + fileName));
            }
            //map = Content.Load<string>("map");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            var kstate = Keyboard.GetState();
            //if (!kstate.IsKeyDown(Keys.S) && kstate.IsKeyDown(Keys.W)) visualObjects[0].yPos -= (int)(cameraSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);
            //else if (!kstate.IsKeyDown(Keys.W) && kstate.IsKeyDown(Keys.S)) visualObjects[0].yPos += (int)(cameraSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);
            //if (!kstate.IsKeyDown(Keys.D) && kstate.IsKeyDown(Keys.A)) visualObjects[0].xPos -= (int)(cameraSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);
            //else if (!kstate.IsKeyDown(Keys.A) && kstate.IsKeyDown(Keys.D)) visualObjects[0].xPos += (int)(cameraSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);

            string xDirection = "";
            string yDirection = "";
            if (!kstate.IsKeyDown(Keys.S) && kstate.IsKeyDown(Keys.W)) yDirection = "n";
            else if (!kstate.IsKeyDown(Keys.W) && kstate.IsKeyDown(Keys.S)) yDirection = "s";
            if (!kstate.IsKeyDown(Keys.D) && kstate.IsKeyDown(Keys.A)) xDirection = "e";
            else if (!kstate.IsKeyDown(Keys.A) && kstate.IsKeyDown(Keys.D)) xDirection = "w";
            player.MovePlayer(xDirection, yDirection, playerSpeed, (double)gameTime.ElapsedGameTime.TotalSeconds, visualObjects);

            //if (yDirection == "n") player.spriteComponents[0].SwitchAnimation("player_walk_up");
            //else if (yDirection == "s") player.spriteComponents[0].SwitchAnimation("player_walk_down");
            //else player.spriteComponents[0].SwitchAnimation("player_stand_down");

            //if (xDirection == "e") player.spriteComponents[0].SwitchAnimation("player_walk_left");
            //else if (xDirection == "w") player.spriteComponents[0].SwitchAnimation("player_walk_right");
            //else player.spriteComponents[0].SwitchAnimation("player_stand_down");

            cameraXPos = player.xPos;
            cameraYPos = player.yPos;

            foreach (VisualObject visualObject in visualObjects)
            {
                foreach (SpriteComponent spriteComponent in visualObject.spriteComponents)
                {
                    spriteComponent.UpdateAnimation(gameTime.ElapsedGameTime.TotalSeconds);
                }
            }
            player.spriteComponents[0].UpdateAnimation(gameTime.ElapsedGameTime.TotalSeconds);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            int windowWidth = GraphicsDevice.PresentationParameters.BackBufferWidth;
            int windowHeight = GraphicsDevice.PresentationParameters.BackBufferHeight;

            List<VisualObject> sortedVisualObjects = new List<VisualObject>();
            sortedVisualObjects.Add(player);
            for (int i = 0; i < visualObjects.Count; i++)
            {
                bool added = false;
                for (int j = 0; j<sortedVisualObjects.Count; j++)
                {
                    if (visualObjects[i].layer <= sortedVisualObjects[j].layer)
                    {
                        sortedVisualObjects.Insert(j, visualObjects[i]);
                        added = true;
                        break;
                    }
                }
                if (!added) sortedVisualObjects.Insert(0, visualObjects[i]);
            }

            foreach (VisualObject visualObject in sortedVisualObjects)
            {
                foreach (SpriteComponent spriteComponent in visualObject.spriteComponents)
                {
                    _spriteBatch.Draw(textures[spriteComponent.currentSprite], 
                        new Vector2(visualObject.xPos - (int)cameraXPos + windowWidth / 2 + spriteComponent.xOffset, 
                        visualObject.yPos - (int)cameraYPos + windowHeight / 2 + spriteComponent.yOffset), 
                        Color.White);
                }
                Debug.WriteLine(visualObject.layer);
            }
            Debug.WriteLine("");
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}