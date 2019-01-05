/*
 *  Author:     Bill Sun        
 *  Comments:   walls class, will adjust its own bounding box based on type of wall it is
 *  Revision History: 
 *      2018-12-9: latest
 *      2018-11-2: created
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace BSunFinalProject
{
    class Walls
    {
        private WALLTYPE wallType;
        private Vector3 wallPosition;
        private BoundingBox boundingBox;
        private bool isDestroyable; //can the wall be destroyed? wood walls can, concrete can't
        private bool canWalkThrough;
        private int wallHealth;
        private float wallRotation;
        private float rotation90 = MathHelper.ToRadians(90);
        private float rotation270 = MathHelper.ToRadians(270);

        public Walls(Vector3 wallPosition, BoundingBox boundingBox, WALLTYPE wallType, bool isDestroyable, float wallRotation)
        {
            if (wallType == WALLTYPE.CONCRETE1X ||
                wallType == WALLTYPE.CONCRETE1XCORNER ||
                wallType == WALLTYPE.CONCRETE1XHALFCORNER ||
                wallType == WALLTYPE.CONCRETE1XOUTSIDE ||
                wallType == WALLTYPE.WOOD1XFULLDMG ||
                wallType == WALLTYPE.WOOD1XHALFDMG ||
                wallType == WALLTYPE.WOOD1XNODMG ||
                wallType == WALLTYPE.WOOD1XQUARTERDMG)
            {
                boundingBox.Max = wallPosition + new Vector3(0.25f, 2.5f, 0.25f);
                boundingBox.Min = wallPosition + new Vector3(-0.25f, 0.0f, -0.25f);
            }
            if ((wallType == WALLTYPE.CONCRETE2X ||
                wallType == WALLTYPE.CONCRETE2XOUTSIDE ||
                wallType == WALLTYPE.WOOD2XFULLDMG ||
                wallType == WALLTYPE.WOOD2XHALFDMG ||
                wallType == WALLTYPE.WOOD2XNODMG ||
                wallType == WALLTYPE.WOOD2XQUARTERDMG) && (wallRotation != rotation90 || wallRotation != rotation270))
            {
                boundingBox.Max = wallPosition + new Vector3(0.5f, 2.5f, 0.25f);
                boundingBox.Min = wallPosition + new Vector3(-0.5f, 0.0f, -0.25f);
            }
            if ((wallType == WALLTYPE.CONCRETE2X ||
                wallType == WALLTYPE.CONCRETE2XOUTSIDE ||
                wallType == WALLTYPE.WOOD2XFULLDMG ||
                wallType == WALLTYPE.WOOD2XHALFDMG ||
                wallType == WALLTYPE.WOOD2XNODMG ||
                wallType == WALLTYPE.WOOD2XQUARTERDMG) && (wallRotation == rotation90 || wallRotation == rotation270))
            {
                boundingBox.Max = wallPosition + new Vector3(0.25f, 2.5f, 0.5f);
                boundingBox.Min = wallPosition + new Vector3(-0.25f, 0.0f, -0.5f);
            }

            this.wallPosition = wallPosition;
            this.boundingBox = boundingBox;
            this.wallType = wallType;
            this.isDestroyable = isDestroyable;
            this.wallRotation = wallRotation;

            canWalkThrough = false; //if the wall is completely destroyed, player can walk through it

            if (isDestroyable)
            {
                wallHealth = 100; //inital hp for all walls
            }
        }

        //setters and getters
        public Vector3 WallPosition
        {
            get { return wallPosition; }
            set { wallPosition = value; }
        }

        public BoundingBox BoundingBox
        {
            get { return boundingBox; }
            set { boundingBox = value; }
        }

        public WALLTYPE WallType
        {
            get { return wallType; }
            set { wallType = value; }
        }

        public bool CanWalkThrough
        {
            get { return canWalkThrough; }
            set { canWalkThrough = value; }
        }

        public int WallHealth
        {
            get { return wallHealth; }
            set { wallHealth = value; }
        }

        public float WallRotation
        {
            get { return wallRotation; }
        }

        public bool IsDestroyable
        {
            get { return isDestroyable; }
        }
    }
}
