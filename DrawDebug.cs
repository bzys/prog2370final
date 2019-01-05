/*
 *  Author:     Bill Sun        
 *  Comments:   this is the draw debug class for drawing debug text to screen
 *  Revision History: 
 *      i forgot when i created this
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
    class DrawDebug : DrawableGameComponent
    {
        private SpriteBatch spriteBatch;
        private SpriteFont spriteFont;

        private Vector2 pos = new Vector2(50f, 50f);

        private string debugText = "";

        public DrawDebug(Game game, SpriteBatch spriteBatch, SpriteFont spriteFont) : base(game)
        {
            this.spriteBatch = spriteBatch;
            this.spriteFont = spriteFont;
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            spriteBatch.DrawString(spriteFont, debugText, new Vector2(pos.X, pos.Y), Color.Yellow);

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
            base.Update(gameTime);
        }

        public string DebugText
        {
            get { return debugText; }
            set { debugText = value; }
        }
    }
}
