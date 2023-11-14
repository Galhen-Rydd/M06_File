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
            Console.WriteLine("Welcome to Pau's summarizer tool.\nEnter the URI of file to sumarize or enter 0 to exit: ");
            while(true)
            {
                //String path = Console.ReadLine();
                String path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\test.txt";
                if (path.Equals("0"))
                {
                    Console.WriteLine("Exiting, thank you for choosing this program");
                    break;
                }
                if(File.Exists(path))
                {
                    Console.WriteLine("Starting summary...");
                    Fichero file = new(path);
                    try{
                        file.DoInfo();
                        Thread.Sleep(1000);
                        Console.WriteLine("The file was summarized successfully");
                    }catch(Exception ex)
                    {
                        Console.WriteLine("There was a problem summarizing!");
                    }
                    break; // Eliminate or comment when finished
                }
            }
        }
    }
}