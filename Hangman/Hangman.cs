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
        public static void Play()
        {
            StringBuilder incorrectGuesses = new StringBuilder();
            int lives = 10;
            string secretWord = PickSecretFromList(ReadWordFromFile(path));
            char[] correctGuesses = Enumerable.Repeat('_', secretWord.Length).ToArray();
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
                            incorrectGuesses.Append(guess);
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
                WriteLine("You Loose.");
        }

        private static void PrintProgress(
            int lives, 
            char[] correctGuesses,
            StringBuilder incorrectGuesses)
        {
            Clear();
            WriteLine("Play Hangman\n");
            Write(DrawHangman(lives));
            WriteLine($"You have {lives} lives left.\n");
            WriteLine($"{string.Join(" ", correctGuesses)}\n");
            WriteLine("Guessed wrong so far: {0}\n",
                string.Join(", ", incorrectGuesses.ToString().ToArray()));
        }

        /**
         * 
         */
        private static string DrawHangman(int lives)
        {
            //    0    1    2    3    4    5    6    7    8    9    10
            // { ' ', ' ', ' ', '-', '-', '-', '-', '-', ' ', ' ', '\n' } 0
            // { ' ', ' ', ' ', '|', ' ', ' ', '\', '|', ' ', ' ', '\n' } 1 
            // { ' ', ' ', ' ', 'O', ' ', ' ', ' ', '|', ' ', ' ', '\n' } 2
            // { ' ', ' ', '/', '|', '\', ' ', ' ', '|', ' ', ' ', '\n' } 3
            // { ' ', ' ', '/', ' ', '\', ' ', ' ', '|', ' ', ' ', '\n' } 4
            // { ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', ' ', '\n' } 5
            // { '~', '~', '~', '~', '~', '~', '~', '~', '~', '~', '\n' } 6

            StringBuilder sb = new StringBuilder();
            sb.Append(new String(' ', 66));
            sb.Append(new String('~', 11));

            for (int i = 10; i < sb.Length; i += 11)
            {
                sb[i] = '\n';
            }

            char[] a1 = sb.ToString().ToArray();
            char[,] hanging = new char[7, 11];
            Buffer.BlockCopy(
                a1, 0, 
                hanging, 0, 
                77 * sizeof(char));

            //for (int i = 0; i < hanging.GetLength(0); i++)
            //{
            //    hanging[i, 10] = '\n';
            //}

            if (lives < 1) hanging[4, 4] = '\\';
            if (lives < 2) hanging[4, 2] = '/';
            if (lives < 3) hanging[3, 4] = '\\';
            if (lives < 4) hanging[3, 2] = '/';
            if (lives < 5) hanging[3, 3] = '|';
            if (lives < 6) hanging[2, 3] = 'O';
            if (lives < 7) hanging[1, 3] = '|';
            if (lives < 8)
            {
                hanging[0, 3] = '_';
                hanging[0, 4] = '_';
                hanging[0, 5] = '_';
                hanging[0, 6] = '_';
                hanging[0, 7] = '_';
            }
            if (lives < 9) hanging[1, 6] = '\\';
            if (lives < 10)
            {
                hanging[1, 7] = '|';
                hanging[2, 7] = '|';
                hanging[3, 7] = '|';
                hanging[4, 7] = '|';
                hanging[5, 7] = '|';
            }

            return String.Join("", hanging.Cast<char>().ToArray()).ToString();
        }

        private static string PickSecretFromList(string[] wordlist)
        {
            return wordlist[random.Next(0, wordlist.Length)];
        }

        /**
         * Read a word from a comma separated text file.
         */
        private static string[] ReadWordFromFile(string path)
        {
            StringBuilder word = new StringBuilder();
            
            try
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    long streamSize = sr.BaseStream.Length;
                    int offset = random.Next(0, (int)streamSize);
                    
                    // seek to random offset
                    sr.BaseStream.Seek(offset, SeekOrigin.Begin);

                    // go to next comma
                    while (!sr.EndOfStream && (char)sr.Read() != ',') {  }

                    // if eos, go to beginning of stream (first word)
                    if (sr.EndOfStream)
                        sr.BaseStream.Seek(0, SeekOrigin.Begin);

                    // read characters until next comma
                    while (!sr.EndOfStream && (char)sr.Peek() != ',')
                        word.Append((char)sr.Read());
                }
            }
            catch (Exception ex)
            {
                Error.WriteLine(ex.Message);
            }

            return new string[] { word.ToString() };
        }

        private static readonly string path = Environment.CurrentDirectory + "\\wordlist.txt";
        private static readonly Random random = new Random();
    }
}
