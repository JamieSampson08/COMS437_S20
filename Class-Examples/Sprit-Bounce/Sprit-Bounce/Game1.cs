using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Sprit_Bounce
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class BounceMain : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D colorStrip;
        Texture2D ball;
        Texture2D im;

        Vector2 upperLeft;
        Vector2 upperRight;
        Vector2 lowerLeft;
        Vector2 lowerRight;

        Rectangle northWall;
        Rectangle southWall;
        Rectangle eastWall;
        Rectangle westWall;
        Rectangle wallColor;

        float eastWallAngle;
        float westWallAngle;
        int lineThickness;

        Vector2 ballPos;
        Vector2 ballVel;

        Vector2 imPos;
        Vector2 imVel;

        Color[] ballData;
        Color[] imData;

        Color[,] ballColor;
        Color[,] imColor;

        SpriteFont dfont;
        Vector2 ballTextPos;

        bool colliding = true;

        Random rnd = new Random();


        public BounceMain()
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
            spriteBatch = new SpriteBatch(GraphicsDevice);
            dfont = Content.Load<SpriteFont>("File");


            lineThickness = 4;

            upperLeft = new Vector2(20, 20);
            upperRight = new Vector2(GraphicsDevice.Viewport.Width - 20, 20);
            lowerLeft = new Vector2(40, GraphicsDevice.Viewport.Height - 100);
            lowerRight = new Vector2(GraphicsDevice.Viewport.Width - 40, GraphicsDevice.Viewport.Height - 100);

            northWall = new Rectangle((int)(upperLeft.X - lineThickness), (int)(upperLeft.Y - lineThickness), (int)(upperRight.X - upperLeft.X), lineThickness);
            southWall = new Rectangle((int)lowerLeft.X, (int)lowerLeft.Y, (int)(lowerRight.X - lowerLeft.X), lineThickness);

            Vector2 eastWallVectorAngle = Vector2.Subtract(upperLeft, lowerLeft);
            eastWall = new Rectangle((int)upperLeft.X, (int)upperLeft.Y, (int)eastWallVectorAngle.Length() + lineThickness, lineThickness);
            eastWallAngle = (float)Math.Atan2(eastWallVectorAngle.Y, eastWallVectorAngle.X);

            Vector2 westWallVectorAngle = Vector2.Subtract(upperRight, lowerRight);
            westWall = new Rectangle((
                int)upperRight.X, (int)(upperRight.Y - lineThickness), (int)westWallVectorAngle.Length() + lineThickness, lineThickness);
            westWallAngle = (float)Math.Atan2(westWallVectorAngle.Y, westWallVectorAngle.X);


            colorStrip = Content.Load<Texture2D>("colorStrip");
            wallColor = new Rectangle(5, 0, 1, 1);

            ball = Content.Load<Texture2D>("ball");
            im = Content.Load<Texture2D>("b1");

            ballPos = new Vector2((float)((rnd.NextDouble() * ((upperRight.X - upperLeft.X) - ball.Width)) + upperLeft.X),
                (float)((rnd.NextDouble() * ((lowerRight.Y - upperRight.Y) - ball.Height)) + upperRight.Y));

            imPos = new Vector2((float)((rnd.NextDouble() * ((upperRight.X - upperLeft.X) - im.Width)) + upperLeft.X),
                (float)((rnd.NextDouble() * ((lowerRight.Y - upperRight.Y) - im.Height)) + upperRight.Y));

            ballVel = new Vector2((float)((rnd.NextDouble() - 0.5) * 19.0));
            imVel = new Vector2((float)((rnd.NextDouble() - 0.5) * 3.0));

            ballData = new Color[ball.Width * ball.Height];
            imData = new Color[im.Width * im.Height];

            ball.GetData<Color>(ballData);
            im.GetData<Color>(imData);

            ballColor = new Color[ball.Width, ball.Height];
            imColor = new Color[im.Width, im.Height];

            for (int x = 0; x < ball.Width; x++)
            {
                for (int y = 0; y < ball.Height; y++)
                {
                    ballColor[x, y] = ballData[x + y * ball.Width];
                }
            }

            for (int x = 0; x < im.Width; x++)
            {
                for (int y = 0; y < im.Height; y++)
                {
                    imColor[x, y] = imData[x + y * im.Width];
                }
            }

            ballTextPos = new Vector2(50, GraphicsDevice.Viewport.Height - 50);

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

            ballPos += ballVel;

            if (ballPos.Y <= upperLeft.Y)
            {
                ballVel = Vector2.Reflect(ballVel, Vector2.UnitY);
            }

            if (ballPos.Y + ball.Height >= lowerLeft.Y)
            {
                ballVel = Vector2.Reflect(ballVel, Vector2.UnitY);
            }

            float m = (lowerLeft.Y - upperLeft.Y) / (lowerLeft.X - upperLeft.X);
            float b = (upperLeft.Y - (m * upperLeft.X));
            float xcheck = (ballPos.Y - b) / m;

            if (ballPos.X <= xcheck)
            {
                Vector2 slope = upperLeft - lowerLeft;
                slope.Normalize();
                Vector3 normal3D = Vector3.Cross(new Vector3(slope, 0), new Vector3(0, 0, -1));
                Vector2 norm = new Vector2(normal3D.X, normal3D.Y);
                ballVel = Vector2.Reflect(ballVel, norm);
            }

            m = (lowerRight.Y - upperRight.Y) / (lowerRight.X - upperRight.X);
            b = (upperRight.Y - (m * upperRight.X));
            xcheck = (ballPos.Y - b) / m;

            if (ballPos.X + ball.Width >= xcheck)
            {
                Vector2 slope = upperRight - lowerRight;
                slope.Normalize();
                Vector3 normal3D = Vector3.Cross(new Vector3(slope, 0), new Vector3(0, 0, -1));
                Vector2 norm = new Vector2(normal3D.X, normal3D.Y);
                ballVel = Vector2.Reflect(ballVel, norm);
            }

            imPos += imVel;

            if (imPos.Y <= upperLeft.Y)
            {
                imVel = Vector2.Reflect(imVel, Vector2.UnitY);
            }

            if (imPos.Y + im.Height >= lowerLeft.Y)
            {
                imVel = Vector2.Reflect(imVel, Vector2.UnitY);
            }

            m = (lowerLeft.Y - upperLeft.Y) / (lowerLeft.X - upperLeft.X);
            b = (upperLeft.Y - (m * upperLeft.X));
            xcheck = (imPos.Y - b) / m;

            if (imPos.X <= xcheck)
            {
                Vector2 slope = upperLeft - lowerLeft;
                slope.Normalize();
                Vector3 normal3D = Vector3.Cross(new Vector3(slope, 0), new Vector3(0, 0, -1));
                Vector2 norm = new Vector2(normal3D.X, normal3D.Y);
                imVel = Vector2.Reflect(imVel, norm);
            }

            m = (lowerRight.Y - upperRight.Y) / (lowerRight.X - upperRight.X);
            b = (upperRight.Y - (m * upperRight.X));
            xcheck = (imPos.Y - b) / m;

            if (imPos.X + im.Width >= xcheck)
            {
                Vector2 slope = upperRight - lowerRight;
                slope.Normalize();
                Vector3 normal3D = Vector3.Cross(new Vector3(slope, 0), new Vector3(0, 0, -1));
                Vector2 norm = new Vector2(normal3D.X, normal3D.Y);
                imVel = Vector2.Reflect(imVel, norm);
            }

            Rectangle c1 = new Rectangle((int)ballPos.X, (int)ballPos.Y, (int)ball.Width, (int)ball.Height);
            Rectangle c2 = new Rectangle((int)imPos.X, (int)imPos.Y, (int)im.Width, (int)im.Height);

            bool collided = false;
            if (c1.Intersects(c2))
            {
                Rectangle intersect = Rectangle.Intersect(c1, c2);


                for (int x = intersect.X; x < intersect.X + intersect.Width; x++)
                {
                    for (int y = intersect.Y; y < intersect.Y + intersect.Height; y++)
                    {
                        int a1 = ballColor[x - (int)ballPos.X, y - (int)ballPos.Y].A;
                        int a2 = imColor[x - (int)imPos.X, y - (int)imPos.Y].A;
                        if ((a1 != 0) && (a2 != 0))
                        {
                            if (!colliding)
                            {
                                Vector2 c1Normalized = Vector2.Normalize(imVel);
                                Vector2 c2Normalized = Vector2.Normalize(ballVel);
                                Vector3 c1Normal3D = Vector3.Cross(new Vector3(c1Normalized, 0), new Vector3(0, 0, -1));
                                Vector3 c2Normal3D = Vector3.Cross(new Vector3(c2Normalized, 0), new Vector3(0, 0, -1));
                                Vector2 c1Norm = new Vector2(c1Normal3D.X, c1Normal3D.Y);
                                Vector2 c2Norm = new Vector2(c2Normal3D.X, c2Normal3D.Y);
                                ballVel = Vector2.Reflect(ballVel, c1Norm);
                                imVel = Vector2.Reflect(imVel, c2Norm);
                                colliding = true;
                                collided = true;
                            }

                        }

                    }
                }


            }
            if (colliding && !collided)
            {
                colliding = false;
            }



            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            spriteBatch.Draw(colorStrip, northWall, wallColor, Color.White);
            spriteBatch.Draw(colorStrip, southWall, wallColor, Color.White);
            spriteBatch.Draw(colorStrip, eastWall, wallColor, Color.White, (float)(eastWallAngle + Math.PI), Vector2.Zero, SpriteEffects.None, 0.0f);
            spriteBatch.Draw(colorStrip, westWall, wallColor, Color.White, (float)(westWallAngle + Math.PI), Vector2.Zero, SpriteEffects.None, 0.0f);
            spriteBatch.Draw(ball, ballPos, Color.White);
            spriteBatch.Draw(im, imPos, Color.White);
            spriteBatch.DrawString(dfont, (int)ballPos.X + ", " + (int)ballPos.Y + " : " + (int)imPos.X + ", " + (int)imPos.Y, ballTextPos, Color.White);
            spriteBatch.End();



            base.Draw(gameTime);
        }
    }
}
