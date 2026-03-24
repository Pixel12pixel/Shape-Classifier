using System;
using System.Collections;
using System.Collections.Generic;
using ShapeClassifier.Models;
using ShapeClassifier.Services;
using Object = System.Object;
using ShapeClassifier.Converters;
using BitConverter = ShapeClassifier.Converters.BitConverter;

namespace ShapeClassifier;

class Program
{
    static async Task Main(string[] args)
    {
        
        Intro();

        ulong[][] dataset = new ulong[][] { };
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

        Console.ReadLine();
        
        var figureFunctions = new FigureFunctions();
        
        var bitConverter = new BitConverter();
        
        
        var radialRatios = new float[dataset.Length];
        
        var extents = new float[dataset.Length];
        
        var inertiaRatios = new float[dataset.Length];
        
        var centralSymmetry = new float[dataset.Length];


        int processedCount = 0;
        
        Parallel.For(0, dataset.Length, new ParallelOptions
        {
            MaxDegreeOfParallelism = Environment.ProcessorCount
        }, i =>
        {
            var img = bitConverter.ToBits(dataset[i]);
            var metric = figureFunctions.CalculateCentersAndAreasAsync(img);
            radialRatios[i] = figureFunctions.CalculateRadialRatiosAsync(img, metric);
            extents[i] = figureFunctions.CalculateExtentsAsync(img, metric.area);
            inertiaRatios[i] = figureFunctions.CalculateInertiaRatiosAsync(img, metric);
            centralSymmetry[i] = figureFunctions.CalculateCentralSymmetryAsync(img, metric);
            var current = Interlocked.Increment(ref processedCount);
            if (current % 1000 == 0)
            {
                Console.WriteLine($"Processed {current} / {dataset.Length}");
            }
        });
        
        
        //figureFunctions.normalize_minmax(perimeters);
        
        //figureFunctions.normalize_minmax(areas);
        
        CSVConverter csvConverter = new CSVConverter();
        

        
        csvConverter.SaveToCsv(radialRatios, nameof(radialRatios));
        
        csvConverter.SaveToCsv(centralSymmetry, nameof(centralSymmetry));
        
        csvConverter.SaveToCsv(inertiaRatios, nameof(inertiaRatios));
        
        csvConverter.SaveToCsv(extents, nameof(extents));
        
        csvConverter.SaveToCsv(labels, "shape");
        
        csvConverter.MergeCsvFiles();

        //var (bestAverage, smallestImpurity) = figureFunctions.GiniImpurity(perimeters, labels);
        //var (bestAverage2, smallestImpurity2) = figureFunctions.GiniImpurity(areas, labels);
        
        //Console.WriteLine($"Best average is {bestAverage}");
        //Console.WriteLine($"Smallest impurity is {smallestImpurity}");
        //Console.WriteLine($"Best average is {bestAverage2}");
        //Console.WriteLine($"Smallest impurity is {smallestImpurity2}");

    }

    private static void Intro()
    {
        Console.WriteLine("Shape Classifier");
        Console.WriteLine("This program classifies shapes based on their features.\n");
        
        Console.WriteLine("1. Prepare dataset");
        Console.WriteLine("2. Load prepared dataset\n");
        Console.Write("> ");
    }

    private static (ulong[][], List<Shape>) PrepareDataset()
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