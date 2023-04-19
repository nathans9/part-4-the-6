using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

//TO DO:
//BONUS GAME


namespace prat_4_and_a_half
{
    internal class Program
    {
        public static int speedMultiplier = 1; //My laptop could not handle pong moving faster than one tick per .1 seconds, but most computers should.  Current pong is very slow, and I recommend increading this value upon marking (The current timer interval is divided by this value).
        //If pong runs poorly, I recommend lowering the multiplier (unless it is already at 1), or removing the console.beeps

        //importin' some dlls
        const int MF_BYCOMMAND = 0x00000000;
        public const int SC_CLOSE = 0xF060;
        const int SC_MINIMIZE = 0xF020;
        const int SC_MAXIMIZE = 0xF030;
        const int SC_SIZE = 0xF000;

        [DllImport("user32.dll")]
        public static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();
        //Make joke count 4 to just test the pong game without having to hear ever joke monkey joke
        public static int jokeCount = 0, playerScore = 0, monkeyScore = 0, playerTop = 11, playerBottom = 18, monkeyTop = 11, monkeyBottom = 18, prevPos = 1, monkeySpeed = 1, ballX = 60, ballY = 14, ballXSpeed = 1, ballYSpeed = 1, randBallSpeed = 0;  //Change back to 0 after testing is done.

        public static bool joke1 = false, joke2 = false, joke3 = false, joke4 = false;

        public static bool visited = false, gameOn = false;

        public static Random generator = new Random();

        public static char[] asset = { '-', '|', '_', '█' };

        static ConsoleKeyInfo cki = new ConsoleKeyInfo();

        public static System.Timers.Timer timer = new System.Timers.Timer();

        public static string anyKey = "[Press any key to continue]";
        public static string[] scoreOne =
            {
                "██",
                " █",
                " █",
                " █",
                " █"
            };
        public static string[] scoreTwo =
        {
                "███",
                "  █",
                "███",
                "█  ",
                "███"
            };
        public static string[] scoreThree =
        {
                "███",
                "  █",
                " ██",
                "  █",
                "███"
            };
        public static string[] youWin =
        {
            "█ █  ███  █ █   █   █  ███  █  █  █",
            "█ █  █ █  █ █   █   █   █   ██ █  █",
            " █   █ █  █ █   █ █ █   █   █ ██   ",
            " █   ███  ███    █ █   ███  █  █  █"
        };
        public static string[] youLose =
        {
            "█ █  ███  █ █   █    ███  ███  ███       ",
            "█ █  █ █  █ █   █    █ █  ██   █_        ",
            " █   █ █  █ █   █    █ █   ██  █‾        ",
            " █   ███  ███   ███  ███  ███  ███  █ █ █"
        };

        //The third ascii is the pong game.  Just keep in mind that I remove your ability to close the window once you are challenged by the joke monkey.  You must play pong to completion.
        private static void PongGame()
        {
            string[] prepare = { "The monkey has challenged you to a game of Pong!", "[Press any key when ready]" };

            bool playAgain = true;

            while (playAgain)
            {
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.Clear();
                Console.CursorVisible = false;

                for (int i = 0; i < prepare[0].Length; i++)
                {
                    Console.Write(prepare[0][i]);
                    Thread.Sleep(15);
                }

                Console.ForegroundColor = ConsoleColor.Black;
                Console.WriteLine("");
                Thread.Sleep(200);

                for (int i = 0; i < prepare[1].Length; i++)
                {
                    Console.Write(prepare[1][i]);
                    Thread.Sleep(15);
                }

                Console.ReadKey();

                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.Black;

                Console.Clear();

                for (int i = 0; i < 5; i++)

                {
                    Console.SetCursorPosition(45, 14);
                    Console.WriteLine("MOVE USING THE ARROW KEYS");
                    Thread.Sleep(500);
                    Console.Clear();
                    Thread.Sleep(500);
                }

                //START OF THE GAME

                timer.Elapsed += new ElapsedEventHandler(TimerTime);
                timer.Interval = 100 / speedMultiplier;
                timer.Enabled = true;

                bool gameOn = true;

                if (gameOn)
                    DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_CLOSE, MF_BYCOMMAND);

                for (int i = 0; i < 8; i++)
                {
                    Console.SetCursorPosition(1, (playerTop + i));
                    Console.Write(asset[3]);
                }

                randBallSpeed = generator.Next(1, 7);

                switch (randBallSpeed)
                {
                    case 1:
                        ballXSpeed = 1;
                        ballYSpeed = 1;
                        break;
                    case 2:
                        ballXSpeed = 1;
                        ballYSpeed = -1;
                        break;
                    case 3:
                        ballXSpeed = 1;
                        ballYSpeed = 1;
                        break;
                    case 4:
                        ballXSpeed = 1;
                        ballYSpeed = 2;
                        break;
                    case 5:
                        ballXSpeed = 1;
                        ballYSpeed = -1;
                        break;
                    case 6:
                        ballXSpeed = 1;
                        ballYSpeed = -2;
                        break;
                }

                while (gameOn)
                {
                    PlayerMove();
                }

            }

        } 

