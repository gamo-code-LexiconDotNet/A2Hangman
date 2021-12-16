using System;
using System.IO;
using System.Text;
using static System.Console;

namespace Hangman
{
    public static partial class Hangman
    {
        private static readonly string path =
            Environment.CurrentDirectory + "\\wordlist.txt";

        private static string ReadWordFromFile(string path)
        {
            try
            {
                using StreamReader sr = new StreamReader(path);
                return ReadWordFromStream(sr);
            }
            catch (Exception ex)
            {
                WriteLine(ex.Message);
                return "";
            }
        }

        private static string ReadWordFromStream(StreamReader sr)
        {
            StringBuilder word = new StringBuilder();

            try
            {
                    long streamSize = sr.BaseStream.Length;
                    int offset = random.Next(0, (int)streamSize);

                    // seek to random offset
                    sr.BaseStream.Seek(offset, SeekOrigin.Begin);

                    // go to next comma or eos
                    while (!sr.EndOfStream && (char)sr.Read() != ',') { }

                    // if eos, go to beginning of stream (first word)
                    if (sr.EndOfStream)
                        sr.BaseStream.Seek(0, SeekOrigin.Begin);

                    // read characters until next comma or eos
                    while (!sr.EndOfStream && (char)sr.Peek() != ',')
                        word.Append((char)sr.Read());
            }
            catch (Exception ex)
            {
                Error.WriteLine(ex.Message);
            }

            return word.ToString();
        }
    }
}
