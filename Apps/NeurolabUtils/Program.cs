using NeurolabUtils;
class Program
{
    static void Main(string[] args)
    {
        try
        {
            string baseInputFolderPath = string.Empty;
            string baseInputFolderPathForResize = string.Empty;
            string baseOutputFolderPath = string.Empty;
            string baseInputFolderTotalClasses = string.Empty;
            List<string> classesToCut = new List<string>();
            string[] oldOrder = new string[0];
            string[] newOrder = new string[0];
            int basePercent = 20;
            if (args.Length < 6)
            {
                throw new ArgumentException("Check if the path is entered correctly");
            }

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "-c")
                {
                    if (args[i + 1] == "restruct")
                    {
                        baseInputFolderPath = args[i + 3];
                        baseOutputFolderPath = args[i + 5];
                        if (int.TryParse(args[i + 7], out basePercent) == false)
                            basePercent = 20;
                    }

                    else if (args[i + 1] == "total")
                        baseInputFolderTotalClasses = args[i + 3];

                    else if (args[i + 1] == "cut")
                    {

                        for (int j = i + 3; j < args.Length; j++)
                        {
                            if (!args[j].StartsWith("-"))
                                classesToCut.Add(args[j]);
                            else
                                break;
                        }
                    }

                    else if (args[i + 1] == "normalize")
                    {
                        if (args[i + 2] == "-p")
                        {
                            oldOrder = args[i + 3].Split(',');
                            newOrder = args[i + 5].Split(',');
                            baseInputFolderPath = args[i + 7];
                        }

                    }
                    
                    else if (args[i+1]=="resize")
                    {
                        if (args[i+2] =="-i")
                        {
                            baseInputFolderPathForResize = args[i+3];
                        }
                    }
                }
            }
            try
            {
                Resize resize = new Resize();
                resize.ChangeSize(baseInputFolderPathForResize);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Введен не полный путь {ex.Message}");
        }
    }
}