        //This timer is used to trigger the ball moving and the monkey moving, since I couldn't be bothered to program the monkey's AI
        private static void TimerTime(object source, ElapsedEventArgs e)
        {

            if (monkeyTop == 0)
                monkeySpeed = 1;
            else if (monkeyBottom == 28)
                monkeySpeed = -1;

            Console.ForegroundColor = ConsoleColor.White;

            switch (monkeySpeed)
            {
                case 1:
                    MonkeyMoveDown();
                    break;
                case -1:
                    MonkeyMoveUp();
                    break;
            }

            BallMove();
        }
        //This moves the ball
        private static void BallMove()
        {
            int lastBallX = ballX;
            int lastBallY = ballY;
            ballX = ballX - ballXSpeed;
            ballY = ballY - ballYSpeed;

            if (ballX == 2 && ballY >= playerTop && ballY <= playerBottom)
            {
                ballXSpeed = -ballXSpeed;
                Console.Beep(300, 200);
            }

            else if (ballX == 119 && ballY >= monkeyTop && ballY <= monkeyBottom)
            {
                ballXSpeed = -ballXSpeed;
                Console.Beep(300, 200);
            }

            if (ballY == 28 || ballY == 0)
            {
                ballYSpeed = -ballYSpeed;
                Console.Beep(150, 200);
            }

            switch (ballX)
            {
                case 120:
                    PlayerScore();
                    break;
                case 0:
                    MonkeyScore();
                    break;

            }    

            Console.SetCursorPosition(ballX, ballY);
            Console.Write(asset[3]);
            Console.SetCursorPosition(lastBallX, lastBallY);
            Console.Write(" ");
        } 
        //This tracks the player's movements
        static void PlayerMove()
        {
            cki = Console.ReadKey();
            if (ConsoleKey.UpArrow == cki.Key)
            {
                if (playerTop != 0)
                {
                    playerTop = playerTop - 1;
                    playerBottom = playerBottom - 1;
                }
                for (int i = 0; i < 8; i++)
                {
                    Console.SetCursorPosition(1, (playerTop + i));
                    Console.Write(asset[3]);
                }

                Console.SetCursorPosition(1, playerBottom + 1);
                Console.Write(" ");
            }
            else if (ConsoleKey.DownArrow == cki.Key)
            {
                if (playerBottom != 28)
                {
                    playerTop = playerTop + 1;
                    playerBottom = playerBottom + 1;
                }
                for (int i = 0; i < 8; i++)
                {
                    Console.SetCursorPosition(1, (playerTop + i));
                    Console.Write(asset[3]);
                }
                Console.SetCursorPosition(1, playerTop - 1);
                Console.Write(" ");
                
            }
        }
        //This moves the joke monkey down
        private static void MonkeyMoveDown()
        {
            monkeyTop = monkeyTop + monkeySpeed;
            monkeyBottom = monkeyBottom + monkeySpeed;

            for (int i = 0; i < 8; i++)
            {
                Console.SetCursorPosition(119, (monkeyTop + i));
                Console.Write(asset[3]);
            }

            if (monkeyTop != 0)
            {
                Console.SetCursorPosition(119, monkeyTop - 1);
                Console.Write(" ");
            }
        }
        //This moves the joke monkey up (don't ask why they're seperate methods...)
        private static void MonkeyMoveUp()
        {
            monkeyTop = monkeyTop + monkeySpeed;
            monkeyBottom = monkeyBottom + monkeySpeed;

            for (int i = 0; i < 8; i++)
            {
                Console.SetCursorPosition(119, (monkeyBottom - i));
                Console.Write(asset[3]);
            }

            if (monkeyBottom != 0)
            {
                Console.SetCursorPosition(119, monkeyBottom + 1);
                Console.Write(" ");
            }
        }
        //This tracks the player's score
        private static void PlayerScore()
        {
            Console.Clear();
            timer.Enabled = false;

            Console.Beep(200, 200);
            Thread.Sleep(200);
            Console.Beep(400, 200);

            playerScore = playerScore + 1;

            playerTop = 11; playerBottom = 18; monkeyTop = 11; monkeyBottom = 18; monkeySpeed = 1; ballX = 60; ballY = 14; ballXSpeed = 1; ballYSpeed = 1; randBallSpeed = 0;

            randBallSpeed = generator.Next(1, 7);

            switch (randBallSpeed)
            {
                case 1:
                    ballXSpeed = 1;
                    ballYSpeed = 1;
                    break;
                case 2:
                    ballXSpeed = 1;
                    ballYSpeed = -1;
                    break;
                case 3:
                    ballXSpeed = 2;
                    ballYSpeed = 1;
                    break;
                case 4:
                    ballXSpeed = 1;
                    ballYSpeed = 2;
                    break;
                case 5:
                    ballXSpeed = 2;
                    ballYSpeed = -1;
                    break;
                case 6:
                    ballXSpeed = 1;
                    ballYSpeed = -2;
                    break;
            }

            switch (playerScore)
            {
                case 1:
                    for (int i = 0; i < 5; i++)
                    {
                        Console.SetCursorPosition(29, 11 + i);
                        Console.Write(scoreOne[i]);
                    }
                    break;
                case 2:
                    for (int i = 0; i < 5; i++)
                    {
                        Console.SetCursorPosition(29, 11 + i);
                        Console.Write(scoreTwo[i]);
                    }
                    break;
                case 3:
                    for (int i = 0; i < 5; i++)
                    {
                        Console.SetCursorPosition(29, 11 + i);
                        Console.Write(scoreThree[i]);
                    }
                    break;
                case 4:
                    for (int i = 0; i < 4; i++)
                    {
                        Console.SetCursorPosition(42, 11 + i);
                        Console.Write(youWin[i]);
                    }
                    timer.Enabled = false;
                    Thread.Sleep(2000);
                    for (int i = 0; i < anyKey.Length; i++)
                    {
                        Console.SetCursorPosition(47, 19);
                        Console.Write(anyKey[i]);
                    }
                    gameOn = false;
                    Win();
                    break;

            }

                Thread.Sleep(2000);
                Console.Clear();
                for (int i = 0; i < 8; i++)
                {
                    Console.SetCursorPosition(1, (playerTop + i));
                    Console.Write(asset[3]);
                }

                timer.Enabled = true;
        }
        //This tracks the monkey's score
        private static void MonkeyScore()
        {
            Console.Clear();
            timer.Enabled = false;

            Console.Beep(400, 200);
            Thread.Sleep(200);
            Console.Beep(200, 200);

            monkeyScore = monkeyScore + 1;

            playerTop = 11; playerBottom = 18; monkeyTop = 11; monkeyBottom = 18; monkeySpeed = 1; ballX = 60; ballY = 14; ballXSpeed = 1; ballYSpeed = 1; randBallSpeed = 0;

            randBallSpeed = generator.Next(1, 7);

            switch (randBallSpeed)
            {
                case 1:
                    ballXSpeed = 1;
                    ballYSpeed = 1;
                    break;
                case 2:
                    ballXSpeed = 1;
                    ballYSpeed = -1;
                    break;
                case 3:
                    ballXSpeed = 2;
                    ballYSpeed = 1;
                    break;
                case 4:
                    ballXSpeed = 1;
                    ballYSpeed = 2;
                    break;
                case 5:
                    ballXSpeed = 2;
                    ballYSpeed = -1;
                    break;
                case 6:
                    ballXSpeed = 1;
                    ballYSpeed = -2;
                    break;
            }

            switch (monkeyScore)
            {
                case 1:
                    for (int i = 0; i < 5; i++)
                    {
                        Console.SetCursorPosition(89, 11 + i);
                        Console.Write(scoreOne[i]);
                    }
                    break;
                case 2:
                    for (int i = 0; i < 5; i++)
                    {
                        Console.SetCursorPosition(89, 11 + i);
                        Console.Write(scoreTwo[i]);
                    }
                    break;
                case 3:
                    for (int i = 0; i < 5; i++)
                    {
                        Console.SetCursorPosition(89, 11 + i);
                        Console.Write(scoreThree[i]);
                    }
                    break;
                case 4:
                    for (int i = 0; i < 4; i++)
                    {
                        Console.SetCursorPosition(40, 11 + i);
                        Console.Write(youLose[i]);
                    }
                    timer.Enabled = false;
                    Thread.Sleep(2000);
                    for (int i = 0; i < anyKey.Length; i++)
                    {
                        Console.SetCursorPosition(47, 19);
                        Console.Write(anyKey[i]);
                    }
                    gameOn = false;
                    Lose();
                    break;
            }

                Thread.Sleep(2000);
                Console.Clear();
                for (int i = 0; i < 8; i++)
                {
                    Console.SetCursorPosition(1, (playerTop + i));
                    Console.Write(asset[3]);
                }

                timer.Enabled = true;
            

        }
        //This is queued if you win
        private static void Win()
        {
            Console.Beep(400, 100);
            Thread.Sleep(100);
            Console.Beep(200, 100);
            Thread.Sleep(100);
            Console.Beep(400, 100);
            Thread.Sleep(100);
            Console.Beep(600, 300);

            Console.SetCursorPosition(0, 0);
            string[] winner = { "[The monkey is outraged at its loss]", "[Before you can react, it grabs its glock and shoots you in the chest, causing you to fall into a deep slumber...]" };

            for (int i = 0; i < winner[0].Length; i++)
            {
                Console.Write(winner[0][i]);
                Thread.Sleep(15);
            }
            Console.WriteLine("");
            Thread.Sleep(500);
            Console.ForegroundColor = ConsoleColor.Red;
            for (int i = 0; i < winner[1].Length; i++)
            {
                Console.Write(winner[1][i]);
                Thread.Sleep(15);
            }
            Thread.Sleep(3000);
            Environment.Exit(0);
        }
        //This plays is queued if you lose
        private static void Lose()
        {
            Console.Beep(400, 100);
            Thread.Sleep(100);
            Console.Beep(200, 100);
            Thread.Sleep(100);
            Console.Beep(400, 100);
            Thread.Sleep(100);
            Console.Beep(100, 300);

            Console.SetCursorPosition(0, 0);
            string loser = "[Losing to a primate shatters your ego, and you fall into a deep depression...]";

            for (int i = 0; i < loser.Length; i++)
            {
                Console.Write(loser[i]);
                Thread.Sleep(15);
            }
            for (int i = 0; i < loser.Length; i++)
            {
                Console.SetCursorPosition(0, loser.Length - i);
                Console.Write(" ");
                Thread.Sleep(40);
            }

            Thread.Sleep(3000);
            Environment.Exit(0);
        }
        //This is the joke monkey's first joke
        private static void Joke1()
        {
            if (!joke1)
                jokeCount = jokeCount + 1;
            joke1 = true;

            string jokeAns1, jokeAns2, realAns1 = "who's there?", realAns2 = "alice who?";
            string[] joke = { "Knock, knock.", "Alice.", "Alice fair in love and war.", "[The monkey loads its glock]", "No, you're supposed to say \"Who's there?\"", "No, you're supposed to say \"Alice who?\"" };
            bool goodAns1 = false, goodAns2 = false;

            for (int i = 0; i < joke[0].Length; i++)
            {
                Console.Write(joke[0][i]);
                Thread.Sleep(15);
            }

            Console.WriteLine("");


            while (!goodAns1)
            {
                Console.SetCursorPosition(0, 53);
                jokeAns1 = Console.ReadLine().ToLower();
                if (jokeAns1 == realAns1)
                    goodAns1 = true;
                else
                {
                    for (int i = 0; i < joke[4].Length; i++)
                    {
                        Console.Write(joke[4][i]);
                        Thread.Sleep(15);
                    }
                }
            }

            Console.SetCursorPosition(0, 54);

            for (int i = 0; i < Console.WindowWidth; i++)
            {
                Console.Write(" ");
            }

            Console.SetCursorPosition(0, 54);

            for (int i = 0; i < joke[1].Length; i++)
            {
                Console.Write(joke[1][i]);
                Thread.Sleep(15);
            }


            while (!goodAns2)
            {
                Console.SetCursorPosition(0, 55);
                jokeAns2 = Console.ReadLine().ToLower();
                if (jokeAns2 == realAns2)
                    goodAns2 = true;
                else
                {
                    for (int i = 0; i < joke[5].Length; i++)
                    {
                        Console.Write(joke[5][i]);
                        Thread.Sleep(15);
                    }
                }
            }

            Console.SetCursorPosition(0, 56);

            for (int i = 0; i < Console.WindowWidth; i++)
            {
                Console.Write(" ");
            }

            Console.SetCursorPosition(0, 56);

            for (int i = 0; i < joke[2].Length; i++)
            {
                Console.Write(joke[2][i]);
                Thread.Sleep(15);
            }

            Console.WriteLine("");
            Thread.Sleep(500);

            for (int i = 0; i < joke[3].Length; i++)
            {
                Console.Write(joke[3][i]);
                Thread.Sleep(15);
            }
        }
        //This is the joke monkey's second joke
        private static void Joke2()
        {
            if (!joke2)
                jokeCount = jokeCount + 1;
            joke2 = true;

            string jokeAns1, jokeAns2, realAns1 = "who's there?", realAns2 = "who who?";
            string[] joke = { "Knock, knock.", "Who.", "What are you, an owl?", "[The monkey grabs a baby owl from the Pac-Man machine and eats it]", "No, you're supposed to say \"Who's there?\"", "No, you're supposed to say \"Who who?\"" };
            bool goodAns1 = false, goodAns2 = false;

            for (int i = 0; i < joke[0].Length; i++)
            {
                Console.Write(joke[0][i]);
                Thread.Sleep(15);
            }

            Console.WriteLine("");


            while (!goodAns1)
            {
                Console.SetCursorPosition(0, 53);
                jokeAns1 = Console.ReadLine().ToLower();
                if (jokeAns1 == realAns1)
                    goodAns1 = true;
                else
                {
                    for (int i = 0; i < joke[4].Length; i++)
                    {
                        Console.Write(joke[4][i]);
                        Thread.Sleep(15);
                    }
                }
            }

            Console.SetCursorPosition(0, 54);

            for (int i = 0; i < Console.WindowWidth; i++)
            {
                Console.Write(" ");
            }

            Console.SetCursorPosition(0, 54);

            for (int i = 0; i < joke[1].Length; i++)
            {
                Console.Write(joke[1][i]);
                Thread.Sleep(15);
            }


            while (!goodAns2)
            {
                Console.SetCursorPosition(0, 55);
                jokeAns2 = Console.ReadLine().ToLower();
                if (jokeAns2 == realAns2)
                    goodAns2 = true;
                else
                {
                    for (int i = 0; i < joke[5].Length; i++)
                    {
                        Console.Write(joke[5][i]);
                        Thread.Sleep(15);
                    }
                }
            }

            Console.SetCursorPosition(0, 56);

            for (int i = 0; i < Console.WindowWidth; i++)
            {
                Console.Write(" ");
            }

            Console.SetCursorPosition(0, 56);

            for (int i = 0; i < joke[2].Length; i++)
            {
                Console.Write(joke[2][i]);
                Thread.Sleep(15);
            }

            Console.WriteLine("");
            Thread.Sleep(500);

            for (int i = 0; i < joke[3].Length; i++)
            {
                Console.Write(joke[3][i]);
                Thread.Sleep(15);
            }
        }
        //This is the joke monkey's third joke
        private static void Joke3()
        {
            if (!joke3)
                jokeCount = jokeCount + 1;
            joke3 = true;

            string jokeAns1, jokeAns2, realAns1 = "who's there?", realAns2 = "abby who?";
            string[] joke = { "Knock, knock.", "Abby.", "Abby birthday to you!", "[The monkey lights its tail on fire and gestures to you to blow out the flame]", "No, you're supposed to say \"Who's there?\"", "No, you're supposed to say \"Abby who?\"" };
            bool goodAns1 = false, goodAns2 = false;

            for (int i = 0; i < joke[0].Length; i++)
            {
                Console.Write(joke[0][i]);
                Thread.Sleep(15);
            }

            Console.WriteLine("");


            while (!goodAns1)
            {
                Console.SetCursorPosition(0, 53);
                jokeAns1 = Console.ReadLine().ToLower();
                if (jokeAns1 == realAns1)
                    goodAns1 = true;
                else
                {
                    for (int i = 0; i < joke[4].Length; i++)
                    {
                        Console.Write(joke[4][i]);
                        Thread.Sleep(15);
                    }
                }
            }

            Console.SetCursorPosition(0, 54);

            for (int i = 0; i < Console.WindowWidth; i++)
            {
                Console.Write(" ");
            }

            Console.SetCursorPosition(0, 54);

            for (int i = 0; i < joke[1].Length; i++)
            {
                Console.Write(joke[1][i]);
                Thread.Sleep(15);
            }


            while (!goodAns2)
            {
                Console.SetCursorPosition(0, 55);
                jokeAns2 = Console.ReadLine().ToLower();
                if (jokeAns2 == realAns2)
                    goodAns2 = true;
                else
                {
                    for (int i = 0; i < joke[5].Length; i++)
                    {
                        Console.Write(joke[5][i]);
                        Thread.Sleep(15);
                    }
                }
            }

            Console.SetCursorPosition(0, 56);

            for (int i = 0; i < Console.WindowWidth; i++)
            {
                Console.Write(" ");
            }

            Console.SetCursorPosition(0, 56);

            for (int i = 0; i < joke[2].Length; i++)
            {
                Console.Write(joke[2][i]);
                Thread.Sleep(15);
            }

            Console.WriteLine("");
            Thread.Sleep(500);

            for (int i = 0; i < joke[3].Length; i++)
            {
                Console.Write(joke[3][i]);
                Thread.Sleep(15);
            }
        }
        //This is the joke monkey's fourth joke
        private static void Joke4()
        {
            if (!joke4)
                jokeCount = jokeCount + 1;
            joke4 = true;

            string jokeAns1, jokeAns2, realAns1 = "who's there?", realAns2 = "lena who?";
            string[] joke = { "Knock, knock.", "Lena.", "Lena little closer and I'll tell you!", "[The monkey holds up four fingers, then gestures towards the blank machine, and then the joke counter on the wall.]", "No, you're supposed to say \"Who's there?\"", "No, you're supposed to say \"Alice who?\"", "[It reads: " };
            bool goodAns1 = false, goodAns2 = false;

            for (int i = 0; i < joke[0].Length; i++)
            {
                Console.Write(joke[0][i]);
                Thread.Sleep(15);
            }

            Console.WriteLine("");


            while (!goodAns1)
            {
                Console.SetCursorPosition(0, 53);
                jokeAns1 = Console.ReadLine().ToLower();
                if (jokeAns1 == realAns1)
                    goodAns1 = true;
                else
                {
                    for (int i = 0; i < joke[4].Length; i++)
                    {
                        Console.Write(joke[4][i]);
                        Thread.Sleep(15);
                    }
                }
            }

            Console.SetCursorPosition(0, 54);

            for (int i = 0; i < Console.WindowWidth; i++)
            {
                Console.Write(" ");
            }

            Console.SetCursorPosition(0, 54);

            for (int i = 0; i < joke[1].Length; i++)
            {
                Console.Write(joke[1][i]);
                Thread.Sleep(15);
            }


            while (!goodAns2)
            {
                Console.SetCursorPosition(0, 55);
                jokeAns2 = Console.ReadLine().ToLower();
                if (jokeAns2 == realAns2)
                    goodAns2 = true;
                else
                {
                    for (int i = 0; i < joke[5].Length; i++)
                    {
                        Console.Write(joke[5][i]);
                        Thread.Sleep(15);
                    }
                }
            }

            Console.SetCursorPosition(0, 56);

            for (int i = 0; i < Console.WindowWidth; i++)
            {
                Console.Write(" ");
            }

            Console.SetCursorPosition(0, 56);

            for (int i = 0; i < joke[2].Length; i++)
            {
                Console.Write(joke[2][i]);
                Thread.Sleep(15);
            }

            Console.WriteLine("");
            Thread.Sleep(500);

            for (int i = 0; i < joke[3].Length; i++)
            {
                Console.Write(joke[3][i]);
                Thread.Sleep(15);
            }

            Console.WriteLine("");
            Thread.Sleep(200);

            for (int i = 0; i < joke[6].Length; i++)
            {
                Console.Write(joke[6][i]);
                Thread.Sleep(15);
            }

            Console.Write(jokeCount);
            Thread.Sleep(15);
            Console.Write("]");
        }
        //This is the first ascii - the one from my first assignment
        private static void PacManASCII()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Clear();

