using Microsoft.Xna.Framework;

namespace SpaceDocker
{
    class Helpers
    {
        private Vector3 maxLinearMomentum = new Vector3(30f, 30f, 30f);
        private Vector3 minLinearMomentum = new Vector3(-30f, -30f, -30f);

        private Vector3 maxAngularMomentum = new Vector3(10f, 10f, 10f);
        private Vector3 minAngularMomentum = new Vector3(-10f, -10f, -10f);
        private float rotationDegree = .5f;

        public Helpers(){ }

        public Vector3 CheckLinearMomentumBounds(Vector3 linearMomentum)
        {
            Vector3 validLinearMomentum = linearMomentum;

            if (linearMomentum.X > maxLinearMomentum.X)
            {
                validLinearMomentum = new Vector3(maxLinearMomentum.X, validLinearMomentum.Y, validLinearMomentum.Z);
            }
            if (linearMomentum.X < minLinearMomentum.X)
            {
                validLinearMomentum = new Vector3(minLinearMomentum.X, validLinearMomentum.Y, validLinearMomentum.Z);
            }

            if (linearMomentum.Y > maxLinearMomentum.Y)
            {
                validLinearMomentum = new Vector3(validLinearMomentum.X, maxLinearMomentum.Y, validLinearMomentum.Z);
            }
            if (linearMomentum.Y < minLinearMomentum.Y)
            {
                validLinearMomentum = new Vector3(validLinearMomentum.X, minLinearMomentum.Y, validLinearMomentum.Z);
            }

            if (linearMomentum.Z > maxLinearMomentum.Z)
            {
                validLinearMomentum = new Vector3(validLinearMomentum.X, validLinearMomentum.Y, maxLinearMomentum.Z);
            }
            if (linearMomentum.Z < minLinearMomentum.Z)
            {
                validLinearMomentum = new Vector3(validLinearMomentum.X, validLinearMomentum.Y, minLinearMomentum.Z);
            }

            return validLinearMomentum;
        }

        public Vector3 CheckAngularMomentumBounds(Vector3 angularMomentum, string direction)
        {
            Vector3 tempMomentum;
            float tempZ = 0;
            float tempY = 0;
            float tempX = 0;

            switch (direction)
            {
                case "YL":
                    tempMomentum = angularMomentum - new Vector3(0, 0, rotationDegree);
                    tempZ = tempMomentum.Z;
                    if (tempZ < minAngularMomentum.Z)
                    {
                        tempZ = minAngularMomentum.Z;
                    }
                    tempX = tempMomentum.X;
                    tempY = tempMomentum.Y;
                    break;
                case "YR":
                    tempMomentum = angularMomentum + new Vector3(0, 0, rotationDegree);
                    tempZ = tempMomentum.Z;
                    if (tempZ > maxAngularMomentum.Z)
                    {
                        tempZ = maxAngularMomentum.Z;
                    }
                    tempX = tempMomentum.X;
                    tempY = tempMomentum.Y;
                    break;
                case "PR":
                    tempMomentum = angularMomentum - new Vector3(0, rotationDegree, 0);
                    tempY = tempMomentum.Y;
                    if (tempY < minAngularMomentum.Y)
                    {
                        tempY = minAngularMomentum.Y;
                    }
                    tempX = tempMomentum.X;
                    tempZ = tempMomentum.Z;
                    break;
                case "PL":
                    tempMomentum = angularMomentum + new Vector3(0, rotationDegree, 0);
                    tempY = tempMomentum.Y;
                    if (tempY > maxAngularMomentum.Y)
                    {
                        tempY = maxAngularMomentum.Y;
                    }
                    tempX = tempMomentum.X;
                    tempZ = tempMomentum.Z;
                    break;
                case "RF":
                    tempMomentum = angularMomentum - new Vector3(rotationDegree, 0, 0);
                    tempX = tempMomentum.X;

                    if (tempX < minAngularMomentum.X)
                    {
                        tempX = minAngularMomentum.X;
                    }
                    tempY = tempMomentum.Y;
                    tempZ = tempMomentum.Z;
                    break;
                case "RB":
                    tempMomentum = angularMomentum + new Vector3(rotationDegree, 0, 0);
                    tempX = tempMomentum.X;
                    
                    if (tempX> maxAngularMomentum.X)
                    {
                        tempX = maxAngularMomentum.X;
                    }
                    tempY = tempMomentum.Y;
                    tempZ = tempMomentum.Z;
                    break;
            }

            return new Vector3(tempX, tempY, tempZ);
        }
    }
}
