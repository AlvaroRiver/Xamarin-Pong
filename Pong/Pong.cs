﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Xml;

namespace MonoGameTest
{
    public class Pong : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch mainBatch;
        private Agent leftPlayer, rightPlayer, humanPlayer, AIplayer, ball;
        private SpriteFont scoreFont;
        private int leftPlayerScore, rightPlayerScore, gameScore;
        private Vector2 scorePosition;
        private int Sensivity = 5, pauseTime, AIspeed = 9;
        private TouchCollection touchCollection;
        private float ballSpeed = 18f, momentumInfluence = 0.1f, maxVerticalRatio = 1.25f, currentSpeedMod = 1f, speedGain = 1.02f;
        private bool pause = false;
        private Color themeColor;
        private string difficulty = "Normal";

        public int ScreenWidth => _graphics.GraphicsDevice.Viewport.Width;
        public int ScreenHeight => _graphics.GraphicsDevice.Viewport.Height;


        public Pong()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
            scorePosition = new Vector2(ScreenWidth / 2, 10);
            readSettings();

            //Aliases for specializing behaviour, we can set human at left or right paddle
            humanPlayer = leftPlayer;
            AIplayer = rightPlayer;

            //Initial ball direction
            ball.generateBallDirection();
            adjustBallDirection();

        }

        protected override void LoadContent()
        {
            mainBatch = new SpriteBatch(GraphicsDevice);
           
            scoreFont = Content.Load<SpriteFont>("Score");
                            

            leftPlayer = new Agent(new Point(0, ScreenHeight/2), //Center vertically
                new Point(ScreenWidth/30, ScreenHeight/5),        //Scale according to screen dimensions
                Content.Load<Texture2D>("Images/paddle"));
            leftPlayer.Translate(0, -leftPlayer.Height / 2); //Adjust player position

            rightPlayer = new Agent(new Point(ScreenWidth, ScreenHeight / 2 - leftPlayer.Height / 2), 
                new Point(ScreenWidth / 30, ScreenHeight / 5),                             
                Content.Load<Texture2D>("Images/paddle"));
            rightPlayer.Translate(-rightPlayer.Width , -leftPlayer.Width / 2); 

            ball = new Agent(new Point(ScreenWidth/2, ScreenHeight/2),
                    new Point(ScreenWidth / 30, ScreenWidth / 30),
                    Content.Load<Texture2D>("Images/ball"));


        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (pause) return;

            humanPlayerMovement();
            ballMovement();
            AIPlayerMovement();
            BallInteraction();
            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(themeColor);

            mainBatch.Begin();

            leftPlayer.Draw(mainBatch);
            rightPlayer.Draw(mainBatch);
            ball.Draw(mainBatch);
            mainBatch.DrawString(scoreFont, leftPlayerScore + "  |  " + rightPlayerScore + " " + difficulty + "\nScore: " + gameScore, scorePosition, Color.White);

            mainBatch.End();

            base.Draw(gameTime);
        }

        public void humanPlayerMovement()
        {
            touchCollection = TouchPanel.GetState();
            if (touchCollection.Count > 0)
            {
                //Move player according to touch being over/underneath Y position
                //Only one finger considered(0 index)
                int displacement = ((int)touchCollection[0].Position.Y - humanPlayer.Center.Y) / Sensivity;

                //Prevent movement from being too sudden
                if (displacement > ScreenHeight / 12)
                    displacement = ScreenHeight / 12;
                else if (displacement < -ScreenHeight / 12)
                    displacement = -ScreenHeight / 12;
                else
                    humanPlayer.decayMomentum();

                humanPlayer.Translate(0, displacement);

            }

            //Keep in screen bounds
            if (humanPlayer.Y < 0)
                humanPlayer.Translate(0, -humanPlayer.Y);

            else if (humanPlayer.Y > ScreenHeight - humanPlayer.Height)
                humanPlayer.Translate(0, ScreenHeight - humanPlayer.Y - humanPlayer.Height);

        }

        public void AIPlayerMovement()
        {
          
            int displacement = (ball.Y - AIplayer.Center.Y);

            //Prevent movement from being too sudden
            if (displacement > AIspeed)
                displacement = AIspeed;
            else if (displacement < -AIspeed)
                displacement = -AIspeed;

            //Keep in screen bounds
            if ((displacement > 0 && AIplayer.Y + AIplayer.Height >= ScreenHeight) ||
                    (displacement < 0 && AIplayer.Y <= 0))
                displacement = 0;
                
                AIplayer.Translate(0, displacement);
            
        }


        public void BallInteraction()
        {
            //Collision check
            if(ball.Center.Y >= leftPlayer.Center.Y-leftPlayer.Height/2 && ball.Center.Y <= leftPlayer.Center.Y + leftPlayer.Height/2
                && ball.Center.X < leftPlayer.Width + ball.Width/2)
                {
                    ball.direction.X = -ball.direction.X;
                    ball.direction.Y = -leftPlayer.momentum * momentumInfluence;
                adjustBallDirection();
                currentSpeedMod *= speedGain; //Increase speed each time
                ball.X = leftPlayer.X + leftPlayer.Width;
                if (humanPlayer == leftPlayer)
                    gameScore += AIspeed;
                
            }

           if (ball.Center.Y >= rightPlayer.Center.Y - rightPlayer.Height / 2 && ball.Center.Y <= rightPlayer.Center.Y + rightPlayer.Height / 2
                && ball.Center.X > rightPlayer.Center.X -ball.Width/2 - rightPlayer.Width/2)
                {
                    ball.direction.X = -ball.direction.X;
                    ball.direction.Y = -rightPlayer.momentum * momentumInfluence;
                adjustBallDirection();
                currentSpeedMod *= speedGain; //Increase speed each time
                ball.X = rightPlayer.X - ball.Width;
                if (humanPlayer == rightPlayer)
                    gameScore += AIspeed;

            }

        }


        public void Reset()
        {
            leftPlayer.Y = ScreenHeight / 2;
            rightPlayer.Y = ScreenHeight / 2 - leftPlayer.Height / 2;
            ball.Position = new Point(ScreenWidth / 2, ScreenHeight / 2);
            ball.generateBallDirection();
            currentSpeedMod = 1f;
            
        }


        public void ballMovement()
        {

            //Keep in screen bounds and bounce
            if (ball.Y < 0)
            {
                ball.Translate(0, -ball.Y);
                ball.direction.Y = -ball.direction.Y;
            }
            else if (ball.Y > ScreenHeight - ball.Height)
            {
                ball.Translate(0, ScreenHeight - ball.Y - ball.Height);
                ball.direction.Y = -ball.direction.Y;
            }

            if (ball.X < 0)
            {
                rightPlayerScore++;
                if (humanPlayer == rightPlayer)
                    gameScore += AIspeed * 10; ;
                Reset();
            }
            else if (ball.X > ScreenWidth - ball.Width)
            {
                leftPlayerScore++;
                if (humanPlayer == leftPlayer)
                    gameScore += AIspeed *10;
                Reset();
            }

            adjustBallDirection();
            ball.direction.Normalize();
            ball.Translate(ball.direction * ballSpeed);

        }

        public void adjustBallDirection() //We want the ball to move mostyle horizontally
        {
            if (ball.direction.Y / ball.direction.X > maxVerticalRatio)
                if (ball.direction.Y < 0) ball.direction.Y = -ball.direction.X * maxVerticalRatio;
                else ball.direction.Y = ball.direction.X * maxVerticalRatio;
        }

        public void readSettings()
        {
            themeColor = new Color(Settings.R, Settings.G, Settings.B);
            switch (Settings.Difficulty)
            {
                case 1 : { AIspeed = 8; difficulty = "Easy"; } break;
                case 2 : { AIspeed = 10; difficulty = "Normal"; } break;
                case 3 : { AIspeed = 12; difficulty = "Hard"; }break;
                default: { AIspeed = 10; difficulty = "Normal"; } break;
            }
        }
    }
}
