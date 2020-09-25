using Android.App;
using Android.Text.Format;
using Java.Lang;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Xml;

namespace XamarinPong
{
    public class Pong : Microsoft.Xna.Framework.Game
    {
        public static bool inBackground = false;
        private GraphicsDeviceManager _graphics;
        private SpriteBatch mainBatch;
        private Agent leftPlayer, rightPlayer, humanPlayer, AIplayer;
        private Ball ball;
        private SpriteFont scoreFont, debugFont, promptFont;
        private int leftPlayerScore, rightPlayerScore, gameScore, Sensivity, resumeTime = 1, gamePoints = 5;
        private Vector2 scorePosition, debugStringPosition, promtPosition;
        private TouchCollection touchCollection;
        private float AIspeed, ballSpeed = 18f, momentumInfluence = 0.05f, maxVerticalRatio = 1.25f, currentSpeedMod = 1f, speedGain = 1.02f;
        private bool pause = false, debugMode = false, mustReset = false, resetGame = false;
        private Color themeColor;
        private string difficulty = "Normal", playerSprite = "Paddle1", ballSprite = "Ball1", 
            pointText = "Point!", winText ="You win!", loseText= "Game over\nYou Lose", prompt ="";
        private Texture2D fieldSprite;
        private Random random;


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
            readSettings();
            random = new Random();
            scorePosition = new Vector2(ScreenWidth / 2 - ScreenWidth / 20, 10);
            debugStringPosition = new Vector2(0, ScreenHeight - 24);
            promtPosition = new Vector2(ScreenWidth /2 - ScreenWidth/20, ScreenHeight / 2);

            //Initial ball direction
            ball.generateBallDirection();
            adjustBallDirection();

            debugMode = true;

        }

        protected override void LoadContent()
        {
            
            mainBatch = new SpriteBatch(GraphicsDevice);
            readSettings();
            scoreFont = Content.Load<SpriteFont>("Score");
            promptFont = Content.Load<SpriteFont>("Prompt");
            debugFont = Content.Load<SpriteFont>("DebugFont");
            fieldSprite = Content.Load<Texture2D>("Images/Field");


            leftPlayer = new Agent(new Point(0, ScreenHeight/2), //Center vertically
                new Point(ScreenWidth/30, ScreenHeight/5),        //Scale according to screen dimensions
                Content.Load<Texture2D>("Images/" + playerSprite));
            leftPlayer.Translate(0, -leftPlayer.Height / 2); //Adjust player position

            rightPlayer = new Agent(new Point(ScreenWidth, ScreenHeight / 2 - leftPlayer.Height / 2), 
                new Point(ScreenWidth / 30, ScreenHeight / 5),                             
                Content.Load<Texture2D>("Images/" + playerSprite));
            rightPlayer.Translate(-rightPlayer.Width , -leftPlayer.Width / 2); 

            ball = new Ball(new Point(ScreenWidth/2, ScreenHeight/2),
                    new Point(ScreenWidth / 30, ScreenWidth / 30),
                    Content.Load<Texture2D>("Images/" + ballSprite));
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            

            //Handle Android back button
            if (GamePad.GetState(0).IsButtonDown(Buttons.Back))
            {
                Exit();
            }

            if (gameTime.TotalGameTime.Seconds == 0)
                resumeTime = 1;
            
            if (!pause && gameTime.TotalGameTime.Seconds > resumeTime)
            {
                if (mustReset) Reset();
                humanPlayerMovement();
                ballMovement(gameTime);
                AIPlayerMovement();
                BallInteraction();
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(themeColor);
            mainBatch.Begin();

            mainBatch.Draw(fieldSprite, new Rectangle(0, 0, ScreenWidth, ScreenHeight), new Color(255, 255, 255, 22));
            leftPlayer.Draw(mainBatch);
            rightPlayer.Draw(mainBatch);
            ball.Draw(mainBatch);
            mainBatch.DrawString(scoreFont, leftPlayerScore + "  |  " + rightPlayerScore + " " + difficulty + "\nScore: " + gameScore, scorePosition, Color.Black);
            mainBatch.DrawString(promptFont, prompt, promtPosition, Color.Black);
            if(debugMode)
            mainBatch.DrawString(debugFont, 
            ScreenWidth + "x" + ScreenHeight + " Model:" + playerSprite + " RPModel:" + rightPlayer.sprite.Name + " BallDir:" + ball.direction + "Elapsed-Resume: " + gameTime.TotalGameTime.Seconds + "-" + resumeTime
            ,debugStringPosition, Color.Black);

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
          
            float displacement = (ball.Y - AIplayer.Center.Y);

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
                    //Add some randomness to bounce direction
                    ball.direction.Y = -leftPlayer.momentum * momentumInfluence + (float)random.NextDouble() * 0.5f - 0.25f;
                    adjustBallDirection();
                    currentSpeedMod *= speedGain; //Increase speed each time
                    ball.X = leftPlayer.X + leftPlayer.Width;
                    if (humanPlayer == leftPlayer)
                        gameScore += Settings.Difficulty;
                
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
                        gameScore += Settings.Difficulty;
            }
        }

