/*
 *  Author:     Bill Sun        
 *  Comments:   the weapons class, handles drawing the weapon itself, and setting weapons attribute 
 *  Revision History: 
 *      2018-12-6 latest
 *      2018-11-18: created
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
    class WeaponClass : DrawableGameComponent
    {
        private Matrix cameraViewMatrix;
        private Model weaponModel;
        private Vector3 offsetOrigin = new Vector3(0.25f, 0.9f, -0.5f);
        private Vector3 modelPosition;
        private float weaponRotation;
        private WEAPONTYPE weaponType;
        private int weaponAmmoInWeapon;
        private int weaponMagSize;
        private int weaponMaxAmmo;
        private int weaponROF;
        private int reloadTime;

        public Vector3 DiffuseVector { set; get; }
        public Vector3 LightingDirection { set; get; }
        public Vector3 SpecularVector { set; get; }

        public WeaponClass(Game game, WEAPONTYPE weaponType, Vector3 modelPosition) : base(game)
        {
            this.modelPosition = modelPosition;
            this.weaponType = weaponType;

            //based on input weapon type, will set its weapon attributes to different things
            if (weaponType == WEAPONTYPE.AK47)
            {
                weaponModel = game.Content.Load<Model>("gunAK47");
                weaponAmmoInWeapon = 30;
                weaponMagSize = 30;
                weaponMaxAmmo = 120;
                weaponROF = 100; //rounds per second
                reloadTime = 4000; //3 seconds for mag
            }
            if (weaponType == WEAPONTYPE.SHOTGUN)
            {
                weaponModel = game.Content.Load<Model>("gunPumpShotgun");
                weaponAmmoInWeapon = 7;
                weaponMagSize = 7;
                weaponMaxAmmo = 42;
                weaponROF = 1000; //1 round per 2 seconds
                reloadTime = 1500; //1.5 seconds per shell
            }
            if (weaponType == WEAPONTYPE.GRENADE)
            {
                weaponModel = game.Content.Load<Model>("gunGrenade");
                weaponAmmoInWeapon = 1;
                weaponMagSize = 1;
                weaponMaxAmmo = 3;
                weaponROF = 5000; //1 grenade per 5 seconds
                reloadTime = 5000;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (var mesh in weaponModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;

                    effect.World = Matrix.Identity;
                    effect.World = Matrix.CreateTranslation(offsetOrigin);
                    effect.World *= Matrix.CreateFromAxisAngle(effect.World.Up, weaponRotation);
                    effect.World *= Matrix.CreateTranslation(modelPosition);

                    effect.View = cameraViewMatrix;
                    effect.Projection = _Initializations.GetProjectionMatrix();

                    effect.LightingEnabled = true;
                    
                    effect.DirectionalLight0.DiffuseColor = DiffuseVector; //RGB values
                    effect.DirectionalLight0.Direction = LightingDirection; //vector3 position of camera
                    effect.DirectionalLight0.SpecularColor = SpecularVector; //RGB values
                }
                mesh.Draw();
            }

            base.Draw(gameTime);
        }

        //getters and setters
        public float WeaponRotation
        {
            get { return weaponRotation; }
            set { weaponRotation = value; }
        }

        public Matrix CameraViewMatrix
        {
            set { cameraViewMatrix = value; }
        }

        public Vector3 WeaponPosition
        {
            get { return modelPosition; }
            set { modelPosition = value; }
        }

        public WEAPONTYPE WeaponType
        {
            get { return weaponType; }
        }

        public int WeaponAmmoInWeapon
        {
            get { return weaponAmmoInWeapon; }
            set { weaponAmmoInWeapon = value; }
        }

        public int WeaponMagSize
        {
            get { return weaponMagSize; }
            set { weaponMagSize = value; }
        }
        public int WeaponMaxAmmo
        {
            get { return weaponMaxAmmo; }
            set { weaponMaxAmmo = value; }
        }
        public int WeaponROF
        {
            get { return weaponROF; }
            set { weaponROF = value; }
        }
        public int WeaponReloadTime
        {
            get { return reloadTime; }
            set { reloadTime = value; }
        }
    }
}
