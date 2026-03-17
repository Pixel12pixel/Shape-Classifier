using System.Drawing;
using System.Drawing.Drawing2D;

namespace ShapeClassifier.FileHandlers;

public class ImagesLoader
{
    private const int ImageSize = 224;
    
    
    public (List<bool[]>, List<string>) LoadImages(string pathToImages)
    {
        var files = Directory.GetFiles(pathToImages);
        var imagesFiles = files.Where(file => file.EndsWith(".jpg") || file.EndsWith(".png")).ToList();
        
        var imagesData = new List<bool[]>();
        var labels = new List<string>();
        
        var shuffleService = new Services.ShuffleService();
        imagesFiles = shuffleService.Shuffle(imagesFiles);
        
        foreach (var file in imagesFiles)
        {
            try
            {
                using var image = new Bitmap(file);
                var normalizedImage = NormalizeImage(image);
                var binaryArray = ConvertToBinaryArray(normalizedImage);
                var flattenedArray = binaryArray.Cast<bool>().ToArray();
                
                imagesData.Add(flattenedArray);
                labels.Add(Path.GetFileNameWithoutExtension(file));
            }
            catch (Exception e)
            {
                Console.WriteLine("Error loading image: " + e.Message);
            }
        }
        return (imagesData, labels);
    }

    private Bitmap NormalizeImage(Bitmap image)
    {
        var resizedImage = new Bitmap(ImageSize, ImageSize);
        try
        {
            using var graphics = Graphics.FromImage(resizedImage);
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.DrawImage(image, 0, 0, ImageSize, ImageSize);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error normalizing image: " + e.Message);
            throw;
        }
        
        return resizedImage;
    }
    
    private bool[,] ConvertToBinaryArray(Bitmap image)
    {
        var binaryArray = new bool[ImageSize, ImageSize];
        for (int y = 0; y < ImageSize; y++)
        {
            for (int x = 0; x < ImageSize; x++)
            {
                var pixel = image.GetPixel(x, y);
                binaryArray[y, x] = pixel == Color.White;
            }
        }
        return binaryArray;
    }

}