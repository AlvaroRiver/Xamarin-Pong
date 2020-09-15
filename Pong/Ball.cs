using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using System;


namespace MonoGameTest
{
    class Ball : Agent
    {
        private Random random;
        public Vector2 direction;

        public Ball(Point position, Point size, Texture2D sprite) : base(position, size, sprite) { }

        public void generateBallDirection()
        {
            random = new Random();
            direction = new Vector2((float)random.NextDouble() - 0.5f, ((float)random.NextDouble()) - 0.5f * 0.5f);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sprite,
                new Rectangle(collision.X + collision.Width/2, collision.Y+collision.Height/2, collision.Width, collision.Height),
                new Rectangle(0,0, sprite.Width, sprite.Height),
                Color.White, 
                collision.Y/20,
                new Vector2(sprite.Width/2, sprite.Height/2),
                SpriteEffects.None, 0);
        }

    }
}