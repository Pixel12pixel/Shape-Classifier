using ShapeClassifier.Models;
using ShapeClassifier.Services;
using Object = System.Object;
using ShapeClassifier.Converters;

namespace ShapeClassifier;

class Program
{
    static async Task Main(string[] args)
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
        
        List<bool[,]> datasetDiv = new List<bool[,]>();

        foreach (var data in dataset)
        {
            bool[,] image2D = new bool[224, 224];
            for (var i = 0; i < data.Length; i++)
            {
                int row = i / 224;
                int col = i % 224;
                image2D[row, col] = data[i];
                
            }
            datasetDiv.Add(image2D);
        }

        FigureFunctions figureFunctions = new FigureFunctions();

        List<float> perimeters = await figureFunctions.CalculatePerimeters(datasetDiv);

        List<float> areas = await figureFunctions.CalculateFields(datasetDiv);
        
        List<float> circularity = await figureFunctions.CalculateCircularity("areas.csv", "perimeters.csv");
        
        List<float> radialRatios = await figureFunctions.CalculateRadialRatiosAsync(datasetDiv);
        
        List<float> centralSymmetry = await figureFunctions.CalculateCentralSymmetryAsync(datasetDiv);
        
        List<float> inertiaRatios = await figureFunctions.CalculateInertiaRatiosAsync(datasetDiv);
        
        List<float> peakCounts = await figureFunctions.CalculatePeakCountsAsync(datasetDiv);
        
        List<float> extents = await figureFunctions.CalculateExtentsAsync(datasetDiv, areas);
        
        //figureFunctions.normalize_minmax(perimeters);
        
        //figureFunctions.normalize_minmax(areas);
        
        CSVConverter csvConverter = new CSVConverter();
        
        csvConverter.SaveToCsv(perimeters, nameof(perimeters));
        
        csvConverter.SaveToCsv(areas, nameof(areas));
        
        csvConverter.SaveToCsv(circularity, nameof(circularity));
        
        csvConverter.SaveToCsv(radialRatios, nameof(radialRatios));
        
        csvConverter.SaveToCsv(centralSymmetry, nameof(centralSymmetry));
        
        csvConverter.SaveToCsv(inertiaRatios, nameof(inertiaRatios));
        
        csvConverter.SaveToCsv(peakCounts, nameof(peakCounts));
        
        csvConverter.SaveToCsv(extents, nameof(extents));
        
        csvConverter.SaveToCsv(labels, "shape");
        
        csvConverter.MergeCsvFiles();

        //var (bestAverage, smallestImpurity) = figureFunctions.GiniImpurity(perimeters, labels);
        //var (bestAverage2, smallestImpurity2) = figureFunctions.GiniImpurity(areas, labels);
        
        //Console.WriteLine($"Best average is {bestAverage}");
        //Console.WriteLine($"Smallest impurity is {smallestImpurity}");
        //Console.WriteLine($"Best average is {bestAverage2}");
        //Console.WriteLine($"Smallest impurity is {smallestImpurity2}");

        //ImageGenerator imageGenerator = new ImageGenerator();






        //
        // Temporary code to test image generation and moving to center
        //
        /*FigureFunctions figureFunctions = new FigureFunctions();
        var figure = figureFunctions.MoveImageToCenter(image2D);

        bool[] movedImage = new bool[224 * 224];
        for (var i = 0; i < figure.GetLength(0); i++)
        {
            for (var j = 0; j < figure.GetLength(1); j++)
            {
                movedImage[i * 224 + j] = figure[i, j];
            }
        }

        imageGenerator.GenerateImage(movedImage);*/

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
        //var pathToImages = Console.ReadLine();
        var pathToImages = @"D:\Projekty\SystemySztucznejInteligencji\ProjektZaliczeniowy_Dorian\ShapeClassifier\Shapes\shapes";
        
        Console.WriteLine("Loading images...");
        
        var imagesLoader = new FileHandlers.ImagesLoader();
        var (imagesData, labels) = imagesLoader.LoadImages(pathToImages);
        
        Console.WriteLine("Done");
        
        
        return (imagesData, labels);
        
        Console.WriteLine("Enter file path to save prepared dataset:");
        Console.Write("> ");
        
    }

}