using System;
using System.Reflection;
using Xunit;
using System.IO;
using System.Text;
using System.Linq;

namespace Hangman.Tests
{
    public class HangmanShould
    {
        [Theory]
        [InlineData("x", 10, "secret", "______", "", true)]
        [InlineData("t", 10, "secret", "______", "", true)]
        [InlineData("secret", 10, "secret", "______", "", false)]
        [InlineData("c", 10, "secret", "se_ret", "", false)]
        public void PlayLogically(string guess, int lives, string secretWord,
            string cG, string iG, bool expected)
        {
            // Assemble
            char[] correctGuesses = cG.ToCharArray();
            StringBuilder incorrectGuesses = new StringBuilder(iG);
            object[] arguments = new object[] 
            { 
                guess,
                lives,
                secretWord,
                correctGuesses,
                incorrectGuesses, 
            };

            // Act
            bool result = CallStaticPrivateMethod<bool>(
                typeof(Hangman), "GameLogic",
                arguments);

            //Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(10, "~ \n", true)]
        [InlineData(10, "~ O\n", false)]
        [InlineData(5, "~ |_\\O\n", true)]
        public void DrawHangingMan(int lives, string tokens, bool expected) 
        {
            // Act
            string result = CallStaticPrivateMethod<string>(
                typeof(Hangman), "DrawHangman",
                lives);

            // Assert

            // must be a better C# way to do this
            // lambda, linq, ...
            bool inResult = true;
            foreach (char t in tokens)
            {
                if (!result.Contains(t))
                {
                    inResult = false;
                    break;
                }
            }

            Assert.Equal(inResult, expected);
        }

        [Theory]
        [InlineData("y", "y", true)]
        [InlineData("Y", "y", true)]
        [InlineData("n", "y", false)]
        [InlineData("01dfsd", "dsfds", false)]
        public void Confirm(string input, string option, bool expected)
        {
            // Act
            bool result = CallStaticPrivateMethod<bool>(
                typeof(Hangman), "IsConfirmed",
                input, option);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("abolition", "_________", "a", 10, 8)]
        [InlineData("coolaide", "coo_a_d_", "l", 5, 3)]
        [InlineData("cork", "cork", "", 5, 5)]
        public void GetHint(string secretWord, string cG,
            string returnedLetter, int livesBefore, int livesAfter)
        {
            // Assemble
            char[] correctGuesses = cG.ToCharArray();
            object[] arguments = new object[] { secretWord, correctGuesses, livesBefore };

            // Act
            string letter = CallStaticPrivateMethod<string>(
                typeof(Hangman), "GetHint",
                arguments);

            // Assert
            Assert.Equal(returnedLetter, letter);
            Assert.True((int)arguments[2] == livesAfter);
        }

        [Theory]
        [InlineData("one,two,three")]
        [InlineData("four")]
        [InlineData("")]
        public void ReadWordFromStream(string csv)
        {
            // Assemble
            byte[] bytes = Encoding.UTF8.GetBytes(csv);
            MemoryStream ms = new MemoryStream(bytes);
            StreamReader sr = new StreamReader(ms);

            // Act
            string word = CallStaticPrivateMethod<string>(
                typeof(Hangman), "ReadWordFromStream",
                sr).ToString();

            // Assert
            Assert.Contains(word, csv);
        }

        [Theory]
        [InlineData(new object[] { new string[] { "One", "Two", "Three", "Four", "Five" } })]
        [InlineData(new object[] { new string[] { "One" } })]
        public void PickWordFromArray(string[] wordList)
        {
            // Act
            string word = CallStaticPrivateMethod<string>(
                typeof(Hangman), "PickFromList",
                wordList);

            // Assert
            Assert.Contains(word, wordList);
        }

        /**
         * Call private static method in a static class using reflection
         * 
         * Pay close attention to match arguments passed to target method parameters
         */
        private static TReturn CallStaticPrivateMethod<TReturn>(
            Type className, string methodName, params object[] arguments)
        {
            return (TReturn)
                className
                .GetMethod(
                    methodName,
                    BindingFlags.Static
                    | BindingFlags.NonPublic)
                .Invoke(
                    null,
                    arguments.GetType() != typeof(object[]) ?
                        new object[] { arguments } : arguments);
        }
    }
}
