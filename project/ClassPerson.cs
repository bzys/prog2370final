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
    class ClassPerson : DrawableGameComponent
    {
        private Texture3D model;
        private int health = 100;
        private int moveSpeed = 10;

        public ClassPerson(Game game) : base(game)
        {
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
