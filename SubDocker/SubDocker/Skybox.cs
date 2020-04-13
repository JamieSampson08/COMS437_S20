using BEPUphysics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceDocker
{
    public class Skybox : DrawableGameComponent
    {

        Matrix scaleMatrix = Matrix.CreateScale(200f);

        // Base Requirements
        private Model model;
        private BEPUphysics.Entities.Prefabs.Sphere physicsObject;

        private Texture2D waterTexture;

        public Vector3 modelPosition
        {
            get { return ConversionHelper.MathConverter.Convert(physicsObject.Position); }
            set { physicsObject.Position = ConversionHelper.MathConverter.Convert(value); }
        }

        public Skybox(Game game) : base(game)
        {
            game.Components.Add(this);
        }

        public Skybox(Game game, Vector3 pos, string id) : this(game)
        {
            physicsObject = new BEPUphysics.Entities.Prefabs.Sphere(ConversionHelper.MathConverter.Convert(pos), 250);
            physicsObject.AngularDamping = 0f;
            physicsObject.LinearDamping = 0f;
            physicsObject.Tag = id;

            Game.Services.GetService<Space>().Add(physicsObject);
        }

        protected override void LoadContent()
        {
            waterTexture = Game.Content.Load<Texture2D>("skybox_water");
            model = Game.Content.Load<Model>("skyWater");

            physicsObject.Radius = model.Meshes[0].BoundingSphere.Radius;
            base.LoadContent();
        }

        public override void Draw(GameTime gameTime)
        {
            // draw model
            foreach (var mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.Alpha = 1f;
                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;
                    effect.World = scaleMatrix * ConversionHelper.MathConverter.Convert(physicsObject.WorldTransform);
                    effect.View = Main.camera.View;
                    effect.Projection = Main.camera.Projection;
                }
                mesh.Draw();
            }
            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            modelPosition = Main.ship.modelPosition;
            base.Update(gameTime);
        }
    }
}
