using BEPUphysics;
using BEPUphysics.BroadPhaseEntries.MobileCollidables;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;

namespace SpaceDocker
{
    // skeleton code from: APE Asteroids.cs
    public class RubberDuck : DrawableGameComponent
    {
        // Base Requirements
        private Model model;
        private BEPUphysics.Entities.Prefabs.Sphere physicsObject;

        public int duckID;

        private Vector3 Position
        {
            get
            {
                return ConversionHelper.MathConverter.Convert(physicsObject.Position);
            }
        }
        public RubberDuck(Game game) : base(game)
        {
            game.Components.Add(this);
        }
        public RubberDuck(Game game, Vector3 pos, string id) : this(game)
        {
            physicsObject = new BEPUphysics.Entities.Prefabs.Sphere(ConversionHelper.MathConverter.Convert(pos), 1);
            physicsObject.AngularDamping = 0f;
            physicsObject.LinearDamping = 0f;
            physicsObject.CollisionInformation.Events.InitialCollisionDetected += Events_InitialCollisionDetected;
            physicsObject.Tag = id;
            duckID = Int32.Parse(id.Substring(5));

            Game.Services.GetService<Space>().Add(physicsObject);
        }


        private void Events_InitialCollisionDetected(BEPUphysics.BroadPhaseEntries.MobileCollidables.EntityCollidable sender, BEPUphysics.BroadPhaseEntries.Collidable other, BEPUphysics.NarrowPhaseSystems.Pairs.CollidablePairHandler pair)
        {
            Console.WriteLine(duckID + " Collision");

            var otherEntityInformation = other as EntityCollidable;
            string tag = (string)otherEntityInformation.Entity.Tag;

            Game.Services.GetService<Space>().Remove(physicsObject);
            Game.Components.Remove(this);
        }

        public RubberDuck(Game game, Vector3 pos, string id, float mass) : this(game, pos, id)
        {
            physicsObject.Mass = mass;
        }

        public RubberDuck(Game game, Vector3 pos, string id, float mass, Vector3 linMomentum) : this(game, pos, id, mass)
        {
            physicsObject.LinearMomentum = ConversionHelper.MathConverter.Convert(linMomentum);
        }

        public RubberDuck(Game game, Vector3 pos, string id, float mass, Vector3 linMomentum, Vector3 angMomentum) : this(game, pos, id, mass, linMomentum)
        {
            physicsObject.AngularMomentum = ConversionHelper.MathConverter.Convert(angMomentum);
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            model = Game.Content.Load<Model>("rubber_duck2");
            physicsObject.Radius = model.Meshes[0].BoundingSphere.Radius;

            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (var mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.PreferPerPixelLighting = true;
                    effect.EnableDefaultLighting();
                    effect.World = ConversionHelper.MathConverter.Convert(physicsObject.WorldTransform);
                    effect.View = Main.camera.View;
                    effect.Projection = Main.camera.Projection;
                }
                mesh.Draw();
            }
            base.Draw(gameTime);
        }
    }
}
