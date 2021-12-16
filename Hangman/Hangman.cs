using System;
using System.Linq;
using System.Text;
using static System.Console;

namespace Hangman
{
    public static partial class Hangman
    {
        public static void Play()
        {
            StringBuilder incorrectGuesses = new StringBuilder();
            int lives = 10;
            string[] wordList = new string[] { ReadWordFromFile(path) };
            string secretWord = PickFromList(wordList);
            char[] correctGuesses =
                Enumerable.Repeat('_', secretWord.Length).ToArray();

            // game loop
            while (lives > 0)
            {
                PrintProgress(lives, correctGuesses, incorrectGuesses);
                Write("Guess a letter or a word (0 to quit{0}: ", lives > 2 ? ", ? for hint)" : ")" );
                string guess = ReadLine().ToLower();

                // special or incorrect input handling
                if (guess.Equals("0"))
                {
                    PrintProgress(lives, correctGuesses, incorrectGuesses);
                    return;
                }
                else if (guess.Equals("?") && lives > 2)
                {
                    PrintProgress(lives, correctGuesses, incorrectGuesses);

                    if (AskConfirmation("Are you sure you want a hint, " +
                        "it will cost you 2 lifes? (y/n): "))
                        guess = GetHint(secretWord, correctGuesses, ref lives);
                    else
                        continue;
                }
                else if (!guess.All(char.IsLetter))
                    continue;

                if (!GameLogic(guess, ref lives, secretWord,
                               ref correctGuesses, ref incorrectGuesses))
                    break;
            }

            // end of game summary
            PrintProgress(lives, correctGuesses, incorrectGuesses);
            if (lives > 0)
                WriteLine("You Win.");
            else
                WriteLine("You Loose. (The word was \"{0}\").", secretWord);
        }

        private static bool GameLogic(
            string guess, ref int lives, string secretWord, 
            ref char[] correctGuesses, ref StringBuilder incorrectGuesses)
         {
            if (guess.Length > 1)
            {
                if (guess.Equals(secretWord))
                {
                    correctGuesses = secretWord.ToCharArray();
                    return false;
                }
                else if (!incorrectGuesses.ToString().Split(',').Contains(guess))
                {
                    incorrectGuesses.Append(
                            (incorrectGuesses.Length > 0 ? "," : "")
                            + guess);
                    lives--;
                }
            }
            else
            {
                if (incorrectGuesses.ToString().Split(',').Contains(guess))
                    return true;

                bool correct = false;
                for (int i = 0; i < secretWord.Length; i++)
                    if (secretWord[i].ToString().Equals(guess))
                    {
                        correctGuesses[i] = secretWord[i];
                        correct = true;
                    }

                if (!correct)
                {
                    incorrectGuesses.Append(
                        (incorrectGuesses.Length > 0 ? "," : "")
                        + guess);
                    lives--;
                }
            }

            if (!string.Join("", correctGuesses).Contains("_"))
                return false;

            return true;
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
                string.Join(", ", incorrectGuesses.ToString().Split(',')));
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
    }
}
