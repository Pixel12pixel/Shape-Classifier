
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ShapeClassifier.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;


namespace ShapeClassifier.FileHandlers;

public class ImagesLoader
{
    private const int ImageSize = 224;
    
    /// <summary>
    /// Loads images from the specified directory, normalizes them to a fixed size, converts them to binary arrays, and extracts labels from file names. The method returns a tuple containing a list of binary arrays representing the images and a list of recognized shape labels.
    /// </summary>
    /// <param name="pathToImages">Path to images</param>
    public (List<bool[]>, List<Shape>) LoadImages(string pathToImages)
    {
        var files = Directory.GetFiles(pathToImages);
        var imagesFiles = files.Where(file => file.EndsWith(".jpg") || file.EndsWith(".png")).ToList();
        
        List<bool[]> imagesData;
        var labels = new List<string>();
        
        var shuffleService = new Services.ShuffleService();
        imagesFiles = shuffleService.Shuffle(imagesFiles);

        var processedImages = new bool[imagesFiles.Count][];
        
        int processedCount = 0;
        
        Parallel.For(0, imagesFiles.Count, new ParallelOptions
        {
            MaxDegreeOfParallelism = Environment.ProcessorCount
        }, i =>
        {
            processedImages[i] = Process(imagesFiles[i]);

            var current = Interlocked.Increment(ref processedCount);
            if (current % 1000 == 0)
            {
                Console.WriteLine($"Processed {current} / {imagesFiles.Count}");
            }
        });

        imagesData = new List<bool[]>(processedImages.ToArray());
        
        /*
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
        */
        var labelRecogniser = new Services.LabelRecogniser();
        var recognisedLabels = labelRecogniser.RecogniseLabels(labels);
        
        return (imagesData, recognisedLabels);
    }

    private bool[] Process(string file)
    {
        try
        {
            using var image = Image.Load<Rgba32>(file);
            NormalizeImage(image);
            return ConvertToBinaryArray(image).Cast<bool>().ToArray();
        }
        catch (Exception e)
        {
            Console.WriteLine("Error processing image: " + e.Message);
            throw;
        }
    }
    
    
    private void NormalizeImage(Image<Rgba32> image)
    {
        image.Mutate(ctx => ctx.Resize(new ResizeOptions
        {
            Size = new Size(ImageSize, ImageSize),
            Mode = ResizeMode.Stretch,
            Sampler = KnownResamplers.Bicubic
        }));
    }
    
    
    private Image<Rgba32> NormalizeImage1(Image image)
    {
        try
        {
            image.Mutate(ctx => ctx.Resize(new ResizeOptions
            {
                Size = new Size(ImageSize, ImageSize),
                Mode = ResizeMode.Stretch,
                Sampler = KnownResamplers.Bicubic
            }));

            return image.CloneAs<Rgba32>();
        }
        catch (Exception e)
        {
            Console.WriteLine("Error normalizing image: " + e.Message);
            throw;
        }
    }
    
    private bool[,] ConvertToBinaryArray(Image<Rgba32> image)
    {
        int width = image.Width;
        int height = image.Height;

        var binaryArray = new bool[height, width];

        image.ProcessPixelRows(accessor =>
        {
            for (int y = 0; y < height; y++)
            {
                Span<Rgba32> row = accessor.GetRowSpan(y);

                for (int x = 0; x < width; x++)
                {
                    ref Rgba32 pixel = ref row[x];

                    binaryArray[y, x] = !(pixel.R == 255 && pixel.G == 255 && pixel.B == 255);
                }
            }
        });
        
        image.Dispose();
        
        return binaryArray;
    }
    
    
    
    
    
    
    
    
    
    
    /*
    
    /// <summary>
    /// Normalizes the input image by resizing it to a fixed size using high-quality bicubic interpolation. This process ensures that all images have the same dimensions, which is essential for consistent feature extraction and classification. The method creates a new Bitmap object with the specified size, draws the original image onto it using the Graphics class, and returns the normalized image. If any errors occur during this process, they are caught and logged to the console.
    /// </summary>
    /// <param name="image">Image in bitmap format</param>
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
    
    /// <summary>
    /// Converts the normalized image into a binary array, where each pixel is represented as a boolean value (false for white pixels and true for non-white pixels). The method iterates through each pixel of the image, checks its color, and populates a 2D boolean array accordingly. This binary representation is useful for feature extraction and classification tasks, as it simplifies the image data while retaining essential information about the shape and structure of the objects in the image.
    /// </summary>
    /// <param name="image">Image in bitmap format</param>
    private bool[,] ConvertToBinaryArray(Bitmap image)
    {
        var binaryArray = new bool[ImageSize, ImageSize];
        for (int y = 0; y < ImageSize; y++)
        {
            for (int x = 0; x < ImageSize; x++)
            {
                var pixel = image.GetPixel(x, y);
                binaryArray[y, x] = (pixel.Name != "ffffffff");
            }
        }
        return binaryArray;
    }
    */
}