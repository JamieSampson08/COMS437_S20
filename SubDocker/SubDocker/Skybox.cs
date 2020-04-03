using BEPUphysics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceDocker
{
    public class Skybox : DrawableGameComponent
    {

        Matrix scale = Matrix.CreateScale(100f);

        // Base Requirements
        private Model model;
        private BEPUphysics.Entities.Prefabs.Sphere physicsObject;
        private GraphicsDevice graphicsDevice;
        

        // Sprite Stuff
        SpriteBatch spriteBatch;


        public Skybox(Game game) : base(game)
        {
            game.Components.Add(this);
            graphicsDevice = game.GraphicsDevice;
        }

        public Skybox(Game game, Vector3 pos, string id) : this(game)
        {
            physicsObject = new BEPUphysics.Entities.Prefabs.Sphere(ConversionHelper.MathConverter.Convert(pos), 1);
            physicsObject.AngularDamping = 0f;
            physicsObject.LinearDamping = 0f;
            physicsObject.Tag = id;

            Game.Services.GetService<Space>().Add(physicsObject);
        }

        public override void Initialize()
        {
            base.Initialize();
        }


        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            model = Game.Content.Load<Model>("skybox");

            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
        }

        public override void Draw(GameTime gameTime)
        {

            // draw model
            foreach (var mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.Alpha = .05f;
                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;
                    effect.World = scale * ConversionHelper.MathConverter.Convert(physicsObject.WorldTransform);
                    effect.View = Main.camera.View;
                    effect.Projection = Main.camera.Projection;
                }
                mesh.Draw();
            }

            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            physicsObject.WorldTransform = ConversionHelper.MathConverter.Convert(scale) * physicsObject.WorldTransform;
            base.Update(gameTime);
        }
    }
}
