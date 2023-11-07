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
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Transactions;

namespace MonoGameTest
{
    public class Game1 : Game
    {
        readonly string PATH = Directory.GetCurrentDirectory() + "\\Content\\";

        List<VisualObject> visualObjects;
        List<VisualObject> sortedVisualObjects = new List<VisualObject>();
        IDictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();
        IDictionary<string, bool> map = new Dictionary<string, bool>();
       
        float cameraXPos = 0;
        float cameraYPos = 0;

        char yDirection = 'n';
        char xDirection = 'e';

        Player player;
        List<NPC> NPCs = new List<NPC>();

        bool isInDialogue = false;
        string NPCToInteractWith;

        SpriteFont font;

        InputHandler inputHandler;

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
            //_graphics.IsFullScreen = true;
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
            _graphics.ApplyChanges();

            var animations = new Dictionary<string, Dictionary<string, Animation>>();

            inputHandler = new InputHandler();

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

            string[] dialogueFile = File.ReadAllLines(PATH + "dialogue.txt");
            string currentSet = "";
            IDictionary<string, List<string>> dialogue = new Dictionary<string, List<string>>();
            foreach (string line in dialogueFile)
            {
                if (string.IsNullOrEmpty(line)) continue;
                if (line.Substring(0, 1) == "#")
                {
                    dialogue.Add(line.Substring(1).Trim(), new List<string>());
                    currentSet = line.Substring(1).Trim();
                }
                else
                {
                    dialogue[currentSet].Add(line.Trim());
                }
            }

            visualObjects = new List<VisualObject>();

            player = new Player(animations["player"], 0, 0, 18, 36, 5, "player_stand_down", 150d);
            player.SetHitboxOffset(16, 6);

            NPCs.Add(new NPC(animations["worker"], 0, 0, 18, 36, 5, "worker_stand_down", 100d, "Worker", "worker1"));
            NPCs[0].SetHitboxOffset(27, 21);
            NPCs[0].SetBehavior("home", 400, -430);
            NPCs[0].AddDialogue("basic", dialogue["worker"]);
            NPCs[0].dialogueAvailable = true;
            NPCs[0].currentDialogue = "basic";

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

                string key = $"{data[2]} {data[3]}";
                bool value = Convert.ToBoolean(Convert.ToInt32(data[7]));

                if (map.ContainsKey(key))
                {
                    if (!map[key]) map[key] = value;
                }
                else
                {
                    map.Add(key, value);
                }
            }

            SortVisualObjects();
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();

            inputHandler.Update();

            string xDirection = "";
            string yDirection = "";
            if (!inputHandler.IsPressed(Keys.S) && inputHandler.IsPressed(Keys.W)) yDirection = "n";
            else if (!inputHandler.IsPressed(Keys.W) && inputHandler.IsPressed(Keys.S)) yDirection = "s";
            if (!inputHandler.IsPressed(Keys.D) && inputHandler.IsPressed(Keys.A)) xDirection = "e";
            else if (!inputHandler.IsPressed(Keys.A) && inputHandler.IsPressed(Keys.D)) xDirection = "w";

            if (inputHandler.KeyDown(Keys.E) && NPCToInteractWith != null && !isInDialogue)
            {
                isInDialogue = true;
                NPCs[GetNPCIndexById(NPCToInteractWith)].movementLocked = true;
            }
            else if (inputHandler.KeyDown(Keys.E) && isInDialogue)
            {
                ExitDialogue();
            }

            if (isInDialogue && (inputHandler.KeyDown(Keys.Space) || inputHandler.MouseButtonDown("LMB")))
            {
                NPC npc = NPCs[GetNPCIndexById(NPCToInteractWith)];
                if (npc.dialogueProgress + 1 < npc.dialogue[npc.currentDialogue].Count) npc.dialogueProgress++;
                else ExitDialogue();
            }

            player.Move(xDirection, yDirection, (double)gameTime.ElapsedGameTime.TotalSeconds, visualObjects, -1);
            cameraXPos = player.xPos + 25;
            cameraYPos = player.yPos + 24;

            SortVisualObjects();

