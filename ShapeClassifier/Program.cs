using System;
using System.Collections;
using System.Collections.Generic;
using ShapeClassifier.Models;
using ShapeClassifier.Services;

namespace ShapeClassifier;

class Program
{
    static void Main(string[] args)
    {
        
        Intro();

        ulong[][] dataset;
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