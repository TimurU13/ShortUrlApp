namespace NeurolabUtils;
using System.Drawing;
using System.Drawing.Imaging;
class Resize
{
    public void ChangeSize(string baseInputFolderPath)
    {
        if (string.IsNullOrEmpty(baseInputFolderPath) || !Directory.Exists(baseInputFolderPath))
            throw new ArgumentNullException(nameof(baseInputFolderPath), "Input path is invalid or does not exist.");

        string outputFolderPath = Path.Combine(baseInputFolderPath, "output");
        if (Directory.Exists(outputFolderPath))
            Directory.Delete(outputFolderPath, true);

        Directory.CreateDirectory(outputFolderPath);
        string[] imageFiles = Directory.GetFiles(baseInputFolderPath, "*.*");

        foreach (string imageFile in imageFiles)
        {
            FileInfo fileInfo = new FileInfo(imageFile);
            if (fileInfo.Length > 1024 * 1024)
            {
                Convert(imageFile, outputFolderPath);
            }
            else
            {
                string fileName = Path.GetFileName(imageFile);
                string outputFile = Path.Combine(outputFolderPath, fileName);
                File.Copy(imageFile, outputFile);
            }
        }
    }
    private static void Convert(string imagePath, string outputFolderPath)
    {
        using (Bitmap bmp1 = new Bitmap(imagePath))
        {
            ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);
            System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
            EncoderParameters myEncoderParameters = new EncoderParameters(1);
            EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 50L);
            myEncoderParameters.Param[0] = myEncoderParameter;

            string jpgPath = Path.Combine(outputFolderPath, Path.GetFileName(imagePath));
            bmp1.Save(jpgPath, jpgEncoder, myEncoderParameters);

        }
    }
    private static ImageCodecInfo GetEncoder(ImageFormat format)
    {
        ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

        foreach (ImageCodecInfo codec in codecs)
        {
            if (codec.FormatID == format.Guid)
            {
                return codec;
            }
        }
        return null;
    }
}
