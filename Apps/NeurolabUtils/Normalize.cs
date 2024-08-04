namespace NeurolabUtils;
class Normalize
{
    public void NormalizeLabels(List<string> oldOrder, List<string> newOrder, string baseInputFolderPath)
    {
        if (string.IsNullOrEmpty(baseInputFolderPath) || !Directory.Exists(baseInputFolderPath))
            throw new ArgumentNullException(nameof(baseInputFolderPath), "Input path is invalid or does not exist.");
        
        Dictionary<int, int> oldIndextoNewIndex = new Dictionary<int, int>();

        for (int i = 0; i < oldOrder.Count; i++)
        {
            int oldIndex = oldOrder.IndexOf(newOrder[i]);
            if (oldIndex != -1)
                oldIndextoNewIndex[oldIndex] = i;
            else if (oldIndex == -1)
            {
                throw new Exception("Такой элемент не найден в списке классов");
            }
        }
        string outputLabelsPath = Path.Combine(baseInputFolderPath, "output");

        if (Directory.Exists(outputLabelsPath))
            Directory.Delete(outputLabelsPath, true);

        string imagesInputPath = Path.Combine(baseInputFolderPath, "images");
        string imagesOutputPath = Path.Combine(baseInputFolderPath, "output", "images");
        Directory.CreateDirectory(imagesOutputPath);

        string txtFilePath = Directory.GetFiles(baseInputFolderPath, "*.txt").FirstOrDefault();
        string outputTxtFilePath = Path.Combine(baseInputFolderPath, "output", Path.GetFileName(txtFilePath));

        File.WriteAllLines(outputTxtFilePath, newOrder);

        string jsonFilePath = Directory.GetFiles(baseInputFolderPath, "*.json").FirstOrDefault();
        string outputJsonFilePath = Path.Combine(baseInputFolderPath, "output", Path.GetFileName(jsonFilePath));

        File.Copy(jsonFilePath, outputJsonFilePath, true);


        foreach (string imagePath in Directory.GetFiles(imagesInputPath))
        {
            string imageName = Path.GetFileName(imagePath);
            string outputImagePath = Path.Combine(imagesOutputPath, imageName);

            File.Copy(imagePath, outputImagePath);
        }

        foreach (string filePath in Directory.GetFiles(Path.Combine(baseInputFolderPath, "labels"), "*.txt"))
            NormalizeLabelsInFile(filePath, oldIndextoNewIndex, baseInputFolderPath);
    }

    private void NormalizeLabelsInFile(string filePath, Dictionary<int, int> oldIndextoNewIndex, string baseInputFolderPath)
    {
        string[] lines = File.ReadAllLines(filePath);
        List<string> normalizedLines = new List<string>();
        string outputLabelsPath = Path.Combine(baseInputFolderPath, "output", "labels");
        Directory.CreateDirectory(outputLabelsPath);

        foreach (string line in lines)
        {
            string[] parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length > 0 && oldIndextoNewIndex.ContainsKey(int.Parse(parts[0])))
            {
                int oldIndex = int.Parse(parts[0]);
                int newIndex = oldIndextoNewIndex[oldIndex];
                parts[0] = newIndex.ToString();
                normalizedLines.Add(string.Join(" ", parts));
            }
        }
        string outputFilePath = Path.Combine(outputLabelsPath, Path.GetFileName(filePath));

        if (normalizedLines.Count > 0)
            File.WriteAllLines(outputFilePath, normalizedLines);

        else
            File.Copy(filePath, outputFilePath, true);
    }
}



