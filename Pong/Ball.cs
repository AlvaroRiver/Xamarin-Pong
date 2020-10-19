
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;


namespace XamarinPong
{
    class Ball : Agent
    {
        private Random random;
        public Vector2 direction;
        private const float maxVerticalRatio = 1.25f;
        
        public Ball(Point position, Point size, Texture2D sprite) : base(position, size, sprite) { }

        public void generateBallDirection()
        {
            random = new Random();
            direction = new Vector2((float)random.NextDouble() - 0.5f, ((float)random.NextDouble()) - 0.5f * 0.5f);         
        }

        public void adjustBallDirection()
        {
            if (direction.Y / direction.X > maxVerticalRatio)
            {
                if (direction.Y < 0)
                    direction.Y = -direction.X * maxVerticalRatio;
                else
                    direction.Y = direction.X * maxVerticalRatio;
            }
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