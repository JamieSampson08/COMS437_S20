using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace SpaceDocker
{
    public class Camera : GameComponent
    {
        private GraphicsDevice graphicsDevice;

        private Matrix camWorld = Matrix.Identity;

        public Matrix Projection
        {
            get
            {
                float fieldOfView = MathHelper.PiOver4;
                float nearClipPlane = 1;
                float farClipPlane = 200;
                float aspectRatio = graphicsDevice.Viewport.Width / (float)graphicsDevice.Viewport.Height;

                return Matrix.CreatePerspectiveFieldOfView(
                    fieldOfView, aspectRatio, nearClipPlane, farClipPlane);
            }
        }

        public Vector3 Position
        {
            get { return camWorld.Translation; }
        }

        public Matrix View;

        public Camera(Game game) : base(game)
        {
            game.Components.Add(this);
            this.graphicsDevice = game.GraphicsDevice;

            UpdateCamera();
        }

        public override void Update(GameTime gameTime)
        {
            UpdateCamera();
        }

        public void UpdateCamera()
        {
            camWorld = Matrix.CreateWorld(Main.ship.shipWorld.Translation, Vector3.Up, Vector3.Forward);

            Vector3 cameraTarget = Vector3.Transform(camWorld.Forward, Main.ship.modelOrientation);
            Vector3 cameraUpVector = Vector3.Transform(camWorld.Up, Main.ship.modelOrientation);
            View = Matrix.CreateLookAt(camWorld.Translation, camWorld.Translation + cameraTarget, cameraUpVector);
        }
    }
}