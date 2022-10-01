using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
using System.Collections;
using System.IO;

using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;
using System.Timers;
using Newtonsoft.Json;

namespace GuessAWord
{
    class Program
    {
        static List<string> wordarray = new List<string>(); //list of words
        static List<string> hecklearray = new List<string>();//list of cackles
        static List<string> hecklebackup = new List<string>();//list of cackles

        static List<string> hardarray = new List<string>();//list of words previously missed

        static int winnum = 0;
        static Random rand = new Random(DateTime.Now.Millisecond);

        static int level;
        static void resethecklelist()
        {
            hecklebackup.Clear();
            TextReader tr;
            tr = File.OpenText("hecklelist.txt");

            string line1;
            line1 = tr.ReadLine();
            while (line1 != null)
            {
                hecklebackup.Add(line1);
                line1 = tr.ReadLine();
            }
            tr.Close();
            hecklearray = hecklebackup;
        }


        static void Main(string[] args)
        {
            TextReader missedwordreader;
            missedwordreader = File.OpenText("missedwords.txt");

            string line1;
            line1 = missedwordreader.ReadLine();
            while (line1 != null)
            {
                hardarray.Add(line1);
                line1 = missedwordreader.ReadLine();
            }
            missedwordreader.Close();

        ugh:
            try
            {
                Write("Enter your word-length level (0 - No preference, 1-shortest, 2-medium or 3-long, 4-Superlong): ");
                level = Convert.ToInt16(ReadLine());
                if ((level != 1) && (level != 2) && (level != 3) && (level != 4) && (level != 0))
                {
                    WriteLine("\n\nError: Enter 0, 1, 2, 3, or 4");
                    goto ugh; //ignore this
                }

            }
            catch (Exception e)
            {
                WriteLine("\n\nError: " + e.Message);
                goto ugh;
            }


            resethecklelist();

            TextReader t;
            t = File.OpenText("wordlist.txt");
            string line;
            line = t.ReadLine();
            while (line != null)
            {
                wordarray.Add(line);
                line = t.ReadLine();
            }
            t.Close();

            string word;
            char[] letters;
            ArrayList man = new ArrayList();
            string lettersGuessed = "Letters Guessed:";

            char[] wordDisplay;

            char input;
            bool go = true;

            //main game loop:
            while (go)
            {
                //setup
                resethecklelist();

                word = getRandomWord().ToLower();

                letters = word.ToCharArray();
                wordDisplay = new char[letters.Length];
                for (int i = 0; i < wordDisplay.Length; i++)
                {
                    if (letters[i] != ' ') wordDisplay[i] = '_';
                }

                bool sameword = true;
                while (sameword)
                {
                    Write(getBoard(letters, lettersGuessed, wordDisplay, man, winnum));

                Tryagain:
                    try
                    {
                        input = (Convert.ToChar(ReadLine().ToLower()));

                    }
                    catch (Exception e)
                    {
                        WriteLine("Invalid Entry\nPress enter to continue");
                        ReadLine();
                        WriteLine(getBoard(letters, lettersGuessed, wordDisplay, man, winnum));
                        goto Tryagain;
                    }

                    bool there = false;
                    for (int i = 0; i < letters.Length; i++)
                    {
                        if (letters[i] == input)
                        {
                            there = true;
                            wordDisplay[i] = input;
                        }
                    }
                    if (!there)
                    {
                        lettersGuessed += " " + Convert.ToString(input) + ",";
                        man = addtoperson(man, word);
                    }
                    WriteLine(getBoard(letters, lettersGuessed, wordDisplay, man, winnum));

                    if (man.Count == 11)
                    {
                        WriteLine("\nYou Loose. Press any key to continue with a new word and try again");
                        WriteLine("\nDefinition of the word \"" + word + "\" is \"" + getDefinition(word) + "\"");
                        winnum = 0;
                        hardarray.Add(word);
                        ReadKey();

                        sameword = false;
                        man.Clear();
                        lettersGuessed = "Letters Guessed:";

                    }
                    if ((Array.IndexOf(wordDisplay, '_')) == -1)
                    {
                        WriteLine("\nDefinition of the word \"" + word + "\" is \"" + getDefinition(word) + "\"");
                        WriteLine("\nYou Win!! Press any key to continue with a new word and play again!");
                        winnum++;

                        //WriteLine("Definition: " + getDefinition(word));
                        ReadKey();

                        Clear();
                        if (winnum >= 3)
                        {
                            WriteLine("You got at least three words in a row!\nYour prize is entering in your own heckling phrase!\nPlease nothing innapropriate! \nEnter your phrase:");
                            string phrase = ReadLine();
                            WriteLine("{0}  --Awesome! Thanks!\npress enter to continue or click the X to cancel:", phrase);
                            ReadLine();
                            hecklebackup.Add(phrase);

                            File.WriteAllLines(@"hecklelist.txt", hecklebackup.ToArray(), Encoding.UTF8);
                            winnum = 0;
                        }

                        sameword = false;
                        man.Clear();
                        lettersGuessed = "Letters Guessed:";
                    }
                }

                File.WriteAllLines(@"missedWords.txt", hardarray.ToArray(), Encoding.UTF8);
            }
        }

