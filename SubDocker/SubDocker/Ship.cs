using BEPUphysics;
using BEPUphysics.BroadPhaseEntries.MobileCollidables;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;
using System.Collections.Generic;

namespace SpaceDocker
{
    public class Ship : DrawableGameComponent
    {
        private int difficulty = 1;

        // variables that scale with difficulty
        private int health;
        private int maxHealth;
        private int duckDamage;

        private int torpedoCount;
        private int maxTorpedoCount;

        private float fuelLevel;
        private float maxFuelLevel;

        private float fuelPackValue;

        private float thrustDeduction;

        private float speed = 1f;
        private Vector3 maxVelocityToWin = Vector3.Zero;

        // mode flags
        private bool fireMode = false;
        private String conclusionMessage = null;

        // Base Requirements
        private Model model;
        private BEPUphysics.Entities.Prefabs.Sphere physicsObject;

        // Input Methods
        GamePadState currentGamePadState;
        KeyboardState currentKeyboardState;

        // Sprite Stuff
        SpriteBatch spriteBatch;
        SpriteFont annoucement;
        SpriteFont shipInfo;

        private Game game;

        // text locations
        Vector2 healthTextPos;
        Vector2 fuelTextPos;
        Vector2 torpedoTextPos;
        Vector2 fireModeTextPos;
        Vector2 goalReachedTextPos;

        List<Torepedo> allTorpedos;
        public float torpedoSpeed;

        // helper class
        private Helpers helper;

        public Vector3 modelPosition
        {
            get { return ConversionHelper.MathConverter.Convert(physicsObject.Position); }
            set { physicsObject.Position = ConversionHelper.MathConverter.Convert(value); }
        }

        public Quaternion modelOrientation
        {
            get { return ConversionHelper.MathConverter.Convert(physicsObject.Orientation); }
            set { physicsObject.Orientation = ConversionHelper.MathConverter.Convert(value); }
        }

        public Vector3 linearMomentum
        {
            get { return ConversionHelper.MathConverter.Convert(physicsObject.LinearMomentum); }
            set { physicsObject.LinearMomentum = ConversionHelper.MathConverter.Convert(value); }
        }

        public Vector3 linearVelocity
        {
            get { return ConversionHelper.MathConverter.Convert(physicsObject.LinearVelocity); }
            set { physicsObject.LinearVelocity = ConversionHelper.MathConverter.Convert(value); }
        }

        public Vector3 angularVelocity
        {
            get { return ConversionHelper.MathConverter.Convert(physicsObject.AngularVelocity); }
            set { physicsObject.AngularVelocity = ConversionHelper.MathConverter.Convert(value); }
        }

        public Vector3 angularMomentum
        {
            get { return ConversionHelper.MathConverter.Convert(physicsObject.AngularMomentum); }
            set { physicsObject.AngularMomentum = ConversionHelper.MathConverter.Convert(value); }
        }

        public Matrix shipWorld
        {
            get { return ConversionHelper.MathConverter.Convert(physicsObject.WorldTransform); }
            set { physicsObject.WorldTransform = ConversionHelper.MathConverter.Convert(value); }
        }

        public Ship(Game game) : base(game)
        {
            game.Components.Add(this);
        }

        public Ship(Game game, Vector3 pos, string id) : this(game)
        {
            helper = new Helpers();

            physicsObject = new BEPUphysics.Entities.Prefabs.Sphere(ConversionHelper.MathConverter.Convert(pos), 1);
            physicsObject.LinearDamping = .1f;
            physicsObject.AngularDamping = .2f;
            physicsObject.CollisionInformation.Events.InitialCollisionDetected += Events_InitialCollisionDetected;
            physicsObject.Tag = id;

            this.game = game;

            InitalOrientation();
            GenerateTorpedos();

            Game.Services.GetService<Space>().Add(physicsObject);

            InitItemDifficulty(this.difficulty); // defaults to 1
        }

        private void GenerateTorpedos()
        {
            allTorpedos = new List<Torepedo>();

            // initalize max # of torpedos
            for (int i = 0; i < maxTorpedoCount; i++)
            {
                Torepedo currentTorepedo = new Torepedo(game, this, "torpedo " + i);
                allTorpedos.Add(currentTorepedo);
            }
        }

        private void RemoveTorepedos()
        {
            foreach (Torepedo t in allTorpedos)
            {
                t.RemoveFromGame();
            }
        }

        private void InitalOrientation()
        {
            // orient ship in a way that you can use Vector.x on the camera
            Matrix rotation = Matrix.CreateRotationX(MathHelper.ToRadians(85));
            Quaternion rot = Quaternion.CreateFromRotationMatrix(rotation);
            modelOrientation *= rot;
        }

