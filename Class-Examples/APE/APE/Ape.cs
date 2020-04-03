using BEPUphysics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace AsteroidsPhysicsExample
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Ape : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public static Vector3 CameraPosition
        {
            get;
            private set;
        }

        public static Vector3 CameraDirection
        {
            get;
            private set;
        }

        public Ape()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            // Make our BEPU Physics space a service
            Services.AddService<Space>(new Space());

            // Create two asteroids.  Note that asteroids automatically add themselves to
            // as a DrawableGameComponent as well as add an object into Bepu physics
            // that represents the asteroid.

            new Asteroid(this, new Vector3(-2, 0, -5), "A", 2, new Vector3(0.2f, 0, 0), new Vector3(0.3f, 0.5f, 0.5f));
            new Asteroid(this, new Vector3(2, 0, -5), "B", 3, new Vector3(-0.2f, 0, 0), new Vector3(-0.5f, -0.6f, 0.2f));

            CameraPosition = new Vector3(0, 0, 0);
            CameraDirection = Vector3.Forward;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();


            Services.GetService<Space>().Update((float)gameTime.ElapsedGameTime.TotalSeconds);


            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            base.Draw(gameTime);
        }
    }
}