        static ArrayList addtoperson(ArrayList man, string word)
        {
            switch (man.Count)
            {
                case 0:
                    man.Add("\tO -\"ow\"");
                    break;
                case 1:
                    man[0] = ("\tO");
                    man.Add("\t^");
                    break;
                case 2:
                    man.Add("\t|");
                    break;
                case 3:
                    man.Add("\t^");
                    break;
                case 4:
                    man[0] = "\tO -\"" + getRandomHeckle() + "\"";
                    man.Add("");
                    break;
                case 5:
                    man[0] = "\tO -\"" + getRandomHeckle() + "\"";
                    man.Add("");
                    break;
                case 6:
                    man[0] = "\tO -\"" + getRandomHeckle() + "\"";
                    man.Add("");
                    break;
                case 7:
                    man[0] = "\tO -\"" + getRandomHeckle() + "\"";
                    man.Add("");
                    break;
                case 8:
                    man[0] = "\tO -\"" + getRandomHeckle() + "\"";
                    man.Add("");
                    break;
                case 9:
                    man[0] = "\tO -\"ONE... ONE MORE CHANCE!\"";
                    man.Add("");
                    break;
                case 10:
                    man[0] = "\tO -\"I have a neck of steel. You still loose tho. The word was " + word + "\"";
                    man.Add("");
                    break;

            }
            return man;
        }
        static string getRandomWord()
        {
            int r;
            string word;
            if (level == 0)
            {
                r = rand.Next(0, wordarray.Count);
            }
            else if (level == 1)
            {
                r = rand.Next(0, 1000);
            }
            else if (level == 2)
            {
                r = rand.Next(1000, (2 * (wordarray.Count / 3)));
            }
            else if (level == 3)
            {
                r = rand.Next((2 * (wordarray.Count / 3)), wordarray.Count - 500);
            }
            else
            {
                r = rand.Next(wordarray.Count - 500, wordarray.Count);
            }

            word = wordarray[r];
            wordarray.Remove(wordarray[r]);

            string temp = getDefinition(word);
            if (temp.Equals("--NOT AVAILIBLE--"))
            {
                word = getRandomWord();
            }

            return word;
        }
        static string getRandomHeckle()
        {
            string word;
            int r = rand.Next(0, hecklearray.Count);
            word = hecklearray[r];
            hecklearray.Remove(hecklearray[r]);
            return word;
        }
        static string getBoard(char[] letters, string lettersGuessed, char[] wordDisplay, ArrayList man, int wincount)
        {
            Clear();
            string[] winray = { "[ ]", "[ ]", "[ ]" };
            string winstring = "Wins: ";
            for (int i = 0; i < wincount; i++)
            {
                winray[i] = "[X]";
            }
            for (int i = 0; i < winray.Length; i++)
            {
                winstring += winray[i];
            }
            winstring += "\n";
            string topLine = "WELCOME TO HANGMAN\nGUESS THE WORD\nSAVE THE FELLOW\n\n";
            string board = "";
            string trunk = "             |||     ";
            string bottomrow = "       , -=-~  .-^- _";
            string treetop = @"          &&& &&  & &&
      && &\/&\|& ()|/ @, &&
      &\/(/&/&||/& /_/)_&/_&
   &() &\/&|()|/&\/ '%  & ()
  &_\_&&_\ |& |&&/&__%_/_& &&
&&   && & &| &| /& & % ()& /&&
&&   && & &| &| /& & % ()& /&&
 ()&_---()&\&\|&&-&&--%---()~
     &&     \|||	|";


            treetop += "\n";


            //build board:
            board += winstring;
            board += topLine;
            board += lettersGuessed + "\n";
            board += "Word: ";
            for (int i = 0; i < wordDisplay.Length; i++)
            {
                board += wordDisplay[i] + " ";
            }
            board += "\n";
            board += "\n";

            board += treetop;


            if (man.Count == 0)
            {

                for (int i = 0; i < 3; i++)
                    board += trunk + "\n";
            }
            else if (man.Count < 3)
            {
                for (int i = 0; i < man.Count; i++)
                {
                    board += trunk + (string)man[i] + "\n";
                }
                for (int i = 0; i < 3 - man.Count; i++)
                    board += trunk + "\n";
            }
            else
            {
                for (int i = 0; i < man.Count; i++)
                {
                    board += trunk + (string)man[i] + "\n";
                }
            }
            board += bottomrow + "\n\nNext Letter Guess: ";

            return board;
        }
        static string getDefinition(String word)
        {
            string definition;

            string url = "https://api.dictionaryapi.dev/api/v2/entries/en/" + word;

            try
            {
                using (WebClient wc = new WebClient())
                {
                    var json = wc.DownloadString(url);
                    List<Root> def = JsonConvert.DeserializeObject<List<Root>>(json);
                    definition = def[0].meanings[0].definitions[0].definition;
                }
            }
            catch (Exception e)
            {
                definition = "--NOT AVAILIBLE--";
            }
            
            return definition;
        }
    }
}