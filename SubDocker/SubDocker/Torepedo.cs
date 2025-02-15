﻿using BEPUphysics;
using BEPUphysics.BroadPhaseEntries.MobileCollidables;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;

namespace SpaceDocker
{
    class Torpedo : DrawableGameComponent
    {
        private Model model;
        private BEPUphysics.Entities.Prefabs.Sphere physicsObject;

        private float speed;

        public int torpedoID;
        private Vector3 offsetFromShip;

        // torpedo variables
        private int torpedoDegree = 0;
        private int rotationAimSpeed = 2;
        private Quaternion initalAngle;

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

        public Vector3 linearVelocity
        {
            get { return ConversionHelper.MathConverter.Convert(physicsObject.LinearVelocity); }
            set { physicsObject.LinearVelocity = ConversionHelper.MathConverter.Convert(value); }
        }

        public Vector3 linearMomentum
        {
            get { return ConversionHelper.MathConverter.Convert(physicsObject.LinearMomentum); }
            set { physicsObject.LinearMomentum = ConversionHelper.MathConverter.Convert(value); }
        }

        public Vector3 angularVelocity
        {
            get { return ConversionHelper.MathConverter.Convert(physicsObject.AngularVelocity); }
            set { physicsObject.AngularVelocity = ConversionHelper.MathConverter.Convert(value); }
        }

        public Vector3 angularMomentum
        {
            get { return ConversionHelper.MathConverter.Convert(physicsObject.AngularMomentum); }
            set { physicsObject.AngularMomentum = ConversionHelper.MathConverter.Convert(value); }
        }

        public Torpedo(Game game) : base(game)
        {
            game.Components.Add(this);
        }

        public Torpedo(Game game, float speed, string id) : this(game)
        {
            physicsObject = new BEPUphysics.Entities.Prefabs.Sphere(ConversionHelper.MathConverter.Convert(Vector3.Zero), 1);
            physicsObject.AngularDamping = 0f;
            physicsObject.LinearDamping = 0f;
            physicsObject.CollisionInformation.Events.InitialCollisionDetected += Events_InitialCollisionDetected;
            physicsObject.Tag = id;

            torpedoID = Int32.Parse(id.Substring(7));

            this.speed = speed;
            offsetFromShip = new Vector3(0, 0, 10f);

            Game.Services.GetService<Space>().Add(physicsObject);
        }

        private void Events_InitialCollisionDetected(BEPUphysics.BroadPhaseEntries.MobileCollidables.EntityCollidable sender, BEPUphysics.BroadPhaseEntries.Collidable other, BEPUphysics.NarrowPhaseSystems.Pairs.CollidablePairHandler pair)
        {
            Console.WriteLine(torpedoID + " Collision");
            var otherEntityInformation = other as EntityCollidable;
            string tag = (string)otherEntityInformation.Entity.Tag;

            // hitting the ship doesn't remove the torpedo (would interfer with firing logic
            if (!tag.Equals("ship"))
            {
                if (Game.Components.Contains(this))
                {
                    RemoveFromGame();
                }
            }
        }

        protected override void LoadContent()
        {
            model = Game.Content.Load<Model>("rocket4");
            // TODO - physicsObject is null when trying to run AddTorpedo() & resetting torpedos
            physicsObject.Radius = model.Meshes[0].BoundingSphere.Radius;

            base.LoadContent();
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

        public override void Update(GameTime gameTime)
        {
            modelOrientation = Main.ship.modelOrientation;
            modelPosition = Main.ship.modelPosition;

            base.Update(gameTime);
        }
        public void RemoveFromGame()
        {
            if (Game.Components.Contains(this))
            {
                Game.Services.GetService<Space>().Remove(physicsObject);
                Game.Components.Remove(this);
            }
        }

        /// <summary>
        /// Applies rotation on the torpedo
        /// </summary>
        /// <param name="degree"></param>
        public void RotateTorpedo(int degree)
        {
            // Aim (20 degree cone)
            Matrix rotation = Matrix.CreateRotationZ(MathHelper.ToRadians(degree));
            Quaternion rot = Quaternion.CreateFromRotationMatrix(rotation);
            modelOrientation *= rot;
        }

        /// <summary>
        /// Shoots the torpedo in the directioin it is facing
        /// </summary>
        public void ShootTorpedo()
        {
            Vector3 torpedoOffset = Vector3.Up + new Vector3(0, 1f, 0);
            // TODO - give torpedo x linear momentum
        }

        /// <summary>
        /// Rotate the torpedo to the right
        /// </summary>
        public void RotateRight()
        {
            // TODO - edit angles
            if (!(torpedoDegree > 90))
            {
                torpedoDegree += rotationAimSpeed;
                this.RotateTorpedo(torpedoDegree);
            }
        }

        /// <summary>
        /// Rotate the torpedo to the left
        /// </summary>
        public void RotateLeft()
        {
            // TODO - edit angles
            if (!(torpedoDegree < -90))
            {
                torpedoDegree -= rotationAimSpeed;
                this.RotateTorpedo(torpedoDegree);
            }
        }
    }
}
