using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using System;

namespace MonoGameTest
{
    class Agent
    {
        protected Texture2D sprite;
        protected Rectangle collision;
        public float momentum;
        
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
        }

        public void Translate(float x, float y)
        {
            momentum = y;
            collision.Offset(x,y);
        }

        public void Translate(Vector2 vector)
        {
            momentum = vector.Y;
            collision.Offset(vector);
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sprite, new Rectangle(collision.Location, collision.Size), new Rectangle(0,0, sprite.Width, sprite.Height), Color.White);
        }
        
        public void decayMomentum()
        {
            momentum -= 0.0001f;  
        }

    }
    
}