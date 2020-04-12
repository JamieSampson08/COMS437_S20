using BEPUphysics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;

namespace SpaceDocker
{
    class FuelPack : DrawableGameComponent
    {

        private Model model;
        private BEPUphysics.Entities.Prefabs.Sphere physicsObject;

        private int fuelValue;
        private int fuelPackID;

        SpriteBatch spriteBatch;

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

        public FuelPack(Game game) : base(game)
        {
            game.Components.Add(this);
        }

        public FuelPack(Game game, int healthValue, Vector3 pos, string id) : this(game)
        {
            physicsObject = new BEPUphysics.Entities.Prefabs.Sphere(ConversionHelper.MathConverter.Convert(pos), 1);
            physicsObject.CollisionInformation.Events.InitialCollisionDetected += Events_InitialCollisionDetected;
            physicsObject.Tag = id;
            fuelPackID = Int32.Parse(id.Substring(9));

            modelPosition = pos;
            this.fuelValue = healthValue;

            Game.Services.GetService<Space>().Add(physicsObject);
        }
        private void Events_InitialCollisionDetected(BEPUphysics.BroadPhaseEntries.MobileCollidables.EntityCollidable sender, BEPUphysics.BroadPhaseEntries.Collidable other, BEPUphysics.NarrowPhaseSystems.Pairs.CollidablePairHandler pair)
        {
            Console.WriteLine("FuelPack " + fuelPackID + " Collision");
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            model = Game.Content.Load<Model>("FuelPack");

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
            base.Update(gameTime);
        }

        public void Reset()
        {
            
        }
    }
}