        public Ship(Game game, Vector3 pos, string id, int mass) : this(game, pos, id)
        {
            physicsObject.Mass = mass;
        }

        public Ship(Game game, Vector3 pos, string id, int mass, int difficulty) : this(game, pos, id, mass)
        {
            this.difficulty = difficulty;
        }

        private void Events_InitialCollisionDetected(BEPUphysics.BroadPhaseEntries.MobileCollidables.EntityCollidable sender, BEPUphysics.BroadPhaseEntries.Collidable other, BEPUphysics.NarrowPhaseSystems.Pairs.CollidablePairHandler pair)
        {

            // issues with motion upon impact with other objects
            Console.WriteLine("Ship Collision");
            var otherEntityInformation = other as EntityCollidable;
            string tag = (string)otherEntityInformation.Entity.Tag;

            if (tag.Contains("duck"))
            {
                Console.WriteLine("Hit DUCK");
                health -= duckDamage;
            }
            if (tag.Equals("jellyfish"))
            {
                Console.WriteLine("Hit Jellyfish");
                if (Vector3.Distance(linearVelocity, maxVelocityToWin) <= 2f)

                {
                    conclusionMessage = "You successfully claimed the sacred Jellyfish!";
                }
                else
                {
                    conclusionMessage = "Your ship was going too fast! Failed to safely aquire the jellyfish.";
                }
            }
            if (tag.Equals("fuelPack"))
            {
                float newValue = fuelLevel + fuelPackValue;

                // can't go over 100 fuel
                if (newValue > 100)
                {
                    fuelLevel = 100;
                }
                else
                {
                    fuelLevel = newValue;
                }
            }
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            model = Game.Content.Load<Model>("SpaceFighter");

            // load fonts
            annoucement = Game.Content.Load<SpriteFont>("annoucement");
            shipInfo = Game.Content.Load<SpriteFont>("shipInfo");

            // crate physics object
            physicsObject.Radius = model.Meshes[0].BoundingSphere.Radius;

            // init ship diagnostics locations
            healthTextPos = new Vector2((float)(GraphicsDevice.Viewport.Width / 24), (float)(GraphicsDevice.Viewport.Height - 100));
            fuelTextPos = new Vector2((float)(GraphicsDevice.Viewport.Width / 24), (float)(GraphicsDevice.Viewport.Height - 150));
            torpedoTextPos = new Vector2((float)(GraphicsDevice.Viewport.Width / 24), (float)(GraphicsDevice.Viewport.Height - 200));
            fireModeTextPos = new Vector2((float)(GraphicsDevice.Viewport.Width / 24), (float)(GraphicsDevice.Viewport.Height - 250));
            goalReachedTextPos = new Vector2((float)(GraphicsDevice.Viewport.Width / 2), (float)(GraphicsDevice.Viewport.Height / 2));


            base.LoadContent();
        }


        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            // handle notificatons of game ending states
            if (health <= 0)
            {
                conclusionMessage = "Failed to aquire the jellyfish! You ran out of health. The Rubber Ducks captured you!";
            }
            else if ((float)Math.Floor(fuelLevel) <= 0)
            {
                conclusionMessage = "Failed to aquire the jellyfish! You ran out of fuel. The Rubber Ducks captured you!";
            }

            if (conclusionMessage != null)
            {
                spriteBatch.DrawString(annoucement, conclusionMessage, goalReachedTextPos, Color.White);
            }

            // show ship diagnostics
            if (torpedoCount > 0)
            {
                spriteBatch.DrawString(shipInfo, "Torpedos Left: " + torpedoCount, torpedoTextPos, LevelStatus(torpedoCount, maxTorpedoCount));
            }
            else
            {
                spriteBatch.DrawString(shipInfo, "NO TORPEDOS", torpedoTextPos, Color.Red);
            }

            spriteBatch.DrawString(shipInfo, "Health: " + health, healthTextPos, LevelStatus(health, maxHealth));
            spriteBatch.DrawString(shipInfo, "Fuel Level: " + fuelLevel, fuelTextPos, LevelStatus(fuelLevel, maxFuelLevel));
            spriteBatch.DrawString(shipInfo, "Fire Mode Engaged: " + fireMode, fireModeTextPos, Color.White);

            spriteBatch.End();

            // draw model
            foreach (var mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.Alpha = 0.7f;
                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;
                    effect.World = ConversionHelper.MathConverter.Convert(physicsObject.WorldTransform);
                    effect.View = Main.camera.View;
                    effect.Projection = Main.camera.Projection;
                }
                mesh.Draw();
            }
            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            currentGamePadState = GamePad.GetState(PlayerIndex.One);
            currentKeyboardState = Keyboard.GetState();