        public void Reset()
        {

            leftPlayer.Y = ScreenHeight / 2;
            rightPlayer.Y = ScreenHeight / 2 - leftPlayer.Height / 2;
            ball.Position = new Point(ScreenWidth / 2, ScreenHeight / 2);
            ball.generateBallDirection();
            currentSpeedMod = 1f;
            prompt = "";
            mustReset = false;

            if(resetGame)
                ResetGame();
            
        }

        public void ResetGame()
        {
            if(gameScore > Settings.highScore) 
                Settings.highScore = gameScore;

            resetGame = false;
            leftPlayerScore = rightPlayerScore = gameScore = 0;
        }

        public void ballMovement(GameTime gameTime)
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
                ScorePoint(gameTime, rightPlayer);
            }
            else if (ball.X > ScreenWidth - ball.Width)
            {
                ScorePoint(gameTime, leftPlayer);
            }

            adjustBallDirection();
            ball.direction.Normalize();
            ball.Translate(ball.direction * ballSpeed * currentSpeedMod);
        }

        //We want the ball to move mostly horizontally
        public void adjustBallDirection()
        {
            if (ball.direction.Y / ball.direction.X > maxVerticalRatio)
            {
                if (ball.direction.Y < 0)
                    ball.direction.Y = -ball.direction.X * maxVerticalRatio;
                else
                    ball.direction.Y = ball.direction.X * maxVerticalRatio;
            }
        }

        public void ScorePoint(GameTime gameTime, Agent player)
        {
            mustReset = true;
            if(player == leftPlayer)
            {
                leftPlayerScore++;
                if (humanPlayer == leftPlayer)
                    gameScore += Settings.Difficulty * 10;
                pauseForTime(gameTime, 1);
            }
            else if(player == rightPlayer)
            {
                rightPlayerScore++;
                if (humanPlayer == rightPlayer)
                {
                    gameScore += Settings.Difficulty * 10;
                }
                   
                pauseForTime(gameTime, 1);
            }

            if ((leftPlayer == humanPlayer && leftPlayerScore == gamePoints) || (rightPlayer == humanPlayer && rightPlayerScore == gamePoints))
            {
                if(gameScore > Settings.highScore)
                    prompt = winText + "\n New highscore: " + gameScore;
                resetGame = true;
                
            }
            else if ((leftPlayer == AIplayer && leftPlayerScore == gamePoints) || (rightPlayer == AIplayer && rightPlayerScore == gamePoints))
            {
                if (gameScore > Settings.highScore)
                    prompt = loseText + "\n New highscore: " + gameScore;
                resetGame = true;
            }
            else
                prompt = pointText;
        }

        public void pauseForTime(GameTime gameTime, int seconds)
        {
            
            resumeTime = (gameTime.TotalGameTime.Seconds + seconds) % 60;
            
        }

        public void readSettings()
        {
            themeColor = new Color(Settings.R, Settings.G, Settings.B, 122);
            Sensivity = 10 - Settings.Sensivity;
            gamePoints = Settings.maxScore;

            if(!Settings.RightPaddle)
            {
                humanPlayer = leftPlayer;
                AIplayer = rightPlayer;
            }
            else
            {
                AIplayer = leftPlayer;
                humanPlayer = rightPlayer;
            }

            switch (Settings.Difficulty)
            {
                case 1 : { AIspeed = ScreenHeight * 0.01f; difficulty = "Easy"; } break;
                case 2 : { AIspeed = ScreenHeight * 0.012f; difficulty = "Normal"; } break;
                case 3 : { AIspeed = ScreenHeight * 0.014f; difficulty = "Hard"; } break;
                default: { AIspeed = ScreenHeight * 0.012f; difficulty = "Normal"; } break;
            }

            switch (Settings.player)
            {
                case 0: playerSprite = "Paddle1"; break;
                case 1: playerSprite = "Paddle2"; break;
                case 2: playerSprite = "Paddle3"; break;
                default: playerSprite = "Paddle1"; break;
            }

           switch (Settings.ball)
            {
                case 0: ballSprite = "Ball1"; break;
                case 1: ballSprite = "Ball2"; break;
                case 2: ballSprite = "Ball3"; break;
                default: ballSprite = "Ball1"; break;
            }
        }
    }
}
