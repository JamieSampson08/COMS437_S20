using BEPUphysics;
using BEPUphysics.BroadPhaseEntries.MobileCollidables;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;

namespace SpaceDocker
{
    class FuelPack : DrawableGameComponent
    {

        private Model model;
        private BEPUphysics.Entities.Prefabs.Sphere physicsObject;

        private int fuelPackID;

        public FuelPack(Game game) : base(game)
        {
            game.Components.Add(this);
        }

        public FuelPack(Game game, Vector3 pos, string id) : this(game)
        {
            physicsObject = new BEPUphysics.Entities.Prefabs.Sphere(ConversionHelper.MathConverter.Convert(pos), 1);
            physicsObject.CollisionInformation.Events.InitialCollisionDetected += Events_InitialCollisionDetected;
            physicsObject.Tag = id;
            fuelPackID = Int32.Parse(id.Substring(9));

            Game.Services.GetService<Space>().Add(physicsObject);
        }

        private void Events_InitialCollisionDetected(BEPUphysics.BroadPhaseEntries.MobileCollidables.EntityCollidable sender, BEPUphysics.BroadPhaseEntries.Collidable other, BEPUphysics.NarrowPhaseSystems.Pairs.CollidablePairHandler pair)
        {
            Console.WriteLine(fuelPackID + " FuellPack Collision");
            var otherEntityInformation = other as EntityCollidable;
            string tag = (string)otherEntityInformation.Entity.Tag;

            // only remove object if comes in contact with the ship
            if (tag.Equals("ship"))
            {
                if (Game.Components.Contains(this))
                {
                    RemoveFromGame();
                }
            }
        }

        protected override void LoadContent()
        {
            model = Game.Content.Load<Model>("fuelTank");

            base.LoadContent();
        }

        public void RemoveFromGame()
        {
            Game.Services.GetService<Space>().Remove(physicsObject);
            Game.Components.Remove(this);
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

    }
}
