/*
 *  Author:     Bill Sun        
 *  Comments:   this class is the game controller class, handles things like player movement, also handles translating 2d mouse position to 3d world space position
 *  Revision History: 
 *      2018-12-9: latest
 *      2018-10-27: created
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BSunFinalProject
{
    class GameController : DrawableGameComponent
    {
        private Vector3 modelPosition;
        private Matrix cameraViewMatrix;

        //camera matrix variables
        private Vector3 cameraPosition;
        private Vector3 cameraOffset = _Initializations.cameraPosition;
        private Vector3 cameraLookAtVector = Vector3.Zero;
        private Vector3 cameraUpVector = Vector3.UnitY;

        //3d mouse position
        GraphicsDevice graphicsDevice;
        Vector3 mousePosition;
        Matrix worldMatrix = Matrix.Identity;

        //character
        private Vector3 velocity; //for player movement
        private bool isPlayerDead = false;
        private bool isMoving = false; //is the character moving? will be used for leg movement
        private const float SPEED = 0.05f; //move speed of character
        private const float SPEEDMOD = 1.5f; //move speed modifier so diagonal move speed is same as horizontal/vertical
        private const float RUNSPEED = 0.05f; //run speed modifier for if left shift or left alt is pressed

        //player lighting
        private Vector3 playerDiffuse;
        private Vector3 playerLightDirection;
        private Vector3 playerSpecular;

        private List<Walls> basementWalls;
        private BoundingBox playerBulletColBox;
        private BoundingBox boundingBoxXPos;
        private BoundingBox boundingBoxXNeg;
        private BoundingBox boundingBoxZPos;
        private BoundingBox boundingBoxZNeg;

        bool isColliding;

        //which side is colliding?
        bool isCollidingXPos = false;
        bool isCollidingXNeg = false;
        bool isCollidingZPos = false;
        bool isCollidingZNeg = false;

        public GameController(Game game) : base(game)
        {
            graphicsDevice = game.GraphicsDevice;

            playerDiffuse = new Vector3(0.0237f, 0.0218f, 0.0149f) * 1.2f; //rgb(237, 218, 149)
            playerLightDirection = new Vector3(0, 0, 0);
            playerSpecular = new Vector3(1f, 1f, 0); //highlights, also in rgb values

            basementWalls = new List<Walls>(); //list of walls, will be used for calculating collisions
            isColliding = false;
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            #region move player

            KeyboardState keyPress = Keyboard.GetState();

            velocity.X = 0; //resetting velocity
            velocity.Z = 0;

            bool keyW = false;
            bool keyA = false;
            bool keyS = false;
            bool keyD = false;
            bool keyShift = false;
            bool keyAlt = false;

            float runMod = 0;

            //movement depends on which keys are pressed, individually or together
            if (keyPress.IsKeyDown(Keys.W))
                keyW = true;
            if (keyPress.IsKeyDown(Keys.A))
                keyA = true;
            if (keyPress.IsKeyDown(Keys.S))
                keyS = true;
            if (keyPress.IsKeyDown(Keys.D))
                keyD = true;

            if (keyPress.IsKeyDown(Keys.LeftShift)) //for running
                keyShift = true;
            if (keyPress.IsKeyDown(Keys.LeftAlt)) //for walking slower
                keyAlt = true;

            if (keyShift) //applying the appropreate move speed modifier
            {
                runMod = RUNSPEED; //move faster
            }
            else if (keyAlt)
            {
                runMod = -(RUNSPEED / 2); //move slower
            }

            if (keyW && !keyA && !keyD) //moving negative along x and positive z axis
            {
                velocity.X = -SPEED - runMod;
                velocity.Z = SPEED + runMod;
            }
            else if (keyA && !keyW && !keyS) //moving positive along x and z axis
            {
                velocity.X = SPEED + runMod;
                velocity.Z = SPEED + runMod;
            }
            else if (keyS && !keyA && !keyD) //moving positive along x axis and negative along z axis
            {
                velocity.X = SPEED + runMod;
                velocity.Z = -SPEED - runMod;
            }
            else if (keyD && !keyS && !keyW) //moving negative along x and z axis
            {
                velocity.X = -SPEED - runMod;
                velocity.Z = -SPEED - runMod;
            }
            else if (keyW && keyA) //moving positive along z axis
            {
                velocity.Z = (SPEED + runMod) * SPEEDMOD;
            }
            else if (keyW && keyD) //moving negative along x axis
            {
                velocity.X = (-SPEED - runMod) * SPEEDMOD;
            }
            else if (keyA && keyS) //moving positive along x axis
            {
                velocity.X = (SPEED + runMod) * SPEEDMOD;
            }
            else if (keyD && keyS) //moving negative along z axis
            {
                velocity.Z = (-SPEED - runMod) * SPEEDMOD;
            }

            if (velocity.X != 0 || velocity.Z != 0)
            {
                isMoving = true;
            }
            else
            {
                isMoving = false;
            }

            #endregion

            #region is colliding?

            Vector3 proposedLocation = new Vector3(modelPosition.X + velocity.X, 0f, modelPosition.Z + velocity.Z);

            foreach (Walls wall in basementWalls)
            {
                if (boundingBoxXNeg.Intersects(wall.BoundingBox) && !wall.CanWalkThrough)
                {
                    isCollidingXNeg = true;
                    if(velocity.X < 0)
                    {
                        proposedLocation.X = modelPosition.X - velocity.X;
                        //modelPosition.X += velocity.X * -0.1f;
                        velocity.X = 0;
                    }
                }
                else
                {
                    isCollidingXNeg = false;
                }
                if (boundingBoxXPos.Intersects(wall.BoundingBox) && !wall.CanWalkThrough)
                {
                    isCollidingXPos = true;
                    if(velocity.X > 0)
                    {
                        proposedLocation.X = modelPosition.X - velocity.X;
                        //modelPosition.X += velocity.X * -0.1f;
                        velocity.X = 0;
                    }  
                }
                else
                {
                    isCollidingXPos = false;
                }
                if (boundingBoxZNeg.Intersects(wall.BoundingBox) && !wall.CanWalkThrough)
                {
                    isCollidingZNeg = true;
                    if(velocity.Z < 0)
                    {
                        proposedLocation.Z = modelPosition.Z - velocity.Z;
                        velocity.Z = 0;
                    }
                }
                else
                {
                    isCollidingZNeg = false;
                }
                if (boundingBoxZPos.Intersects(wall.BoundingBox) && !wall.CanWalkThrough)
                {
                    isCollidingZPos = true;
                    if(velocity.Z > 0)
                    {
                        proposedLocation.Z = modelPosition.Z - velocity.Z;
                        velocity.Z = 0;
                    }
                }
                else
                {
                    isCollidingZPos = false;
                }
            }

            #endregion

            if (!isPlayerDead) //while player is not dead, can move
            {
                //applying the move for the player 
                modelPosition.X = proposedLocation.X;
                modelPosition.Z = proposedLocation.Z;
            }

            #region move camera

            cameraPosition = modelPosition + cameraOffset; //camera is always fixed on the player
            cameraViewMatrix = Matrix.CreateLookAt(cameraPosition, modelPosition, cameraUpVector);

            #endregion

            #region getting mouse position

            //code modified from here
            //https://gamedev.stackexchange.com/questions/57625/getting-a-3d-mouse-position
            //https://gamedev.stackexchange.com/questions/23395/how-to-convert-screen-space-into-3d-world-space

            //need to create a fake world matrix where center of unproject will always be same as player character
            worldMatrix = Matrix.CreateTranslation(modelPosition);

            MouseState mouseState = Mouse.GetState();

            Vector3 nearSource = new Vector3((float)mouseState.X, (float)mouseState.Y, 0f);
            Vector3 farSource = new Vector3((float)mouseState.X, (float)mouseState.Y, 1f);
            Vector3 nearPoint = graphicsDevice.Viewport.Unproject(nearSource, _Initializations.GetProjectionMatrix(), cameraViewMatrix, worldMatrix);
            Vector3 farPoint = graphicsDevice.Viewport.Unproject(farSource, _Initializations.GetProjectionMatrix(), cameraViewMatrix, worldMatrix);

            // Create a ray from the near clip plane to the far clip plane.
            Vector3 direction = farPoint - nearPoint;
            direction.Normalize();

            // Create a ray.
            Ray ray = new Ray(nearPoint, direction);

            // Calculate the ray-plane intersection point.
            Vector3 n = new Vector3(0f, 1f, 0f);
            Plane p = new Plane(n, 0f);

            // Calculate distance of intersection point from r.origin.
            float denominator = Vector3.Dot(p.Normal, ray.Direction);
            float numerator = Vector3.Dot(p.Normal, ray.Position) + p.D;
            float t = -(numerator / denominator);

            // Calculate the picked position on the y = 0 plane.
            mousePosition = nearPoint + direction * t;

            #endregion

            #region player lighting

            playerLightDirection = -cameraPosition;

            #endregion

            base.Update(gameTime);
        }

        //getters and setters

        #region player and camera position

        public Vector3 PlayerPosition
        {
            get { return modelPosition; }
            set { modelPosition = value; }
        }

        public Vector3 CameraPosition
        {
            get { return cameraPosition; }
        }

        public Matrix CameraMatrix
        {
            get { return cameraViewMatrix; }
            set { cameraViewMatrix = value; }
        }

        public Vector3 MousePosition3D
        {
            get { return mousePosition; }
            set { mousePosition = value; }
        }

        public Matrix WorldMatrix
        {
            set { worldMatrix = value; }
        }

        public bool IsMoving
        {
            get { return isMoving; }
        }

        #endregion

        #region playerLighting

        public Vector3 PlayerDiffuse
        {
            get { return playerDiffuse; }
        }
        public Vector3 PlayerLightDirection
        {
            get { return playerLightDirection; }
        }
        public Vector3 PlayerSpecular
        {
            get { return playerSpecular; }
        }

        #endregion

        public List<Walls> BasementWalls
        {
            get { return basementWalls; }
            set { basementWalls = value; }
        }

        public BoundingBox PlayerBulletColBox
        {
            get { return playerBulletColBox; }
            set { playerBulletColBox = value; }
        }

        public BoundingBox PlayerBoundingBoxXNeg
        {
            get { return boundingBoxXNeg; }
            set { boundingBoxXNeg = value; }
        }

        public BoundingBox PlayerBoundingBoxXPos
        {
            get { return boundingBoxXPos; }
            set { boundingBoxXPos = value; }
        }

        public BoundingBox PlayerBoundingBoxZNeg
        {
            get { return boundingBoxZNeg; }
            set { boundingBoxZNeg = value; }
        }

        public BoundingBox PlayerBoundingBoxZPos
        {
            get { return boundingBoxZPos; }
            set { boundingBoxZPos = value; }
        }

        public bool IsColliding
        {
            get { return isColliding; }
        }

        public bool IsCollidingXNeg
        {
            get { return isCollidingXNeg; }
        }

        public bool IsCollidingXPos
        {
            get { return isCollidingXPos; }
        }

        public bool IsCollidingZNeg
        {
            get { return isCollidingZNeg; }
        }

        public bool IsCollidingZPos
        {
            get { return isCollidingZPos; }
        }

        public bool IsPlayerDead
        {
            set { isPlayerDead = value; }
        }
    }
}
