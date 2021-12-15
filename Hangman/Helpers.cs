using System;
using static System.Console;

namespace Hangman
{
    public static partial class Hangman
    {
        private static readonly Random random = new Random();

        private static bool AskConfirmation(string message = "Confirm? (y/n): ")
        {
            Write(message);
            return ReadLine().ToString().ToLower().Equals("y");
        }

        private static bool CommaSeparatedStringContains(string csv, string str)
        {
            string[] csva = csv.Split(",");
            for (int i = 0; i < csva.Length; i++)
                if (csva[i].Equals(str))
                    return true;
            return false;
        }

        private static string PickFromList(string[] wordList)
        {
            return wordList[random.Next(wordList.Length)];
        }
    }
}
