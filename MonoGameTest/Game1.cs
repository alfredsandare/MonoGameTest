using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
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
        List<NPC> NPCs = new List<NPC>();

        bool isInDialogue = false;
        string NPCToInteractWith;

        SpriteFont font;


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
            _graphics.IsFullScreen = true;

            var animations = new Dictionary<string, Dictionary<string, Animation>>();

            string[] animationsFile = File.ReadAllLines(PATH + "animations.txt");
            string trimmedLine;
            string currentAnimationSet = "";
            string currentAnimation = "";
            foreach (string line in animationsFile)
            {
                trimmedLine = line.Trim();
                if (trimmedLine.Length < 3) continue;
                if ((trimmedLine[0] == '/' && trimmedLine[1] == '/') || trimmedLine == "") continue;

                if (trimmedLine[0] == '#' && trimmedLine[1] == '#')
                {
                    currentAnimation = trimmedLine.Substring(2).Trim();

                    animations[currentAnimationSet].Add(currentAnimation, new Animation());
                }
                else if (trimmedLine[0] == '#')
                {
                    currentAnimationSet = trimmedLine.Substring(1).Trim();
                    animations.Add(currentAnimationSet, new Dictionary<string, Animation>());
                }
                else
                {
                    CultureInfo CI = (CultureInfo)CultureInfo.CurrentCulture.Clone();
                    CI.NumberFormat.CurrencyDecimalSeparator = ".";
                    string str = trimmedLine.Split()[0];
                    float point = float.Parse(str, NumberStyles.Any, CI);
                    animations[currentAnimationSet][currentAnimation].Add(trimmedLine.Split()[1], point);
                }
            }


            visualObjects = new List<VisualObject>();

            player = new Player(animations["player"], 0, 0, 18, 36, 1, "player_stand_down", 150d);
            player.SetHitboxOffset(16, 6);

            NPCs.Add(new NPC(animations["worker"], 0, 0, 18, 36, 1, "worker_stand_down", 100d, "Worker", "worker1"));
            NPCs[0].SetHitboxOffset(27, 21);

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
            foreach(NPC npc in NPCs) sortedVisualObjects.Add(npc);
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
                    string fileName = subfolder + "/" + file.Split("\\")[file.Split("\\").Length - 1];
                    fileName = fileName.Substring(0, fileName.Length - 4);
                    textures.Add(fileName, Content.Load<Texture2D>("graphics\\" + fileName));
                }
            }

            font = Content.Load<SpriteFont>("myFont");
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
            player.Move(xDirection, yDirection, (double)gameTime.ElapsedGameTime.TotalSeconds, visualObjects, -1);
            
            cameraXPos = player.xPos;
            cameraYPos = player.yPos;

            foreach (VisualObject visualObject in visualObjects) visualObject.UpdateAnimation(gameTime.ElapsedGameTime.TotalSeconds);
            foreach (NPC npc in NPCs) npc.Update(gameTime.ElapsedGameTime.TotalSeconds, visualObjects);

            int playerX = player.xPos + player.hitboxXOffset + (int)player.hitboxWidth / 2;
            int playerY = player.yPos + player.hitboxYOffset + (int)player.hitboxHeight / 2;
            bool foundNPC = false;
            for (int i = 0; i<NPCs.Count; i++)
            {
                int NPCX = NPCs[i].xPos + NPCs[i].hitboxXOffset + (int)NPCs[i].hitboxWidth / 2;
                int NPCY = NPCs[i].yPos + NPCs[i].hitboxYOffset + (int)NPCs[i].hitboxHeight / 2;
                if (Distance(NPCX, NPCY, playerX, playerY) < 60)
                {
                    NPCToInteractWith = NPCs[i].id;
                    foundNPC = true;
                    break;
                }
            }
            if (!foundNPC)
            {
                NPCToInteractWith = null;
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

            Debug.WriteLine($"width: {windowWidth}, height: {windowHeight}");

            foreach (VisualObject visualObject in sortedVisualObjects)
            {
                if (visualObject.currentSprite == null) continue;
                
                _spriteBatch.Draw(textures[visualObject.currentSprite],
                    new System.Numerics.Vector2(visualObject.xPos - (int)cameraXPos + windowWidth / 2,
                    visualObject.yPos - (int)cameraYPos + windowHeight / 2),
                    Color.White);
                
                /*
                Rectangle rect = new Rectangle(
                    visualObject.xPos - (int)cameraXPos + windowWidth / 2,
                    visualObject.yPos - (int)cameraYPos + windowHeight / 2,
                    32,
                    32);
                _spriteBatch.Draw(textures[visualObject.currentSprite], rect, Color.White);
                */
            }

            if (isInDialogue)
            {
                _spriteBatch.Draw(textures["dialogue"], new System.Numerics.Vector2(200, 240), Color.White);
                _spriteBatch.DrawString(font, "Hello There", new System.Numerics.Vector2(200, 240), Color.Black);
            }
            else if (NPCToInteractWith != null)
            {
                _spriteBatch.DrawString(font, "Press E to interact with Worker", new System.Numerics.Vector2(200, 240), Color.Black);
            }

            _spriteBatch.End();

            base.Draw(gameTime);

            double framerate = (1 / gameTime.ElapsedGameTime.TotalSeconds);
            //Debug.WriteLine(framerate);
        }

        private double Distance(int x1, int y1, int x2, int y2)
        {
            return Math.Pow(Math.Pow(x2 - x1, 2) + Math.Pow(y2-y1, 2), 0.5);
        }

        private int GetNPCIndexById(string id)
        {
            for (int i = 0; i < NPCs.Count;  i++)
            {
                if (NPCs[i].id == id)
                {
                    return i;
                }
            }
            throw new Exception($"No NPC with id \"{id}\" found.");
        }
    }
}