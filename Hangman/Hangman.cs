using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static System.Console;

namespace Hangman
{
    public class Hangman
    {
        static Hangman()
        {
            InitializeDrawHangman();
        }

        public static void Play()
        {
            StringBuilder incorrectGuesses = new StringBuilder();
            int lives = 10;
            string[] wordList = ReadWordFromFile(path);
            string secretWord = PickSecretFromList(wordList);
            char[] correctGuesses = 
                Enumerable.Repeat('_', secretWord.Length).ToArray();
            string guess = "";

            while (lives > 0)
            {
                PrintProgress(lives, correctGuesses, incorrectGuesses);

                Write("Guess a letter or a word: ");
                guess = ReadLine().ToLower();

                if (guess.Length > 1)
                {
                    if (guess.Equals(secretWord))
                    {
                        correctGuesses = secretWord.ToCharArray();
                        break;
                    }
                    else
                        lives--;
                }
                else
                {
                    bool correctGuess = false;
                    for (int i = 0; i < secretWord.Length; i++)
                        if (secretWord[i].ToString().Equals(guess))
                        {
                            correctGuesses[i] = secretWord[i];
                            correctGuess = true;
                        }

                    if (!correctGuess)
                    {
                        if (!incorrectGuesses.ToString().Contains(guess))
                        {
                            incorrectGuesses.Append(
                                (incorrectGuesses.Length > 0 ? ", " : "")
                                + guess);
                            lives--;
                        }
                    }
                }

                if (!string.Join("", correctGuesses).Contains("_"))
                    break;
            }

            PrintProgress(lives, correctGuesses, incorrectGuesses);
            if (lives > 0)
                WriteLine("You Win.");
            else
                WriteLine("You Loose. (The word was \"{0}\").", secretWord);
        }

        private static void PrintProgress(
            int lives, 
            char[] correctGuesses,
            StringBuilder incorrectGuesses)
        {
            Clear();
            WriteLine("Play Hangman\n");
            DrawHangman(lives);
            WriteLine($"You have {lives} lives left.\n");
            WriteLine($"{string.Join(" ", correctGuesses)}\n");
            WriteLine("Guessed wrong so far: {0}\n", 
                incorrectGuesses.ToString());
        }

        private static void InitializeDrawHangman()
        {
            //       0    1    2    3    4    5    6    7    8    9    10
            //  0 { ' ', ' ', ' ', '-', '-', '-', '-', '-', ' ', ' ', '\n' } 0, 11
            // 12 { ' ', ' ', ' ', '|', ' ', ' ', '\', '|', ' ', ' ', '\n' } 1, 22
            // 23 { ' ', ' ', ' ', 'O', ' ', ' ', ' ', '|', ' ', ' ', '\n' } 2, 33
            // 34 { ' ', ' ', '/', '|', '\', ' ', ' ', '|', ' ', ' ', '\n' } 3, 44
            // 45 { ' ', ' ', '/', ' ', '\', ' ', ' ', '|', ' ', ' ', '\n' } 4, 55
            // 56 { ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', ' ', '\n' } 5, 66
            // 67 { '~', '~', '~', '~', '~', '~', '~', '~', '~', '~', '\n' } 6, 77
            //       1    2    3    4    5    6    7    8    9    10   11

            hanging = new StringBuilder();
            hanging.Append(new String(' ', 66));
            hanging.Append(new String('~', 11));

            for (int i = 10; i < hanging.Length; i += 11)
                hanging[i] = '\n';
        }

        private static void DrawHangman(int lives)
        {
            if (lives < 1) hanging[48] = '\\';
            if (lives < 2) hanging[46] = '/';
            if (lives < 3) hanging[37] = '\\';
            if (lives < 4) hanging[35] = '/';
            if (lives < 5) hanging[36] = '|';
            if (lives < 6) hanging[25] = 'O';
            if (lives < 7) hanging[14] = '|';
            if (lives < 8)
            {
                hanging[3] = '_';
                hanging[4] = '_';
                hanging[5] = '_';
                hanging[6] = '_';
                hanging[7] = '_';
            }
            if (lives < 9) hanging[17] = '\\';
            if (lives < 10)
            {
                hanging[18] = '|';
                hanging[29] = '|';
                hanging[40] = '|';
                hanging[51] = '|';
                hanging[62] = '|';
            }

            WriteLine(hanging.ToString());
        }

        private static string PickSecretFromList(string[] wordlist)
        {
            return wordlist[random.Next(0, wordlist.Length)];
        }

        private static string[] ReadWordFromFile(string path)
        {
            StringBuilder word = new StringBuilder();
            
            try
            {
                using StreamReader sr = new StreamReader(path);
                long streamSize = sr.BaseStream.Length;
                int offset = random.Next(0, (int)streamSize);

                // seek to random offset
                sr.BaseStream.Seek(offset, SeekOrigin.Begin);

                // go to next comma
                while (!sr.EndOfStream && (char)sr.Read() != ',') { }

                // if eos, go to beginning of stream (first word)
                if (sr.EndOfStream)
                    sr.BaseStream.Seek(0, SeekOrigin.Begin);

                // read characters until next comma
                while (!sr.EndOfStream && (char)sr.Peek() != ',')
                    word.Append((char)sr.Read());
            }
            catch (Exception ex)
            {
                Error.WriteLine(ex.Message);
            }

            return new string[] { word.ToString() };
        }

        private static StringBuilder hanging;
        private static readonly string path = 
            Environment.CurrentDirectory + "\\wordlist.txt";
        private static readonly Random random = new Random();
    }
}
