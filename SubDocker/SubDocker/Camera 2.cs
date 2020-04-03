using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace SpaceDocker
{
    public class Camera2 : GameComponent
    {
        private GraphicsDevice graphicsDevice;

        private Matrix camWorld = Matrix.Identity;

        public Matrix Projection
        {
            get
            {
                float fieldOfView = Microsoft.Xna.Framework.MathHelper.PiOver4;
                float nearClipPlane = 1;
                float farClipPlane = 200;
                float aspectRatio = graphicsDevice.Viewport.Width / (float)graphicsDevice.Viewport.Height;

                return Matrix.CreatePerspectiveFieldOfView(
                    fieldOfView, aspectRatio, nearClipPlane, farClipPlane);
            }
        }

        public Matrix View;

        public Camera2(Game game) : base(game)
        {
            game.Components.Add(this);
            this.graphicsDevice = game.GraphicsDevice;

        }

        public Camera2(Game game, Vector3 pos) : this(game)
        {
            camWorld = Matrix.CreateWorld(pos, Vector3.Up, Vector3.Forward);
            View = Matrix.CreateLookAt(camWorld.Translation, camWorld.Forward, camWorld.Up);
        }

        public override void Update(GameTime gameTime)
        {

        }
    }
}