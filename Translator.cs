/// Internationalization Program
/// 
/// @author  : Taylor Wilson
/// @version : March 20, 2014
///

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Interactions;

namespace Internationalization
{
    class Translator
    {
        /// <summary>
        /// This is the entry point for Internationalization program. Internationalization parses a text 
        /// (.txt) file line-by-line. Each line of text is then translated into a variety of languages. 
        /// A new file for each language will be created in which the entire contents of the parameter file
        /// will be translated into the newly created file. This program only receives 1 parameter file and 
        /// will ignore any other parameters received. If no parameters are received, the program will shutdown.
        /// Parameter files are assumed to contain only plain text data that is to be translated. Output files
        /// will likewise only contain plain text data.
        /// </summary>
        /// <param name="args">Plain text (.txt) file expected at position 0</param>
        static void Main(string[] args)
        {
            try
            {
                CheckInputArguments(args);
                String[] languages = SetLanguages();
                TranslateInputFile(args[0], languages);
            }
            catch (Exception e)
            {
                Console.WriteLine("The program crashed. Sorry. " + e.Message);
                Console.Read();
            }
        }


        /// <summary>
        /// Translates the input filename into the all the languages specified in the
        /// input parameter array of languages. This method assumes that the file name
        /// is legal and represents plain text (.txt). This method also assumes that 
        /// the languages array in not empty and that each element of the array is 
        /// supported by Google's Translate. Results of this method are output to individual
        /// files named after the translated language.
        /// </summary>
        /// <param name="filename">A plain text file to be translated</param>
        /// <param name="languages">A list of languages to translate to</param>
        static void TranslateInputFile(String filename, String[] languages)
        {
            // Open a new Firefox window
            IWebDriver driver = new FirefoxDriver();

            // Translate into each language in the paramter array
            foreach (String lang in languages)
            {
                if (!lang.Equals("es"))
                {
                    continue;
                }

                try
                {
                    // Navigate the browser window to Google's Translate URL; specify the target language here
                    driver.Navigate().GoToUrl("http://translate.google.com/#auto/" + lang);
                }
                catch(Exception e)
                {
                    // Either the langauge is not supported or Google has changed the how URLs are generated
                    Console.WriteLine("Unable to translate the following language code: " + lang + ". " + e.Message);
                    continue;
                }
                // Create a new file to output the results 
                System.IO.StreamWriter output = new System.IO.StreamWriter("../../Results/" + lang + ".txt");
               
                // Read in all the lines of text of the parameter file
                String curr = null;
                String[] lines = System.IO.File.ReadAllLines(filename);

                // Send each line of text out to Google Translate via Selenium and retrieve result
                // ID for user input on Google Translate = "source"
                // ID for result     on Google Translate = "result_box"
                for (int i = 0; i < lines.Length; i++)
                {
                    if(lines[i].Equals(""))
                    {
                        output.WriteLine("");
                        Console.WriteLine();
                        continue;
                    }

                    bool   multiple = false;
                    String one      = "";
                    String two      = "";
                    
                    // In the event a line has a # indicator, the line must be translated in two steps
                    if(lines[i].Contains('#'))
                    {
                        String[] phrase = lines[i].Split('#');
                        one = phrase[0];
                        two = phrase[1];
                        multiple = true;
                    }

                    if (multiple)
                    {
                        // Input a line to be translated
                        driver.FindElement(By.Id("source")).Click();
                        driver.FindElement(By.Id("source")).SendKeys(one.ToLower());

                        // Wait for Google's response before retrieving result
                        Thread.Sleep(1000);
                        curr = driver.FindElement(By.Id("result_box")).Text;

                        // Store each result in a new file
                        // ??? How should capitalization be conducted for NON-GERMANIC languages ???
                        output.Write(curr);

                        // Clear the Google Translate input fields and wait for Google to clear result
                        driver.FindElement(By.Id("source")).Clear();
                        Thread.Sleep(1000);

                        output.Write("#");
                        Console.Write(one + "#" + two + " --> " + curr);

                        // Input a line to be translated
                        driver.FindElement(By.Id("source")).Click();
                        driver.FindElement(By.Id("source")).SendKeys(two.ToLower());

                        // Wait for Google's response before retrieving result
                        Thread.Sleep(1000);
                        curr = driver.FindElement(By.Id("result_box")).Text;
                        
                        // Store each result in a new file
                        // ??? How should capitalization be conducted for NON-GERMANIC languages ???
                        output.WriteLine(curr);

                        // Clear the Google Translate input fields and wait for Google to clear result
                        driver.FindElement(By.Id("source")).Clear();
                        Thread.Sleep(1000);

                        Console.WriteLine("#" + curr);
                    }
                    else
                    {
                        // Input a line to be translated
                        driver.FindElement(By.Id("source")).Click();
                        driver.FindElement(By.Id("source")).SendKeys(lines[i].ToLower());

                        // Wait for Google's response before retrieving result
                        Thread.Sleep(1000);
                        curr = driver.FindElement(By.Id("result_box")).Text;

                        // Store each result in a new file
                        // ??? How should capitalization be conducted for NON-GERMANIC languages ???
                        output.WriteLine(curr);

                        // Clear the Google Translate input fields and wait for Google to clear result
                        driver.FindElement(By.Id("source")).Clear();
                        Thread.Sleep(1000);

                        Console.WriteLine(lines[i] + " --> " + curr);
                    }
                }

                // Close the output file
                output.Close();
            }
        }

