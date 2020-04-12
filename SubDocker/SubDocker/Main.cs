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

        private int maxFuelPack;
        private int maxTorpedoPack;
  
        // debugging 
        Vector2 shipLocationTextPos;
        Vector2 shipLinearMomentumTextPos;
        Vector2 shipLinearVelocityTextPos;
        Vector2 shipAngularMomentumTextPos;
        Vector2 shipAngularVelocityTextPos;
        Vector2 jellyfishLinearMomentumTextPos;
        Vector2 jellyfishLinearVelocityTextPos;
        Vector2 jellyfishAnuglarMomentumTextPos;
        Vector2 jellyfishAngularVelocityTextPos;
        Vector2 jellyfishLocationTextPos;
        Vector2 cameraLocationTextPos;
        SpriteFont shipInfo;

        private int difficulty = 1;
        private int numDuckGeneration;
        private List<RubberDuck> allRubberducks;
        private List<FuelPack> allFuelPacks;
        private List<TorepedoPack> allTorpedoPacks;

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

            InitItemDifficulty(difficulty);
            
            GenerateRubberDucks();
            GenerateFuelPacks();
            GenerateTorepedoPacks();

            // ship setup
            ship = new Ship(this, Vector3.Zero, "ship", 2, difficulty);

            // camera setup
            camera = new Camera(this); // right/left, behind/infront, above/below 

            // test
            // FuelPack fp = new FuelPack(this, Vector3.One, "fuelPack-99");
            
            // TODO - Need to rotate to point straight out, maybe mimic direction
            Torepedo t = new Torepedo(this, 2f, "torepedo99");

            Vector3 angMomentum = new Vector3((float)rnd.NextDouble() * rnd.Next(-2, 2), (float)rnd.NextDouble() * rnd.Next(-2, 2), (float)rnd.NextDouble() * rnd.Next(-2, 2));
            Vector3 linMomentum = new Vector3((float)rnd.NextDouble() * rnd.Next(-30, 30), (float)rnd.NextDouble() * rnd.Next(-30, 30), (float)rnd.NextDouble() * rnd.Next(-30, 30));
            turtle = new Target(this, "turtle", linMomentum, angMomentum);

            // skybox setup DOESN'T WORK YET
            // skybox = new Skybox(this, ship.modelPosition, "skybox");

            base.Initialize();
        }
        
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

        private void RemoveFuelPacks()
        {
            foreach (FuelPack fp in allFuelPacks)
            {
                fp.RemoveFromGame();
            }
        }

        private void GenerateFuelPacks()
        {

            allFuelPacks = new List<FuelPack>();

            for (int i = 0; i < maxFuelPack; i++)
            {
                Vector3 distanceMultipler = new Vector3(100, 100, 100);
                Vector3 position = (new Vector3(
                    (float)rnd.Next(-(int)(distanceMultipler.X), (int)(distanceMultipler.X)),
                    (float)rnd.Next(-(int)(distanceMultipler.Y), (int)(distanceMultipler.Y)),
                    (float)rnd.Next(-(int)(distanceMultipler.Z), (int)distanceMultipler.Z))
                    );

                FuelPack fp = new FuelPack(this, position, "fuelPack-" + i);
                allFuelPacks.Add(fp);
            }
        }

        private void RemoveRubberDucks()
        {
            foreach(RubberDuck rd in allRubberducks)
            {
                rd.RemoveFromGame();
            }
        }

        private void GenerateTorepedoPacks()
        {
            allTorpedoPacks = new List<TorepedoPack>();

            Vector3 position = Vector3.One;
            for (int i = 0; i < maxTorpedoPack; i++)
            {
                TorepedoPack tp = new TorepedoPack(this, position, "torepedoPack-" + i);
                allTorpedoPacks.Add(tp);
            }
        }

        private void RemoveTorepdeoPacks()
        {
            foreach (TorepedoPack tp in allTorpedoPacks)
            {
                tp.RemoveFromGame();
            }
        }


        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            shipInfo = Content.Load<SpriteFont>("shipInfo");

            cameraLocationTextPos = new Vector2(50f, (float)(GraphicsDevice.Viewport.Height - (GraphicsDevice.Viewport.Height - 100)));

            shipLocationTextPos = new Vector2((float)(GraphicsDevice.Viewport.Width - 500), (float)(GraphicsDevice.Viewport.Height - 100));
            shipLinearMomentumTextPos = new Vector2((float)(GraphicsDevice.Viewport.Width - 500), (float)(GraphicsDevice.Viewport.Height - 150)); ;
            shipLinearVelocityTextPos = new Vector2((float)(GraphicsDevice.Viewport.Width - 500), (float)(GraphicsDevice.Viewport.Height - 200)); ;
            shipAngularMomentumTextPos = new Vector2((float)(GraphicsDevice.Viewport.Width - 500), (float)(GraphicsDevice.Viewport.Height - 250)); ;
            shipAngularVelocityTextPos = new Vector2((float)(GraphicsDevice.Viewport.Width - 500), (float)(GraphicsDevice.Viewport.Height - 300)); ;

            jellyfishLocationTextPos = new Vector2((float)(GraphicsDevice.Viewport.Width - 500), (float)(GraphicsDevice.Viewport.Height - (GraphicsDevice.Viewport.Height - 100)));
            jellyfishLinearMomentumTextPos = new Vector2((float)(GraphicsDevice.Viewport.Width - 500), (float)(GraphicsDevice.Viewport.Height - (GraphicsDevice.Viewport.Height - 150))); ;
            jellyfishLinearVelocityTextPos = new Vector2((float)(GraphicsDevice.Viewport.Width - 500), (float)(GraphicsDevice.Viewport.Height - (GraphicsDevice.Viewport.Height - 200))); ;
            jellyfishAnuglarMomentumTextPos = new Vector2((float)(GraphicsDevice.Viewport.Width - 500), (float)(GraphicsDevice.Viewport.Height - (GraphicsDevice.Viewport.Height - 250))); ;
            jellyfishAngularVelocityTextPos = new Vector2((float)(GraphicsDevice.Viewport.Width - 500), (float)(GraphicsDevice.Viewport.Height - (GraphicsDevice.Viewport.Height - 300))); ;
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

            // restart game
            if (currentKeyboardState.IsKeyDown(Keys.R))
            {
                ship.Reset();
                // skybox.Reset();
                turtle.Reset();
                // RemoveRubberDucks();
                // GenerateRubberDucks();
                // RemoveFuelPacks();
                // GenerateFuelPacks();
                // RemoveTorepdoPacks();
                // GenerateTorepedoPacks();
            }

            Services.GetService<Space>().Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // debugging
            spriteBatch.Begin();

            spriteBatch.DrawString(shipInfo, "Camera Location: x: " + (int)camera.Position.X + " y: " + (int)camera.Position.Y + " z: " + (int)camera.Position.Z, cameraLocationTextPos, Color.White);

            spriteBatch.DrawString(shipInfo, "Ship Location: x: " + (int)ship.modelPosition.X + " y: " + (int)ship.modelPosition.Y + " z: " + (int)ship.modelPosition.Z, shipLocationTextPos, Color.White);
            // spriteBatch.DrawString(shipInfo, "Ship Orientation: x: " + (int)ship.modelOrientation.X+ " y: " + (int)ship.modelOrientation.Y + " z: " + (int)ship.modelOrientation.Z, shipLocationTextPos, Color.White);
            spriteBatch.DrawString(shipInfo, "Ship Linear Momentum: x: " + (int)ship.linearMomentum.X + " y: " + (int)ship.linearMomentum.Y + " z: " + (int)ship.linearMomentum.Z, shipLinearMomentumTextPos, Color.White);
            spriteBatch.DrawString(shipInfo, "Ship Linear Velocity: x: " + (int)ship.linearVelocity.X + " y: " + (int)ship.linearVelocity.Y + " z: " + (int)ship.linearVelocity.Z, shipLinearVelocityTextPos, Color.White);
            spriteBatch.DrawString(shipInfo, "Ship Angular Momentum: x: " + (int)ship.angularMomentum.X + " y: " + (int)ship.angularMomentum.Y + " z: " + (int)ship.angularMomentum.Z, shipAngularMomentumTextPos, Color.White);
            spriteBatch.DrawString(shipInfo, "Ship Angular Velocity: x: " + (int)ship.angularVelocity.X + " y: " + (int)ship.angularVelocity.Y + " z: " + (int)ship.angularVelocity.Z, shipAngularVelocityTextPos, Color.White);

            spriteBatch.DrawString(shipInfo, "Target Location: x: " + (int)turtle.modelPosition.X + " y: " + (int)turtle.modelPosition.Y + " z: " + (int)turtle.modelPosition.Z, jellyfishLocationTextPos, Color.White);
            //spriteBatch.DrawString(shipInfo, "Target Linear Momentum: x: " + (int)jellyfish.linearMomentum.X + " y: " + (int)jellyfish.linearMomentum.Y + " z: " + (int)jellyfish.linearMomentum.Z, jellyfishLinearMomentumTextPos, Color.White);
            //spriteBatch.DrawString(shipInfo, "Target Linear Velocity: x: " + (int)jellyfish.linearVelocity.X + " y: " + (int)jellyfish.linearVelocity.Y + " z: " + (int)jellyfish.linearVelocity.Z, jellyfishLinearVelocityTextPos, Color.White);
            //spriteBatch.DrawString(shipInfo, "Target Angular Momentum: x: " + (int)jellyfish.angularMomentum.X + " y: " + (int)jellyfish.angularMomentum.Y + " z: " + (int)jellyfish.angularMomentum.Z, jellyfishAnuglarMomentumTextPos, Color.White);
            //spriteBatch.DrawString(shipInfo, "Target Angular Velocity: x: " + (int)jellyfish.angularVelocity.X + " y: " + (int)jellyfish.angularVelocity.Y + " z: " + (int)jellyfish.angularVelocity.Z, jellyfishAngularVelocityTextPos, Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
