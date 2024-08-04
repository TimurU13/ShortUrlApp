namespace NeurolabUtils;
class Restructuring
{
    public class RestructResult
    {
        public int TotalTrainImagesMoved { get; set; }
        public int TotalValImagesMoved { get; set; }
        public int TotalTrainLabelsMoved { get; set; }
        public int TotalValLabelsMoved { get; set; }
    }
    public RestructResult Restruct(string baseOutputFolderPath, string baseInputFolderPath, int basePercent)
    {
        if (string.IsNullOrEmpty(baseOutputFolderPath) || !Directory.Exists(baseOutputFolderPath))
            throw new ArgumentNullException(nameof(baseOutputFolderPath), "Output path is invalid or does not exist.");

        if (string.IsNullOrEmpty(baseInputFolderPath) || !Directory.Exists(baseInputFolderPath))
            throw new ArgumentNullException(nameof(baseInputFolderPath), "Input path is invalid or does not exist.");

        if (basePercent <= 0 || basePercent > 100)
            throw new ArgumentOutOfRangeException("Range of base persentage must be between 1 and 100.");

        RestructResult result = new RestructResult();
        int runCounter = 1;
        string runFolder = Path.Combine(baseOutputFolderPath, $"run{runCounter}");
        while (Directory.Exists(runFolder))
        {
            runCounter++;
            runFolder = Path.Combine(baseOutputFolderPath, $"run{runCounter}");
        }

        string imagesFolder = Path.Combine(runFolder, "images");
        string labelsFolder = Path.Combine(runFolder, "labels");
        string imagesTrainFolder = Path.Combine(imagesFolder, "train");
        string imagesValFolder = Path.Combine(imagesFolder, "val");
        string labelsTrainFolder = Path.Combine(labelsFolder, "train");
        string labelsValFolder = Path.Combine(labelsFolder, "val");
        string[] directoriesToCreate = new string[]
         {
        runFolder, imagesFolder, labelsFolder, imagesTrainFolder,
        imagesValFolder, labelsTrainFolder, labelsValFolder
         };
        foreach (string directoryPath in directoriesToCreate)
            Directory.CreateDirectory(directoryPath);

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

        int pairCount = storePairs.Count - unpairedFiles;
        int totalTrainFiles = (int)(pairCount * (basePercent * 0.01));
        int movedTrainFilesCounter = 0;

        foreach (var pair in storePairs)
        {
            if (totalTrainFiles > movedTrainFilesCounter)
            {
                string inputLabelTrainFile = Path.Combine(baseInputFolderPath, "labels", pair.Key + ".txt");
                string outputLabelTrainFile = Path.Combine(labelsTrainFolder, pair.Key + ".txt");
                File.Copy(inputLabelTrainFile, outputLabelTrainFile);
                string inputImagesTrainFile = pair.Value;
                string outputFileName = Path.GetFileName(inputImagesTrainFile);
                string outputNumberTrainFile = Path.Combine(imagesTrainFolder, outputFileName);
                File.Copy(inputImagesTrainFile, outputNumberTrainFile);
                movedTrainFilesCounter++;
            }
            else
            {
                string inputLabelTrainFile = Path.Combine(baseInputFolderPath, "labels", pair.Key + ".txt");
                string outputLabelValFile = Path.Combine(labelsValFolder, pair.Key + ".txt");
                File.Copy(inputLabelTrainFile, outputLabelValFile);
                string inputImagesValFile = pair.Value;
                string outputFileName = Path.GetFileName(inputImagesValFile);
                string outputImagesValFile = Path.Combine(imagesValFolder, outputFileName);
                File.Copy(inputImagesValFile, outputImagesValFile);
            }
        }
        result.TotalTrainImagesMoved = totalTrainFiles;
        result.TotalValImagesMoved = pairCount - totalTrainFiles;
        result.TotalTrainLabelsMoved = totalTrainFiles;
        result.TotalValLabelsMoved = pairCount - totalTrainFiles;

        return result;
    }
}

