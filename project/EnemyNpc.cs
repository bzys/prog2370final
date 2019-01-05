/*
 *  Author:     Bill Sun        
 *  Comments:   this class is an enemy npc class
 *  Revision History: 
 *      2018-12-9: latest
 *      2018-12-1: created
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;

namespace BSunFinalProject
{
    class EnemyNpc : DrawableGameComponent
    {
        private Matrix cameraViewMatrix;
        private Vector3 cameraPosition; //position of camera
        private Vector3 modelPosition; //position of model
        private Vector3 playerPosition;
        private Vector3 rotVector;
        const float deathMaxRotAngle = 1.5708f; // 90 degree rotation
        private float rotAngle = 0f;
        private float deathRotAngle = 0f;
        private float delta = 0f;

        //player health
        private int npcHealth = 100;
        private bool npcIsDead = false;
        private int deathAnimationCounter = 1000; //2 seconds for death animation

        //these are my bounding boxes for collision, I have 5 separate ones
        private BoundingBox bulletCollisionBox;
        private Vector3 bulletColBoxMin = new Vector3(-0.3f, 0f, -0.3f);
        private Vector3 bulletColBoxMax = new Vector3(0.3f, 1.8f, 0.3f);
        private BoundingBox boundingBoxXPos;
        private BoundingBox boundingBoxXNeg;
        private BoundingBox boundingBoxZPos;
        private BoundingBox boundingBoxZNeg;
        private Vector3 bbXNMin = new Vector3(-0.4f, 0f, -0.3f);
        private Vector3 bbXNMax = new Vector3(-0.3f, 1.8f, 0.3f);
        private Vector3 bbXPMin = new Vector3(0.3f, 0f, -0.3f);
        private Vector3 bbXPMax = new Vector3(0.4f, 1.8f, 0.3f);
        private Vector3 bbZNMin = new Vector3(-0.3f, 0f, -0.4f);
        private Vector3 bbZNMax = new Vector3(0.3f, 1.8f, -0.3f);
        private Vector3 bbZPMin = new Vector3(-0.3f, 0f, 0.3f);
        private Vector3 bbZPMax = new Vector3(0.3f, 1.8f, 0.4f);

        //npcs only have 1 weapon
        private WeaponClass weaponClass1;

        //for bullet
        private List<BulletClass> bulletList;
        private int timeLastBullet = 0;
        private int ROFInterrupt = 0;
        private bool reloadWeapon = false;
        private bool outOfAmmo = false;
        int reloadTimer = 0;

        //sound effects
        SoundEffect sfxAk47Shot;
        SoundEffect sfxAk47Charge;
        SoundEffect sfxMoss500Shot;
        SoundEffect sfxMoss500pump;

        #region limb movement
        //variables for limb movement
        private bool isMoving = false; //is the character currently moving?

        private float prevLegRotAngle = 0f;
        private bool legForward = true; //is the leg moving forward or backwards? true is forwards
        private float prevArmRotAngle = 0f;
        private bool armUp = true; //is arm moving up or down? true is up
        private float prevHeadRotAngle = 0f;
        private bool headLimit = true; //has end of head movment ended, time to turn other way?

        private static float MAXHEADANGLE = MathHelper.ToRadians(15);
        private static float LEGANGLEPERTICK = MathHelper.ToRadians(0.1f);
        private static float MAXLEGANGLE = MathHelper.ToRadians(20);
        private static float HEADANGLEPERTICK = MathHelper.ToRadians(0.02f);
        private static float MAXARMANGLE = MathHelper.ToRadians(10);
        private static float ARMANGLEPERTICK = MathHelper.ToRadians(0.02f);
        #endregion

        #region model body parts
        private Model modelDead;
        private Model modelTorso;
        private Model modelHead;
        private Model modelArmL;
        private Model modelArmR;
        private Model modelLegL;
        private Model modelLegR;

        private Vector3 torsoOffset;
        private Vector3 headOffset;
        private Vector3 armLeftOffset;
        private Vector3 armRightOffSet;
        private Vector3 legLeftOffset;
        private Vector3 legRightOffset;
        #endregion

        #region model lighting

        private Vector3 diffuseVector;
        private Vector3 lightingDirection;
        private Vector3 specularColor;

        #endregion

        public EnemyNpc(Game game, Vector3 modelPosition, WEAPONTYPE weapon) : base(game)
        {
            this.modelPosition = modelPosition;

            //loading the models for the player, the player has separate head, torso and limbs
            modelTorso = game.Content.Load<Model>("personCubeTorso");
            modelHead = game.Content.Load<Model>("personCubeHead");
            modelArmL = game.Content.Load<Model>("personCubeArm");
            modelArmR = game.Content.Load<Model>("personCubeArm");
            modelLegL = game.Content.Load<Model>("personCubeLeg");
            modelLegR = game.Content.Load<Model>("personCubeLeg");
            modelDead = game.Content.Load<Model>("personCubeDead");

            sfxAk47Shot = game.Content.Load<SoundEffect>("SFX/sfxAK47gunshot");
            sfxAk47Charge = game.Content.Load<SoundEffect>("SFX/sfxAK47charge");
            sfxMoss500Shot = game.Content.Load<SoundEffect>("SFX/sfxMoss500gunshot");
            sfxMoss500pump = game.Content.Load<SoundEffect>("SFX/sfxMoss500pump");

            //offsetting where the limbs and stuff are
            torsoOffset = new Vector3(0, 0.4f, 0);
            headOffset = new Vector3(0, 1.0f, 0);
            armLeftOffset = new Vector3(-0.25f, 0.9f, 0);
            armRightOffSet = new Vector3(0.25f, 0.9f, 0);
            legLeftOffset = new Vector3(-0.11f, 0.4f, 0);
            legRightOffset = new Vector3(0.11f, 0.4f, 0);

            //bounding box, first is min, second is max
            bulletCollisionBox = new BoundingBox(bulletColBoxMin, bulletColBoxMax);
            bulletList = new List<BulletClass>(); //bullet list

            //the bounding boxes for collision detection
            boundingBoxXNeg = new BoundingBox(bbXNMin, bbXNMax);
            boundingBoxXPos = new BoundingBox(bbXPMax, bbXNMax);
            boundingBoxZNeg = new BoundingBox(bbZNMin, bbZNMax);
            boundingBoxZPos = new BoundingBox(bbZPMin, bbZPMax);

            weaponClass1 = new WeaponClass(game, weapon, modelPosition); //weapon class will load weapon model based on WEAPONTYPE
            game.Components.Add(weaponClass1);
        }

        public override void Draw(GameTime gameTime)
        {
            if (!npcIsDead)
            {
                //draw body
                foreach (var mesh in modelTorso.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.PreferPerPixelLighting = true;

                        effect.World = Matrix.CreateTranslation(torsoOffset);
                        effect.World *= Matrix.CreateFromAxisAngle(effect.World.Up, rotAngle);
                        effect.World *= Matrix.CreateTranslation(modelPosition);

                        effect.View = cameraViewMatrix;
                        effect.Projection = _Initializations.GetProjectionMatrix();

                        effect.LightingEnabled = true;
                        effect.DirectionalLight0.DiffuseColor = diffuseVector; //RGB values
                        effect.DirectionalLight0.Direction = lightingDirection; //vector3 position of camera
                        effect.DirectionalLight0.SpecularColor = specularColor; //RGB values
                    }
                    mesh.Draw();
                }
                //draw head
                foreach (var mesh in modelHead.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.PreferPerPixelLighting = true;

                        effect.World = Matrix.CreateTranslation(headOffset);
                        effect.World *= Matrix.CreateFromAxisAngle(effect.World.Up, rotAngle);
                        effect.World *= Matrix.CreateFromAxisAngle(effect.World.Up, prevHeadRotAngle);
                        effect.World *= Matrix.CreateTranslation(modelPosition);

                        effect.View = cameraViewMatrix;
                        effect.Projection = _Initializations.GetProjectionMatrix();

                        effect.LightingEnabled = true;
                        effect.DirectionalLight0.DiffuseColor = diffuseVector; //RGB values
                        effect.DirectionalLight0.Direction = lightingDirection; //vector3 position of camera
                        effect.DirectionalLight0.SpecularColor = specularColor; //RGB values

                    }
                    mesh.Draw();
                }
                //draw left arm
                foreach (var mesh in modelArmL.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.PreferPerPixelLighting = true;

                        effect.World = Matrix.CreateTranslation(armLeftOffset);
                        effect.World *= Matrix.CreateFromAxisAngle(effect.World.Up, rotAngle);
                        effect.World *= Matrix.CreateTranslation(new Vector3(0, -0.9f, 0))
                                        * Matrix.CreateFromAxisAngle(effect.World.Left, prevArmRotAngle)
                                        * Matrix.CreateTranslation(new Vector3(0, 0.9f, 0));
                        effect.World *= Matrix.CreateTranslation(modelPosition);

                        effect.View = cameraViewMatrix;
                        effect.Projection = _Initializations.GetProjectionMatrix();

                        effect.LightingEnabled = true;
                        effect.DirectionalLight0.DiffuseColor = diffuseVector; //RGB values
                        effect.DirectionalLight0.Direction = lightingDirection; //vector3 position of camera
                        effect.DirectionalLight0.SpecularColor = specularColor; //RGB values
                    }
                    mesh.Draw();
                }
                //draw right arm
                foreach (var mesh in modelArmR.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.PreferPerPixelLighting = true;

                        effect.World = Matrix.CreateTranslation(armRightOffSet);
                        effect.World *= Matrix.CreateFromAxisAngle(effect.World.Up, rotAngle);
                        effect.World *= Matrix.CreateTranslation(modelPosition);

                        effect.View = cameraViewMatrix;
                        effect.Projection = _Initializations.GetProjectionMatrix();

                        effect.LightingEnabled = true;
                        effect.DirectionalLight0.DiffuseColor = diffuseVector; //RGB values
                        effect.DirectionalLight0.Direction = lightingDirection; //vector3 position of camera
                        effect.DirectionalLight0.SpecularColor = specularColor; //RGB values
                    }
                    mesh.Draw();
                }
                //draw left leg
                foreach (var mesh in modelLegL.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.PreferPerPixelLighting = true;

                        effect.World = Matrix.CreateTranslation(legLeftOffset);
                        effect.World *= Matrix.CreateFromAxisAngle(effect.World.Up, rotAngle);
                        effect.World *= Matrix.CreateTranslation(new Vector3(0, -0.4f, 0))
                                        * Matrix.CreateFromAxisAngle(effect.World.Left, -prevLegRotAngle)
                                        * Matrix.CreateTranslation(new Vector3(0, 0.4f, 0));
                        effect.World *= Matrix.CreateTranslation(modelPosition);

                        effect.View = cameraViewMatrix;
                        effect.Projection = _Initializations.GetProjectionMatrix();

                        effect.LightingEnabled = true;
                        effect.DirectionalLight0.DiffuseColor = diffuseVector; //RGB values
                        effect.DirectionalLight0.Direction = lightingDirection; //vector3 position of camera
                        effect.DirectionalLight0.SpecularColor = specularColor; //RGB values
                    }
                    mesh.Draw();
                }
                //draw right leg
                foreach (var mesh in modelLegR.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.PreferPerPixelLighting = true;

                        effect.World = Matrix.CreateTranslation(legRightOffset);
                        effect.World *= Matrix.CreateFromAxisAngle(effect.World.Up, rotAngle);
                        effect.World *= Matrix.CreateTranslation(new Vector3(0, -0.4f, 0))
                                        * Matrix.CreateFromAxisAngle(effect.World.Left, prevLegRotAngle)
                                        * Matrix.CreateTranslation(new Vector3(0, 0.4f, 0));
                        effect.World *= Matrix.CreateTranslation(modelPosition);

                        effect.View = cameraViewMatrix;
                        effect.Projection = _Initializations.GetProjectionMatrix();

                        effect.LightingEnabled = true;
                        effect.DirectionalLight0.DiffuseColor = diffuseVector; //RGB values
                        effect.DirectionalLight0.Direction = lightingDirection; //vector3 position of camera
                        effect.DirectionalLight0.SpecularColor = specularColor; //RGB values
                    }
                    mesh.Draw();
                }
            }
            else
            {
                //draw the one-piece dead model
                foreach (var mesh in modelDead.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.PreferPerPixelLighting = true;

                        effect.World = Matrix.CreateTranslation(torsoOffset);
                        effect.World *= Matrix.CreateFromAxisAngle(effect.World.Up, rotAngle);
                        effect.World *= Matrix.CreateFromAxisAngle(effect.World.Left, deathRotAngle);
                        effect.World *= Matrix.CreateTranslation(modelPosition);

                        effect.View = cameraViewMatrix;
                        effect.Projection = _Initializations.GetProjectionMatrix();

                        effect.LightingEnabled = true;
                        effect.DirectionalLight0.DiffuseColor = diffuseVector; //RGB values
                        effect.DirectionalLight0.Direction = lightingDirection; //vector3 position of camera
                        effect.DirectionalLight0.SpecularColor = specularColor; //RGB values
                    }
                    mesh.Draw();
                }
            }

            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            //what do do on death
            if (npcHealth <= 0)
            {
                weaponClass1.Enabled = false;
                weaponClass1.Visible = false;
                npcIsDead = true;
            }

            //if npc is not death, do these
            if (!npcIsDead)
            {
                //rotation angle for the entire player model
                rotVector = new Vector3(modelPosition.X - playerPosition.X, 0f, modelPosition.Z - playerPosition.Z);
                rotAngle = (float)Math.Atan2(rotVector.X, rotVector.Z);

                //updating npc bullet collision box
                bulletCollisionBox.Min = modelPosition + bulletColBoxMin;
                bulletCollisionBox.Max = modelPosition + bulletColBoxMax;

                //weapon 1 model position, camera matrix, rotation
                weaponClass1.WeaponPosition = modelPosition; //moving weapon based on player model
                weaponClass1.CameraViewMatrix = cameraViewMatrix; //moving weapon camera based on player model
                weaponClass1.WeaponRotation = rotAngle;

                weaponClass1.DiffuseVector = diffuseVector;
                weaponClass1.LightingDirection = lightingDirection;
                weaponClass1.SpecularVector = specularColor;

                #region leg rotations
                float legAngle = 0f;
                legAngle = prevLegRotAngle;

                if (legAngle < MAXLEGANGLE && legForward == true && isMoving) //if leg is moving forwards and can move forwards
                {
                    prevLegRotAngle += LEGANGLEPERTICK * delta;
                }
                else if (legAngle > MAXLEGANGLE) //if leg can't move forwards anymore
                {
                    legForward = false; //leg is now moving backwards
                }
                if (legAngle > -MAXLEGANGLE && legForward == false && isMoving) //if leg is moving backwards and can move backwards
                {
                    prevLegRotAngle -= LEGANGLEPERTICK * delta;
                }
                else if (legAngle < -MAXLEGANGLE) //if leg can't move backwards anymore
                {
                    legForward = true; //leg is now moving forwards
                }

                if (!isMoving)
                {
                    prevLegRotAngle = 0f;
                }
                #endregion

                #region head movement

                float headAngle = 0f;
                headAngle = prevHeadRotAngle;

                if (headAngle < MAXHEADANGLE && headLimit && !isMoving) //if head has not reached limit of rotation to one side, and char is not moving
                {
                    prevHeadRotAngle += HEADANGLEPERTICK * delta;
                }
                else if (headAngle > MAXHEADANGLE)
                {
                    headLimit = false;
                }
                if (headAngle > -MAXHEADANGLE && !headLimit && !isMoving)
                {
                    prevHeadRotAngle -= HEADANGLEPERTICK * delta;
                }
                else if (headAngle < -MAXHEADANGLE)
                {
                    headLimit = true;
                }

                if (isMoving) //if character is moving, stop head rotation
                {
                    prevHeadRotAngle = 0f;
                }

                #endregion

                #region arm movement

                float armAngle = 0f;
                armAngle = prevArmRotAngle;

                if (armAngle < MAXARMANGLE && armUp)
                {
                    prevArmRotAngle += ARMANGLEPERTICK * delta;
                }
                else if (armAngle > MAXARMANGLE)
                {
                    armUp = false;
                }
                if (armAngle > -MAXARMANGLE && !armUp)
                {
                    prevArmRotAngle -= ARMANGLEPERTICK * delta;
                }
                else if (armAngle < -MAXARMANGLE)
                {
                    armUp = true;
                }

                #endregion
            }
            else
            {
                if (deathAnimationCounter > 0) //the death animation will happen over the time it takes for this counter to count down
                {
                    deathRotAngle -= (deathMaxRotAngle / 65);

                    deathAnimationCounter -= gameTime.ElapsedGameTime.Milliseconds; //decrement timer
                }
            }

            //rotation angles for the limbs 

            
            /*
            //this is commented out because it doesn't work
            //calculating distance between npc and player
            float distanceBetween = Vector3.Distance(playerPosition, modelPosition);
            reloadTimer += gameTime.ElapsedGameTime.Milliseconds;

            if (distanceBetween < 10) //if distance between player and npc is less than 10 units, start shooting
            {
                timeLastBullet += gameTime.ElapsedGameTime.Milliseconds;

                Random random = new Random();
                if (weaponClass1.WeaponType == WEAPONTYPE.AK47)
                {
                    if (timeLastBullet > 100 && weaponClass1.WeaponAmmoInWeapon > 0) //&& weaponClass1.WeaponAmmoInWeapon > 0
                    {
                        double offSetAmount = 0.25; //the amount to offset the mouse position

                        //calculating the offset for X and Z
                        float offsetX = (float)(random.NextDouble() * (((modelPosition.X * offSetAmount)) - ((modelPosition.X * offSetAmount)) + ((modelPosition.X * offSetAmount))));
                        float offsetZ = (float)(random.NextDouble() * (((modelPosition.Z * offSetAmount)) - ((modelPosition.Z * offSetAmount)) + ((modelPosition.Z * offSetAmount))));
                        offsetX += modelPosition.X; //applying offset to X and Z
                        offsetZ += modelPosition.Z;

                        float rotAngleOffset = (float)Math.Atan2(offsetX, offsetZ); //calculating the bullet angle based off of the offset
                        Vector3 bulletIncrement = new Vector3((offsetX * 10) + offsetX, 0, (offsetZ * 10) + offsetZ); //amount to increment bullet per update by

                        bulletIncrement.Normalize();

                        bulletList.Add(new BulletClass(weaponClass1.WeaponPosition, playerPosition, bulletIncrement * 0.5f, rotAngle, WEAPONTYPE.SHOTGUN)); //adding new bullet to bulletList
                        weaponClass1.WeaponAmmoInWeapon--; //subtracting ammo in weapon per bullet fired
                        timeLastBullet = 0;
                        sfxAk47Shot.Play(); //play sound effect for ak47
                    }
                    else if (weaponClass1.WeaponAmmoInWeapon == 0) //if ammo is not greater than 0, means gun has run out of bullet
                    {
                        reloadWeapon = true;
                        reloadTimer = 0;
                    }
                }
                if (weaponClass1.WeaponType == WEAPONTYPE.SHOTGUN) // && weaponClass2.WeaponAmmoInWeapon > 0
                {
                    if (timeLastBullet > 1000 && ROFInterrupt <= 0 && weaponClass1.WeaponAmmoInWeapon > 0)
                    {
                        double offSetAmountX = 0.5; //the amount to offset the mouse position
                        double offSetAmountZ = 0.5;

                        //these 4 if statements will adjust the amount of x and z offsets for the shotgun spread, this is needed as the offset
                        //is randomly generated based on a set amount from the mouse position, and will change based on the current mouse x and y position
                        if (playerPosition.X > -1.5 && playerPosition.X < 1.5)
                            offSetAmountX = 3;
                        if (playerPosition.Z > -1.5 && playerPosition.Z < 1.5)
                            offSetAmountZ = 3;
                        if (playerPosition.X > -0.5 && playerPosition.X < 0.5)
                            offSetAmountX = 10;
                        if (playerPosition.Z > -0.5 && playerPosition.Z < 0.5)
                            offSetAmountZ = 10;

                        for (int i = 0; i < 7; i++) //this for loop will add 7 bullets to the bulletList every time the shotgun is fired
                        {
                            //calculating the offset for X and Z
                            float offsetX = (float)(random.NextDouble() * (((playerPosition.X * offSetAmountX)) - ((playerPosition.X * offSetAmountX)) + ((playerPosition.X * offSetAmountX))));
                            float offsetZ = (float)(random.NextDouble() * (((playerPosition.Z * offSetAmountZ)) - ((playerPosition.Z * offSetAmountZ)) + ((playerPosition.Z * offSetAmountZ))));
                            offsetX += playerPosition.X; //applying offset to X and Z
                            offsetZ += playerPosition.Z;

                            float rotAngleOffset = (float)Math.Atan2(offsetX, offsetZ); //calculating the bullet angle based off of the offset
                            Vector3 bulletIncrement = new Vector3((offsetX * 10) + offsetX, 0, (offsetZ * 10) + offsetZ); //amount to increment bullet per update by

                            bulletIncrement.Normalize();

                            bulletList.Add(new BulletClass(weaponClass1.WeaponPosition, playerPosition, bulletIncrement * 0.3f, rotAngleOffset, weaponClass1.WeaponType)); //adding new bullet to bulletList
                        }
                        weaponClass1.WeaponAmmoInWeapon--; //subtracting ammo in weapon per bullet fired
                        timeLastBullet = 0; //if fired, set to 0
                        ROFInterrupt = 1000; //if fired, set the weapon reload time, which is 1 second
                        sfxMoss500Shot.Play();
                    }
                    else if (weaponClass1.WeaponAmmoInWeapon == 0) //if ammo is not greater than 0, means gun has run out of bullet
                    {
                        reloadWeapon = true;
                    }
                    else if (reloadWeapon == true)
                    {
                        ROFInterrupt -= gameTime.ElapsedGameTime.Milliseconds;
                    }
                }
            }
            
            if (reloadWeapon)
            {
                if (weaponClass1.WeaponType == WEAPONTYPE.AK47)
                {
                    if (reloadTimer > weaponClass1.WeaponReloadTime)
                    {
                        weaponClass1.WeaponAmmoInWeapon = weaponClass1.WeaponMagSize;
                        reloadWeapon = false;
                    }
                }
                if (weaponClass1.WeaponType == WEAPONTYPE.SHOTGUN && weaponClass1.WeaponAmmoInWeapon < weaponClass1.WeaponMagSize)
                {
                    if (reloadTimer > weaponClass1.WeaponReloadTime)
                    {
                        weaponClass1.WeaponAmmoInWeapon++;
                    }
                }
                else if (weaponClass1.WeaponAmmoInWeapon >= weaponClass1.WeaponMagSize)
                {
                    reloadWeapon = false;
                }
            }
            */

            base.Update(gameTime);
        }

        //setters and getters
        public Matrix CameraMatrix
        {
            get { return cameraViewMatrix; }
            set { cameraViewMatrix = value; }
        }
        public Vector3 CameraPosition
        {
            set { cameraPosition = value; }
        }
        public Vector3 PlayerPosition
        {
            set { playerPosition = value; }
        }
        public Vector3 DiffuseVector
        {
            set { diffuseVector = value; }
        }
        public Vector3 LightingDirection
        {
            set { lightingDirection = value; }
        }
        public Vector3 SpecularVector
        {
            set { specularColor = value; }
        }
        public List<BulletClass> BulletList
        {
            get { return bulletList; }
            set { bulletList = value; }
        }
        public BoundingBox BulletCollisionBox
        {
            get { return bulletCollisionBox; }
        }
        public int NpcHealth
        {
            get { return npcHealth; }
            set { npcHealth = value; }
        }
    }
}
