/*
 *  Author:     Bill Sun     
 *  Comments:   ahaha oh god this is terrible, note to self: get rid of this
 *  Revision History: 
 *      2018-11-2: latest
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
    public class _Initializations
    {
        //screen width and height, also used for calculating aspect ratio
        public static int preferredBackBufferWidth { get => 1920; }
        public static int preferredBackBufferHeight { get => 1080; }

        //NOTE TO SELF:
        //Y-AXIS IS UP/DOWN
        //public static Vector3 cameraPosition = new Vector3(12f, 24f, -12f);
        public static Vector3 cameraPosition = new Vector3(9f, 18f, -9f);

        private static Vector3 cameraLookAtVector = Vector3.Zero;
        private static Vector3 cameraUpVector = Vector3.UnitY;

        //for projection matrix
        private static float aspectRatio = preferredBackBufferWidth / (float)preferredBackBufferHeight;
        private static float fieldOfView = Microsoft.Xna.Framework.MathHelper.PiOver4;
        private static float nearClipPlane = 1f;
        private static float farClipPlane = 200f;

        //setters and getters
        public static Matrix GetWorldMatrix()
        {
            return Matrix.Identity;
        }
        public static Matrix GetViewMatrix()
        {
            return Matrix.CreateLookAt(cameraPosition, cameraLookAtVector, cameraUpVector);
        }
        public static Matrix GetProjectionMatrix()
        {
            return Matrix.CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, nearClipPlane, farClipPlane);
        }
    }
}
