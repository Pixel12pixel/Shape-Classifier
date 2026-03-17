using ShapeClassifier.Models;
using ShapeClassifier.Services;

namespace ShapeClassifier;

class Program
{
    static void Main(string[] args)
    {
        Intro();

        var dataset = new List<bool[]>();
        var labels = new List<Shape>();
        
        while (true)
        {
            var input = Console.ReadLine();

            if (input == "1")
            {
                Console.WriteLine("Preparing dataset...");
                (dataset, labels) = PrepareDataset();
                break;
            }
            else if (input == "2")
            {
                Console.WriteLine("Loading prepared dataset...");
                // Call method to load prepared dataset
                break;
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter 1 or 2.");
            }
        }
        
        ImageGenerator imageGenerator = new ImageGenerator();
        
        
        
        bool[,] image2D = new bool[224, 224];
        for (var i = 0; i < dataset[0].Length; i++)
        {
            int row = i / 224;
            int col = i % 224;
            image2D[row, col] = dataset[0][i];
        }
        
        
        //
        // Temporary code to test image generation and moving to center
        //
        FigureFunctions figureFunctions = new FigureFunctions();
        var figure = figureFunctions.MoveImageToCenter(image2D);
        
        bool[] movedImage = new bool[224 * 224];
        for (var i = 0; i < figure.GetLength(0); i++)
        {
            for (var j = 0; j < figure.GetLength(1); j++)   
            {
                movedImage[i * 224 + j] = figure[i, j];
            }
        }
        
        imageGenerator.GenerateImage(movedImage);
        
        //
        // End of temporary code
        //
        
        //loaded dataset
    }

    private static void Intro()
    {
        Console.WriteLine("Shape Classifier");
        Console.WriteLine("This program classifies shapes based on their features.\n");
        
        Console.WriteLine("1. Prepare dataset");
        Console.WriteLine("2. Load prepared dataset\n");
        Console.Write("> ");
    }

    private static (List<bool[]>, List<Shape>) PrepareDataset()
    {
        Console.Clear();
        Console.WriteLine("Input path to images:");
        Console.Write("> ");
        var pathToImages = Console.ReadLine();
        
        Console.WriteLine("Loading images...");
        
        var imagesLoader = new FileHandlers.ImagesLoader();
        var (imagesData, labels) = imagesLoader.LoadImages(pathToImages);
        
        Console.WriteLine("Done");
        
        
        return (imagesData, labels);
        
        Console.WriteLine("Enter file path to save prepared dataset:");
        Console.Write("> ");
        
    }

}