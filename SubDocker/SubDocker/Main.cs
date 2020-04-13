using BEPUphysics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;
using System.Collections.Generic;

namespace SpaceDocker
{
    public class Main : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Random rnd = new Random();

        private int difficulty = 1;
        private Vector3 distanceMultipler;
        private int maxFuelPack;
        private int maxTorpedoPack;
        private int numDuckGeneration;

        SpriteFont shipInfo;
        private bool showTargetLocation = false;
        Vector2 shipLocationTextPos;
        Vector2 targetLocationTextPos;

        private List<RubberDuck> allRubberducks;
        private List<FuelPack> allFuelPacks;
        private List<TorpedoPack> allTorpedoPacks;

        public static Camera camera { get; private set; }
        public static Ship ship { get; private set; }

        public static Target turtle;

        public static Skybox skybox { get; set; }

        public Main()
        {
            graphics = new GraphicsDeviceManager(this);
            // changes window size
            graphics.PreferredBackBufferWidth = 3000;
            graphics.PreferredBackBufferHeight = 1500;
            graphics.ApplyChanges();
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            // Make our BEPU Physics space a service
            Services.AddService<Space>(new Space());

            // range that items will be placed from 0,0,0
            distanceMultipler = new Vector3(200, 200, 200);
            
            // set up scaling game variables
            InitItemDifficulty(difficulty);
            
            // generate all randomly placed objects
            GenerateRubberDucks();
            GenerateFuelPacks();
            GenerateTorpedoPacks();

            // ship setup
            ship = new Ship(this, Vector3.Zero, "ship", 2, difficulty);

            // camera setup
            camera = new Camera(this);
            
            // TODO - Need to rotate to point straight out, maybe mimic direction
            // Torpedo t = new Torpedo(this, 2f, "torpedo99");

            Vector3 angMomentum = new Vector3((float)rnd.NextDouble() * rnd.Next(-2, 2), (float)rnd.NextDouble() * rnd.Next(-2, 2), (float)rnd.NextDouble() * rnd.Next(-2, 2));
            Vector3 linMomentum = new Vector3((float)rnd.NextDouble() * rnd.Next(-30, 30), (float)rnd.NextDouble() * rnd.Next(-30, 30), (float)rnd.NextDouble() * rnd.Next(-30, 30));
            turtle = new Target(this, "turtle", linMomentum, angMomentum);

            // skybox setup
            skybox = new Skybox(this, ship.modelPosition, "skybox");

            base.Initialize();
        }
        
        /// <summary>
        /// Variables that are needed in main that scales with difficulty
        /// </summary>
        /// <param name="difficulty"></param>
        private void InitItemDifficulty(int difficulty)
        {
            switch (difficulty)
            {
                case 1:
                    maxFuelPack = 5;
                    maxTorpedoPack = 5;
                    numDuckGeneration = 40;
                    return;
                case 2:
                    Console.WriteLine("Level 2 Not Implemented");
                    return;
                case 3:
                    Console.WriteLine("Level 3 Not Implemented");
                    return;
            }
        }

        /// <summary>
        /// Generate x many rubber ducks and places in them in the game
        /// </summary>
        private void GenerateRubberDucks()
        {
            allRubberducks = new List<RubberDuck>();

            // randomly generates x rubber ducks
            for (int i = 0; i < numDuckGeneration; i++)
            {
                int minRange = -100;
                int maxRange = 100;
                Vector3 angMomentum = new Vector3((float)rnd.NextDouble() * rnd.Next(-2, 2), (float)rnd.NextDouble() * rnd.Next(-2, 2), (float)rnd.NextDouble() * rnd.Next(-2, 2));
                Vector3 linMomentum = new Vector3((float)rnd.NextDouble() * rnd.Next(-20, 20), (float)rnd.NextDouble() * rnd.Next(-20, 20), (float)rnd.NextDouble() * rnd.Next(-20, 20));
                Vector3 pos = new Vector3((float)rnd.NextDouble() * rnd.Next(minRange, maxRange), (float)rnd.NextDouble() * rnd.Next(minRange, maxRange), (float)rnd.NextDouble() * rnd.Next(minRange, maxRange));
                RubberDuck r = new RubberDuck(this, pos, "duck-" + i, 2, linMomentum, angMomentum);
                allRubberducks.Add(r);
            }
        }