            string[] pacMan = { "[The machine doesn't turn on, and the outline of several baby owls can be seen through the screen]", "[All the artwork has peeled off, except for the right side of the machine, which displays the titular Pac-Man]" };

            for (int i = 0; i < pacMan[0].Length; i++)
            {
                Console.Write(pacMan[0][i]);
                Thread.Sleep(15);
            }

            Console.WriteLine("");
            Console.WriteLine("");

            Thread.Sleep(500);

            for (int i = 0; i < pacMan[1].Length; i++)
            {
                Console.Write(pacMan[1][i]);
                Thread.Sleep(15);
            }

            Console.WriteLine("");
            Console.WriteLine("");

            Console.WriteLine("-----####------------------------------------------");
            Console.WriteLine("----#######---------------------####---------------");
            Console.WriteLine("---##########-----------------########-------------");
            Console.WriteLine("--############---------------##########------------");
            Console.WriteLine("-#############--------------###--####--#-----------");
            Console.WriteLine("############----------------##----##---------------");
            Console.WriteLine("##########------------------##--88##--88-----------");
            Console.WriteLine("########-------------------###--88##--88#----------");
            Console.WriteLine("######----O--O--O--O--O--O-####--####--##--O--O--O-");
            Console.WriteLine("########-------------------##############----------");
            Console.WriteLine("##########-----------------##############----------");
            Console.WriteLine("-###########---------------##############----------");
            Console.WriteLine("--############-------------##############----------");
            Console.WriteLine("---###########-------------##-###--###-##----------");
            Console.WriteLine("----#########--------------#---##--##---#----------");
            Console.WriteLine("-----######----------------------------------------");
            Console.WriteLine("pac man pac man pac man pac man pac man pac man pac");

