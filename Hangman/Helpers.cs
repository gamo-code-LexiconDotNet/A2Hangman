using System;
using System.Linq;
using static System.Console;

namespace Hangman
{
    public static partial class Hangman
    {
        private static readonly Random random = new Random();

        private static bool AskConfirmation(string message = "Confirm? (y/n): ", string option = "y")
        {
            Write(message);
            return IsConfirmed(ReadLine().ToString(), option);
        }

        private static bool IsConfirmed(string response, string option)
        {
            return response.ToLower().Equals(option);
        }

        private static string PickFromList(string[] wordList)
        {
            return wordList[random.Next(wordList.Length)];
        }
    }
}
