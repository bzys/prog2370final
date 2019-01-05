/*
 *  Author:     Bill Sun        
 *  Comments:   this class is the bullet class, does bullet stuff
 *  Revision History: 
 *      2018-12-5: latest
 *      2018-11-19: created
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
    public class BulletClass
    {
        private Vector3 position;
        private Vector3 mousePosition;
        private Vector3 increment;
        private float rotation;
        private BoundingBox boundingBox;
        private WEAPONTYPE bulletFromWhatWeapon;
        private int bulletDmg;
        private int bulletWallDmg;

        //constructor for bulletclass, each bullet is an object and takes a initial position, position of mouse, increment amount, rotation direction and what kind of gun its fired from
        public BulletClass(Vector3 position, Vector3 mousePosition, Vector3 increment, float rotation, WEAPONTYPE bulletFromWhatWeapon)
        {
            this.position = position + new Vector3(0f, 0.9f, 0f);
            this.mousePosition = mousePosition + new Vector3(0f, 0.9f, 0f);
            this.increment = increment;
            this.rotation = rotation;
            this.bulletFromWhatWeapon = bulletFromWhatWeapon;

            boundingBox = new BoundingBox();

            if (bulletFromWhatWeapon == WEAPONTYPE.AK47) //if bullet is from ak47
            {
                boundingBox.Max = new Vector3(0.05f, 0.9f, 0.05f); // + (increment * 0.5f)
                boundingBox.Min = new Vector3(-0.05f, 0.8f, -0.05f); // + (increment * 0.5f)
                bulletDmg = 30; //amount of damage it does do person
                bulletWallDmg = 5; //amount of damage it does to wall
            }
            if (bulletFromWhatWeapon == WEAPONTYPE.SHOTGUN) //if bullet is from shotgun
            {
                boundingBox.Max = position + new Vector3(0.05f, 0.9f, 0.05f) + (increment * 5);
                boundingBox.Min = position + new Vector3(-0.05f, 0.8f, -0.05f) + (increment * 5);
                bulletDmg = 10;
                bulletWallDmg = 10;
            }
            if (bulletFromWhatWeapon == WEAPONTYPE.GRENADE) //if bullet is from grenade
            {
                boundingBox.Max = position + new Vector3(0.05f, 0.9f, 0.05f);
                boundingBox.Min = position + new Vector3(-0.05f, 0.8f, -0.05f);
                bulletDmg = 51;
                bulletWallDmg = 150;
            }
        }

        //setters and getters
        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }
        public Vector3 MousePosition
        {
            get { return mousePosition; }
            set { mousePosition = value; }
        }
        public Vector3 Increment
        {
            get { return increment; }
            set { increment = value; }
        }
        public float Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }
        public BoundingBox BoundingBox
        {
            get { return boundingBox; }
            set { boundingBox = value; }
        }
        public int BulletDmg
        {
            get { return bulletDmg; }
            set { bulletDmg = value; }
        }
        public int BulletWallDmg
        {
            get { return bulletWallDmg; }
            set { bulletWallDmg = value; }
        }
        public WEAPONTYPE BulletFromWhatWeapon
        {
            get { return bulletFromWhatWeapon; }
            set { bulletFromWhatWeapon = value; }
        }
    }
}
