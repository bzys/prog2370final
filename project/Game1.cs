/*
 *  Author:     Bill Sun     
 *  Project:    Seige!
 *  Comments:   default game1.cs for monogame
 *  Revision History: 
 *      2018-12-11: latest
 *      2018-10-27: created
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BSunFinalProject
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //game assets
        //Player player;
        Player player;
        PlayerUI playerUI;
        Vector3 startingPosition = new Vector3(0, 0, 0);
        MapHouse mapHouseBase;
        GameController gameController;
        BulletController bulletController;
        StartMenu gameMenu;
        DrawDebug drawDebug;
        NpcController npcController;

        private const int PAUSEBEFORESTARTING = 200; //wait 0.2 seconds before starting/restarting game
        private int pauseDelayIncrement = 0; //increment counter for the pause

        private bool playerIsDead = false; //is player dead
        private bool pauseGame = false; //is game paused

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = _Initializations.preferredBackBufferHeight;
            graphics.PreferredBackBufferWidth = _Initializations.preferredBackBufferWidth;
            graphics.ApplyChanges();
            
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            //IsMouseVisible = true; //showing mouse cursor

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //instantiate player 
            player = new Player(this, graphics, startingPosition, spriteBatch, WEAPONTYPE.AK47, WEAPONTYPE.SHOTGUN);
            npcController = new NpcController(this);

            //instantiate map 
            mapHouseBase = new MapHouse(this);

            //instantiate game controller 
            gameController = new GameController(this);
            gameController.PlayerPosition = player.PlayerPosition; //setting initial player position
            gameController.CameraMatrix = _Initializations.GetViewMatrix(); //setting initial camera matrix
;
            //bullet controller
            bulletController = new BulletController(this, graphics, spriteBatch);

            //game menu
            gameMenu = new StartMenu(this, spriteBatch);

            //debug text
            SpriteFont spriteFont = Content.Load<SpriteFont>("defaultFont");
            drawDebug = new DrawDebug(this, spriteBatch, spriteFont); //drawing debug

            gameController.BasementWalls = mapHouseBase.BasementWalls; //passing the list of walls from basement map to game controller
            bulletController.WallList = mapHouseBase.BasementWalls; //passing list of walls to bullet controller for wall and bullet collision detection
            bulletController.PlayerHealth = player.PlayerHealth; //passing player health to bullet controller

            playerUI = new PlayerUI(this, spriteBatch);

            //adding components
            Components.Add(gameController);
            Components.Add(mapHouseBase);
            
            Components.Add(player);
            Components.Add(npcController);
            Components.Add(bulletController);

            Components.Add(playerUI);
            Components.Add(gameMenu);

            Components.Add(drawDebug); //get rid of this

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            MouseState ms = Mouse.GetState();

            //note to self: wow this is horrible, get rid of it
            player.PlayerPosition = gameController.PlayerPosition;
            player.CameraMatrix = gameController.CameraMatrix; //updating player camera
            player.CameraPosition = gameController.CameraPosition;
            npcController.CameraMatrix = gameController.CameraMatrix;
            npcController.CameraPosition = gameController.CameraPosition;
            npcController.PlayerPosition = player.PlayerPosition;
            mapHouseBase.SetCameraMatrix = gameController.CameraMatrix; //updating map camera
            gameController.WorldMatrix = mapHouseBase.WorldMatrix;

            player.MousePosition3D = gameController.MousePosition3D;

            player.IsMoving = gameController.IsMoving;
            bulletController.PlayerBoundingBox = player.PlayerBulletColBox;
            player.PlayerHealth = bulletController.PlayerHealth;

            gameController.PlayerBulletColBox = player.PlayerBulletColBox;
            gameController.PlayerBoundingBoxXNeg = player.PlayerBoundingBoxXNeg;
            gameController.PlayerBoundingBoxXPos = player.PlayerBoundingBoxXPos;
            gameController.PlayerBoundingBoxZNeg = player.PlayerBoundingBoxZNeg;
            gameController.PlayerBoundingBoxZPos = player.PlayerBoundingBoxZPos;
            gameController.IsPlayerDead = player.IsPlayerDead;

            //setting player model color
            player.DiffuseVector = gameController.PlayerDiffuse;
            player.LightingDirection = gameController.PlayerLightDirection;
            player.SpecularVector = gameController.PlayerSpecular;

            bulletController.BulletList = player.BulletList;
            bulletController.SetCameraMatrix = gameController.CameraMatrix;

            npcController.BulletList = bulletController.BulletList;
            playerUI.TotalEnemies = npcController.TotalEnemies;
            playerUI.EnemiesLeft = npcController.EnemiesLeft;

            mapHouseBase.BasementWalls = bulletController.WallList;

            //commented out because npc shooting doesn't work yet
            //npcController.WallList = mapHouseBase.BasementWalls;
            //mapHouseBase.BasementWalls = npcController.WallList;

            //passing stuff from player to player ui to be drawn
            playerUI.CurrWeapon = player.WeaponType;
            playerUI.CurrWeaponAmmoInWeapon = player.WeaponAmmoInWeapon;
            playerUI.CurrWeaponMagSize = player.WeaponMagSize;
            playerUI.CurrWeaponAmmoTotal = player.WeaponMaxAmmo;
            playerUI.CurrPlayerHealth = player.PlayerHealth;
            playerUI.IsReloading = player.IsReloading;
            playerUI.OutOfAmmo = player.OutOfAmmo;

            gameMenu.IsPlayerDead = player.IsPlayerDead; //if player is dead do stuff
            gameMenu.IsPlayerVictory = npcController.PlayerVictory; //if player is won do stuff

            //if pause game is true
            if (pauseGame == true)
            {
                //disabling them all while menu is in effect
                gameController.Enabled = false;
                mapHouseBase.Enabled = false;
                player.Enabled = false;
                bulletController.Enabled = false;
                playerUI.Enabled = false;
                pauseDelayIncrement = 0; //resetting amount of delay time
            }
            else //means game is not paused
            {
                if (pauseDelayIncrement > PAUSEBEFORESTARTING) //wait a tiny bit before restarting the game
                {
                    gameController.Enabled = true;
                    mapHouseBase.Enabled = true;
                    player.Enabled = true;
                    bulletController.Enabled = true;
                    playerUI.Enabled = true;
                }
                else
                {
                    pauseDelayIncrement += gameTime.ElapsedGameTime.Milliseconds;
                }
            }

            if (gameMenu.QuitGame)
            {
                Exit();
            }

            pauseGame = gameMenu.PauseGame;

            /**--------------------------------------------------------------------------------------------------------**/
            /*
            //this is all commented out because its debug stuff
            string wallStr = "";

            foreach (Walls wall in gameController.BasementWalls)
            {
                wallStr += wall.BoundingBox.ToString() + "\n";
            }

            string bulletStr = "";

            foreach (BulletClass bullet in player.BulletList)
            {
                bulletStr += bullet.Position.ToString() + " | " + bullet.Increment.ToString() + "\n";
            }

            
            //debug text
            drawDebug.DebugText = "Gametime seconds: " + gameTime.ElapsedGameTime.Seconds +
                                    "\n\nX: " + gameController.MousePosition3D.X.ToString() +
                                    "\nY: " + gameController.MousePosition3D.Y.ToString() +
                                    "\nZ: " + gameController.MousePosition3D.Z.ToString() +
                                    "\n\nPlayer X Pos: " + gameController.PlayerPosition.X.ToString() +
                                    "\nPlayer Y Pos: " + gameController.PlayerPosition.Y.ToString() +
                                    "\nPlayer Z Pos: " + gameController.PlayerPosition.Z.ToString() +
                                    "\n\nScroll Curr: " + ms.ScrollWheelValue +
                                    "\n\nGameTime since last update: " + player.GameTimeSinceLastUpdate.ToString() +
                                    "\n\nIs Leg Moving: " + gameController.IsMoving.ToString() +
                                    "\n\nIs Player Colliding: " + gameController.IsColliding.ToString() +
                                    "\n\nPlayer Bounding Box: " + player.PlayerBulletColBox.ToString() +
                                    "\nPlayer AABB XNeg: " + gameController.IsCollidingXNeg.ToString() +
                                    "\nPlayer AABB XPos: " + gameController.IsCollidingXPos.ToString() +
                                    "\nPlayer AABB ZNeg: " + gameController.IsCollidingZNeg.ToString() +
                                    "\nPlayer AABB ZPos: " + gameController.IsCollidingZPos.ToString() +
                                    "\nGame Controller Player bullet collision box: " + gameController.PlayerBulletColBox.ToString() +
                                    "\n\nNumber of walls: " + gameController.BasementWalls.Count.ToString() +
                                    "\n\nBullet Position: \n" + bulletStr;
            */
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            //DrawGround();
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
