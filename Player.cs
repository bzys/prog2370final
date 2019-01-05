/*
 *  Author:     Bill Sun        
 *  Comments:   the player class, handles drawing the player, which weapons the player has, and shooting weapons
 *  Revision History: 
 *      2018-12-9: latest
 *      2018-11-10: created
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;

namespace BSunFinalProject
{
    public class Player : DrawableGameComponent
    {
        SpriteBatch spriteBatch;

        private Matrix cameraViewMatrix;
        private Vector3 cameraPosition; //position of camera
        private Vector3 modelPosition; //position of model
        private Vector3 mousePosition; //position of mouse cursor
        private float rotAngle = 0f;
        private float delta = 0f;

        //player health
        private int playerHealth = 100;
        private bool playerIsDead = false;

        //these are my bounding boxes for collision, I have 5 separate ones
        private BoundingBox bulletCollisionBox;
        //the player has a separate bounding box for collisions vs bullets
        private Vector3 bulletColBoxMin = new Vector3(-0.3f, 0f, -0.3f);
        private Vector3 bulletColBoxMax = new Vector3(0.3f, 1.8f, 0.3f);
        private BoundingBox boundingBoxXPos;
        private BoundingBox boundingBoxXNeg;
        private BoundingBox boundingBoxZPos;
        private BoundingBox boundingBoxZNeg;
        //these vectors are the player collision with walls bounding boxes
        private Vector3 bbXNMin = new Vector3(-0.4f, 0f, -0.3f); 
        private Vector3 bbXNMax = new Vector3(-0.3f, 1.8f, 0.3f);
        private Vector3 bbXPMin = new Vector3(0.3f, 0f, -0.3f); 
        private Vector3 bbXPMax = new Vector3(0.4f, 1.8f, 0.3f);
        private Vector3 bbZNMin = new Vector3(-0.3f, 0f, -0.4f);
        private Vector3 bbZNMax = new Vector3(0.3f, 1.8f, -0.3f);
        private Vector3 bbZPMin = new Vector3(-0.3f, 0f, 0.3f);
        private Vector3 bbZPMax = new Vector3(0.3f, 1.8f, 0.4f);

        //player can have up to 3 weapons
        private WeaponClass weaponClass1;
        private WeaponClass weaponClass2;
        private WeaponClass weaponClass3;

        //variables for currently held weapon
        private WEAPONTYPE currWeapon;
        private WEAPONTYPE weaponSwitchTo;
        private int currWeaponROF;
        private int currWeaponAmmoInWeapon;
        private int currWeaponMagSize;
        private int currWeaponAmmoTotal;
        private int currWeaponReloadTime;
        private float previousScrollWheelValue = 0;

        //for bullet
        private List<BulletClass> bulletList;
        private int timeLastBullet = 0;
        private int ROFInterrupt = 0;
        private bool reloadWeapon = false;
        private bool outOfAmmo = false;

        //for grenade throwing
        private Vector3 grenadeIncrement;
        private Vector3 grenadeInitialPosition;
        private int grenadeInterrupt = 0;

        BasicEffect boxEffect;

        //sound effects
        SoundEffect sfxAk47Shot;
        SoundEffect sfxAk47Charge;
        SoundEffect sfxMoss500Shot;
        SoundEffect sfxMoss500pump;
        Song sfxWeaponSwitch;
        Song sfxWeaponDryFire;
        bool isDryFiring = false;

        #region limb movement
        //variables for limb movement
        private bool isMoving = false; //is the character currently moving?

        private float prevLegRotAngle = 0f;
        private bool legForward = true; //is the leg moving forward or backwards? true is forwards
        private float prevArmRotAngle = 0f;
        private bool armUp = true; //is arm moving up or down? true is up
        private float prevHeadRotAngle = 0f;
        private bool headLimit = true; //has end of head movment ended, time to turn other way?

        //these are the various idle animation angles to move limbs and stuff
        private static float MAXHEADANGLE = MathHelper.ToRadians(15);
        private static float LEGANGLEPERTICK = MathHelper.ToRadians(0.1f);
        private static float MAXLEGANGLE = MathHelper.ToRadians(20);
        private static float HEADANGLEPERTICK = MathHelper.ToRadians(0.02f);
        private static float MAXARMANGLE = MathHelper.ToRadians(10);
        private static float ARMANGLEPERTICK = MathHelper.ToRadians(0.02f);
        #endregion

        #region model body parts
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

        public Vector3 DiffuseVector { set; get; }
        public Vector3 LightingDirection { set; get; }
        public Vector3 SpecularVector { set; get; }

        #endregion
        
        public Player(Game game, GraphicsDeviceManager graphics, Vector3 startingPosition, SpriteBatch spriteBatch, WEAPONTYPE weapon1, WEAPONTYPE weapon2) : base(game)
        {
            this.spriteBatch = spriteBatch;

            modelPosition = startingPosition + new Vector3(2.0f, 0.0f, -2.0f); //moving the player's starting position

            //loading the models for the player, the player has separate head, torso and limbs
            modelTorso = game.Content.Load<Model>("personCubeTorso");
            modelHead = game.Content.Load<Model>("personCubeHead");
            modelArmL = game.Content.Load<Model>("personCubeArm");
            modelArmR = game.Content.Load<Model>("personCubeArm");
            modelLegL = game.Content.Load<Model>("personCubeLeg");
            modelLegR = game.Content.Load<Model>("personCubeLeg");

            //loading sound effects for various weapons
            sfxAk47Shot = game.Content.Load<SoundEffect>("SFX/sfxAK47gunshot");
            sfxAk47Charge = game.Content.Load<SoundEffect>("SFX/sfxAK47charge");
            sfxMoss500Shot = game.Content.Load<SoundEffect>("SFX/sfxMoss500gunshot");
            sfxMoss500pump = game.Content.Load<SoundEffect>("SFX/sfxMoss500pump");
            sfxWeaponSwitch = game.Content.Load<Song>("SFX/sfxWeaponSwitch");
            sfxWeaponDryFire = game.Content.Load<Song>("SFX/sfxDryFire");

            //the offsets for each limb, relative to 0, 0, 0
            torsoOffset = new Vector3(0, 0.4f, 0);
            headOffset = new Vector3(0, 1.0f, 0);
            armLeftOffset = new Vector3(-0.25f, 0.9f, 0);
            armRightOffSet = new Vector3(0.25f, 0.9f, 0);
            legLeftOffset = new Vector3(-0.11f, 0.4f, 0);
            legRightOffset = new Vector3(0.11f, 0.4f, 0);

            //new effect for debug drawing
            boxEffect = new BasicEffect(game.GraphicsDevice);
            //bounding box, first is min, second is max
            bulletCollisionBox = new BoundingBox(bulletColBoxMin, bulletColBoxMax);
            bulletList = new List<BulletClass>();

            //bounding boxes for each axis
            boundingBoxXNeg = new BoundingBox(bbXNMin, bbXNMax);
            boundingBoxXPos = new BoundingBox(bbXPMax, bbXNMax);
            boundingBoxZNeg = new BoundingBox(bbZNMin, bbZNMax);
            boundingBoxZPos = new BoundingBox(bbZPMin, bbZPMax);

            currWeapon = weapon1; //the player will spawn with the first weapon
            weaponSwitchTo = weapon1;

            weaponClass1 = new WeaponClass(game, weapon1, modelPosition); //weapon class will load weapon model based on WEAPONTYPE
            weaponClass2 = new WeaponClass(game, weapon2, modelPosition);
            weaponClass3 = new WeaponClass(game, WEAPONTYPE.GRENADE, modelPosition);

            game.Components.Add(weaponClass1);
            game.Components.Add(weaponClass2);
            game.Components.Add(weaponClass3);
        }

        public override void Draw(GameTime gameTime)
        {
            //drawing the bounding boxes for visualization
            //drawBoundingBox(bulletCollisionBox);
            //drawBoundingBox(boundingBoxXNeg);
            //drawBoundingBox(boundingBoxXPos);
            //drawBoundingBox(boundingBoxZNeg);
            //drawBoundingBox(boundingBoxZPos);

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
                    effect.DirectionalLight0.DiffuseColor = DiffuseVector; //RGB values
                    effect.DirectionalLight0.Direction = LightingDirection; //vector3 position of camera
                    effect.DirectionalLight0.SpecularColor = SpecularVector; //RGB values
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
                    effect.DirectionalLight0.DiffuseColor = DiffuseVector; //RGB values
                    effect.DirectionalLight0.Direction = LightingDirection; //vector3 position of camera
                    effect.DirectionalLight0.SpecularColor = SpecularVector; //RGB values
                    
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
                                    *Matrix.CreateFromAxisAngle(effect.World.Left, prevArmRotAngle)
                                    *Matrix.CreateTranslation(new Vector3(0, 0.9f, 0));
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
                    effect.DirectionalLight0.DiffuseColor = DiffuseVector; //RGB values
                    effect.DirectionalLight0.Direction = LightingDirection; //vector3 position of camera
                    effect.DirectionalLight0.SpecularColor = SpecularVector; //RGB values
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
                    effect.DirectionalLight0.DiffuseColor = DiffuseVector; //RGB values
                    effect.DirectionalLight0.Direction = LightingDirection; //vector3 position of camera
                    effect.DirectionalLight0.SpecularColor = SpecularVector; //RGB values
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
                    effect.DirectionalLight0.DiffuseColor = DiffuseVector; //RGB values
                    effect.DirectionalLight0.Direction = LightingDirection; //vector3 position of camera
                    effect.DirectionalLight0.SpecularColor = SpecularVector; //RGB values
                }
                mesh.Draw();
            }

            base.Draw(gameTime);
        }
        
        public override void Update(GameTime gameTime)
        {
            //delta = gameTime.ElapsedGameTime.Milliseconds;

            MouseState mouseState = Mouse.GetState();
            KeyboardState keyPress = Keyboard.GetState();

            //rotation angle for the entire player model
            //how I learned to use Atan2: https://stackoverflow.com/questions/13694003/rotating-an-image-towards-the-mouses-current-position

            if (!playerIsDead) //only update player rotation movement if player is not dead
            {
                rotAngle = (float)Math.Atan2(-mousePosition.X, -mousePosition.Z);
            }

            //updating collision bounding boxs
            bulletCollisionBox.Min = modelPosition + bulletColBoxMin;
            bulletCollisionBox.Max = modelPosition + bulletColBoxMax;
            //updating collision bounding boxs
            boundingBoxXNeg.Min = modelPosition + bbXNMin;
            boundingBoxXNeg.Max = modelPosition + bbXNMax;
            boundingBoxXPos.Min = modelPosition + bbXPMin; 
            boundingBoxXPos.Max = modelPosition + bbXPMax;
            boundingBoxZNeg.Min = modelPosition + bbZNMin; 
            boundingBoxZNeg.Max = modelPosition + bbZNMax;
            boundingBoxZPos.Min = modelPosition + bbZPMin;
            boundingBoxZPos.Max = modelPosition + bbZPMax;

            //rotation angles for the limbs 

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

            //weapon 1 model position, camera matrix, rotation
            weaponClass1.WeaponPosition = modelPosition; //moving weapon based on player model
            weaponClass1.CameraViewMatrix = cameraViewMatrix; //moving weapon camera based on player model
            weaponClass1.WeaponRotation = rotAngle;

            weaponClass1.DiffuseVector = DiffuseVector;
            weaponClass1.LightingDirection = LightingDirection;
            weaponClass1.SpecularVector = SpecularVector;

            //weapon 2 model position, camera matrix, rotation
            weaponClass2.WeaponPosition = modelPosition; //moving weapon based on player model
            weaponClass2.CameraViewMatrix = cameraViewMatrix; //moving weapon camera based on player model
            weaponClass2.WeaponRotation = rotAngle;

            weaponClass2.DiffuseVector = DiffuseVector;
            weaponClass2.LightingDirection = LightingDirection;
            weaponClass2.SpecularVector = SpecularVector;

            //weapon 3 model position, camera matrix, rotation
            weaponClass3.WeaponPosition = modelPosition; //moving weapon based on player model
            weaponClass3.CameraViewMatrix = cameraViewMatrix; //moving weapon camera based on player model
            weaponClass3.WeaponRotation = rotAngle;

            weaponClass3.DiffuseVector = DiffuseVector;
            weaponClass3.LightingDirection = LightingDirection;
            weaponClass3.SpecularVector = SpecularVector;

            //changing which weapon the player holds based on which 1-3 key is pressed
            if (keyPress.IsKeyDown(Keys.D1))
            {
                MediaPlayer.Play(sfxWeaponSwitch);
                weaponSwitchTo = weaponClass1.WeaponType;
                reloadWeapon = false;
            }
            else if (keyPress.IsKeyDown(Keys.D2))
            {
                MediaPlayer.Play(sfxWeaponSwitch);
                weaponSwitchTo = weaponClass2.WeaponType;
                reloadWeapon = false;
            }
            else if (keyPress.IsKeyDown(Keys.D3))
            {
                MediaPlayer.Play(sfxWeaponSwitch);
                weaponSwitchTo = weaponClass3.WeaponType;
                reloadWeapon = false;
            }

            //changing which weapon the player holds based on current mouse wheel scroll position
            if (mouseState.ScrollWheelValue > previousScrollWheelValue)
            {
                MediaPlayer.Play(sfxWeaponSwitch);
                if (currWeapon == weaponClass1.WeaponType)
                {
                    weaponSwitchTo = weaponClass3.WeaponType;
                }
                else if (currWeapon == weaponClass2.WeaponType)
                {
                    weaponSwitchTo = weaponClass1.WeaponType;
                }
                else
                {
                    weaponSwitchTo = weaponClass2.WeaponType;
                }
                reloadWeapon = false;
            }
            if (mouseState.ScrollWheelValue < previousScrollWheelValue)
            {
                MediaPlayer.Play(sfxWeaponSwitch);
                if (currWeapon == weaponClass1.WeaponType)
                {
                    weaponSwitchTo = weaponClass2.WeaponType;
                }
                else if (currWeapon == weaponClass2.WeaponType)
                {
                    weaponSwitchTo = weaponClass3.WeaponType;
                }
                else
                {
                    weaponSwitchTo = weaponClass1.WeaponType;
                }
                reloadWeapon = false;
            }
            previousScrollWheelValue = mouseState.ScrollWheelValue;

            #region logic for weapon switching

            //weapon switching logic, when one weapon is selected, all other weapons are disabled and invisible. current player held weapon attributes also change
            if (currWeapon == weaponClass1.WeaponType)
            {
                weaponClass1.Enabled = true;
                weaponClass1.Visible = true;
                weaponClass2.Enabled = false;
                weaponClass2.Visible = false;
                weaponClass3.Enabled = false;
                weaponClass3.Visible = false;
                currWeaponAmmoInWeapon = weaponClass1.WeaponAmmoInWeapon;
                currWeaponAmmoTotal = weaponClass1.WeaponMaxAmmo;
                currWeaponMagSize = weaponClass1.WeaponMagSize;
                currWeaponROF = weaponClass1.WeaponROF;
                if(!reloadWeapon)
                    currWeaponReloadTime = weaponClass1.WeaponReloadTime;

                if (weaponSwitchTo == weaponClass2.WeaponType)
                {
                    weaponClass1.Enabled = false;
                    weaponClass1.Visible = false;
                    weaponClass2.Enabled = true;
                    weaponClass2.Visible = true;
                    weaponClass3.Enabled = false;
                    weaponClass3.Visible = false;
                    weaponClass1.WeaponAmmoInWeapon = currWeaponAmmoInWeapon;
                    weaponClass1.WeaponMaxAmmo = currWeaponAmmoTotal;
                    weaponClass1.WeaponMagSize = currWeaponMagSize;
                    currWeaponAmmoInWeapon = weaponClass2.WeaponAmmoInWeapon;
                    currWeaponAmmoTotal = weaponClass2.WeaponMaxAmmo;
                    currWeaponMagSize = weaponClass2.WeaponMagSize;
                    currWeaponROF = weaponClass2.WeaponROF;
                    currWeaponReloadTime = weaponClass2.WeaponReloadTime;
                }
                else if (weaponSwitchTo == weaponClass3.WeaponType)
                {
                    weaponClass1.Enabled = false;
                    weaponClass1.Visible = false;
                    weaponClass2.Enabled = false;
                    weaponClass2.Visible = false;
                    weaponClass3.Enabled = true;
                    if (weaponClass3.WeaponAmmoInWeapon != 0)
                    {
                        weaponClass3.Visible = true;
                    }
                    weaponClass1.WeaponAmmoInWeapon = currWeaponAmmoInWeapon;
                    weaponClass1.WeaponMaxAmmo = currWeaponAmmoTotal;
                    weaponClass1.WeaponMagSize = currWeaponMagSize;
                    currWeaponAmmoInWeapon = weaponClass3.WeaponAmmoInWeapon;
                    currWeaponAmmoTotal = weaponClass3.WeaponMaxAmmo;
                    currWeaponMagSize = weaponClass3.WeaponMagSize;
                    currWeaponROF = weaponClass3.WeaponROF;
                    currWeaponReloadTime = weaponClass3.WeaponReloadTime;
                }
            }
            else if (currWeapon == weaponClass2.WeaponType)
            {
                weaponClass2.Enabled = true;
                weaponClass2.Visible = true;
                weaponClass1.Enabled = false;
                weaponClass1.Visible = false;
                weaponClass3.Enabled = false;
                weaponClass3.Visible = false;
                currWeaponAmmoInWeapon = weaponClass2.WeaponAmmoInWeapon;
                currWeaponAmmoTotal = weaponClass2.WeaponMaxAmmo;
                currWeaponMagSize = weaponClass2.WeaponMagSize;
                currWeaponROF = weaponClass2.WeaponROF;
                if (!reloadWeapon)
                    currWeaponReloadTime = weaponClass2.WeaponReloadTime;

                if (weaponSwitchTo == weaponClass1.WeaponType)
                {
                    weaponClass1.Enabled = true;
                    weaponClass1.Visible = true;
                    weaponClass2.Enabled = false;
                    weaponClass2.Visible = false;
                    weaponClass3.Enabled = false;
                    weaponClass3.Visible = false;
                    weaponClass2.WeaponAmmoInWeapon = currWeaponAmmoInWeapon;
                    weaponClass2.WeaponMaxAmmo = currWeaponAmmoTotal;
                    weaponClass2.WeaponMagSize = currWeaponMagSize;
                    currWeaponAmmoInWeapon = weaponClass1.WeaponAmmoInWeapon;
                    currWeaponAmmoTotal = weaponClass1.WeaponMaxAmmo;
                    currWeaponMagSize = weaponClass1.WeaponMagSize;
                    currWeaponROF = weaponClass1.WeaponROF;
                    currWeaponReloadTime = weaponClass1.WeaponReloadTime;
                }
                else if (weaponSwitchTo == weaponClass3.WeaponType)
                {
                    weaponClass1.Enabled = false;
                    weaponClass1.Visible = false;
                    weaponClass2.Enabled = false;
                    weaponClass2.Visible = false;
                    weaponClass3.Enabled = true;
                    if (weaponClass3.WeaponAmmoInWeapon != 0)
                    {
                        weaponClass3.Visible = true;
                    }
                    weaponClass2.WeaponAmmoInWeapon = currWeaponAmmoInWeapon;
                    weaponClass2.WeaponMaxAmmo = currWeaponAmmoTotal;
                    weaponClass2.WeaponMagSize = currWeaponMagSize;
                    currWeaponAmmoInWeapon = weaponClass3.WeaponAmmoInWeapon;
                    currWeaponAmmoTotal = weaponClass3.WeaponMaxAmmo;
                    currWeaponMagSize = weaponClass3.WeaponMagSize;
                    currWeaponROF = weaponClass3.WeaponROF;
                    currWeaponReloadTime = weaponClass3.WeaponReloadTime;
                }
            }
            else if (currWeapon == weaponClass3.WeaponType)
            {
                weaponClass3.Enabled = true;
                if (weaponClass3.WeaponAmmoInWeapon != 0)
                {
                    weaponClass3.Visible = true;
                }
                weaponClass2.Enabled = false;
                weaponClass2.Visible = false;
                weaponClass1.Enabled = false;
                weaponClass1.Visible = false;
                currWeaponAmmoInWeapon = weaponClass3.WeaponAmmoInWeapon;
                currWeaponAmmoTotal = weaponClass3.WeaponMaxAmmo;
                currWeaponMagSize = weaponClass3.WeaponMagSize;
                currWeaponROF = weaponClass3.WeaponROF;
                if (!reloadWeapon)
                    currWeaponReloadTime = weaponClass3.WeaponReloadTime;

                if (weaponSwitchTo == weaponClass1.WeaponType)
                {
                    weaponClass1.Enabled = true;
                    weaponClass1.Visible = true;
                    weaponClass2.Enabled = false;
                    weaponClass2.Visible = false;
                    weaponClass3.Enabled = false;
                    weaponClass3.Visible = false;
                    weaponClass3.WeaponAmmoInWeapon = currWeaponAmmoInWeapon;
                    weaponClass3.WeaponMaxAmmo = currWeaponAmmoTotal;
                    weaponClass3.WeaponMagSize = currWeaponMagSize;
                    currWeaponAmmoInWeapon = weaponClass1.WeaponAmmoInWeapon;
                    currWeaponAmmoTotal = weaponClass1.WeaponMaxAmmo;
                    currWeaponMagSize = weaponClass1.WeaponMagSize;
                    currWeaponROF = weaponClass1.WeaponROF;
                    currWeaponReloadTime = weaponClass1.WeaponReloadTime;
                }
                else if (weaponSwitchTo == weaponClass2.WeaponType)
                {
                    weaponClass1.Enabled = false;
                    weaponClass1.Visible = false;
                    weaponClass2.Enabled = true;
                    weaponClass2.Visible = true;
                    weaponClass3.Enabled = false;
                    weaponClass3.Visible = false;
                    weaponClass3.WeaponAmmoInWeapon = currWeaponAmmoInWeapon;
                    weaponClass3.WeaponMaxAmmo = currWeaponAmmoTotal;
                    weaponClass3.WeaponMagSize = currWeaponMagSize;
                    currWeaponAmmoInWeapon = weaponClass2.WeaponAmmoInWeapon;
                    currWeaponAmmoTotal = weaponClass2.WeaponMaxAmmo;
                    currWeaponMagSize = weaponClass2.WeaponMagSize;
                    currWeaponROF = weaponClass2.WeaponROF;
                    currWeaponReloadTime = weaponClass2.WeaponReloadTime;
                }
            }
            currWeapon = weaponSwitchTo;

            #endregion

            if (keyPress.IsKeyDown(Keys.R) && currWeaponAmmoTotal > 0) //player can manually trigger reloading of weapon, if weapon is switched during reloading, will reset reload timer
            {
                reloadWeapon = true;
                if (currWeapon == WEAPONTYPE.AK47)
                {
                    currWeaponAmmoInWeapon = 0;
                    weaponClass1.WeaponAmmoInWeapon = 0;
                }
            }

            //if left mouse button is held down, means player is shooting
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                //this is used as interrupt for how fast bullet cam fire
                timeLastBullet += gameTime.ElapsedGameTime.Milliseconds;

                if (currWeapon == WEAPONTYPE.AK47)
                {
                    if (timeLastBullet > 100 && currWeaponAmmoInWeapon > 0) //&& weaponClass1.WeaponAmmoInWeapon > 0
                    {
                        Random random = new Random();
                        double offSetAmount = 0.25; //the amount to offset the mouse position
                        
                        //calculating the offset for X and Z
                        float offsetX = (float)(random.NextDouble() * (((mousePosition.X * offSetAmount)) - ((mousePosition.X * offSetAmount)) + ((mousePosition.X * offSetAmount))));
                        float offsetZ = (float)(random.NextDouble() * (((mousePosition.Z * offSetAmount)) - ((mousePosition.Z * offSetAmount)) + ((mousePosition.Z * offSetAmount))));
                        offsetX += mousePosition.X; //applying offset to X and Z
                        offsetZ += mousePosition.Z;

                        float rotAngleOffset = (float)Math.Atan2(-offsetX, -offsetZ); //calculating the bullet angle based off of the offset
                        Vector3 bulletIncrement = new Vector3((offsetX * 10) - offsetX, 0, (offsetZ * 10) - offsetZ); //amount to increment bullet per update by

                        bulletIncrement.Normalize();

                        //adding bullet to bullet list, bullet appears based on current position and etc
                        bulletList.Add(new BulletClass(weaponClass1.WeaponPosition, mousePosition, bulletIncrement * 0.5f, rotAngleOffset, weaponClass2.WeaponType)); //adding new bullet to bulletList
                        currWeaponAmmoInWeapon = weaponClass1.WeaponAmmoInWeapon--; //subtracting ammo in weapon per bullet fired
                        timeLastBullet = 0;
                        sfxAk47Shot.Play(); //play sound effect for ak47
                    }
                    else if (currWeaponAmmoInWeapon == 0 && currWeaponAmmoTotal > 0) //if ammo is not greater than 0, means gun has run out of bullet
                    {
                        reloadWeapon = true;
                    }
                    else if (currWeaponAmmoInWeapon == 0 && currWeaponAmmoTotal == 0 && !isDryFiring) 
                    {
                        MediaPlayer.Play(sfxWeaponDryFire);
                        isDryFiring = true;
                    }
                }
                if (currWeapon == WEAPONTYPE.SHOTGUN) // && weaponClass2.WeaponAmmoInWeapon > 0
                {
                    Random random = new Random();
                    if (timeLastBullet > 1000 && ROFInterrupt <= 0 && currWeaponAmmoInWeapon > 0)
                    {
                        double offSetAmountX = 0.5; //the amount to offset the mouse position
                        double offSetAmountZ = 0.5;

                        //these 4 if statements will adjust the amount of x and z offsets for the shotgun spread, this is needed as the offset
                        //is randomly generated based on a set amount from the mouse position, and will change based on the current mouse x and y position
                        if (mousePosition.X > -1.5 && mousePosition.X < 1.5)
                            offSetAmountX = 3;
                        if (mousePosition.Z > -1.5 && mousePosition.Z < 1.5)
                            offSetAmountZ = 3;
                        if (mousePosition.X > -0.5 && mousePosition.X < 0.5)
                            offSetAmountX = 10;
                        if (mousePosition.Z > -0.5 && mousePosition.Z < 0.5)
                            offSetAmountZ = 10;

                        for (int i = 0; i < 7; i++) //this for loop will add 7 bullets to the bulletList every time the shotgun is fired
                        {
                            //calculating the offset for X and Z
                            float offsetX = (float)(random.NextDouble() * (((mousePosition.X * offSetAmountX)) - ((mousePosition.X * offSetAmountX)) + ((mousePosition.X * offSetAmountX))));
                            float offsetZ = (float)(random.NextDouble() * (((mousePosition.Z * offSetAmountZ)) - ((mousePosition.Z * offSetAmountZ)) + ((mousePosition.Z * offSetAmountZ))));
                            offsetX += mousePosition.X; //applying offset to X and Z
                            offsetZ += mousePosition.Z;

                            float rotAngleOffset = (float)Math.Atan2(-offsetX, -offsetZ); //calculating the bullet angle based off of the offset
                            Vector3 bulletIncrement = new Vector3((offsetX * 10) - offsetX, 0, (offsetZ * 10) - offsetZ); //amount to increment bullet per update by

                            bulletIncrement.Normalize();

                            bulletList.Add(new BulletClass(weaponClass2.WeaponPosition, mousePosition, bulletIncrement * 0.3f, rotAngleOffset, weaponClass2.WeaponType)); //adding new bullet to bulletList
                        }
                        currWeaponAmmoInWeapon = weaponClass2.WeaponAmmoInWeapon--; //subtracting ammo in weapon per bullet fired
                        timeLastBullet = 0; //if fired, set to 0
                        ROFInterrupt = 1000; //if fired, set the weapon reload time, which is 1 second
                        sfxMoss500Shot.Play();
                    }
                    else if (currWeaponAmmoInWeapon == 0 && currWeaponAmmoTotal > 0) //if ammo is not greater than 0, means gun has run out of bullet
                    {
                        reloadWeapon = true;
                    }
                    else if (reloadWeapon == true)
                    {
                        ROFInterrupt -= gameTime.ElapsedGameTime.Milliseconds;
                    }
                    else if(currWeaponAmmoInWeapon == 0 && currWeaponAmmoTotal == 0 && !isDryFiring)
                    {
                        MediaPlayer.Play(sfxWeaponDryFire);
                        isDryFiring = true;
                    }
                }
                if (currWeapon == WEAPONTYPE.GRENADE)
                {
                    if (currWeaponAmmoInWeapon != 0 && timeLastBullet >= 5000 && grenadeInterrupt <= 0)
                    {
                        //getting direction vector between mouse and player position
                        grenadeIncrement = new Vector3(mousePosition.X * 10 - modelPosition.X, 0f, mousePosition.Z * 10 - modelPosition.Z);
                        grenadeIncrement.Normalize(); //normalizing for increment vector
                        grenadeIncrement *= 0.3f; //grenade move speed
                        grenadeInitialPosition = weaponClass3.WeaponPosition;

                        bulletList.Add(new BulletClass(weaponClass3.WeaponPosition, mousePosition, grenadeIncrement, rotAngle, weaponClass3.WeaponType));

                        currWeaponAmmoInWeapon = weaponClass3.WeaponAmmoInWeapon--;
                        timeLastBullet = 0; //if fired, set to 0
                        grenadeInterrupt = 5000; //if fired, set the weapon reload time, which is 5 seconds

                        weaponClass3.Visible = false;
                    }
                    else if (currWeaponAmmoInWeapon == 0 && currWeaponAmmoTotal > 0) //if ammo is not greater than 0, means gun has run out of bullet
                    {
                        reloadWeapon = true;
                    }
                    else
                    {
                        grenadeInterrupt -= gameTime.ElapsedGameTime.Milliseconds;
                    }
                }
            }
            else
            {
                if (currWeapon == WEAPONTYPE.AK47) //interrupt for ak bullet fire rate
                    timeLastBullet = 100;
                if (currWeapon == WEAPONTYPE.SHOTGUN) //interrupt for shotgun fire rate
                {
                    timeLastBullet = 1000;
                    if (ROFInterrupt <= 0)
                    {
                        timeLastBullet = 1000;
                    }
                    else
                    {
                        ROFInterrupt -= gameTime.ElapsedGameTime.Milliseconds; //subtracting from time until shotgun can be fired again
                    }
                }
                if (currWeapon == WEAPONTYPE.GRENADE)
                {
                    timeLastBullet = 5000;
                    if (grenadeInterrupt <= 0)
                    {
                        timeLastBullet = 5000;
                    }
                    else
                    {
                        grenadeInterrupt -= gameTime.ElapsedGameTime.Milliseconds; //subtracting from time until shotgun can be fired again
                    }
                }
                isDryFiring = false;
            }

            if (reloadWeapon) //if bool for weapon reload is true
            {
                if (currWeaponReloadTime <= 0) //if current reload time is less than or equal to zero, this is how long it takes for weapon to reload
                {
                    if (currWeaponAmmoTotal > 0) //of there are ammo left for the weapon
                    {
                        if (currWeapon == WEAPONTYPE.AK47) //if weapon is ak-47
                        {
                            currWeaponAmmoInWeapon = currWeaponMagSize;
                            weaponClass1.WeaponAmmoInWeapon = currWeaponAmmoInWeapon;
                            currWeaponAmmoTotal -= currWeaponAmmoInWeapon;
                            weaponClass1.WeaponMaxAmmo = currWeaponAmmoTotal;
                            reloadWeapon = false;
                            sfxAk47Charge.Play();
                        }
                        if (currWeapon == WEAPONTYPE.SHOTGUN && currWeaponAmmoInWeapon < 7 && mouseState.LeftButton != ButtonState.Pressed) //if weapon is shotgun
                        {
                            currWeaponAmmoInWeapon++;
                            weaponClass2.WeaponAmmoInWeapon++;
                            currWeaponAmmoTotal--;
                            weaponClass2.WeaponMaxAmmo--;
                            currWeaponReloadTime = weaponClass2.WeaponReloadTime;
                            sfxMoss500pump.Play();
                        }
                        else if (currWeaponAmmoInWeapon == 7)
                        {
                            reloadWeapon = false;
                        }
                        if (currWeapon == WEAPONTYPE.GRENADE) //if weapon is grenade
                        {
                            currWeaponAmmoInWeapon++;
                            weaponClass3.WeaponAmmoInWeapon++;
                            currWeaponAmmoTotal--;
                            weaponClass3.WeaponMaxAmmo--;
                            currWeaponReloadTime = weaponClass3.WeaponReloadTime;

                            reloadWeapon = false;
                            weaponClass3.Visible = true;
                        }
                    }
                }
                else
                {
                    currWeaponReloadTime -= gameTime.ElapsedGameTime.Milliseconds; //decrementing the reload time, guns will be reloaded when relaod time reaches zero
                }
            }

            //if there are no ammo in the current weapon, and there is no ammo left to reload
            if (currWeaponAmmoInWeapon == 0 && currWeaponAmmoTotal == 0)
            {
                outOfAmmo = true;
            }
            else
            {
                outOfAmmo = false;
            }

            //if player health is less than 0, player is dead, set player hp to 0 for visuals
            if (playerHealth <= 0)
            {
                playerIsDead = true;
                playerHealth = 0;
            }

            base.Update(gameTime);
        }

        //getters and setters
        public List<BulletClass> BulletList
        {
            get { return bulletList; }
            set { bulletList = value; }
        }

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
            get { return modelPosition; }
            set { modelPosition = value; }
        }

        public Vector3 MousePosition3D
        {
            set { mousePosition = value; }
        }
        //for testing
        public float GameTimeSinceLastUpdate
        {
            get { return delta; }
        }

        public bool IsMoving
        {
            set { isMoving = value; }
        }

        public BoundingBox PlayerBulletColBox
        {
            get { return bulletCollisionBox; }
            set { bulletCollisionBox = value; }
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

        public WEAPONTYPE WeaponType
        {
            get { return currWeapon; }
        }

        public int WeaponAmmoInWeapon
        {
            get { return currWeaponAmmoInWeapon; }
            set { currWeaponAmmoInWeapon = value; }
        }

        public int WeaponMagSize
        {
            get { return currWeaponMagSize; }
            set { currWeaponMagSize = value; }
        }

        public int WeaponMaxAmmo
        {
            get { return currWeaponAmmoTotal; }
            set { currWeaponAmmoTotal = value; }
        }

        public int PlayerHealth
        {
            get { return playerHealth; }
            set { playerHealth = value; }
        }
        public bool IsReloading
        {
            get { return reloadWeapon; }
        }
        public bool OutOfAmmo
        {
            get { return outOfAmmo; }
        }
        public bool IsPlayerDead
        {
            get { return playerIsDead; }
        }

        //for drawing bounding box visualizations
        public void drawBoundingBox(BoundingBox boundingBox)
        {
            //draw collison box for debugging
            //from here: https://electronicmeteor.wordpress.com/2011/10/25/bounding-boxes-for-your-model-meshes/
            // Initialize an array of indices for the box. 12 lines require 24 indices
            short[] bBoxIndices =
                {
                    0, 1, 1, 2, 2, 3, 3, 0, // Front edges
                    4, 5, 5, 6, 6, 7, 7, 4, // Back edges
                    0, 4, 1, 5, 2, 6, 3, 7 // Side edges connecting front and back
                };

            Vector3[] corners = boundingBox.GetCorners();
            VertexPositionColor[] primitiveList = new VertexPositionColor[corners.Length];

            // Assign the 8 box vertices
            for (int i = 0; i < corners.Length; i++)
            {
                primitiveList[i] = new VertexPositionColor(corners[i], Color.White);
            }

            /* Set your own effect parameters here */

            boxEffect.World = Matrix.Identity;
            //boxEffect.World *= Matrix.CreateFromAxisAngle(boxEffect.World.Up, rotAngle);
            //boxEffect.World *= Matrix.CreateTranslation(modelPosition);
            boxEffect.View = cameraViewMatrix;
            boxEffect.Projection = _Initializations.GetProjectionMatrix();
            boxEffect.TextureEnabled = false;

            // Draw the box with a LineList
            foreach (EffectPass pass in boxEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.LineList, primitiveList, 0, 8, bBoxIndices, 0, 12);
            }
        }
    }
}
