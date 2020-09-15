using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using System;

namespace MonoGameTest
{
    class Agent
    {
        private Texture2D sprite;
        private Rectangle collision;
        public Vector2 direction;
        public float momentum;
        private Random random;

        public int Width { get => collision.Width; }
        public int Height { get => collision.Height; }
        public int Y { get => collision.Y; set => collision.Y = value; }
        public int X { get => collision.X; set => collision.X = value; }
        public Point Center { get => collision.Center; }
        public Point Position
        {
            get => collision.Location;
            set => collision.Location = value;
        }

        public Agent(Point position, Point size, Texture2D sprite)
        {
            this.sprite = sprite;
            collision = new Rectangle(position, size);
            random = new Random();
        }

        public void Translate(float x, float y)
        {
            momentum = (int)y;
            collision.Offset(x,y);
   
        }

        public void Translate(Vector2 vector)
        {
            momentum = (int)vector.Y;
            collision.Offset(vector);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sprite, new Rectangle(collision.X, collision.Y, collision.Width, collision.Height), new Rectangle(0, 0, sprite.Width, sprite.Height), Color.White);
        }

        public void Draw(SpriteBatch spriteBatch, Color tint)
        {
            Draw(spriteBatch, tint);
        }

        public void generateBallDirection()
        {
            direction = new Vector2((float)random.NextDouble() - 0.5f, ((float)random.NextDouble()) - 0.5f * 0.5f);
        }

        public void decayMomentum()
        {
            if (momentum > 0)
                momentum--;
            else if (momentum < 0)
                momentum++;
        }

    }
    
}