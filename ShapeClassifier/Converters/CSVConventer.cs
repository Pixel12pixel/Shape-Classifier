using System.Globalization;
using ShapeClassifier.Models;

namespace ShapeClassifier.Converters;

public class CSVConverter
{
    private const string BasePath = @"[PATH]";
    private const string Traits = "traits";
    private const string Marged = "MergedTraits.csv";
    
    public void SaveToCsv(float[] values, string trait)
    {
        string path = Path.Combine(BasePath, Traits, trait + ".csv");
        using var writer = new StreamWriter(path);
        writer.WriteLine(trait);

        foreach (var v in values)
        {
            writer.WriteLine(v.ToString(CultureInfo.InvariantCulture));
        }
        
    }
    
    public void SaveToCsv(List<Shape> values, string trait)
    {
        string path = Path.Combine(BasePath, Traits, trait + ".csv");
        using var writer = new StreamWriter(path);
        writer.WriteLine(trait);

        foreach (var v in values)
        {
            writer.WriteLine((int)v);
        }
        
    }

    public void MergeCsvFiles()
    {
        string folder = Path.Combine(BasePath, Traits);
        string path = Path.Combine(BasePath, Marged);
        
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