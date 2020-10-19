using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Diagnostics;
using Android.Content;
using System.Net.Sockets;

namespace XamarinPong
{
    public class Pong : Microsoft.Xna.Framework.Game
    {
        public static bool inBackground = false;
        private GraphicsDeviceManager _graphics;
        private SpriteBatch mainBatch;
        private Agent leftPlayer, rightPlayer, selfPlayer, AIplayer;
        private Ball ball;
        private SpriteFont scoreFont, debugFont, promptFont;
        private Stopwatch timer;
        private long resumeTime = 500;  //Small pause at start
        private int leftPlayerScore, rightPlayerScore, gameScore, Sensivity, maxPlayerMovement,
            gamePoints = 5,
            scoreMargin = 10,
            debugMargin = 24,
            scoreFactor = 10,
            maxScreenFractionMovement = 12; //Bigger is number is less movement

        private Vector2 scorePosition, debugStringPosition, promtPosition;
        private TouchCollection touchCollection;
        private float AIspeed,
            ballSpeed = 18f,
            momentumInfluence = 0.05f,
            currentSpeedMod = 1f,
            easyFactor = 0.01f, 
            normalFactor = 0.012f, 
            hardFactor = 0.014f,
            speedGain = 1.02f;

        private bool pause = false, debugMode = false, mustReset = false, resetGame = false;
        private Color themeColor;
        private string
            difficulty = "Normal",
            playerSprite = "Paddle1",
            ballSprite = "Ball1",
            pointText = "Point!",
            winText = "You win!",
            loseText = "Game over\nYou Lose", prompt = "";

        private Texture2D fieldSprite;
        private Random random;
        private SoundEffect hitSound, wonPointSound, lostPointSound, wonGameSound, lostGameSound, bounceSound;
        private SoundEffectInstance hit, wonPoint, lostPoint, wonGame, lostGame, bounce;

        public int ScreenWidth => _graphics.GraphicsDevice.Viewport.Width;
        public int ScreenHeight => _graphics.GraphicsDevice.Viewport.Height;
        public Point ScreenCentre;

        //Networking
        public static NetworkStream NetStream;
        public enum Mode { SinglePlayer = 0, Host = 1, Guest = 2 };
        public static Mode mode;


        public Pong()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            timer = new Stopwatch();
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
            ScreenCentre = new Point(ScreenWidth / 2, ScreenHeight / 2);
            readSettings();
            random = new Random();
            scorePosition = new Vector2(ScreenCentre.X - ScreenWidth / 20, scoreMargin);
            debugStringPosition = new Vector2(0, ScreenHeight - debugMargin);
            promtPosition = new Vector2(ScreenCentre.X - ScreenWidth / 20, ScreenCentre.Y);
            maxPlayerMovement = ScreenHeight / maxScreenFractionMovement;
            ball.generateBallDirection();
            ball.adjustBallDirection();
            timer.Start();
        }

