namespace ShapeClassifier;

class Program
{
    static void Main(string[] args)
    {
        Intro();
    }

    private static void Intro()
    {
        Console.WriteLine("Shape Classifier");
        Console.WriteLine("This program classifies shapes based on their features.\n");
        
        Console.WriteLine("1. Prepare dataset");
        Console.WriteLine("2. Load prepared dataset");
    }
}