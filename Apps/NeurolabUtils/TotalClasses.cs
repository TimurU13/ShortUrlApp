namespace NeurolabUtils;
class TotalClasses
{
    public static void CalculateTotalClasses(string baseInputFolderTotalClasses)
    {
        Dictionary<string, int> classNames = new Dictionary<string, int>();
        Dictionary<string, int> classTotals = new Dictionary<string, int>();
        string classesFilePath = Path.Combine(baseInputFolderTotalClasses, "classes.txt");
        if (File.Exists(classesFilePath))
        {
            string[] classLines = File.ReadAllLines(classesFilePath);
            for (int i = 0; i < classLines.Length; i++)
            {
                string className = $"{classLines[i].Trim()}";
                classNames.Add(className, i);
                classTotals.Add(className, 0);
            }
        }
        foreach (string filePath in Directory.GetFiles(Path.Combine(baseInputFolderTotalClasses, "labels"), "*.txt"))
        {
            CalculateClassTotals(filePath, classTotals, classNames);
        }
        foreach (var classTotal in classTotals)
        {
            Console.WriteLine(classTotal.Key + ": " + classTotal.Value);
        }
    }
    static void CalculateClassTotals(string filePath, Dictionary<string, int> classTotals, Dictionary<string, int> classNames)
    {
        string[] lines = File.ReadAllLines(filePath);
        foreach (string line in lines)
        {
            string className = line.Trim().Split(' ')[0];
            foreach (var cName in classNames)
            {
                if (cName.Value.ToString() == className)
                {
                    string classKey = cName.Key;
                    classTotals[classKey]++;
                    break;
                }
            }
        }
    }
}