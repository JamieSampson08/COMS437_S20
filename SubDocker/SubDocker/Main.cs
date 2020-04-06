using BEPUphysics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;

namespace SpaceDocker
{
    public class Main : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Random rnd = new Random();

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
        SpriteFont shipInfo;

        private int difficulty = 1;
        private int numDuckGeneration = 40; // make scale with difficulty

        //public static Camera camera { get; set; }
        public static Camera2 camera { get; set; }
        public static Ship ship { get; set; }
        public static Jellyfish jellyfish { get; set; }

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


            GenerateRubberDucks();

            // ship setup
            ship = new Ship(this, Vector3.Zero, "ship", 2, difficulty);

            // camera setup
            camera = new Camera2(this, ship.modelPosition - new Vector3(0, -.9f, 4f)); // right/left, behind/infront, above/below 
            // camera = new Camera(this);
            // new RubberDuck(this, new Vector3(0, 3f, 8f), "duck-0", 2);

            // jellyfish setup
            Vector3 distanceMultipler = new Vector3(100, 100, 100);
            Vector3 distanceBetween = (new Vector3(
                (float)rnd.Next(-(int)(distanceMultipler.X), (int)(distanceMultipler.X)),
                (float)rnd.Next(-(int)(distanceMultipler.Y), (int)(distanceMultipler.Y)),
                (float)rnd.Next(-(int)(distanceMultipler.Z), (int)distanceMultipler.Z))
                );

           // jellyfish = new Jellyfish(this, distanceBetween, "jellyfish");
           jellyfish = new Jellyfish(this, ship.modelPosition + new Vector3(0, 0, 20f), "jellyfish"); // for submition

            // skybox setup DOESN'T WORK YET
            // skybox = new Skybox(this, ship.modelPosition, "skybox");

            base.Initialize();
        }

        private void GenerateRubberDucks()
        {
            Vector3 angMomentum;
            Vector3 linMomentum;
            Vector3 pos;

            // randomly generates x rubber ducks
            for (int i = 0; i < numDuckGeneration; i++)
            {
                int minRange = -100;
                int maxRange = 100;
                angMomentum = new Vector3((float)rnd.NextDouble() * rnd.Next(-2, 2), (float)rnd.NextDouble() * rnd.Next(-2, 2), (float)rnd.NextDouble() * rnd.Next(-2, 2));
                linMomentum = new Vector3((float)rnd.NextDouble() * rnd.Next(-20, 20), (float)rnd.NextDouble() * rnd.Next(-20, 20), (float)rnd.NextDouble() * rnd.Next(-20, 20));
                pos = new Vector3((float)rnd.NextDouble() * rnd.Next(minRange, maxRange), (float)rnd.NextDouble() * rnd.Next(minRange, maxRange), (float)rnd.NextDouble() * rnd.Next(minRange, maxRange));
                new RubberDuck(this, pos, "duck-" + i, 2, linMomentum, angMomentum);
            }
        }


        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            shipInfo = Content.Load<SpriteFont>("shipInfo");

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
                // remove all ducks from space and re add them
                // reset location of jellyfish
                // remove any torpedos
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
            // spriteBatch.DrawString(shipInfo, "Ship Orientation: x: " + (int)ship.modelOrientation.X+ " y: " + (int)ship.modelOrientation.Y + " z: " + (int)ship.modelOrientation.Z, shipLocationTextPos, Color.White);
            spriteBatch.DrawString(shipInfo, "Ship Linear Momentum: x: " + (int)ship.linearMomentum.X + " y: " + (int)ship.linearMomentum.Y + " z: " + (int)ship.linearMomentum.Z, shipLinearMomentumTextPos, Color.White);
            spriteBatch.DrawString(shipInfo, "Ship Linear Velocity: x: " + (int)ship.linearVelocity.X + " y: " + (int)ship.linearVelocity.Y + " z: " + (int)ship.linearVelocity.Z, shipLinearVelocityTextPos, Color.White);
            spriteBatch.DrawString(shipInfo, "Ship Angular Momentum: x: " + (int)ship.angularMomentum.X + " y: " + (int)ship.angularMomentum.Y + " z: " + (int)ship.angularMomentum.Z, shipAngularMomentumTextPos, Color.White);
            spriteBatch.DrawString(shipInfo, "Ship Angular Velocity: x: " + (int)ship.angularVelocity.X + " y: " + (int)ship.angularVelocity.Y + " z: " + (int)ship.angularVelocity.Z, shipAngularVelocityTextPos, Color.White);

            spriteBatch.DrawString(shipInfo, "Target Location: x: " + (int)jellyfish.modelPosition.X + " y: " + (int)jellyfish.modelPosition.Y + " z: " + (int)jellyfish.modelPosition.Z, jellyfishLocationTextPos, Color.White);
            spriteBatch.DrawString(shipInfo, "Target Linear Momentum: x: " + (int)jellyfish.linearMomentum.X + " y: " + (int)jellyfish.linearMomentum.Y + " z: " + (int)jellyfish.linearMomentum.Z, jellyfishLinearMomentumTextPos, Color.White);
            spriteBatch.DrawString(shipInfo, "Target Linear Velocity: x: " + (int)jellyfish.linearVelocity.X + " y: " + (int)jellyfish.linearVelocity.Y + " z: " + (int)jellyfish.linearVelocity.Z, jellyfishLinearVelocityTextPos, Color.White);
            spriteBatch.DrawString(shipInfo, "Target Angular Momentum: x: " + (int)jellyfish.angularMomentum.X + " y: " + (int)jellyfish.angularMomentum.Y + " z: " + (int)jellyfish.angularMomentum.Z, jellyfishAnuglarMomentumTextPos, Color.White);
            spriteBatch.DrawString(shipInfo, "Target Angular Velocity: x: " + (int)jellyfish.angularVelocity.X + " y: " + (int)jellyfish.angularVelocity.Y + " z: " + (int)jellyfish.angularVelocity.Z, jellyfishAngularVelocityTextPos, Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
