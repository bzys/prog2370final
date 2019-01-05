using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

//the C3.XNA and PROG2370CollisionLibrary library does not belong to me and all rights belong to their original authors
//<insert more legal mumble jumbo here>
using C3.XNA;
using PROG2370CollisionLibrary;

namespace BSunFinalProject
{
    class CollisionBox : DrawableGameComponent
    {
        SpriteBatch spriteBatch;

        private List<Rectangle> rigidBodyList;

        public CollisionBox(Game game, SpriteBatch spriteBatch) : base(game)
        {
            this.spriteBatch = spriteBatch;
            rigidBodyList = new List<Rectangle>();
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            foreach (Rectangle rectangle in rigidBodyList)
            {
                spriteBatch.DrawRectangle(rectangle, Color.Red);
            }

            spriteBatch.End();

            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        //setters and getters
        public List<Rectangle> RigidBodyList
        {
            get { return rigidBodyList; }
        }

        //methods

        public void AddToRigidBodyList(Rectangle newCollisionBox)
        {
            if (newCollisionBox != null)
            {
                rigidBodyList.Add(newCollisionBox);
            }
        }
    }
}
