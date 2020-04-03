using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace SpaceDocker
{
    public class Camera : GameComponent
    {
        private GraphicsDevice graphicsDevice;

        private Matrix camWorld = Matrix.Identity;
        private Vector3 offset = new Vector3(0, -.9f, -2f);

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


        public Camera(Game game) : base(game)
        {
            game.Components.Add(this);
            this.graphicsDevice = game.GraphicsDevice;

            UpdateWorld();
        }

        private void UpdateWorld()
        {
            camWorld.Translation = Main.ship.modelPosition - offset;
            camWorld = Matrix.CreateWorld(camWorld.Translation, Vector3.Down, Vector3.Backward);
            View = Matrix.CreateLookAt(
            camWorld.Translation - (-3f * camWorld.Down),
            Vector3.Transform(camWorld.Down, Main.ship.modelOrientation) + new Vector3(0, 0, 3f),
            Vector3.Transform(camWorld.Backward, Main.ship.modelOrientation));
        }

        public override void Update(GameTime gameTime)
        {
            UpdateWorld();
        }
    }
}