            // Thrust in direction facing
            if (currentKeyboardState.IsKeyDown(Keys.T) || currentGamePadState.Buttons.Y == ButtonState.Pressed)
            {
                if (fuelLevel >= thrustDeduction)
                {
                    Vector3 displacement = Vector3.Up * speed;
                    Vector3 tempMomentum =  linearMomentum + Vector3.Transform(displacement, Matrix.CreateFromQuaternion(modelOrientation));
                    linearMomentum = helper.CheckLinearMomentumBounds(tempMomentum);
                    fuelLevel -= thrustDeduction;
                }
            }

            // ROTATION LOGIC

            // Yaw left (ie. rotate to left)
            if (currentKeyboardState.IsKeyDown(Keys.Q))
            {
                angularMomentum = helper.CheckAngularMomentumBounds(angularMomentum, "YL");
            }

            // Yaw right (ie. rotate to right)
            if (currentKeyboardState.IsKeyDown(Keys.W))
            {
                angularMomentum = helper.CheckAngularMomentumBounds(angularMomentum, "YR");
            }

            // pitch left (-)
            if (currentKeyboardState.IsKeyDown(Keys.Left))
            {
                angularMomentum = helper.CheckAngularMomentumBounds(angularMomentum, "PL");
            }

            // pitch right (+)
            if (currentKeyboardState.IsKeyDown(Keys.Right))
            {
                angularMomentum = helper.CheckAngularMomentumBounds(angularMomentum, "PR");
            }

            // roll forward
            if (currentKeyboardState.IsKeyDown(Keys.Up))
            {
                angularMomentum = helper.CheckAngularMomentumBounds(angularMomentum, "RF");
            }

            // roll backward
            if (currentKeyboardState.IsKeyDown(Keys.Down))
            {
                angularMomentum = helper.CheckAngularMomentumBounds(angularMomentum, "RB");
            }


            // FIREING LOGIC

            // enter fire mode
            if (currentKeyboardState.IsKeyDown(Keys.F) || currentGamePadState.Buttons.B == ButtonState.Pressed)
            {
                if (!fireMode && torpedoCount != 0)
                {
                    // engage fire mode
                    fireMode = true;
                }
            }

            if (currentKeyboardState.IsKeyDown(Keys.Space) && fireMode)
            {
                allTorpedos[0].ShootTorpedo();
                allTorpedos.RemoveAt(0);  // remove from list of avalible torpedos
            }

            // move aim right
            if (currentKeyboardState.IsKeyDown(Keys.P) || currentGamePadState.DPad.Right == ButtonState.Pressed)
            {
                allTorpedos[0].RotateRight();
            }

            // move aim left
            if (currentKeyboardState.IsKeyDown(Keys.O) || currentGamePadState.DPad.Left == ButtonState.Pressed)
            {
                allTorpedos[0].RotateLeft();
            }


            // exit fire mode
            if (currentKeyboardState.IsKeyDown(Keys.G))
            {
                if (fireMode)
                {
                    fireMode = false;
                }
            }

            base.Update(gameTime);
        }


        private Color LevelStatus(float objMeasuring, float maxLevel)
        {
            if (objMeasuring >= (maxLevel / 2))
            {
                return Color.Green;
            }
            else if (objMeasuring >= (maxLevel / 4))
            {
                return Color.Yellow;
            }
            else
            {
                return Color.OrangeRed;
            }
        }

        private void InitItemDifficulty(int difficulty)
        {
            // level choser
            switch (difficulty)
            {
                case 1:
                    torpedoCount = maxTorpedoCount = 5;
                    maxFuelLevel = fuelLevel = 100;
                    health = maxHealth = 100;
                    duckDamage = 10;
                    thrustDeduction = .5f;
                    torpedoSpeed = 10f;
                    fuelPackValue = 40;
                    return;
                case 2:
                    Console.WriteLine("Level 2 Not Implemented");
                    return;
                case 3:
                    Console.WriteLine("Level 3 Not Implemented");
                    return;
            }
        }

        public void Reset()
        {
            InitItemDifficulty(difficulty);
            modelPosition = Vector3.Zero;
            modelOrientation = Quaternion.Identity;
            linearMomentum = Vector3.Zero;
            angularMomentum = Vector3.Zero;

            InitalOrientation();
            // RemoveTorepedos();
            // GenerateTorpedos();

            conclusionMessage = null;
            fireMode = false;
        }
    }
}