        protected override void LoadContent()
        {
            mainBatch = new SpriteBatch(GraphicsDevice);
            readSettings();
            scoreFont = Content.Load<SpriteFont>("Score");
            promptFont = Content.Load<SpriteFont>("Prompt");
            debugFont = Content.Load<SpriteFont>("DebugFont");
            fieldSprite = Content.Load<Texture2D>("Images/Field");
            loadAudio();

            leftPlayer = new Agent(new Point(0, ScreenCentre.Y),  //Center vertically
                new Point(ScreenWidth / 30, ScreenHeight / 5),        //Scale according to screen dimensions
                Content.Load<Texture2D>("Images/" + playerSprite));

            rightPlayer = new Agent(new Point(ScreenWidth, ScreenHeight / 2 - leftPlayer.Height / 2),
                new Point(ScreenWidth / 30, ScreenHeight / 5),
                Content.Load<Texture2D>("Images/" + playerSprite));
            rightPlayer.Translate(-rightPlayer.Width, -leftPlayer.Width / 2); //Adjust player position

            ball = new Ball(new Point(ScreenWidth / 2, ScreenHeight / 2),
                    new Point(ScreenWidth / 30, ScreenWidth / 30),
                    Content.Load<Texture2D>("Images/" + ballSprite));
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (GamePad.GetState(0).IsButtonDown(Buttons.Back)) //Handle Android back button
            {
                this.Exit();
            }

            if (!pause && timer.ElapsedMilliseconds > resumeTime)
            {
                Reset();
                PlayerMovement();
                ballMovement();
                AIMovement();
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

            if (debugMode)
                mainBatch.DrawString(debugFont, ScreenWidth + "x" + ScreenHeight + " Model:" + playerSprite + " RPModel:" +
                rightPlayer.sprite.Name + " BallDir:" + ball.direction + "Elapsed-Resume: " +
                timer.ElapsedMilliseconds + "-" + resumeTime
                , debugStringPosition, Color.Black);

            mainBatch.End();
            base.Draw(gameTime);
        }

        public void PlayerMovement()
        {
            touchCollection = TouchPanel.GetState();
            if (touchCollection.Count < 1) return;

            //Move player according to touch being over/underneath Y position
            //Only one finger considered(0 index)
            int displacement = ((int)touchCollection[0].Position.Y - selfPlayer.Center.Y) / Sensivity;

            //Prevent movement from being too sudden
            displacement = MathHelper.Clamp(displacement, -maxPlayerMovement, maxPlayerMovement);

            selfPlayer.Translate(0, displacement);
            selfPlayer.Y = MathHelper.Clamp(selfPlayer.Y, 0, ScreenHeight - selfPlayer.Height);

            selfPlayer.decayMomentum();
        }

        public void AIMovement()
        {
            float displacement = (ball.Y - AIplayer.Center.Y);

            //Prevent movement from being too sudden
            displacement = MathHelper.Clamp(displacement, -AIspeed, AIspeed);

            //Keep in screen bounds
            if ((displacement > 0 && AIplayer.Y + AIplayer.Height >= ScreenHeight) ||
                    (displacement < 0 && AIplayer.Y <= 0))
                return;

            AIplayer.Translate(0, (int)displacement);
        }

        public void BallInteraction()
        {
            //Collision check
            if (ball.Collides(leftPlayer))
            {
                hit.Play();
                ball.direction.X = -ball.direction.X;
                //Add some randomness to bounce direction
                ball.direction.Y = -leftPlayer.momentum * momentumInfluence + (float)random.NextDouble() * 0.5f - 0.25f;
                ball.adjustBallDirection();
                currentSpeedMod *= speedGain; //Increase speed each time
                ball.X = leftPlayer.X + leftPlayer.Width;
                if (selfPlayer == leftPlayer)
                    gameScore += Settings.Difficulty;
            }

            if (ball.Collides(rightPlayer))
            {
                hit.Play();
                ball.direction.X = -ball.direction.X;
                ball.direction.Y = -leftPlayer.momentum * momentumInfluence + (float)random.NextDouble() * 0.5f - 0.25f;
                ball.adjustBallDirection();
                currentSpeedMod *= speedGain; //Increase speed each time
                ball.X = rightPlayer.X - ball.Width;
                if (selfPlayer == rightPlayer)
                    gameScore += Settings.Difficulty;
            }
        }

        public void Reset()
        {
            if (!mustReset) return;
            leftPlayer.Y = ScreenCentre.Y;
            rightPlayer.Y = ScreenCentre.Y - leftPlayer.Height / 2;
            ball.Position = ScreenCentre;
            ball.generateBallDirection();
            currentSpeedMod = 1f;
            prompt = "";
            mustReset = false;

            if (resetGame)
                ResetGame();
        }

        public void ResetGame()
        {
            if (gameScore > Settings.highScore)
                Settings.highScore = gameScore;

            resetGame = false;
            prompt = "";
            leftPlayerScore = rightPlayerScore = gameScore = 0;
        }

        public void ballMovement()
        {
            //Keep in screen bounds and bounce
            if (ball.Y < 0)
            {
                bounce.Play();
                ball.Translate(0, -ball.Y);
                ball.direction.Y = -ball.direction.Y;
            }
            else if (ball.Y > ScreenHeight - ball.Height)
            {
                bounce.Play();
                ball.Translate(0, ScreenHeight - ball.Y - ball.Height);
                ball.direction.Y = -ball.direction.Y;
            }

            if (ball.X <= 0)
            {
                lostPoint.Play();
                ScorePoint(rightPlayer);
            }
            else if (ball.X >= ScreenWidth)
            {
                lostPoint.Play();
                ScorePoint(leftPlayer);
            }

            ball.adjustBallDirection();
            ball.direction.Normalize();
            ball.Translate(ball.direction * ballSpeed * currentSpeedMod);
        }

        public void ScorePoint(Agent player)
        {
            mustReset = true;
            prompt = pointText;
            pauseForTime(1000);

            if (selfPlayer == player)
            {
                wonPoint.Play();
                gameScore += Settings.Difficulty * scoreFactor;
            }

            if (player == leftPlayer)
                leftPlayerScore++;
            else
                rightPlayerScore++;

            if (leftPlayerScore == gamePoints)
                FinishGame(leftPlayer, 1500);
            else if (rightPlayerScore == gamePoints)
                FinishGame(rightPlayer, 1500);

        }

        public void FinishGame(Agent player, int extraPauseMillis = 0)
        {
            if (player == selfPlayer)
            {
                prompt = winText;
                wonGame.Play();
            }
            else
            {
                prompt = loseText;
                lostGame.Play();
            }

            if (gameScore > Settings.highScore)
            {
                Settings.highScore = gameScore;
                prompt += "\n New highscore: " + gameScore;
            }
            resetGame = true;
            resumeTime += extraPauseMillis; //Extra pause time
        }

        public void pauseForTime(long millis)
        {
            timer.Restart();
            resumeTime = timer.ElapsedMilliseconds + millis;
        }

        public void loadAudio()
        {
            hitSound = Content.Load<SoundEffect>("Audio/hit");
            wonPointSound = Content.Load<SoundEffect>("Audio/wonpoint");
            lostPointSound = Content.Load<SoundEffect>("Audio/lostpoint");
            wonGameSound = Content.Load<SoundEffect>("Audio/wongame");
            lostGameSound = Content.Load<SoundEffect>("Audio/lostgame");
            bounceSound = Content.Load<SoundEffect>("Audio/bounce");

            hit = hitSound.CreateInstance();
            wonPoint = wonPointSound.CreateInstance();
            lostPoint = lostPointSound.CreateInstance();
            wonGame = wonGameSound.CreateInstance();
            lostGame = lostPointSound.CreateInstance();
            bounce = bounceSound.CreateInstance();
        }

        public void readSettings()
        {
            themeColor = new Color(Settings.R, Settings.G, Settings.B, 122);
            Sensivity = 10 - Settings.Sensivity;
            gamePoints = Settings.maxScore;
            debugMode = Settings.DebugMode == 0 ? false : true;

            if (!Settings.RightPaddle)
            {
                selfPlayer = leftPlayer;
                AIplayer = rightPlayer;
            }
            else
            {
                AIplayer = leftPlayer;
                selfPlayer = rightPlayer;
            }

            switch (Settings.Difficulty)
            {
                case 1: { AIspeed = ScreenHeight * easyFactor; difficulty = "Easy"; }
                    break;
                case 2: { AIspeed = ScreenHeight * normalFactor; difficulty = "Normal"; }
                    break;
                case 3: { AIspeed = ScreenHeight * hardFactor; difficulty = "Hard"; }
                    break;
                default: { AIspeed = ScreenHeight * normalFactor; difficulty = "Normal"; }
                    break;
            }

            switch (Settings.player)
            {
                case 0:
                    playerSprite = "Paddle1"; break;
                case 1:
                    playerSprite = "Paddle2"; break;
                case 2:
                    playerSprite = "Paddle3"; break;
                default:
                    playerSprite = "Paddle1"; break;
            }

            switch (Settings.ball)
            {
                case 0:
                    ballSprite = "Ball1"; break;
                case 1:
                    ballSprite = "Ball2"; break;
                case 2:
                    ballSprite = "Ball3"; break;
                default:
                    ballSprite = "Ball1"; break;
            }
        }
    }
}
