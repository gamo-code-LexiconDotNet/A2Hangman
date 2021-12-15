using System;
using static System.Console;

namespace Hangman
{
    public class Program
    {
        static void Main(string[] args)
        {
            do
            {
                Hangman.Play();
                Write("\n\tDo you want to play again (y/n)?: ");
            } while (ReadLine().ToString().ToLower().Equals("y"));
        }
    }
}
