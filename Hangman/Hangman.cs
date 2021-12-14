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
            string[] wordList = ReadWordFromFile(path);
            string secretWord = PickSecretFromList(wordList);
            char[] correctGuesses = 
                Enumerable.Repeat('_', secretWord.Length).ToArray();

            // game loop
            while (lives > 0)
            {
                PrintProgress(lives, correctGuesses, incorrectGuesses);
                Write("Guess a letter or a word (0 to quit, ? for hint): ");
                string guess = ReadLine().ToLower();

                // special or incorrect input handling
                if (guess.Equals("0"))
                {
                    PrintProgress(lives, correctGuesses, incorrectGuesses);
                    return;
                }
                else if (guess.Equals("?"))
                {
                    PrintProgress(lives, correctGuesses, incorrectGuesses);
                    if (AskConfirmation("Are you sure you want a hint, " +
                        "it will cost you 2 lifes? (y/n): "))
                        guess = GetHint(secretWord, correctGuesses, ref lives);
                    else
                        continue;
                }
                else if (int.TryParse(guess, out _))
                    continue;

                // game logic
                if (guess.Length > 1)
                {
                    if (guess.Equals(secretWord))
                    {
                        correctGuesses = secretWord.ToCharArray();
                        break;
                    }
                    else if(!CommaSeparatedStringContains(
                        incorrectGuesses.ToString(), guess))
                    {
                        incorrectGuesses.Append(
                                (incorrectGuesses.Length > 0 ? ", " : "")
                                + guess);
                        lives--;
                    }
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
                        if (!CommaSeparatedStringContains(
                            incorrectGuesses.ToString(), guess))
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

            // end of game summary
            PrintProgress(lives, correctGuesses, incorrectGuesses);
            if (lives > 0)
                WriteLine("You Win.");
            else
                WriteLine("You Loose. (The word was \"{0}\").", secretWord);
        }

        private static bool AskConfirmation(string message = "Confirm? (y/n): ")
        {
            Write(message);
            return ReadLine().ToString().ToLower().Equals("y");
        }

        private static bool CommaSeparatedStringContains(string a, string b)
        {
            string[] aa = a.Split(",");
            for (int i = 0; i < aa.Length; i++)
                if (aa[i].Equals(b))
                    return true;
            return false;
        }

        private static string GetHint(
            string secretWord, 
            char[] correctGuesses,
            ref int lives)
        {
            for (int i = 0; i < secretWord.Length; i++)
                if (!correctGuesses.Contains(secretWord[i]))
                {
                    lives -= 2;
                    return secretWord[i].ToString();
                }

            return "";
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
                incorrectGuesses.ToString());
        }

        private static string DrawHangman(int lives)
        {
            StringBuilder hanging = new StringBuilder();
            hanging.Append(new String(' ', 66));
            hanging.Append(new String('~', 11));

            for (int i = 10; i < hanging.Length; i += 11)
                hanging[i] = '\n';

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

            return hanging.ToString();
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

        private static readonly string path = 
            Environment.CurrentDirectory + "\\wordlist.txt";
        private static readonly Random random = new Random();
    }
}