            foreach (VisualObject visualObject in visualObjects) visualObject.UpdateAnimation(gameTime.ElapsedGameTime.TotalSeconds);
            foreach (NPC npc in NPCs) npc.Update(gameTime.ElapsedGameTime.TotalSeconds, visualObjects);

            foreach (VisualObject visualObject in visualObjects)
            {
                if (!visualObject.isVariableLayer) continue;
                
                if (player.yPos < visualObject.yPos + visualObject.variableLayerYOffset)
                {
                    visualObject.layer = 10;
                }
                else
                {
                    visualObject.layer = 1;
                }
            }

            int playerX = player.xPos + player.hitboxXOffset + (int)player.hitboxWidth / 2;
            int playerY = player.yPos + player.hitboxYOffset + (int)player.hitboxHeight / 2;
            if (!isInDialogue)
            {
                bool foundNPC = false;
                for (int i = 0; i<NPCs.Count; i++)
                {
                    int NPCX = NPCs[i].xPos + NPCs[i].hitboxXOffset + (int)NPCs[i].hitboxWidth / 2;
                    int NPCY = NPCs[i].yPos + NPCs[i].hitboxYOffset + (int)NPCs[i].hitboxHeight / 2;
                    if (Util.Distance(NPCX, NPCY, playerX, playerY) < 60)
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
            }
            else
            {
                int NPCIndex = GetNPCIndexById(NPCToInteractWith);
                int NPCX = NPCs[NPCIndex].xPos + NPCs[NPCIndex].hitboxXOffset + (int)NPCs[NPCIndex].hitboxWidth / 2;
                int NPCY = NPCs[NPCIndex].yPos + NPCs[NPCIndex].hitboxYOffset + (int)NPCs[NPCIndex].hitboxHeight / 2;
                if (Util.Distance(NPCX, NPCY, playerX, playerY) > 90)
                {
                    ExitDialogue();
                }
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
                int dialogueX = windowWidth / 2 - 200;
                int dialogueY = windowHeight - 220;
                NPC npc = NPCs[GetNPCIndexById(NPCToInteractWith)];
                _spriteBatch.Draw(textures["dialogue"], new Vector2(dialogueX, dialogueY), Color.White);
                _spriteBatch.DrawString(font, npc.name, new Vector2(dialogueX+5, dialogueY+5), Color.Black);

                string text = npc.GetDialogueText();
                text = WrapLines(text, 400, font);

                _spriteBatch.DrawString(font, text, new System.Numerics.Vector2(dialogueX + 5, dialogueY + 50), Color.Black);
            }
            else if (NPCToInteractWith != null)
            {
                string text = "Press E to interact with Worker";
                Vector2 pos = GetTextPosByAnchor(windowWidth / 2, windowHeight / 2 + 40, text, "c", font);
                _spriteBatch.DrawString(font, text, pos, Color.Black);
            }

            /* PATHFINDING TESTING
            int i = 0;
            foreach (Vector2 n in PathFind("256 0", "-64 64")) //480 -384
            {
                _spriteBatch.DrawString(font, Convert.ToString(i), new Vector2(n.X - (int)cameraXPos + windowWidth / 2, n.Y - (int)cameraYPos + windowHeight / 2), Color.Black);
                Debug.WriteLine("");
                i++;
            }
            */

            _spriteBatch.End();
            base.Draw(gameTime);

            double framerate = (1 / gameTime.ElapsedGameTime.TotalSeconds);
            //Debug.WriteLine(framerate);
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

        private Vector2 GetTextPosByAnchor(int x, int y, string text, string anchor, SpriteFont font)
        {
            Microsoft.Xna.Framework.Vector2 textSize = font.MeasureString(text);
            float textX = textSize.X;
            float textY = textSize.Y;
            switch (anchor)
            {
                case "nw":
                    return new Vector2(x, y);

                case "n":
                    return new Vector2(x-textX/2, y);

                case "ne":
                    return new Vector2(x-textX, y);

                case "e":
                    return new Vector2(x - textX, y - textY/2);

                case "se":
                    return new Vector2(x - textX, y - textY);

                case "s":
                    return new Vector2(x - textX/2, y + textY);

                case "sw":
                    return new Vector2(x, y + textY);

                case "w":
                    return new Vector2(x, y-textY/2);

                case "c":
                    return new Vector2(x-textX/2, y-textY/2);

                default:
                    throw new Exception($"Invalid text anchor: {anchor}");
            }
        }

        private void ExitDialogue()
        {
            if (NPCToInteractWith == null) return;
            NPCs[GetNPCIndexById(NPCToInteractWith)].movementLocked = false;
            NPCToInteractWith = null;
            isInDialogue = false;
        }

        private string WrapLines(string text, int xMax, SpriteFont font)
        {
            string[] words = text.Split(" ");
            string output = "";
            foreach (string word in words) 
            {
                string latestLine = output.Split("\n")[output.Split("\n").Length - 1];
                if (font.MeasureString(latestLine+word).X > xMax)
                {
                    output = output.Trim();
                    output += "\n";
                }
                output += word + " ";
            }
            return output.Trim();
        }

        private void SortVisualObjects()
        {
            sortedVisualObjects.Clear();
            sortedVisualObjects.Add(player);
            foreach (NPC npc in NPCs) sortedVisualObjects.Add(npc);
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
        }

        private List<Vector2> PathFind(string start, string end)
        {
            int endX = Convert.ToInt32(end.Split(" ")[0]);
            int endY = Convert.ToInt32(end.Split(" ")[1]);
            int startX = Convert.ToInt32(start.Split(" ")[0]);
            int startY = Convert.ToInt32(start.Split(" ")[1]);

            List<int[]> paths = new List<int[]>();
            paths.Add(new int[3] {endX, endY, 0});

            bool done = false;
            int i = 0;
            while (i < paths.Count() && !done)
            {
                int currentX = paths[i][0];
                int currentY = paths[i][1];
                List<int[]> adjacentCells = new List<int[]>();
                try { adjacentCells.Add(new int[3] { currentX, currentY + 32, paths[i][2] + 1 }); } catch { }
                try { adjacentCells.Add(new int[3] { currentX + 32, currentY, paths[i][2] + 1 }); } catch { }
                try { adjacentCells.Add(new int[3] { currentX, currentY - 32, paths[i][2] + 1 }); } catch { }
                try { adjacentCells.Add(new int[3] { currentX - 32, currentY, paths[i][2] + 1 }); } catch { }
                foreach (int[] cell in adjacentCells)
                {
                    if (!map.ContainsKey($"{cell[0]} {cell[1]}")) continue;
                    if (!map[$"{cell[0]} {cell[1]}"] && paths.Where(path => cell[0] == path[0] && cell[1] == path[1]).Count() == 0)
                    {
                        paths.Add(cell);
                    }
                }
                foreach (int[] path in paths)
                {
                    if (path[0] == startX && path[1] == startY) 
                    {
                        done = true;
                        break;
                    }
                }
                i++;
            }

            done = false;
            List<Vector2> finalPath = new List<Vector2>();
            finalPath.Add(new Vector2(startX, startY));
            i = 0;
            while (!done)
            {
                List<int[]> adjacentCells = new List<int[]>();
                foreach (int[] cell in paths)
                {
                    if ((finalPath[i].X == cell[0] - 32 && finalPath[i].Y == cell[1]) ||
                        (finalPath[i].X == cell[0] && finalPath[i].Y == cell[1] - 32) ||
                        (finalPath[i].X == cell[0] + 32 && finalPath[i].Y == cell[1]) ||
                        (finalPath[i].X == cell[0] && finalPath[i].Y == cell[1] + 32))
                    {
                        adjacentCells.Add(cell);
                        break;
                    }
                }

                int smallestNumber = 999999;
                int smallestIndex = 0;
                int j = 0;
                foreach (int[] cell in adjacentCells)
                {
                    if (cell[2] < smallestNumber) { smallestNumber = cell[2]; smallestIndex = j; }
                    j++;
                }
                finalPath.Add(new Vector2(adjacentCells[smallestIndex][0], adjacentCells[smallestIndex][1]));

                if (finalPath[i+1].X == endX && finalPath[i+1].Y  == endY)
                {
                    done = true;
                }

                i++;
            }

            return finalPath;
        }
    }
}