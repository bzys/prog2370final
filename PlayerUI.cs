/*
 *  Author:     Bill Sun        
 *  Comments:   the player ui, displays bullets, hp, objective and whatnot
 *  Revision History: 
 *      2018-12-9: latest
 *      2018-11-11: created
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
    class PlayerUI : DrawableGameComponent
    {
        private WEAPONTYPE currWeapon;
        private int currWeaponAmmoInWeapon;
        private int currWeaponMagSize;
        private int currWeaponAmmoTotal;
        private int currPlayerHealth;
        private bool flashHealth;

        private SpriteBatch spriteBatch;
        private SpriteFont fontUI12;
        private SpriteFont fontUI24;

        private string ammoText;
        private string ammoTextShadow;
        private string weaponName;
        private string playerHealthString;

        Vector2 ammoTextPosition;
        Vector2 ammoTextShadowPosition;
        int ammoUIOffsetX = _Initializations.preferredBackBufferWidth - (_Initializations.preferredBackBufferWidth / 6);
        int ammoUIOffsetY = _Initializations.preferredBackBufferHeight - (_Initializations.preferredBackBufferHeight / 6);
        Color colorUI;
        Color colorUIShadow;
        Color colorPlayerHP;
        Texture2D ammoRectangle;
        Texture2D hpRectangle;
        Texture2D gunHLRectangle;
        Texture2D aimReticle;
        private bool isReloading = false;
        private bool outOfAmmo = false;
        private int reloadFlashTimer = 0;

        private int enemiesLeft = 0;
        private int totalEnemies = 0;

        private int elapsedTime = 0;

        public PlayerUI(Game game, SpriteBatch spriteBatch) : base(game)
        {
            this.spriteBatch = spriteBatch;
            fontUI12 = game.Content.Load<SpriteFont>("UserInterface/fontUI12");
            fontUI24 = game.Content.Load<SpriteFont>("UserInterface/fontUI24");
            ammoRectangle = game.Content.Load<Texture2D>("UserInterface/ammoRectangle");
            hpRectangle = game.Content.Load<Texture2D>("UserInterface/healthRectangle");
            gunHLRectangle = game.Content.Load<Texture2D>("UserInterface/gunHighlightRectangle");
            aimReticle = game.Content.Load<Texture2D>("UserInterface/crosshair");

            currPlayerHealth = 0;
            flashHealth = false;

            ammoText = "";
            ammoTextShadow = "";
            ammoTextPosition = new Vector2(ammoUIOffsetX, ammoUIOffsetY);
            ammoTextShadowPosition = new Vector2(ammoUIOffsetX + 4, ammoUIOffsetY + 3);
            colorUI = Color.White;
            colorPlayerHP = Color.White;
            colorUIShadow = new Color(25, 25, 25);
        }

        public override void Draw(GameTime gameTime)
        {
            MouseState mouseState = Mouse.GetState();

            spriteBatch.Begin();

            spriteBatch.Draw(aimReticle, new Rectangle(mouseState.X - 32, mouseState.Y - 32, 64, 64), Color.White);

            spriteBatch.Draw(ammoRectangle, new Vector2(1570, 867), Color.White);
            spriteBatch.Draw(ammoRectangle, new Vector2(-10, 867), Color.White);

            spriteBatch.DrawString(fontUI24, ammoTextShadow, ammoTextShadowPosition, colorUIShadow);
            spriteBatch.DrawString(fontUI24, ammoText, ammoTextPosition, colorUI);
            spriteBatch.DrawString(fontUI24, "Health", new Vector2(202, 903), colorUIShadow);
            spriteBatch.DrawString(fontUI24, "Health", new Vector2(198, 900), colorUI);

            if (flashHealth) //if player hp is low, flash the health to red
            {
                elapsedTime += gameTime.ElapsedGameTime.Milliseconds; //incrementing the flashing interrupt timer
                if (elapsedTime > 500 && elapsedTime < 1000)
                {
                    spriteBatch.DrawString(fontUI24, playerHealthString, new Vector2(150, 936), colorUIShadow);
                    spriteBatch.DrawString(fontUI24, playerHealthString, new Vector2(146, 933), colorPlayerHP);
                }
                else if (elapsedTime > 1000)
                {
                    elapsedTime = 0;
                }
            }
            else
            {
                spriteBatch.DrawString(fontUI24, playerHealthString, new Vector2(150, 936), colorUIShadow);
                spriteBatch.DrawString(fontUI24, playerHealthString, new Vector2(146, 933), colorPlayerHP);
            }

            if (isReloading && outOfAmmo == false) //if is reloading, display flashing reload 
            {
                string reloadString = "RELOADING";
                reloadFlashTimer += gameTime.ElapsedGameTime.Milliseconds; //incrementing the flashing interrupt timer
                if (reloadFlashTimer > 500 && reloadFlashTimer < 1000)
                {
                    spriteBatch.DrawString(fontUI12, reloadString, new Vector2(mouseState.X - 55, mouseState.Y - 62), colorUIShadow);
                    spriteBatch.DrawString(fontUI12, reloadString, new Vector2(mouseState.X - 57, mouseState.Y - 64), Color.White);
                }
                else if (reloadFlashTimer > 1000)
                {
                    reloadFlashTimer = 0;
                }
            }
            else if (outOfAmmo) //if weapon is out of ammo, display out of ammo
            {
                string outOfAmmo = "NO AMMO";
                reloadFlashTimer += gameTime.ElapsedGameTime.Milliseconds; //incrementing the flashing interrupt timer
                if (reloadFlashTimer > 500 && reloadFlashTimer < 1000)
                {
                    spriteBatch.DrawString(fontUI12, outOfAmmo, new Vector2(mouseState.X - 42, mouseState.Y - 62), colorUIShadow);
                    spriteBatch.DrawString(fontUI12, outOfAmmo, new Vector2(mouseState.X - 44, mouseState.Y - 64), Color.White);
                }
                else if (reloadFlashTimer > 1000)
                {
                    reloadFlashTimer = 0;
                }
            }

            spriteBatch.DrawString(fontUI24, " / 100", new Vector2(218, 936), colorUIShadow);
            spriteBatch.DrawString(fontUI24, " / 100", new Vector2(214, 933), colorUI);

            //drawing game objective
            string objectiveStr = "ENEMIES LEFT: " + enemiesLeft.ToString() + " / " + totalEnemies.ToString();
            spriteBatch.DrawString(fontUI24, "OBJECTIVE: ELEMINATE THE ENEMY", new Vector2(22, 22), colorUIShadow);
            spriteBatch.DrawString(fontUI24, "OBJECTIVE: ELEMINATE THE ENEMY", new Vector2(20, 20), Color.LightYellow);
            spriteBatch.DrawString(fontUI24, objectiveStr, new Vector2(22, 62), colorUIShadow);
            spriteBatch.DrawString(fontUI24, objectiveStr, new Vector2(20, 60), Color.LightYellow);

            spriteBatch.End();

            //resetting the graphics device so 3d can draw properly
            //https://social.msdn.microsoft.com/Forums/en-US/0e380eeb-8fb0-4b1d-ae68-1d8b56e9132a/spritebatch-model-transparent-model?forum=xnaframework
            //https://blogs.msdn.microsoft.com/shawnhar/2010/06/18/spritebatch-and-renderstates-in-xna-game-studio-4-0/
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            if (currWeapon == WEAPONTYPE.AK47)
            {
                weaponName = "AK-47";
            }
            if (currWeapon == WEAPONTYPE.GRENADE)
            {
                weaponName = "Grenade";
            }
            if (currWeapon == WEAPONTYPE.SHOTGUN)
            {
                weaponName = "Mossberg 500";
            }

            //updating the weapon name string to draw based on current held weapon, as well as ammo amount, etc.
            ammoText = "" + weaponName +
                        "\n" + currWeaponAmmoInWeapon.ToString() + " / " + currWeaponAmmoTotal.ToString();
            ammoTextShadow = "" + weaponName +
                        "\n" + currWeaponAmmoInWeapon.ToString() + " / " + currWeaponAmmoTotal.ToString();
            playerHealthString = currPlayerHealth.ToString();

            if (currPlayerHealth < 30) //if player hp falls below 30, flash hp
            {
                flashHealth = true;
                colorPlayerHP = Color.OrangeRed;
            }

            if (currPlayerHealth <= 0) //don't let player hp fall below 0
            {
                currPlayerHealth = 0;
            }

            base.Update(gameTime);
        }

        //setters and getters
        public WEAPONTYPE CurrWeapon
        {
            set { currWeapon = value; }
        }

        public int CurrWeaponAmmoInWeapon
        {
            set { currWeaponAmmoInWeapon = value; }
        }

        public int CurrWeaponMagSize
        {
            set { currWeaponMagSize = value; }
        }

        public int CurrWeaponAmmoTotal
        {
            set { currWeaponAmmoTotal = value; }
        }

        public int CurrPlayerHealth
        {
            set { currPlayerHealth = value; }
        }
        public bool IsReloading
        {
            set { isReloading = value; }
        }
        public bool OutOfAmmo
        {
            set { outOfAmmo = value; }
        }
        public int TotalEnemies
        {
            set { totalEnemies = value; }
        }
        public int EnemiesLeft
        {
            set { enemiesLeft = value; }
        }
    }
}
