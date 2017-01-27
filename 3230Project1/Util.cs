using System;
using System.Collections.Generic;
using System.IO;

namespace _3230Project1
{
    class Util
    {
        public static string[] ReadFile(string filePath)
        {
            //int length = 0; //length of
            int x = 0; ; //pointer in string[]
            string[] output = new string[701496];//preallocated array for ChrisDNA.txt
            string theLine = "";

            try
            {
                using (StreamReader sr = new StreamReader(filePath))
                {
                    while (theLine != null)  //infinite loop with counter x
                    {
                        theLine = sr.ReadLine();
                        output[x] = theLine;
                        x++;
                    }

                }


            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read: ");
                Console.WriteLine(e.Message);
            }

            return output;
        }
        public static void PressKeyToContinue()
        {
            Console.WriteLine("Press enter to continue...");
            Console.In.ReadLine();
        }
    }
}
