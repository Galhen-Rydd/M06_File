using System.Diagnostics.Metrics;
using System;
using System.IO;

namespace ProyectoFicheros
{
    static class Program
    {
        static void Main(string[] args)
        {
            string dir = "PFi";
            if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\" + dir))
            {
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\" + dir);
            }
            Program.menu();
        }

        static void menu()
        {
            int delta;
            Console.WriteLine("Welcome to Pau's summarizer tool.");
            Thread.Sleep(2000);
            while(true)
            {
                Console.Write("Enter the URI of file to sumarize or enter 0 to exit: ");
                String input = Console.ReadLine();
                String path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\" + input;
                //String path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\test.txt";
                if (input.Equals("0"))
                {
                    Console.WriteLine("Exiting, thank you for using this program");
                    Console.ReadLine();
                    break;
                }
                if(File.Exists(path))
                {
                    while (true)
                    {
                        Console.Write("Choose which option to run 1) Normal, 2) With Delta, 3) By Paragraph,\n0) Return to file selection: ");
                        String option = Console.ReadLine();
                        if (option.Equals("1"))
                        {
                            Fichero file = new(path);
                            file.DoInfo();
                            break;
                        }
                        else if (option.Equals("2"))
                        {
                            while (true)
                            {
                                try
                                {
                                    while (true)
                                    {
                                        Console.Write("Enter Delta (0-4): ");
                                        delta = int.Parse(Console.ReadLine());
                                        if (delta >= 0 && delta <= 4)
                                        {
                                            break;
                                        }
                                        else
                                        {
                                            Console.WriteLine("Invalid Delta.");
                                        }
                                    }
                                    break;
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine("Invalid Input Type.");
                                }
                            }
                            FDelta file = new(path, delta);
                            file.DoInfo();
                            break;
                        }
                        else if (option.Equals("3"))
                        {
                            FParag file = new(path);
                            file.DoInfo();
                            break;
                        }
                        else if (option.Equals("0"))
                        {
                            Console.WriteLine("Returning to file selection.\n");
                            Thread.Sleep(500);
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Invalid option.");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Invalid file location.");
                }
            }
        }
    }
}