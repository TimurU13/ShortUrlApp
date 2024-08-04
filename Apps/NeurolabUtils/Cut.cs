namespace NeurolabUtils
{
    class Cut
    {
        public void CutFiles(List<string> classesToCut, string baseInputFolderPath)
        {
            if (string.IsNullOrEmpty(baseInputFolderPath) || !Directory.Exists(baseInputFolderPath))
                throw new ArgumentNullException(nameof(baseInputFolderPath), "Input path is invalid or does not exist.");
            
            string[] images = Directory.GetFiles(Path.Combine(baseInputFolderPath, "images"), "*.*", SearchOption.AllDirectories);
            string[] labels = Directory.GetFiles(Path.Combine(baseInputFolderPath, "labels"), "*.txt", SearchOption.AllDirectories);
            Dictionary<string, string> storePairs = new();

            foreach (string label in labels)
                storePairs.Add(Path.GetFileNameWithoutExtension(label), string.Empty);

            foreach (string image in images)
                storePairs[Path.GetFileNameWithoutExtension(image)] = image;

            int unpairedFiles = 0;
            
            foreach (var pair in storePairs)
            {
                if (string.IsNullOrEmpty(pair.Value))
                {
                    Console.WriteLine($"We found unpaired file {pair.Key}.txt");
                    unpairedFiles++;
                }
            }

            Dictionary<string, int> classes = new Dictionary<string, int>();
            string classesFilePath = Path.Combine(baseInputFolderPath, "classes.txt");
            
            if (File.Exists(classesFilePath))
            {
                string[] classLines = File.ReadAllLines(classesFilePath);
                for (int i = 0; i < classLines.Length; i++)
                {
                    string className = classLines[i].Trim();
                    classes.Add(className, i);
                }
            }
           
            foreach (string filePath in Directory.GetFiles(Path.Combine(baseInputFolderPath, "labels"), "*.txt"))
                CutFilesByClassName(filePath, classesToCut, classes, baseInputFolderPath);
        }
        static void CutFilesByClassName(string filePath, List<string> classesToCut, Dictionary<string, int> classes, string baseInputFolderPath)
        {
            string[] lines = File.ReadAllLines(filePath);
            foreach (string line in lines)
            {
                string classIdentifier = line.Trim().Split(' ')[0];
                foreach (string classToCut in classesToCut)
                {
                    if (classes.ContainsKey(classToCut))
                    {
                        int classValue = classes[classToCut];
                        if (classValue.ToString() == classIdentifier)
                        {
                            string backupPath = Path.Combine(baseInputFolderPath, "backup",classToCut);
                            Directory.CreateDirectory(backupPath);
                            string backupFilePath = Path.Combine(backupPath, Path.GetFileName(filePath));
                            File.AppendAllText(backupFilePath, line + Environment.NewLine);
                            break;
                        }
                    }
                }
            }
        }
    }
}