        /// <summary>
        /// Validates that there exists and proper .txt file in the parameter received.
        /// Returns TRUE if Main application should attempt to parse the file; FALSE otherwise.
        /// </summary>
        /// <param name="args">An array of arguments; Plain text (.txt) file expected at position 0</param>
        /// <returns>TRUE : if filename parameter is a .txt file; otherwise FALSE</returns>
        static bool CheckInputArguments(string[] args)
        {
            // Must have at least 1 argument; more than 1 argument will just be ignored
            if (args.Length < 1)
            {
                return false;
            }

            // If the parameter file is a .txt file, return true
            if(args[0].EndsWith(".txt"))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns an array of language codes. Language codes were obtained from
        /// Google's Translate.
        /// </summary>
        /// <returns>An array of language codes</returns>
        static String[] SetLanguages()
        {
            return new String[81] { "af",   // Afrikaans
                                    "sq",   // Albanian
                                    "ar",   // Arabic
                                    "hy",   // Armenian
                                    "az",   // Azerbaijani
                                    "eu",   // Basque
                                    "be",   // Belarusian
                                    "bn",   // Bengali
                                    "bs",   // Bosnian
                                    "bg",   // Bulgarian
                                    "ca",   // Catalan
                                    "ceb",  // Cebuano
                                    "zh-CN",// Chinese (Simplified)
                                    "zh-TW",// Chinese (Traditional)
                                    "hr",   // Croatian
                                    "cs",   // Czech
                                    "da",   // Danish
                                    "nl",   // Dutch
                                    "en",   // English
                                    "eo",   // Esperanto
                                    "et",   // Estonian
                                    "tl",   // Filipino
                                    "fi",   // Finnish
                                    "fr",   // French
                                    "gl",   // Gallcian
                                    "ka",   // Georgian
                                    "de",   // German
                                    "el",   // Greek
                                    "gu",   // Gujarati
                                    "ht",   // Haitian Creole
                                    "ha",   // Hausa
                                    "iw",   // Hebrew
                                    "hi",   // Hindi
                                    "hmn",  // Hmong
                                    "hu",   // Hungarian
                                    "is",   // Icelandic
                                    "ig",   // Igbo
                                    "id",   // Indonesian
                                    "ga",   // Irish
                                    "it",   // Italian
                                    "ja",   // Japanese
                                    "jw",   // Javanese
                                    "kn",   // Kannada
                                    "km",   // Khmer
                                    "ko",   // Korean
                                    "lo",   // Lao
                                    "la",   // Latin
                                    "lv",   // Latvian
                                    "lt",   // Lithuanian
                                    "mk",   // Macedonian
                                    "ms",   // Malay
                                    "mt",   // Maltese
                                    "mi",   // Maori
                                    "mr",   // Marathi
                                    "mn",   // Mongolian
                                    "ne",   // Nepali
                                    "no",   // Norwegian
                                    "fa",   // Persian
                                    "pl",   // Polish
                                    "pt",   // Portuguese
                                    "pa",   // Punjabi
                                    "ro",   // Romanian
                                    "ru",   // Russian
                                    "sr",   // Serbian
                                    "sk",   // Slovak
                                    "sl",   // Slovenian
                                    "so",   // Somali
                                    "es",   // Spanish
                                    "sw",   // Swahili
                                    "sv",   // Swedish
                                    "ta",   // Tamil
                                    "te",   // Telugu
                                    "th",   // Thai
                                    "tr",   // Turkish
                                    "uk",   // Ukranian
                                    "ur",   // Urdu
                                    "vi",   // Vietnamese
                                    "cy",   // Welsh
                                    "yi",   // Yiddish
                                    "yo",   // Yoruba
                                    "zu",   // Zulu
                                };
        }
    }
}