            Thread.Sleep(5000);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Clear();
            visited = true;
        }
        //This is the second ascii, and it also is the method that contains the joke prompt
        private static void MonkeyASCII()
        {
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();
            string[] monkeyASCII = {
"                                  ██████                                                        ",
"                              ████▒▒▒▒▒▒████                                                    ",
"                          ██▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓██                                                ",
"                        ██▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒██                                              ",
"                      ██▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒██                                            ",
"                      ██▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒██                                            ",
"                    ██▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒██              ██        ██                ",
"                    ██▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒██            ██  ██    ██  ██              ",
"                    ██████▒▒▒▒▒▒████████▒▒▒▒▒▒▒▒▒▒▒▒██            ██  ██    ██  ██              ",
"                    ██    ██▒▒██  ░░    ██▒▒▒▒▒▒▒▒▒▒██            ██  ████████  ██              ",
"                    ██      ██          ██▒▒██████▒▒██            ████████  ██  ██              ",
"                    ██  ██        ██    ██▒▒██  ██▒▒██          ██      ██  ██  ██              ",
"                    ██  ▓▓██    ██▓▓    ██▒▒██  ██▒▒██          ██  ████████  ██                ",
"                    ██  ░░░░    ░░░░    ██▒▒██▓▓▒▒██            ░░▓▓░░░░░░░░  ██                ",
"                    ██████        ██████▒▒▒▒▒▒▒▒▒▒██              ████  ██  ██                  ",
"                    ██▒▒██  ████  ██▒▒▒▒▒▒▒▒▒▒▒▒██                ██▒▒██▒▒██                    ",
"                    ████            ██▒▒▒▒▒▒▒▒▒▒██              ██▒▒▒▒▒▒▒▒▒▒██                  ",
"            ██████  ██    ▓▓▓▓▓▓▓▓    ██▒▒▒▒▒▒██                ██▒▒▒▒▒▒▒▒▒▒██                  ",
"          ▓▓▒▒▒▒▒▒▓▓  ▓▓  ▓▓▓▓▓▓▓▓    ██▒▒████                ▓▓▒▒▒▒▒▒▒▒▒▒██                    ",
"        ▓▓██▓▓▒▒▒▒██  ██  ▓▓▓▓▓▓▓▓    ████▒▒▒▒▓▓██▓▓▓▓▓▓▓▓▓▓████▒▒▒▒▒▒▒▒▒▒██                    ",
"      ▓▓░░░░░░██▒▒██    ▓▓░░░░░░░░  ▓▓██▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒██                      ",
"    ██        ████        ██████████▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒██                      ",
"    ██▓▓▓▓  ▓▓░░░░          ██▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒██░░        ▓▓▓▓▓▓▓▓▓▓▓▓  ",
"    ██▒▒▒▒██▒▒██░░  ░░░░  ██▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒██  ░░░░  ▓▓██▒▒▒▒▒▒▒▒▒▒▒▒██",
"  ▒▒▓▓▒▒▓▓▒▒▒▒▒▒▒▒      ▒▒▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓██▓▓▓▓▓▓▓▓▓▓░░░░    ▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒██",
"▒▒▓▓▒▒▓▓▓▓▒▒▒▒▒▒▓▓▒▒  ▒▒▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒██  ▒▒░░    ░░░░    ██▒▒▒▒▒▒▓▓██▒▒██▓▓░░",
"██████  ▓▓▒▒▒▒▒▒▒▒▒▒▓▓▒▒▒▒▒▒▒▒▒▒▒▒▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒██  ░░░░  ░░░░  ░░▓▓▒▒▒▒▒▒▓▓░░░░▓▓      ",
"          ██▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒██  ██▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓              ██▒▒▒▒██              ",
"            ██▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒██    ██▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒██              ██▒▒▒▒██              ",
"              ██▒▒▒▒▒▒▒▒▒▒▒▒██        ██▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒██            ██▒▒▒▒██              ",
"                ██▒▒▒▒▒▒▒▒██          ██▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒██          ██▒▒▒▒▒▒██              ",
"                  ▓▓▓▓▓▓▓▓░░          ██▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒██        ██▒▒▒▒▓▓░░              ",
"                                    ██▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒██      ██▒▒▒▒▒▒██                ",
"                                  ▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒██▓▓██░░▒▒▒▒▒▒██░░                ",
"                              ████▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒██▒▒▒▒▒▒▒▒▒▒▒▒██                  ",
"                            ██▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒██▒▒▒▒▒▒▒▒▒▒██                    ",
"                        ▓▓██▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒████████████                      ",
"                    ████▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒██▒▒▒▒▒▒▒▒▒▒▒▒██                                ",
"                  ██▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒██████████▒▒▒▒▒▒▒▒▒▒▒▒██                                ",
"                ██▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒██████        ██▒▒▒▒▒▒▒▒▒▒▒▒████████                          ",
"              ██▒▒▒▒▒▒▒▒▒▒▒▒▒▒████              ██▒▒▒▒▒▒▒▒▒▒▒▒██▒▒▒▒▒▒██                        ",
"          ██▒▒▒▒▒▒▒▒▒▒▒▒██                        ██▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒██  ██                      ",
"        ▓▓▒▒▒▒▒▒▒▒▒▒▒▒██                            ██▒▒▒▒▒▒▒▒▒▒▒▒▒▒██  ██                      ",
"      ██▒▒▒▒▒▒▒▒▒▒▒▒██                                ██████████████    ██                      ",
"  ██████▓▓██████▒▒██░░                                  ░░░░▒▒░░  ██    ██                      ",
"██  ░░  ░░░░░░  ██                                              ██      ██                      ",
"██▓▓▓▓▓▓██████▓▓░░                                              ██▓▓▓▓▓▓░░                      " };

            string[] monkey = { "[A monkey can be seen through the screen of the machine.  It smiles]", "I'm the joke monkey!  Would you like to hear a joke? (Y/N): " };
            string[] response = { "What a shame...", "That is not a valid response." };

            for (int i = 0; i < monkey[0].Length; i++)
            {
                Console.Write(monkey[0][i]);
                Thread.Sleep(15);
            }

            Console.WriteLine("");
            Console.WriteLine("");
            Thread.Sleep(500);

            for (int i = 0; i < monkey[1].Length; i++)
            {
                Console.Write(monkey[1][i]);
                Thread.Sleep(15);
            }

            Console.WriteLine("");
            Console.WriteLine("");

            for (int i = 0; i < monkeyASCII.Length; i++)
            {
                Console.WriteLine(monkeyASCII[i]);
            }



            bool properAns = false, jokeWanted = false;

            while (!properAns)
            {
                Console.SetCursorPosition(60, 2);
                Console.ForegroundColor = ConsoleColor.White;
                Console.CursorVisible = true;
                string answer = Console.ReadLine();
                Console.CursorVisible = false;
                int verifY = answer.ToLower().IndexOf("y"), verifN = answer.ToLower().IndexOf("n");

                if (verifY == 0)
                {
                    jokeWanted = true;
                    properAns = true;
                }
                else if (verifN == 0)
                {
                    for (int i = 0; i < response[0].Length; i++)
                    {
                        Console.Write(response[0][i]);
                        Thread.Sleep(15);
                    }
                    properAns = true;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    for (int i = 0; i < response[1].Length; i++)
                    {
                        Console.Write(response[1][i]);
                        Thread.Sleep(15);
                    }
                }
            }



            //move cursor beneath the monkey ascii

            if (jokeWanted)
            {
                Console.SetCursorPosition(0, 52);

                //put jokes in the if statements

                Random generator = new Random();
                int jokeNum = generator.Next(1, 5);
                string noJokes = ("You've already heard all my jokes!");
                bool allJokes = false;
                if (joke1 && joke2 && joke3 && joke4)
                {
                    allJokes = true;
                }

                if (!allJokes)
                {
                    if (joke1)
                    {
                        while (jokeNum == 1)
                        {
                            jokeNum = generator.Next(1, 5);
                        }
                    }

                    if (joke2)
                    {
                        while (jokeNum == 2)
                        {
                            jokeNum = generator.Next(1, 5);
                        }
                    }

                    if (joke3)
                    {
                        while (jokeNum == 3)
                        {
                            jokeNum = generator.Next(1, 5);
                        }
                    }

                    if (joke4)
                    {
                        while (jokeNum == 4)
                        {
                            jokeNum = generator.Next(1, 5);
                        }
                    }

                    if (jokeNum == 1)
                    {
                        Joke1();
                    }

                    else if (jokeNum == 2)
                    {
                        Joke2();
                    }

                    else if (jokeNum == 3)
                    {
                        Joke3();
                    }

                    else
                    {
                        Joke4();
                    }
                }
                else
                {
                    for (int i = 0; i < noJokes.Length; i++)
                    {
                        Console.Write(noJokes[i]);
                        Thread.Sleep(15);
                    }
                }
            }

            Thread.Sleep(5000);
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Clear();
            visited = true;

        }
        //This is a red herring.  You need to hear all of the joke monkey's jokes to access the final ascii (the pong game)
        private static void blankASCII()
        {
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Clear();

            string[] blank = { "[A blank arcade machine]", "[It doesn't seem like there's anything to do...]", "...oh?" };

            for (int i = 0; i < blank[0].Length; i++)
            {
                Console.Write(blank[0][i]);
                Thread.Sleep(15);
            }

            Console.WriteLine("");
            Thread.Sleep(200);

            for (int i = 0; i < blank[1].Length; i++)
            {
                Console.Write(blank[1][i]);
                Thread.Sleep(15);
            }

            Console.WriteLine("");

            if (jokeCount >= 4)
            {
                Console.WriteLine("");
                Console.WriteLine("");
                Thread.Sleep(2000);

                for (int i = 0; i < blank[2].Length; i++)
                {
                    Console.Write(blank[2][i]);
                    Thread.Sleep(15);
                }

                Console.WriteLine("");
                Thread.Sleep(1000);

                PongGame();
            }

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;

            Thread.Sleep(5000);

            Console.Clear();
        }
        //This leaves the program
        private static void Leave()
        {
            string leave = "[The sound of bells jingling could be heard as you exited the building]";

            Console.Clear();

            for (int i = 0; i < leave.Length; i++)
            {
                Console.Write(leave[i]);
                Thread.Sleep(15);
            }

            Thread.Sleep(5000);
            Environment.Exit(0);

        }
        //The main menu
        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            LockWindowSize();

            string intro = "[After escaping from the crow, you stumble across an abandoned building]";
            string enter = "[You open the door, and find nothing bar three arcade machines coated in dust]";
            string decision = "What would you like to do?";
            string[] option = { "1. Visit the Pac-Man arcade machine", "2. Visit the monkey arcade machine", "3. Visit the blank arcade machine", "4. Leave the building" };

            if (!visited)
            {
                for (int i = 0; i < intro.Length; i++)
                {
                    Console.Write(intro[i]);
                    Thread.Sleep(15);
                }

                Thread.Sleep(500);
                Console.WriteLine("");
                Console.WriteLine("");

                for (int i = 0; i < enter.Length; i++)
                {
                    Console.Write(enter[i]);
                    Thread.Sleep(15);
                }

                Thread.Sleep(500);
                Console.WriteLine("");
                Console.WriteLine("");


                for (int i = 0; i < decision.Length; i++)
                {
                    Console.Write(decision[i]);
                    Thread.Sleep(15);
                }

                Thread.Sleep(200);
                Console.WriteLine("");
                Console.WriteLine("");

                string[] choice = { "1", "2", "3", "4" };
                string userChoice, choiceAsk = "Choice (#): ", invalid = "That is not a valid choice.";
                bool chosen = false;

                while (!chosen)
                {
                    Console.BackgroundColor = ConsoleColor.Black;

                    int j;
                    bool quieres = true;

                    if (!visited)
                    {
                        j = 0;
                    }
                    else
                    {
                        j = 7;
                    }

                    while (quieres)
                    {

                        for (int i = 0; i < option[0].Length; i++)
                        {

                            if (i < option[0].Length)
                            {
                                Console.SetCursorPosition(0 + i, 7 - j);
                                Console.Write(option[0][i]);
                            }

                            if (i < option[1].Length)
                            {
                                Console.SetCursorPosition(37 + i, 7 - j);
                                Console.Write(option[1][i]);
                            }

                            if (i < option[2].Length)
                            {
                                Console.SetCursorPosition(0 + i, 9 - j);
                                Console.Write(option[2][i]);
                            }

                            if (i < option[3].Length)
                            {
                                Console.SetCursorPosition(37 + i, 9 - j);
                                Console.Write(option[3][i]);
                            }

                            Thread.Sleep(15);

                            if (i == 34) quieres = false;

                        }
                        //make methods for each option, have them listed in order in a for loop, set cursor coordinates to coordinate + i
                    }
                    Console.SetCursorPosition(0, 11 - j);

                    Thread.Sleep(200);

                    Console.ForegroundColor = ConsoleColor.Gray;

                    for (int i = 0; i < choiceAsk.Length; i++)
                    {
                        Console.Write(choiceAsk[i]);
                        Thread.Sleep(15);
                    }

                    Console.CursorVisible = true;

                    userChoice = Console.ReadLine();

                    Console.CursorVisible = false;

                    if (userChoice == choice[0])
                    {
                        PacManASCII();
                    }

                    else if (userChoice == choice[1])
                    {
                        MonkeyASCII();
                    }

                    else if (userChoice == choice[2])
                    {
                        blankASCII();
                    }

                    else if (userChoice == choice[3])
                    {
                        Leave();
                        chosen = true;
                    }

                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        for (int i = 0; i < invalid.Length; i++)
                        {
                            Console.Write(invalid[i]);
                            Thread.Sleep(15);
                        }
                    }
                }
            }

            Console.WriteLine("done");


        }
        //This locks the window size.  Can't be having people go too crazy...
        private static void LockWindowSize()
        {
            Console.WindowHeight = 30;
            Console.WindowWidth = 121;

            DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_MINIMIZE, MF_BYCOMMAND);
            DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_MAXIMIZE, MF_BYCOMMAND);
            DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_SIZE, MF_BYCOMMAND);
        }
    }
}

