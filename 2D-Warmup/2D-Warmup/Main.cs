using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Galaga_Warmup
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Galaga : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // objects
        Texture2D ship;
        Texture2D missile;
        Texture2D enemy;

        // dimensions for enemy movement box
        Vector2 upperLeft;
        Vector2 upperRight;
        Vector2 lowerLeft;
        Vector2 lowerRight;

        // positions and velocity for objects
        Vector2 shipPos;
        Vector2 shipVel;

        Vector2 missilePos;
        Vector2 missileVel;

        Vector2 enemyPos;
        Vector2 enemeyVel;

        SpriteFont spaceFont;
        Vector2 scoreTextPos;
        Vector2 newGameTextPos;

        Color[] missileData;
        Color[] enemyData;

        Color[,] missileColor;
        Color[,] enemyColor;


        bool colliding = false;
        float fence;
        bool missileShot = false;
        int missilesUsed = 0;

        public Galaga()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 500;
            graphics.PreferredBackBufferHeight = 700;
            graphics.ApplyChanges();
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
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            spaceFont = Content.Load<SpriteFont>("spaceFont");

            // load content
            ship = Content.Load<Texture2D>("ship");
            missile = Content.Load<Texture2D>("missile");
            enemy = Content.Load<Texture2D>("enemy");

            // set init ship position
            shipPos = new Vector2((float)(GraphicsDevice.Viewport.Width / 2), (float)(GraphicsDevice.Viewport.Height - 200));

            missilePos = shipPos;

            // set init enemy position
            enemyPos = new Vector2(30f, 200f);

            // set movement velocities
            shipVel = new Vector2(2f);
            enemeyVel = new Vector2(1f);
            missileVel = new Vector2(6f);

            // game boarder
            fence = 10;

            // enemy boarders
            upperLeft = new Vector2(fence, fence);
            upperRight = new Vector2(GraphicsDevice.Viewport.Width - fence, fence);
            lowerLeft = new Vector2(fence, GraphicsDevice.Viewport.Height - 300);
            lowerRight = new Vector2(GraphicsDevice.Viewport.Width - fence, GraphicsDevice.Viewport.Height - 300);


            scoreTextPos = new Vector2(30f, GraphicsDevice.Viewport.Height - 50);
            newGameTextPos = new Vector2(80f, (float)(GraphicsDevice.Viewport.Height / 2));

            // get color data
            missileData = new Color[missile.Width * missile.Height];
            enemyData = new Color[enemy.Width * enemy.Height];

            missile.GetData<Color>(missileData);
            enemy.GetData<Color>(enemyData);

            // create color arrays
            missileColor = new Color[missile.Width, missile.Height];
            enemyColor = new Color[enemy.Width, enemy.Height];

            // fill missile color array
            for (int x = 0; x < missile.Width; x++)
            {
                for (int y = 0; y < missile.Height; y++)
                {
                    missileColor[x, y] = missileData[x + y * missile.Width];
                }
            }

            // fill enemy color array
            for (int x = 0; x < enemy.Width; x++)
            {
                for (int y = 0; y < enemy.Height; y++)
                {
                    enemyColor[x, y] = enemyData[x + y * enemy.Width];
                }
            }

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
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

            KeyboardState currentKeyState = Keyboard.GetState();

            // reset game
            if (currentKeyState.IsKeyDown(Keys.N))
            {
                missileShot = false;
                colliding = false;
                shipPos = new Vector2((float)(GraphicsDevice.Viewport.Width / 2), (float)(GraphicsDevice.Viewport.Height - 200));
                enemyPos = new Vector2(30f, 200f);
                missilePos = shipPos;
                missilesUsed = 0;
            }

            if (!colliding)
            {

                // SHIP LOGIC

                // move ship to the left
                if (currentKeyState.IsKeyDown(Keys.Left))
                {
                    // hit left bound
                    if ((shipPos.X - shipVel.X - fence) > 0)
                    {
                        shipPos.X -= shipVel.X;
                    }
                }

                // move ship to the right
                if (currentKeyState.IsKeyDown(Keys.Right))
                {
                    // hit right bound
                    if ((shipPos.X + shipVel.X + fence + ship.Width) < GraphicsDevice.Viewport.Width)
                    {
                        shipPos.X += shipVel.X;
                    }
                }

                // MISSILE LOGIC

                // can't shoot more than 1 missle
                // triggers missle movement
                if (currentKeyState.IsKeyDown(Keys.Space) && !missileShot)
                {
                    missilesUsed++;
                    missileShot = true;
                    missilePos = shipPos;
                }

                // missle shot
                if (missileShot)
                {
                    if ((missilePos.Y - missileVel.Y) <= 0)
                    {
                        missileShot = false;
                        missilePos = shipPos;
                    }
                    else
                    {
                        missilePos.Y -= missileVel.Y;
                    }
                }

                // resets missile location
                if (currentKeyState.IsKeyDown(Keys.R))
                {
                    missileShot = false;
                    missilePos = shipPos;
                }

                // ENEMY LOGIC

                enemyPos += enemeyVel;

                if (enemyPos.Y  <= upperLeft.Y)
                {
                    enemeyVel = Vector2.Reflect(enemeyVel, Vector2.UnitY);
                }

                if (enemyPos.Y + enemy.Height >= lowerLeft.Y)
                {
                    enemeyVel = Vector2.Reflect(enemeyVel, Vector2.UnitY); 
                }

                if(enemyPos.X >= 450)
                {
                    int x = 0;
                }
                // upperLeft (10. 10)
                // upperRight (490, 10)
                // lowerLeft (10, 400)
                // lowerRight (490, 400)

                float m = lowerLeft.Y - upperLeft.Y;
                float b = (upperLeft.Y - (m * upperLeft.X));
                float xcheck = (enemyPos.Y - b) / m;

                if (enemyPos.X <= xcheck)
                {
                    Vector2 slope = upperLeft - lowerLeft;
                    slope.Normalize();
                    Vector3 normal3D = Vector3.Cross(new Vector3(slope, 0), new Vector3(0, 0, -1));
                    Vector2 norm = new Vector2(normal3D.X, normal3D.Y);
                    enemeyVel = Vector2.Reflect(enemeyVel, norm);
                }

                m = lowerRight.Y - upperRight.Y;
                b = (upperRight.Y - (m * upperRight.X));
                xcheck = (enemyPos.Y - b) / m;

                if (enemyPos.X + enemy.Width >= xcheck)
                {
                    Vector2 slope = upperRight - lowerRight;
                    slope.Normalize();
                    Vector3 normal3D = Vector3.Cross(new Vector3(slope, 0), new Vector3(0, 0, -1));
                    Vector2 norm = new Vector2(normal3D.X, normal3D.Y);
                    enemeyVel = Vector2.Reflect(enemeyVel, norm);
                }


                // COLLISION LOGIC

                // make hit boxes around images
                Rectangle c1 = new Rectangle((int) missilePos.X, (int) missilePos.Y, (int) missile.Width, (int) missile.Height);
                Rectangle c2 = new Rectangle((int) enemyPos.X, (int) enemyPos.Y, (int) enemy.Width, (int) enemy.Height);

                if (c1.Intersects(c2))
                {
                    Rectangle intersect = Rectangle.Intersect(c1, c2);

                    // look at interseting area
                    for (int x = intersect.X; x < intersect.X + intersect.Width; x++)
                    {
                        for (int y = intersect.Y; y < intersect.Y + intersect.Height; y++)
                        {
                            int a1 = missileColor[x - (int)missilePos.X, y - (int)missilePos.Y].A;
                            int a2 = enemyColor[x - (int)enemyPos.X, y - (int)enemyPos.Y].A;
                            // compare pixel color, if they are different then collided
                            if ((a1 != 0) && (a2 != 0))
                            {
                                colliding = true;
                                missileShot = false;
                                missilePos = shipPos;
                                break;
                            }
                        }
                        if (colliding)
                        {
                            break;
                        }
                    }
                }
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();

            spriteBatch.Draw(ship, shipPos, Color.White);
            // only show missile if shot
            if (missileShot)
            {
                spriteBatch.Draw(missile, missilePos, Color.White);
            }
            // only show enemy if not hit
            if (!colliding) {
                spriteBatch.Draw(enemy, enemyPos, Color.White);
            }

            spriteBatch.DrawString(spaceFont, "Missiles Used: " + missilesUsed, scoreTextPos, Color.White);
            if (colliding)
            {
                spriteBatch.DrawString(spaceFont, "Press 'N' for New Game.", newGameTextPos, Color.White);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
