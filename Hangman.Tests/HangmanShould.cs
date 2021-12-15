using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit;
using Xunit.Abstractions;

namespace Hangman.Tests
{
    public class HangmanShould : IDisposable
    {
        private readonly ITestOutputHelper output;

        public HangmanShould(ITestOutputHelper output)
        {
            this.output = output;
            System.GC.Collect();
        }

        public void Dispose()
        {

        }

        //
        // wonky
        //  fails when running in group but ok when run individually a few times
        //  does get the right data from file but assert fails
        //  file stream reset, sync/async, exceptions, pointer/memory, garbage collection ?
        //
        [Theory]
        [InlineData("file1.txt", new string[] { "one", "two", "three" })]
        [InlineData("file2.txt", new string[] { "four" })]
        [InlineData("file3.txt", new string[] { "" })]
        public void ReadWordFromFile(string fileName, string[] possible)
        {
            string path = Environment.CurrentDirectory + "\\" + fileName;

            System.Threading.Thread.Sleep(1000);

            string word = CallMethod<string>(
                typeof(Hangman), "ReadWordFromFile",
                path).ToString();

            Assert.Contains(word, possible);
        }

        [Theory]
        [InlineData(new object[] { new string[] { "One", "Two", "Three", "Four", "Five" } })]
        [InlineData(new object[] { new string[] { "One" } })]
        public void PickWordFromArray(string[] wordList)
        {
            string word = CallMethod<string>(
                typeof(Hangman), "PickFromList",
                wordList);

            Assert.Contains(word, wordList);
        }

        [Fact]
        public void CompareCommaSeparatedStringsAndString()
        {
            string csv = "one,two,three,four,five";
            string word1 = "three";
            string word2 = "zero;";

            bool cw1 = CallMethod<bool>(
                typeof(Hangman), "CommaSeparatedStringContains", 
                csv, word1);
            bool cw2 = CallMethod<bool>(
                typeof(Hangman), "CommaSeparatedStringContains",
                csv, word2);

            Assert.True(cw1);
            Assert.False(cw2);
        }

        // Call private static method in a static class
        private static TReturn CallMethod<TReturn>(
            Type className, string methodName, params object[] parameters)
        {
            return (TReturn)
                className
                .GetMethod(
                    methodName,
                    BindingFlags.Static
                    | BindingFlags.NonPublic)
                .Invoke(
                    null,
                    parameters.GetType() != typeof(object[]) ?
                        new object[] { parameters } : parameters);
        }
    }
}
