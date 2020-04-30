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

        private int maxTorpedoCount;

        private float fuelLevel;
        private float maxFuelLevel;

        private float fuelPackValue;

        private float thrustDeduction;

        private float speed = 20f;
        private Vector3 maxVelocityToWin = Vector3.Zero;
        private int currentTorpdoIndex;

        // mode flags
        private bool fireMode = false;
        private String conclusionMessage = null;
        private String shiledActiveMessage = null;

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
        Vector2 generalInfoPos;
        Vector2 shiledActiveTextPos;
        private string generalInfoMessage;

        Vector2 boundsTextPos;
        private string boundsMessage;

        private bool shieldActive;
        private float shieldReduction;

        List<Torpedo> allTorpedos;
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

            InitItemDifficulty(this.difficulty); // defaults to 1

            InitalOrientation();
            GenerateTorpedos();
            shieldActive = false;

            Game.Services.GetService<Space>().Add(physicsObject);
        }

        /// <summary>
        /// Generates all the torpedos that the ship has stored on the ship
        /// </summary>
        private void GenerateTorpedos()
        {
            allTorpedos = new List<Torpedo>();

            // initalize max # of torpedos
            for (int i = 0; i < maxTorpedoCount; i++)
            {
                Torpedo currentTorpedo = new Torpedo(game, torpedoSpeed, "torpedo" + i);
                allTorpedos.Add(currentTorpedo);
            }
            currentTorpdoIndex = maxTorpedoCount;
        }

        /// <summary>
        /// Remove any leftover torpeods from the ship
        /// </summary>
        private void RemoveTorpedos()
        {
            foreach (Torpedo t in allTorpedos)
            {
                t.RemoveFromGame();
            }
        }

        /// <summary>
        /// have to change the orientation of the ship, as the model isn't facing in the right direction
        /// </summary>
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
                if (!shieldActive)
                {
                    health -= duckDamage;
                    generalInfoMessage = "DAMAGE TAKEN";
                }
            }
            if (tag.Equals("turtle"))
            {
                Console.WriteLine("Hit Turtle");
                if (Vector3.Distance(linearVelocity, maxVelocityToWin) <= 2f)

                {
                    conclusionMessage = "You successfully claimed the sacred Turtle!";
                }
                else
                {
                    conclusionMessage = "Your ship was going too fast! Failed to safely aquire the Turtle.";
                }
            }
            if (tag.Contains("fuelPack"))
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
                generalInfoMessage = "FUEL PACK AQUIRED ";
            }
            if (tag.Contains("torpedoPack"))
            {
                // TODO - broken
                // AddTorpedo(); 
                generalInfoMessage = "TORPEDO PACK AQUIRED";
            }
        }

        /// <summary>
        /// Add another torpedos to the list of torpedos that the ship has
        /// </summary>
        private void AddTorpedo()
        {
            if (allTorpedos.Count == 0)
            {
                Console.WriteLine("AllTorpedo List not Initalized");
                return;
            }

            Torpedo currentTorpedo = new Torpedo(game, torpedoSpeed, "torpedo" + currentTorpdoIndex);
            allTorpedos.Add(currentTorpedo);
            currentTorpdoIndex++;
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
            
            fireModeTextPos = new Vector2((float)(GraphicsDevice.Viewport.Width / 24), (float)(GraphicsDevice.Viewport.Height - 300));
            goalReachedTextPos = new Vector2((float)(GraphicsDevice.Viewport.Width / 24), (float)(GraphicsDevice.Viewport.Height - (GraphicsDevice.Viewport.Height - 100)));
            shiledActiveTextPos = new Vector2((float)(GraphicsDevice.Viewport.Width / 24), (float)(GraphicsDevice.Viewport.Height - 350));

            generalInfoPos = new Vector2((float)(GraphicsDevice.Viewport.Width / 24), (float)(GraphicsDevice.Viewport.Height - 400));
            boundsTextPos = new Vector2((float)(GraphicsDevice.Viewport.Width - 800), (float)(GraphicsDevice.Viewport.Height - 200));

            base.LoadContent();
        }


        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            // handle notificatons of game ending states
            if (health <= 0)
            {
                conclusionMessage = "Failed to aquire the turtle! You ran out of health. The Rubber Ducks captured you!";
            }
            else if ((float)Math.Floor(fuelLevel) <= 0)
            {
                conclusionMessage = "Failed to aquire the turtle! You ran out of fuel. The Rubber Ducks captured you!";
            }

            if (conclusionMessage != null)
            {
                spriteBatch.DrawString(annoucement, conclusionMessage, goalReachedTextPos, Color.Orange);
            }

            // show ship diagnostics
            if (allTorpedos.Count > 0)
            {
                spriteBatch.DrawString(shipInfo, "Torpedos Left: " + allTorpedos.Count, torpedoTextPos, LevelStatus(allTorpedos.Count, maxTorpedoCount));
            }
            else
            {
                spriteBatch.DrawString(shipInfo, "NO TORPEDOS", torpedoTextPos, Color.Red);
            }

            spriteBatch.DrawString(shipInfo, "Health: " + health, healthTextPos, LevelStatus(health, maxHealth));
            spriteBatch.DrawString(shipInfo, "Fuel Level: " + fuelLevel, fuelTextPos, LevelStatus(fuelLevel, maxFuelLevel));
            
            // show fire mode identifier
            if (fireMode)
            {
                spriteBatch.DrawString(shipInfo, "FIRE MODE ACTIVE", fireModeTextPos, Color.DarkMagenta);
            }
           
            // show gneral information if avialble
            if (generalInfoMessage != null)
            {
                spriteBatch.DrawString(shipInfo, generalInfoMessage, generalInfoPos, Color.Orange);
            }

            // show active shield message if active
            if (shiledActiveMessage != null)
            {
                spriteBatch.DrawString(shipInfo, shiledActiveMessage, shiledActiveTextPos, Color.DarkMagenta);
            }

            if(boundsMessage != null)
            {
                spriteBatch.DrawString(shipInfo, boundsMessage, boundsTextPos, Color.Red);
            }

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

        private void CheckBounds()
        {
            int boundValue = 500;
            if (modelPosition.X > boundValue)
            {
                modelPosition = new Vector3(boundValue, modelPosition.Y, modelPosition.Z);
                boundsMessage = "HIT X BOUND";
            }
            if (modelPosition.X < -boundValue)
            {
                modelPosition = new Vector3(-boundValue, modelPosition.Y, modelPosition.Z);
                boundsMessage = "HIT -X BOUND";
            }

            if (modelPosition.Y > boundValue)
            {
                modelPosition = new Vector3(modelPosition.X, boundValue, modelPosition.Z);
                boundsMessage = "HIT Y BOUND";
            }
            if (modelPosition.Y < -boundValue)
            {
                modelPosition = new Vector3(modelPosition.X, -boundValue, modelPosition.Z);
                boundsMessage = "HIT -Y BOUND";
            }

            if (modelPosition.Z > boundValue)
            {
                modelPosition = new Vector3(modelPosition.X, modelPosition.Y, boundValue);
                boundsMessage = "HIT Z BOUND";
            }
            if (modelPosition.Z < -boundValue)
            {
                modelPosition = new Vector3(modelPosition.X, modelPosition.Y , -boundValue);
                boundsMessage = "HIT -Z BOUND";
            }
        }

        public override void Update(GameTime gameTime)
        {
            currentGamePadState = GamePad.GetState(PlayerIndex.One);
            currentKeyboardState = Keyboard.GetState();

            CheckBounds();

            // Thrust in direction facing
            if (currentKeyboardState.IsKeyDown(Keys.T) || currentGamePadState.Buttons.Y == ButtonState.Pressed)
            {
                if(fuelLevel > 0)
                {
                    Vector3 displacement = Vector3.Up * speed;
                    Vector3 tempMomentum = linearMomentum + Vector3.Transform(displacement, Matrix.CreateFromQuaternion(modelOrientation));
                    linearMomentum = helper.CheckLinearMomentumBounds(tempMomentum);
                    fuelLevel -= thrustDeduction;
                    generalInfoMessage = null;
                    boundsMessage = null;
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

            // SHIELD LOGIC

            // put up shield
            if (currentKeyboardState.IsKeyDown(Keys.S))
            {
                if((fuelLevel - shieldReduction) < 0)
                {
                    shiledActiveMessage = "NOT ENOUGH FUEL";
                }
                else
                {
                    shieldActive = true;
                    shiledActiveMessage = "SHIELD IS ACTIVE";
                }
            }

            // remove shield
            if (currentKeyboardState.IsKeyDown(Keys.D))
            {
                if (shieldActive)
                {
                    shiledActiveMessage = null;
                    shieldActive = false;
                }
            }

            if (shieldActive)
            {
                // use fuel use shield
                float tempValue = fuelLevel - shieldReduction;
                if (tempValue < 0)
                {
                    shiledActiveMessage = null;
                    shieldActive = false;
                }
                else
                {
                    fuelLevel = tempValue;
                }
            }

            // FIREING LOGIC

            // enter fire mode
            if (currentKeyboardState.IsKeyDown(Keys.F) || currentGamePadState.Buttons.B == ButtonState.Pressed)
            {
                if (!fireMode && allTorpedos.Count != 0)
                {
                    // engage fire mode
                    fireMode = true;
                }
            }

            if (currentKeyboardState.IsKeyDown(Keys.Space) && fireMode)
            {
                if (allTorpedos.Count != 0)
                {
                    allTorpedos[0].ShootTorpedo();
                    allTorpedos.RemoveAt(0);  // remove from list of avalible torpedos
                }
                fireMode = false;
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


        /// <summary>
        /// Helper function that changes the color of text based on the value left scaled to the total possible
        /// </summary>
        /// <param name="objMeasuring"></param>
        /// <param name="maxLevel"></param>
        /// <returns></returns>
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

        /// <summary>
        ///  Determines the game scaling of items based on requested difficulty
        /// </summary>
        /// <param name="difficulty"></param>
        private void InitItemDifficulty(int difficulty)
        {
            // level choser
            switch (difficulty)
            {
                case 1:
                    maxTorpedoCount = 5;
                    maxFuelLevel = fuelLevel = 100;
                    health = maxHealth = 100;
                    duckDamage = 10;
                    thrustDeduction = .5f;
                    torpedoSpeed = 10f;
                    fuelPackValue = 40;
                    shieldReduction = .005f;
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
        /// Removes all objects from the game, resets position of the ship and skybox, and re randomizes target location
        /// </summary>
        public void Reset()
        {
            InitItemDifficulty(difficulty);
            modelPosition = Vector3.Zero;
            modelOrientation = Quaternion.Identity;
            linearMomentum = Vector3.Zero;
            angularMomentum = Vector3.Zero;

            InitalOrientation();
            // TODO - having issues with a bug where the physics object is nulll
            // RemoveTorpedos();
            // GenerateTorpedos();

            conclusionMessage = null;
            fireMode = false;
        }
    }
}
