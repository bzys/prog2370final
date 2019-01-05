/*
 *  Author:     Bill Sun        
 *  Comments:   bulletcontroller class, this class draws the bullets and handles collision and damage between bullets and walls
 *  Revision History: 
 *      2018-11-30: latest
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
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace BSunFinalProject
{
    class BulletController : DrawableGameComponent
    {
        private GraphicsDeviceManager device;
        private BasicEffect effect;
        private Model gunBullet;
        private Model gunGrenade;
        private SpriteBatch spriteBatch;
        private List<BulletClass> bulletList; //the list of bullets
        private List<Walls> wallList; //the list of walls
        private Vector3 cameraPosition;
        private Vector3 mousePosition;
        private Matrix cameraViewMatrix;
        private BasicEffect boxEffect;
        private SoundEffect sfxGrenadeExplosion;
        private SoundEffect sfxGrenadeTinnitus;
        private Song sfxBulletImpactConcrete;
        private Song sfxBulletImpactWood;

        //for player damage calculations
        private BoundingBox playerBoundingBox;
        private int playerHealth;

        //for grenades
        private bool explodeGrenade = false;
        private Vector3 grenadePosition;

        //explosion damage bounding boxes for grenades
        //these four are for damage against wood walls
        Vector3 bbMinHalf = new Vector3(-1.5f, -0.9f, -1.5f);
        Vector3 bbMaxHalf = new Vector3(1.5f, 0.9f, 1.5f);
        Vector3 bbMinFull = new Vector3(-0.75f, -0.9f, -0.75f);
        Vector3 bbMaxFull = new Vector3(0.75f, 0.9f, 0.75f);
        //these four are for damage against players, grenades have increased area-of-effect against players
        Vector3 bbPMinHalf = new Vector3(-2.5f, -0.9f, -2.5f);
        Vector3 bbPMaxHalf = new Vector3(2.5f, 0.9f, 2.5f);
        Vector3 bbPMinFull = new Vector3(-1.5f, -0.9f, -1.5f);
        Vector3 bbPMaxFull = new Vector3(1.5f, 0.9f, 1.5f);

        public BulletController(Game game, GraphicsDeviceManager device, SpriteBatch spriteBatch) : base(game)
        {
            this.device = device;
            this.spriteBatch = spriteBatch;
            //this.effect = effect;

            effect = new BasicEffect(device.GraphicsDevice);

            //camera position for bullets
            cameraPosition = _Initializations.cameraPosition;
            //loading of contents and sound effects
            gunBullet = game.Content.Load<Model>("gunBulletXL");
            gunGrenade = game.Content.Load<Model>("gunGrenade");
            sfxGrenadeExplosion = game.Content.Load<SoundEffect>("SFX/sfxGrenadeExplosion");
            sfxGrenadeTinnitus = game.Content.Load<SoundEffect>("SFX/sfxTinnitus");
            sfxBulletImpactConcrete = game.Content.Load<Song>("SFX/sfxBulletConcreteImpact");
            sfxBulletImpactWood = game.Content.Load<Song>("SFX/sfxBulletWoodImpact");
            MediaPlayer.Volume = 0.5f; //testing media volume, not sure if it works or not?

            bulletList = new List<BulletClass>();

            //this is used for drawing debug bounding boxes
            boxEffect = new BasicEffect(game.GraphicsDevice);
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.BlendState = BlendState.AlphaBlend; //setting to blendstate.alphablend so png textures will have transparency

            if (bulletList.Count > 0) //if there are bullets in the list
            {
                foreach (BulletClass bullet in bulletList) //for each bullet in the list
                {
                    //drawBoundingBox(bullet.BoundingBox); //this is for drawing the bullet bounding boxes for debugging

                    if (bullet.BulletFromWhatWeapon != WEAPONTYPE.GRENADE) //if bullet is not from a grenade
                    {
                        foreach (ModelMesh mesh in gunBullet.Meshes)
                        {
                            foreach (BasicEffect effect in mesh.Effects)
                            {
                                effect.EnableDefaultLighting();
                                effect.PreferPerPixelLighting = true;

                                effect.World = _Initializations.GetWorldMatrix();
                                effect.World = Matrix.CreateTranslation(0.25f, 0f, -0.4f);
                                effect.World *= Matrix.CreateRotationY(bullet.Rotation);
                                effect.World *= Matrix.CreateTranslation(bullet.Position);

                                effect.View = cameraViewMatrix;
                                effect.Projection = _Initializations.GetProjectionMatrix();
                            }
                            mesh.Draw();
                        }
                    }
                    else if (bullet.BulletFromWhatWeapon == WEAPONTYPE.GRENADE) //else if it is from a grenade
                    {
                        foreach (ModelMesh mesh in gunGrenade.Meshes)
                        {
                            foreach (BasicEffect effect in mesh.Effects)
                            {
                                effect.EnableDefaultLighting();
                                effect.PreferPerPixelLighting = true;

                                effect.World = _Initializations.GetWorldMatrix();
                                effect.World = Matrix.CreateTranslation(0.25f, 0f, -0.4f);
                                effect.World *= Matrix.CreateRotationY(bullet.Rotation);
                                effect.World *= Matrix.CreateTranslation(bullet.Position);

                                effect.View = cameraViewMatrix;
                                effect.Projection = _Initializations.GetProjectionMatrix();
                            }
                            mesh.Draw();
                        }
                    }
                }
            }
            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            foreach (BulletClass bullet in bulletList) //removing the bullet from the bulletlist once it travels far enough that player can't see it
            {
                bool breakOut = false; //is the bullet to be removed from the list?

                foreach (Walls wall in wallList) //check for each existing wall
                {
                    if (bullet.BoundingBox.Intersects(wall.BoundingBox)) //if intersects
                    {
                        if (bullet.BulletFromWhatWeapon == WEAPONTYPE.GRENADE) //if the bullet fired is a grenade, it will expode and do damage regardless
                        {
                            explodeGrenade = true;
                            grenadePosition = bullet.Position;
                            BulletList.Remove(bullet);
                            breakOut = true;
                            break;
                        }
                        else if (!wall.IsDestroyable) //if the wall isn't destoryable, remove bullet
                        {
                            MediaPlayer.Play(sfxBulletImpactConcrete);
                            bulletList.Remove(bullet); //remove this bullet
                            breakOut = true; //set break out of foreach loop
                            break; //break out of this first loop
                        }
                        else if(wall.IsDestroyable && wall.WallHealth > 0)
                        {
                            MediaPlayer.Play(sfxBulletImpactWood);
                            wall.WallHealth -= bullet.BulletWallDmg;
                            bullet.BulletDmg = (int)(bullet.BulletDmg * 0.75f);
                            bullet.BulletWallDmg = (int)(bullet.BulletWallDmg * 0.5f);
                        }
                    }
                }

                if (breakOut) //if breakout of foreach bullet loop is true
                {
                    break; //break
                }
                else
                {
                    bullet.Position += bullet.Increment; //if not, increment the bullet
                }

                if ((bullet.Position.X > 100 || bullet.Position.Z > 100) || (bullet.Position.X < -100 || bullet.Position.Z < -100)) //if the bullet is too far outside of player view
                {
                    bulletList.Remove(bullet); //remove bullet
                    break; //break out of list so it doesn't error out
                }

                Vector3 bbMin = bullet.BoundingBox.Min + bullet.Increment; //incrementing the bullet bounding box
                Vector3 bbMax = bullet.BoundingBox.Max + bullet.Increment;
                bullet.BoundingBox = new BoundingBox(bbMin, bbMax);
            }

            //if grenade is exploding, this happens when grenade impacts the wall
            if (explodeGrenade)
            {
                sfxGrenadeExplosion.Play();
                sfxGrenadeTinnitus.Play();

                //the bounding boxes for deciding how much damage to apply
                BoundingBox grenadeRadiusHalfDMG = new BoundingBox(bbMinHalf + grenadePosition, bbMaxHalf + grenadePosition);
                BoundingBox grenadeRadiusFullDMG = new BoundingBox(bbMinFull + grenadePosition, bbMaxFull + grenadePosition);
                BoundingBox playerHalfDMG = new BoundingBox(bbPMinHalf + grenadePosition, bbPMaxHalf + grenadePosition);
                BoundingBox playerFullDMG = new BoundingBox(bbPMinFull + grenadePosition, bbPMaxFull + grenadePosition);

                foreach (Walls wall in wallList) //see if walls are destoryable
                {
                    if (wall.IsDestroyable && wall.BoundingBox.Intersects(grenadeRadiusFullDMG))
                    {
                        wall.WallHealth -= 200; //magic numbers for wall damage
                    }
                    else if (wall.IsDestroyable && wall.BoundingBox.Intersects(grenadeRadiusHalfDMG))
                    {
                        wall.WallHealth -= 70;
                    }
                }

                //for player damage, uses difference bounding boxes for max and half damage
                if (playerFullDMG.Intersects(playerBoundingBox))
                {
                    playerHealth -= 50;
                }
                else if (playerHalfDMG.Intersects(playerBoundingBox))
                {
                    playerHealth -= 25;
                }

                explodeGrenade = false;
            }

            base.Update(gameTime);
        }

        //setters and getters
        public Matrix SetCameraMatrix
        {
            get { return cameraViewMatrix; }
            set { cameraViewMatrix = value; }
        }

        public Vector3 CameraPosition
        {
            get { return cameraPosition; }
            set { cameraPosition = value; }
        }

        public Vector3 MousePosition
        {
            set { mousePosition = value; }
        }

        public List<BulletClass> BulletList
        {
            get { return bulletList; }
            set { bulletList = value; }
        }
        public List<Walls> WallList
        {
            get { return wallList; }
            set { wallList = value; }
        }
        public BoundingBox PlayerBoundingBox
        {
            set { playerBoundingBox = value; }
        }
        public int PlayerHealth
        {
            get { return playerHealth; }
            set { playerHealth = value; }
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