        /// <summary>
        /// Removes remaining fuel packs from the game
        /// </summary>
        private void RemoveFuelPacks()
        {
            foreach (FuelPack fp in allFuelPacks)
            {
                fp.RemoveFromGame();
            }
        }

        /// <summary>
        /// Generates x fuel packs randomly in the game
        /// </summary>
        private void GenerateFuelPacks()
        {
            allFuelPacks = new List<FuelPack>();

            for (int i = 0; i < maxFuelPack; i++)
            {
                Vector3 position = (new Vector3(
                    (float)rnd.Next(-(int)(distanceMultipler.X), (int)(distanceMultipler.X)),
                    (float)rnd.Next(-(int)(distanceMultipler.Y), (int)(distanceMultipler.Y)),
                    (float)rnd.Next(-(int)(distanceMultipler.Z), (int)distanceMultipler.Z))
                    );

                FuelPack fp = new FuelPack(this, position, "fuelPack-" + i);
                allFuelPacks.Add(fp);
            }
        }
        /// <summary>
        /// Remove remaining rubber ducks from the game
        /// </summary>
        private void RemoveRubberDucks()
        {
            foreach(RubberDuck rd in allRubberducks)
            {
                rd.RemoveFromGame();
            }
        }

        /// <summary>
        /// Creates x number of torpedo packs and spreads throughout game
        /// </summary>
        private void GenerateTorpedoPacks()
        {
            allTorpedoPacks = new List<TorpedoPack>();

            for (int i = 0; i < maxTorpedoPack; i++)
            {
                Vector3 position = (new Vector3(
                    (float)rnd.Next(-(int)(distanceMultipler.X), (int)(distanceMultipler.X)),
                    (float)rnd.Next(-(int)(distanceMultipler.Y), (int)(distanceMultipler.Y)),
                    (float)rnd.Next(-(int)(distanceMultipler.Z), (int)distanceMultipler.Z))
                    );
                TorpedoPack tp = new TorpedoPack(this, position, "torpedoPack-" + i);
                allTorpedoPacks.Add(tp);
            }
        }

        /// <summary>
        /// Removes all the remaining torpedo packs from the game
        /// </summary>
        private void RemoveTorpdeoPacks()
        {
            foreach (TorpedoPack tp in allTorpedoPacks)
            {
                tp.RemoveFromGame();
            }
        }


        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            shipInfo = Content.Load<SpriteFont>("shipInfo");

            shipLocationTextPos = new Vector2((float)(GraphicsDevice.Viewport.Width - 500), (float)(GraphicsDevice.Viewport.Height - 100));
            targetLocationTextPos = new Vector2((float)(GraphicsDevice.Viewport.Width - 500), (float)(GraphicsDevice.Viewport.Height - (GraphicsDevice.Viewport.Height - 100)));
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {

            GamePadState currentGamePadState = GamePad.GetState(PlayerIndex.One);
            KeyboardState currentKeyboardState = Keyboard.GetState();

            if (currentGamePadState.Buttons.Back == ButtonState.Pressed || currentKeyboardState.IsKeyDown(Keys.Escape))
                Exit();

            // show target location coordinates
            if (currentKeyboardState.IsKeyDown(Keys.H))
            {
                showTargetLocation = true;
            }

            //hide target location coordinates
            if (currentKeyboardState.IsKeyDown(Keys.B))
            {
                showTargetLocation = false;
            }

            // restart game
            if (currentKeyboardState.IsKeyDown(Keys.R))
            {
                ship.Reset();
                skybox.Reset();
                turtle.Reset();
                // RemoveRubberDucks();
                // GenerateRubberDucks();
                RemoveFuelPacks();
                GenerateFuelPacks();
                RemoveTorpdeoPacks();
                GenerateTorpedoPacks();
            }

            Services.GetService<Space>().Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // debugging
            spriteBatch.Begin();

            spriteBatch.DrawString(shipInfo, "Ship Location: x: " + (int)ship.modelPosition.X + " y: " + (int)ship.modelPosition.Y + " z: " + (int)ship.modelPosition.Z, shipLocationTextPos, Color.White);    

            if (showTargetLocation)
            {
                spriteBatch.DrawString(shipInfo, "Target Location: x: " + (int)turtle.modelPosition.X + " y: " + (int)turtle.modelPosition.Y + " z: " + (int)turtle.modelPosition.Z, targetLocationTextPos, Color.White);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
