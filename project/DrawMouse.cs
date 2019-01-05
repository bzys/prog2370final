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
    class DrawMouse : DrawableGameComponent
    {
        private SpriteBatch spriteBatch;
        private SpriteFont spriteFont;

        private Vector2 pos;
        private MouseState mouseState;
        private MouseState previousState;
        private string mousePos = "";

        public DrawMouse(Game game, SpriteBatch spriteBatch, SpriteFont spriteFont) : base(game)
        {
            this.spriteBatch = spriteBatch;
            this.spriteFont = spriteFont;
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            spriteBatch.DrawString(spriteFont, mousePos, new Vector2(pos.X, pos.Y), Color.Yellow);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            previousState = mouseState;
            mouseState = Mouse.GetState();
            pos.X = mouseState.X + 20;
            pos.Y = mouseState.Y;
            mousePos = "X: " + mouseState.X.ToString() + ", Y: " + mouseState.Y.ToString();

            base.Update(gameTime);
        }
    }
}
