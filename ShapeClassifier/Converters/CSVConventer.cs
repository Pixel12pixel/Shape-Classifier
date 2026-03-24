using System.Globalization;
using ShapeClassifier.Models;

namespace ShapeClassifier.Converters;

public class CSVConverter
{
    
    public void SaveToCsv(List<float> values, string trait)
    {
        string path =$@"D:\Projekty\SystemySztucznejInteligencji\ProjektZaliczeniowy_Dorian\ShapeClassifier\CSVFiles\traits\{trait}.csv";
        using var writer = new StreamWriter(path);
        writer.WriteLine(trait);

        foreach (var v in values)
        {
            writer.WriteLine(v.ToString(CultureInfo.InvariantCulture));
        }
        
    }
    
    public void SaveToCsv(List<Shape> values, string trait)
    {
        string path =$@"D:\Projekty\SystemySztucznejInteligencji\ProjektZaliczeniowy_Dorian\ShapeClassifier\CSVFiles\traits\{trait}.csv";
        using var writer = new StreamWriter(path);
        writer.WriteLine(trait);

        foreach (var v in values)
        {
            writer.WriteLine((int)v);
        }
        
    }

    public void MergeCsvFiles()
    {
        string folder = @"D:\Projekty\SystemySztucznejInteligencji\ProjektZaliczeniowy_Dorian\ShapeClassifier\CSVFiles\traits";
        string path = @"D:\Projekty\SystemySztucznejInteligencji\ProjektZaliczeniowy_Dorian\ShapeClassifier\CSVFiles\MergedTraits.csv";
        
        var files =  Directory.GetFiles(folder);
        
        var data = new List<List<float>>();
        var headers = new List<string>();

        foreach (var file in files)
        {
            var lines = File.ReadAllLines(file);
            
            string header = lines[0];
            
            var values = lines.Skip(1).Select(line => float.Parse(line, CultureInfo.InvariantCulture)).ToList();
            
            data.Add(values);
            headers.Add(header);
        }
        
        int maxRows = data.Max(column => column.Count);
        
        using var writer = new StreamWriter(path);
        
        writer.WriteLine(String.Join(",", headers));

        for (int i = 0; i < maxRows; i++)
        {
            var row = new List<string>();

            foreach (var column in data)
            {
                if (i < column.Count)
                {
                    row.Add(column[i].ToString(CultureInfo.InvariantCulture));
                }
                else row.Add(string.Empty);
            }
            
            writer.WriteLine(String.Join(",", row));
        }
    }
}