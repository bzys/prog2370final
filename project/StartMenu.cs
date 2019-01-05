/*
 *  Author:     Bill Sun        
 *  Comments:   the menu, for start game, credits scene, help, game won and game over 
 *  Revision History: 
 *      2018-12-9: latest
 *      2018-12-5: created
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
    class StartMenu : DrawableGameComponent
    {
        private Texture2D splashScreen;
        private Texture2D helpMenu;
        private Texture2D deathMenu;
        private Texture2D winMenu;
        private Texture2D goBackButton;
        private Texture2D goBackButtonShadow;
        private Texture2D menuHighlight;
        private Texture2D aimReticle;
        private Rectangle aimReticleRectangle;
        private Rectangle aimReticleCollisionRectangle;
        private MouseState mouseState;
        private SpriteBatch spriteBatch;
        private SpriteFont fontUI24;
        private SpriteFont fontUI72;
        private Song startMenuMusic;
        private Song deathMenuMusic;
        private Song winMusic;
        private SoundEffect sfxSelectionSwitch;
        private SoundEffect sfxSelect;
        private bool startGame = false;
        private bool pauseGame = false;
        private bool showSplash = true;
        private bool showHelp = false;
        private bool showCredits = false;
        private bool quitGame = false;
        private bool isEscapePreviouslyPressed = false;
        private bool isPlayerDead = false;
        private bool isPlayerVictory = false;
        private bool drawReticle = true;
        private bool playDeathMusic = true;
        private bool playVictoryMusic = true;

        private const string STARTGAMESTRING = "Start";
        private const string RESUMEGAMESTRING = "Resume";
        private const string SHOWHELPSTRING = "Help";
        private const string SHOWCREDITSTRING = "Credits";
        private const string QUITGAMESTRING = "Quit";
        private const string CREDITSTRING = "Created by:\n\n        Bill Sun";
        private const string CREDITSTRINGSONGS = "Menu Song:\n    Red Alert - Hell March\n\nVictory Scene Song:\n    David Glen Eisley - Sweet Victory\n\nDeath Scene Song:\n    Simon & Garfunkle - The Sound of Silence";
        private const string MISSIONFAILEDSTRING = "Mission Failed\n\n    You have died";
        private const string MISSIONSUCCESSSTRING = "Mission\n\n    Complete!";
        private Rectangle startGameRectangle;
        private Rectangle showHelpRectangle;
        private Rectangle showCreditRectangle;
        private Rectangle quitGameRectangle;
        private int ddRecY = -1080; //the starting position for drop-down game over and game won screen
        private Rectangle dropdownRectangle;

        public StartMenu(Game game, SpriteBatch spriteBatch) : base(game)
        {
            this.spriteBatch = spriteBatch;

            //loading assets
            splashScreen = game.Content.Load<Texture2D>("UserInterface/MenuItems/startMenu");
            helpMenu = game.Content.Load<Texture2D>("UserInterface/MenuItems/helpMenu");
            goBackButton = game.Content.Load<Texture2D>("UserInterface/MenuItems/goBack");
            goBackButtonShadow = game.Content.Load<Texture2D>("UserInterface/MenuItems/goBackShadow");
            menuHighlight = game.Content.Load<Texture2D>("UserInterface/MenuItems/menuHighlight");
            aimReticle = game.Content.Load<Texture2D>("UserInterface/crosshair");
            startMenuMusic = game.Content.Load<Song>("Music/RA_Hell_March");
            deathMenuMusic = game.Content.Load<Song>("Music/The_Sound_Of_Silence");
            deathMenu = game.Content.Load<Texture2D>("UserInterface/MenuItems/deathMenu");
            winMusic = game.Content.Load<Song>("Music/Sweet_Victory");
            winMenu = game.Content.Load<Texture2D>("UserInterface/MenuItems/winMenu");
            sfxSelectionSwitch = game.Content.Load<SoundEffect>("SFX/sfxAK47charge");
            sfxSelect = game.Content.Load<SoundEffect>("SFX/sfxAK47gunshot");
            fontUI24 = game.Content.Load<SpriteFont>("UserInterface/fontUI24");
            fontUI72 = game.Content.Load<SpriteFont>("UserInterface/fontUI72");

            //the collision rectangles for clickable elements
            aimReticleRectangle = new Rectangle(0, 0, 64, 64);
            startGameRectangle = new Rectangle(140, 440, 596, 123);
            showHelpRectangle = new Rectangle(140, 570, 596, 123);
            showCreditRectangle = new Rectangle(140, 700, 596, 123);
            quitGameRectangle = new Rectangle(140, 830, 596, 123);
            dropdownRectangle = new Rectangle(0, ddRecY, 1920, 1080);

            //always play splash screen music when showing menu
            MediaPlayer.Play(startMenuMusic); //start playing this song 
            MediaPlayer.IsRepeating = true; //keep repeating it

            showSplash = true; //starts game with showing splash screen
            pauseGame = true; //starts game paused
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            if (showSplash)
            {
                spriteBatch.Draw(splashScreen, new Rectangle(0, 0, 1920, 1080), Color.White);

                //drawing the strings for menu options
                if (startGame == false)
                {
                    if (aimReticleCollisionRectangle.Intersects(startGameRectangle)) //if player is hovering over the start game button
                    {
                        spriteBatch.Draw(menuHighlight, startGameRectangle, Color.White);
                        spriteBatch.DrawString(fontUI72, STARTGAMESTRING, new Vector2(165, 462), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
                        if (mouseState.LeftButton == ButtonState.Pressed) //if player left clicks
                        {
                            startGame = true;
                            drawReticle = false;

                            MediaPlayer.Stop();
                            MediaPlayer.IsRepeating = false;
                        }
                    }
                    else
                    {
                        spriteBatch.DrawString(fontUI72, STARTGAMESTRING, new Vector2(175, 470), Color.White, 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 0);
                    }
                }
                else
                {
                    if (aimReticleCollisionRectangle.Intersects(startGameRectangle)) //if player is hovering hover the start game rectangle, which is now the resume game text
                    {
                        spriteBatch.Draw(menuHighlight, startGameRectangle, Color.White);
                        spriteBatch.DrawString(fontUI72, RESUMEGAMESTRING, new Vector2(165, 462), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
                        if (mouseState.LeftButton == ButtonState.Pressed) //if player left clicks
                        {
                            pauseGame = false;
                            showSplash = false;
                            drawReticle = false;
                            sfxSelect.Play();

                            MediaPlayer.Stop();
                            MediaPlayer.IsRepeating = false;
                        }
                    }
                    else
                    {
                        spriteBatch.DrawString(fontUI72, RESUMEGAMESTRING, new Vector2(175, 470), Color.White, 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 0);
                    }
                }
                
                if (aimReticleCollisionRectangle.Intersects(showHelpRectangle)) //if player is hovering over the help rectangle
                {
                    spriteBatch.Draw(menuHighlight, showHelpRectangle, Color.White);
                    spriteBatch.DrawString(fontUI72, SHOWHELPSTRING, new Vector2(165, 592), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
                    if (mouseState.LeftButton == ButtonState.Pressed) //if player left clicks
                    {
                        showHelp = true;
                        showSplash = false;
                        drawReticle = false;
                        sfxSelect.Play();
                    }
                }
                else
                {
                    spriteBatch.DrawString(fontUI72, SHOWHELPSTRING, new Vector2(175, 600), Color.White, 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 0);
                }
                if (aimReticleCollisionRectangle.Intersects(showCreditRectangle)) //if player is hovering over the show credits button
                {
                    spriteBatch.Draw(menuHighlight, showCreditRectangle, Color.White);
                    spriteBatch.DrawString(fontUI72, SHOWCREDITSTRING, new Vector2(165, 722), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
                    if (mouseState.LeftButton == ButtonState.Pressed) //if player left clicks
                    {
                        showCredits = true;
                        showSplash = false;
                        drawReticle = false;
                        sfxSelect.Play();
                    }
                }
                else
                {
                    spriteBatch.DrawString(fontUI72, SHOWCREDITSTRING, new Vector2(175, 730), Color.White, 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 0);
                }
                if (aimReticleCollisionRectangle.Intersects(quitGameRectangle)) //if player is hovering over the quit game button
                {
                    spriteBatch.Draw(menuHighlight, quitGameRectangle, Color.White);
                    spriteBatch.DrawString(fontUI72, QUITGAMESTRING, new Vector2(165, 852), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
                    if (mouseState.LeftButton == ButtonState.Pressed) //if player left clicks
                    {
                        quitGame = true;
                        sfxSelect.Play();
                    }
                }
                else
                {
                    spriteBatch.DrawString(fontUI72, QUITGAMESTRING, new Vector2(175, 860), Color.White, 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 0);
                }
            }

            if (showHelp) //if show help is true, displays the help screen/picture
            {
                spriteBatch.Draw(helpMenu, new Rectangle(0, 0, 1920, 1080), Color.White);
                spriteBatch.Draw(goBackButton, new Rectangle(10, 995, 225, 75), Color.White);
            }

            if (showCredits) //if show credits is true, show credits
            {
                spriteBatch.Draw(splashScreen, new Rectangle(0, 0, 1920, 1080), Color.White);
                spriteBatch.DrawString(fontUI72, CREDITSTRING, new Vector2(165, 462), Color.White, 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 0);
                spriteBatch.DrawString(fontUI24, CREDITSTRINGSONGS, new Vector2(165, 722), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
                spriteBatch.Draw(goBackButton, new Rectangle(10, 995, 225, 75), Color.White);
            }

            if (isPlayerVictory) //if player won the game
            {
                pauseGame = true; //pause the game
                drawReticle = true; //draw the ui reticle
                spriteBatch.Draw(winMenu, dropdownRectangle, Color.White);

                if (ddRecY >= 0) //if the y position of the victory screen is greater than or equal to 0, draw the following
                {
                    //draw the success string
                    spriteBatch.DrawString(fontUI72, MISSIONSUCCESSSTRING, new Vector2(165, 462), Color.Yellow, 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 0);

                    //player can only quit the game at this point
                    if (aimReticleCollisionRectangle.Intersects(quitGameRectangle))
                    {
                        spriteBatch.Draw(menuHighlight, quitGameRectangle, Color.White);
                        spriteBatch.DrawString(fontUI72, QUITGAMESTRING, new Vector2(165, 852), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
                        if (mouseState.LeftButton == ButtonState.Pressed)
                        {
                            quitGame = true;
                            sfxSelect.Play();
                        }
                    }
                    else
                    {
                        spriteBatch.DrawString(fontUI72, QUITGAMESTRING, new Vector2(175, 860), Color.White, 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 0);
                    }
                }
            }

            if (isPlayerDead) //if player is dead
            {
                pauseGame = true; //pause the game
                drawReticle = true; //draw the ui reticle
                spriteBatch.Draw(deathMenu, dropdownRectangle, Color.White);

                if (ddRecY >= 0) //if y position of game over screen is greater than zero, draw this
                {
                    //the game over string
                    spriteBatch.DrawString(fontUI72, MISSIONFAILEDSTRING, new Vector2(165, 462), Color.Red, 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 0);

                    //player can only quit the game at this point
                    if (aimReticleCollisionRectangle.Intersects(quitGameRectangle))
                    {
                        spriteBatch.Draw(menuHighlight, quitGameRectangle, Color.White);
                        spriteBatch.DrawString(fontUI72, QUITGAMESTRING, new Vector2(165, 852), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
                        if (mouseState.LeftButton == ButtonState.Pressed)
                        {
                            quitGame = true;
                            sfxSelect.Play();
                        }
                    }
                    else
                    {
                        spriteBatch.DrawString(fontUI72, QUITGAMESTRING, new Vector2(175, 860), Color.White, 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 0);
                    }
                }
            }

            if (drawReticle) //if draw reticle is true, draw the reticle
            {
                spriteBatch.Draw(aimReticle, aimReticleRectangle, Color.White);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState keyPress = Keyboard.GetState();
            mouseState = Mouse.GetState();

            //if the esc key is pressed
            if (keyPress.IsKeyDown(Keys.Escape) && !isEscapePreviouslyPressed)
            {
                isEscapePreviouslyPressed = true;

                //game is paused while esc is pressed, it means game was in progress, but now should be paused
                if (pauseGame == false && startGame == true && !isPlayerDead && !isPlayerVictory)
                {
                    drawReticle = true;
                    pauseGame = true;
                    showSplash = true;

                    MediaPlayer.Play(startMenuMusic); //start playing this song 
                    MediaPlayer.IsRepeating = true; //keep repeating it
                }
                else if (showHelp == false && startGame == true && showCredits == false && !isPlayerDead && !isPlayerVictory)
                {
                    pauseGame = false;
                    showSplash = false;
                    drawReticle = false;

                    MediaPlayer.Stop();
                    MediaPlayer.IsRepeating = false;
                }
                if (showHelp)
                {
                    drawReticle = true;
                    showSplash = true;
                    showHelp = false;
                }
                if (showCredits)
                {
                    drawReticle = true;
                    showSplash = true;
                    showCredits = false;
                }
            }

            if (isPlayerDead) //if player is dead
            {
                if (playDeathMusic) //play the death music
                {
                    MediaPlayer.Play(deathMenuMusic);
                    MediaPlayer.IsRepeating = true;
                    playDeathMusic = false;
                }

                if (ddRecY < 0) //start incrementing the y position of drop-down screen
                {
                    ddRecY += 2;
                    dropdownRectangle = new Rectangle(0, ddRecY, 1920, 1080);
                }
            }

            if (isPlayerVictory) //if player won
            {
                if (playVictoryMusic) //play the victory music
                {
                    MediaPlayer.Play(winMusic);
                    MediaPlayer.IsRepeating = true;
                    playVictoryMusic = false;
                }

                if (ddRecY < 0) //start incrementing the y position of drop-down screen
                {
                    ddRecY += 2;
                    dropdownRectangle = new Rectangle(0, ddRecY, 1920, 1080);
                }
            }

            if (drawReticle) //if draw reticle is true, update the reticle position and collisio rectangle
            {
                aimReticleRectangle = new Rectangle(mouseState.X - 32, mouseState.Y - 32, 64, 64);
                aimReticleCollisionRectangle = new Rectangle(mouseState.X - 2, mouseState.Y - 2, 4, 4);
            }

            //this is necessary so escape can only be triggered once every press, needed for toggling
            if (keyPress.IsKeyUp(Keys.Escape))
            {
                isEscapePreviouslyPressed = false;
            }

            base.Update(gameTime);
        }

        //setters and getters
        public bool StartGame
        {
            get { return startGame; }
            set { startGame = value; }
        }
        public bool PauseGame
        {
            get { return pauseGame; }
            set { pauseGame = value; }
        }
        public bool QuitGame
        {
            get { return quitGame; }
        }
        public bool IsPlayerVictory
        {
            set { isPlayerVictory = value; }
        }
        public bool IsPlayerDead
        {
            set { isPlayerDead = value; }
        }
    }
}
