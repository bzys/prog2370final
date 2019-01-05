/*
 *  Author:     Bill Sun        
 *  Comments:   the npc controller class, this class handles how many npcs there are, the npc positions, rotation, as well as the collision between npc and 
 *              player fired bullets. this is not complete, as npc shooting is not added
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
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace BSunFinalProject
{
    class NpcController : DrawableGameComponent
    {
        private int enemiesLeft;
        private int totalEnemies;
        private bool npcEnabled = true;
        private bool playerVictory = false;
        private Matrix cameraViewMatrix;
        private Vector3 cameraPosition; //position of camera
        EnemyNpc npc1;
        EnemyNpc npc2;
        EnemyNpc npc3;
        Vector3 npc1StartPosition = new Vector3(20f, 0f,-10f);
        Vector3 npc2StartPosition = new Vector3(17f, 0f, -26f);
        Vector3 npc3StartPosition = new Vector3(8.7f, 0f, -22f);

        Vector3 playerPosition = new Vector3(0f, 0f, 0f);

        //the colors for the npcs
        Vector3 npcDiffuse = new Vector3(0.1f, 0.04f, 0.04f) * 1.2f; //rgb(200, 75, 75)
        Vector3 npcLightDirection = new Vector3(-9f, -18f, 9f);
        Vector3 npcSpecular = new Vector3(1f, 0f, 0f); //highlights, also in rgb values

        //npc bullets
        private List<BulletClass> bulletList;
        private List<Walls> wallList;
        private Model gunBullet;
        private Song sfxBulletImpactConcrete;
        private Song sfxBulletImpactWood;

        public NpcController(Game game) : base(game)
        {
            npc1 = new EnemyNpc(game, npc1StartPosition, WEAPONTYPE.AK47);
            npc2 = new EnemyNpc(game, npc2StartPosition, WEAPONTYPE.SHOTGUN);
            npc3 = new EnemyNpc(game, npc3StartPosition, WEAPONTYPE.AK47);
            totalEnemies = 3; //3 npcs are created, therefore 3 total enemies

            game.Components.Add(npc1);
            game.Components.Add(npc2);
            game.Components.Add(npc3);

            //the vectors for lighting and light effects
            npc1.DiffuseVector = npcDiffuse;
            npc1.LightingDirection = npcLightDirection;
            npc1.SpecularVector = npcSpecular;
            npc2.DiffuseVector = npcDiffuse;
            npc2.LightingDirection = npcLightDirection;
            npc2.SpecularVector = npcSpecular;
            npc3.DiffuseVector = npcDiffuse;
            npc3.LightingDirection = npcLightDirection;
            npc3.SpecularVector = npcSpecular;

            wallList = new List<Walls>();
            bulletList = new List<BulletClass>();

            //content and effect loading
            gunBullet = game.Content.Load<Model>("gunBulletXL");
            sfxBulletImpactConcrete = game.Content.Load<Song>("SFX/sfxBulletConcreteImpact");
            sfxBulletImpactWood = game.Content.Load<Song>("SFX/sfxBulletWoodImpact");
        }

        public override void Draw(GameTime gameTime)
        {
            /*
            GraphicsDevice.BlendState = BlendState.AlphaBlend;

            if (bulletList.Count > 0)
            {
                foreach (BulletClass bullet in bulletList)
                {
                    //drawBoundingBox(bullet.BoundingBox);
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
            }
            */
            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            if (npcEnabled)
            {
                npc1.Enabled = true;
                npc2.Enabled = true;
                npc3.Enabled = true;
                npc1.Visible = true;
                npc2.Visible = true;
                npc3.Visible = true;
            }
            else if (!npcEnabled)
            {
                npc1.Enabled = false;
                npc2.Enabled = false;
                npc3.Enabled = false;
                npc1.Visible = false;
                npc2.Visible = false;
                npc3.Visible = false;
            }

            //updating the view matrix and camera position vector for the npcs
            npc1.CameraMatrix = cameraViewMatrix;
            npc1.CameraPosition = cameraPosition;
            npc2.CameraMatrix = cameraViewMatrix;
            npc2.CameraPosition = cameraPosition;
            npc3.CameraMatrix = cameraViewMatrix;
            npc3.CameraPosition = cameraPosition;

            //updating each npc to always face the player
            npc1.PlayerPosition = playerPosition;
            npc2.PlayerPosition = playerPosition;
            npc3.PlayerPosition = playerPosition;

            //if there are bullets in the list, check to see if they collide with any of the npcs
            if (bulletList.Count > 0)
            {
                foreach (BulletClass bullet in bulletList)
                {
                    if (npc1.BulletCollisionBox.Intersects(bullet.BoundingBox))
                    {
                        npc1.NpcHealth -= bullet.BulletDmg;
                    }
                    if (npc2.BulletCollisionBox.Intersects(bullet.BoundingBox))
                    {
                        npc2.NpcHealth -= bullet.BulletDmg;
                    }
                    if (npc3.BulletCollisionBox.Intersects(bullet.BoundingBox))
                    {
                        npc3.NpcHealth -= bullet.BulletDmg;
                    }
                }
            }

            enemiesLeft = 0;

            //incrementing how many npcs are left based on their hp, 0 or less means they're dead
            if (npc1.NpcHealth > 0)
            {
                enemiesLeft++;
            }
            if (npc2.NpcHealth > 0)
            {
                enemiesLeft++;
            }
            if (npc3.NpcHealth > 0)
            {
                enemiesLeft++;
            }

            if (enemiesLeft == 0)
            {
                playerVictory = true;
            }
            /*
            //concat lists
            bulletList = npc1.BulletList.Concat(npc2.BulletList).Concat(npc3.BulletList).ToList();

            foreach (BulletClass bullet in bulletList) //removing the bullet from the bulletlist once it travels far enough that player can't see it
            {
                bool breakOut = false; //is the bullet to be removed from the list?
                
                foreach (Walls wall in wallList) //check for each existing wall
                {
                    if (bullet.BoundingBox.Intersects(wall.BoundingBox)) //if intersects
                    {
                        if (!wall.IsDestroyable) //if the wall isn't destoryable, remove bullet
                        {
                            MediaPlayer.Play(sfxBulletImpactConcrete);
                            bulletList.Remove(bullet); //remove this bullet
                            breakOut = true; //set break out of foreach loop
                            break; //break out of this first loop
                        }
                        else if (wall.IsDestroyable && wall.WallHealth > 0)
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
            */

            base.Update(gameTime);
        }

        //setters and getters
        public bool NpcEnabled
        {
            set { npcEnabled = value; }
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
            set { playerPosition = value; }
        }
        
        public List<Walls> WallList
        {
            get { return wallList; }
            set { wallList = value; }
        }
        public List<BulletClass> BulletList
        {
            get { return bulletList; }
            set { bulletList = value; }
        }
        public int TotalEnemies
        {
            get { return totalEnemies; }
        }
        public int EnemiesLeft
        {
            get { return enemiesLeft; }
        }
        public bool PlayerVictory
        {
            get { return playerVictory; }
        }
    }
}
