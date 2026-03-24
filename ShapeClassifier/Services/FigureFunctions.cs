using System;
using System.Globalization;
using ShapeClassifier.Models;


namespace ShapeClassifier.Services;

public class FigureFunctions
{
    private int imageCenterX = 112;
    private int imageCenterY = 112;
    private int imageWidth = 224;
    private int imageHeight = 224;

    public bool[,] MoveImageToCenter(bool[,] img)
    {
        var (dx, dy) = CenterOfMass(img);
        
        if(dx == 0 && dy == 0) return img;
        
        bool[,] newImage = new bool[imageWidth, imageHeight];
        
        for(int y = 0; y < imageHeight; y++)
        {
            for(int x = 0; x < imageWidth; x++)
            {
                int newX = x + dx;
                int newY = y + dy;
                
                if(newX >= 0 && newX < imageWidth && newY >= 0 && newY < imageHeight)
                {
                    newImage[newY, newX] = img[y, x];
                }
            }
        }

        return newImage;
    }
    
    private (int, int) CenterOfMass(bool[,] img)
    {
        float sumX = 0;
        float sumY = 0;
        int count = 0;

        for (int y = 0; y < 224; y++)
        {
            for (int x = 0; x < 224; x++)
            {
                if (img[y, x])
                {
                    sumX += x;
                    sumY += y;
                    count++;
                }
            }
        }
        
        if(count == 0) return (0, 0);
        
        var center = ((int)Math.Round(sumX / count), (int)Math.Round(sumY / count));
        
        int dx = center.Item1 -  imageCenterX;
        int dy = center.Item2 -  imageCenterY;
        
        return (-dx, -dy);
    }
    
    public async Task<List<float>> CalculatePerimeters(List<bool[,]> images)
{
    if (images == null || images.Count == 0) return new List<float>();

    float[] results = new float[images.Count];

    await Task.Run(() =>
    {
        // Parallel.For rozdziela pracę na wszystkie rdzenie CPU
        Parallel.For(0, images.Count, i =>
        {
            var img = images[i];
            int rows = img.GetLength(0);
            int cols = img.GetLength(1);
            double perimeter = 0;

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    // Jeśli to tło, skip
                    if (!img[y, x]) continue;

                    // Pobieramy sąsiadów (true = figura, false = tło/krawędź)
                    bool n = (y > 0) && img[y - 1, x];
                    bool s = (y < rows - 1) && img[y + 1, x];
                    bool w = (x > 0) && img[y, x - 1];
                    bool e = (x < cols - 1) && img[y, x + 1];

                    // Krawędzie proste (kardynalne)
                    if (!n) perimeter += 1.0;
                    if (!s) perimeter += 1.0;
                    if (!w) perimeter += 1.0;
                    if (!e) perimeter += 1.0;

                    // Pobieramy sąsiadów po skosie
                    bool ne = (y > 0 && x < cols - 1) && img[y - 1, x + 1];
                    bool nw = (y > 0 && x > 0) && img[y - 1, x - 1];
                    bool se = (y < rows - 1 && x < cols - 1) && img[y + 1, x + 1];
                    bool sw = (y < rows - 1 && x > 0) && img[y + 1, x - 1];

                    // Korekta narożników (0.414 to przybliżenie sqrt(2) - 1)
                    // Ta część sprawia, że obrócony kwadrat ma taki sam obwód jak prosty
                    if (!n && !w && !nw) perimeter += 0.414;
                    if (!n && !e && !ne) perimeter += 0.414;
                    if (!s && !w && !sw) perimeter += 0.414;
                    if (!s && !e && !se) perimeter += 0.414;
                }
            }
            results[i] = (float)perimeter;
        });
    });

    return results.ToList();
}
    
    public async Task<List<float>> CalculateFields(List<bool[,]> images)
    {
        if (images.Count == 0) return new List<float>();
        
        float[] resultsArray = new float[images.Count];

        await Task.Run(() =>
        {
            Parallel.For(0, images.Count, i =>
            {
                resultsArray[i] = CalculateSingleField(images[i]);
            });
        });
        
        return resultsArray.ToList();
    }

    private float CalculateSingleField(bool[,] image)
    {
        int area = 0;
        int rows = image.GetLength(0);
        int cols = image.GetLength(1);
        
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                if (image[y, x])
                {
                    area++;
                }
            }
        }
        return (float)area;
    }

    public void normalize_minmax(List<float> values)
    {
        float max = float.MinValue;
        float min = float.MaxValue;
        
        foreach (var v in values)
        {
            if (v > max) max = v;
            if (v < min) min = v;
        }
        float diff =  max - min;

        if (diff == 0) return;

        for (int i = 0; i < values.Count; i++)
        {
            values[i] = (values[i] - min)/diff;
        }
    }

    public (float,float) GiniImpurity(List<float> values, List<Shape> shapes)
    {
        var sorted = values.Select((v, idx) => (value: v, shape: shapes[idx])).OrderBy(x => x.value).ToList();
        
        float bestAverage = 0;
        float smallestImpurity = float.MaxValue;
        Dictionary<Shape, int> leftShapesDic =  new Dictionary<Shape, int>();
        Dictionary<Shape, int> rightShapesDic =  new Dictionary<Shape, int>();
        
        for (int i = 1; i < values.Count; i++)
        {
            float average = (sorted[i].value+sorted[i-1].value)/2;

            for (int l = 0; l < values.Count; l++)
            {
                if (sorted[l].value < average)
                {
                    if (leftShapesDic.ContainsKey(sorted[l].shape))
                    {
                        leftShapesDic[sorted[l].shape]++;
                    }
                    else
                    {
                        leftShapesDic[sorted[l].shape] = 1;
                    }
                }
                else
                {
                    if (rightShapesDic.ContainsKey(sorted[l].shape))
                    {
                        rightShapesDic[sorted[l].shape]++;
                    }
                    else
                    {
                        rightShapesDic[sorted[l].shape] = 1;
                    }
                }
            }
            
            float leftImpurity = Impurity(leftShapesDic);
            float rightImpurity = Impurity(rightShapesDic);
            
            float leftCount = leftShapesDic.Values.Sum();
            float rightCount = rightShapesDic.Values.Sum();
            
            float totalImpurity = leftCount/values.Count*leftImpurity + rightCount/values.Count*rightImpurity;

            if (totalImpurity < smallestImpurity)
            {
                smallestImpurity = totalImpurity;
                bestAverage = average;
            }
            
            leftShapesDic.Clear();
            rightShapesDic.Clear();
        }
        
        return (bestAverage, smallestImpurity);
    }

    private float Impurity(Dictionary<Shape, int> shapesDic)
    {
        float impurity = 1;
        int shapesDicCount =  shapesDic.Values.Sum();

        if (shapesDicCount == 0) return 0;

        foreach (var values in shapesDic.Values)
        {
            float p = (float)values / shapesDicCount;
            impurity -= p * p;
        }
        return impurity;
    }
    
    public async Task<List<float>> CalculateCircularity(string areasCsv, string perimetersCsv)
    {
        string basePath = @"D:\Projekty\SystemySztucznejInteligencji\ProjektZaliczeniowy_Dorian\ShapeClassifier\CSVFiles\traits";
        string areasPath = Path.Combine(basePath, areasCsv);
        string perimetersPath = Path.Combine(basePath, perimetersCsv);

        if (!File.Exists(areasPath) || !File.Exists(perimetersPath))
            return null;
        
        var areasTask = File.ReadAllLinesAsync(areasPath);
        var perimetersTask = File.ReadAllLinesAsync(perimetersPath);

        await Task.WhenAll(areasTask, perimetersTask);

        var areasLines = areasTask.Result;
        var perimetersLines = perimetersTask.Result;
        
        int count = Math.Min(areasLines.Length, perimetersLines.Length) - 1;
        if (count <= 0) return new List<float>();
        
        var circularityList = new List<float>(count);
        
        const float fourPi = 4f * (float)Math.PI;

        for (int i = 1; i <= count; i++)
        {
            if (float.TryParse(areasLines[i], CultureInfo.InvariantCulture, out float area) &&
                float.TryParse(perimetersLines[i], CultureInfo.InvariantCulture, out float perimeter))
            {
                if (perimeter > 0)
                {
                    float circularity = (fourPi * area) / (perimeter * perimeter);
                    circularityList.Add(circularity);
                }
                else circularityList.Add(0f);
            }
            else circularityList.Add(0f);
        }
        return circularityList;
    }
    
    public async Task<List<float>> CalculateRadialRatiosAsync(List<bool[,]> images)
    {
        float[] results = new float[images.Count];

        await Task.Run(() =>
        {
            Parallel.For(0, images.Count, i =>
            {
                var img = images[i];
                int rows = img.GetLength(0);
                int cols = img.GetLength(1);

                long sumX = 0, sumY = 0, count = 0;
                // 1. Znajdź środek ciężkości
                for (int y = 0; y < rows; y++)
                for (int x = 0; x < cols; x++)
                    if (img[y, x]) { sumX += x; sumY += y; count++; }

                if (count == 0) { results[i] = 0; return; }
                float cX = (float)sumX / count;
                float cY = (float)sumY / count;

                double maxD = 0;
                double minD = double.MaxValue;

                // 2. Szukaj dystansów krawędzi od środka
                for (int y = 0; y < rows; y++)
                {
                    for (int x = 0; x < cols; x++)
                    {
                        if (img[y, x])
                        {
                            // Prosty test krawędzi: czy ma pustego sąsiada?
                            if (y == 0 || !img[y-1,x] || y == rows-1 || !img[y+1,x] ||
                                x == 0 || !img[y,x-1] || x == cols-1 || !img[y,x+1])
                            {
                                double dist = Math.Sqrt(Math.Pow(x - cX, 2) + Math.Pow(y - cY, 2));
                                if (dist > maxD) maxD = dist;
                                if (dist < minD) minD = dist;
                            }
                        }
                    }
                }
                results[i] = (minD > 0) ? (float)(maxD / minD) : 0;
            });
        });
        return results.ToList();
    }
    
    public async Task<List<float>> CalculateExtentsAsync(List<bool[,]> images, List<float> areas)
    {
        float[] results = new float[images.Count];

        await Task.Run(() =>
        {
            Parallel.For(0, images.Count, i =>
            {
                var img = images[i];
                int rows = img.GetLength(0);
                int cols = img.GetLength(1);

                int minX = cols, maxX = 0, minY = rows, maxY = 0;

                // Znajdź granice figury (Bounding Box)
                for (int y = 0; y < rows; y++)
                {
                    for (int x = 0; x < cols; x++)
                    {
                        if (img[y, x])
                        {
                            if (x < minX) minX = x; if (x > maxX) maxX = x;
                            if (y < minY) minY = y; if (y > maxY) maxY = y;
                        }
                    }
                }

                float width = maxX - minX + 1;
                float height = maxY - minY + 1;
                float boundingBoxArea = width * height;

                results[i] = (boundingBoxArea > 0) ? areas[i] / boundingBoxArea : 0;
            });
        });
        return results.ToList();
    }
    
    public async Task<List<float>> CalculateInertiaRatiosAsync(List<bool[,]> images)
    {
        float[] results = new float[images.Count];
        await Task.Run(() => {
            Parallel.For(0, images.Count, i => {
                var img = images[i];
                double m00 = 0, m10 = 0, m01 = 0;
                for (int y = 0; y < 224; y++)
                for (int x = 0; x < 224; x++)
                    if (img[y, x]) { m00++; m10 += x; m01 += y; }

                if (m00 == 0) return;
                double cX = m10 / m00, cY = m01 / m00;
                double mu20 = 0, mu02 = 0, mu11 = 0;

                for (int y = 0; y < 224; y++)
                for (int x = 0; x < 224; x++)
                    if (img[y, x]) {
                        mu20 += Math.Pow(x - cX, 2);
                        mu02 += Math.Pow(y - cY, 2);
                        mu11 += (x - cX) * (y - cY);
                    }

                double common = Math.Sqrt(Math.Pow(mu20 - mu02, 2) + 4 * Math.Pow(mu11, 2));
                double axisMajor = Math.Sqrt(2 * (mu20 + mu02 + common) / m00);
                double axisMinor = Math.Sqrt(2 * (mu20 + mu02 - common) / m00);
                results[i] = (axisMinor > 0) ? (float)(axisMajor / axisMinor) : 1f;
            });
        });
        return results.ToList();
    }
    
    public async Task<List<float>> CalculatePeakCountsAsync(List<bool[,]> images)
    {
        float[] results = new float[images.Count];
        await Task.Run(() =>
        {
            Parallel.For(0, images.Count, i =>
            {
                var img = images[i];
                int rows = img.GetLength(0);
                int cols = img.GetLength(1);

                double m00 = 0, m10 = 0, m01 = 0;
                for (int y = 0; y < rows; y++)
                    for (int x = 0; x < cols; x++)
                        if (img[y, x]) { m00++; m10 += x; m01 += y; }

                if (m00 == 0) { results[i] = 0; return; }
                double cX = m10 / m00;
                double cY = m01 / m00;

                float[] radial = new float[360];
                for (int y = 0; y < rows; y++)
                {
                    for (int x = 0; x < cols; x++)
                    {
                        if (!img[y, x]) continue;
                        double dx = x - cX;
                        double dy = y - cY;
                        int angle = (int)((Math.Atan2(dy, dx) * 180.0 / Math.PI) + 360) % 360;
                        float dist = (float)Math.Sqrt(dx * dx + dy * dy);
                        if (dist > radial[angle]) radial[angle] = dist;
                    }
                }

                float[] smooth = new float[360];
                int window = 7;
                for (int a = 0; a < 360; a++)
                {
                    float sum = 0; int count = 0;
                    for (int k = -window; k <= window; k++)
                    {
                        int idx = (a + k + 360) % 360;
                        sum += radial[idx]; count++;
                    }
                    smooth[a] = sum / count;
                }

                float maxVal = smooth.Max();
                if (maxVal > 0)
                {
                    for (int a = 0; a < 360; a++) smooth[a] /= maxVal;
                }

                int peaks = 0;
                float threshold = 0.2f;
                for (int a = 0; a < 360; a++)
                {
                    int prev = (a - 1 + 360) % 360;
                    int next = (a + 1) % 360;
                    if (smooth[a] > smooth[prev] && smooth[a] > smooth[next] && smooth[a] > threshold) peaks++;
                }
                results[i] = peaks;
            });
        });
        return results.ToList();
    }
    
    public async Task<List<float>> CalculateCentralSymmetryAsync(List<bool[,]> images)
    {
        float[] results = new float[images.Count];

        await Task.Run(() => {
            Parallel.For(0, images.Count, i => {
                var img = images[i];
            
                // 1. Wyznaczamy środek ciężkości (Centroid)
                double m10 = 0, m01 = 0, m00 = 0;
                for (int y = 0; y < 224; y++) {
                    for (int x = 0; x < 224; x++) {
                        if (img[y, x]) {
                            m00++; m10 += x; m01 += y;
                        }
                    }
                }

                if (m00 == 0) { results[i] = 0; return; }
                double cX = m10 / m00;
                double cY = m01 / m00;

                // 2. Sprawdzamy symetrię
                int symmetricPoints = 0;
                int totalPoints = 0;

                for (int y = 0; y < 224; y++) {
                    for (int x = 0; x < 224; x++) {
                        if (img[y, x]) {
                            totalPoints++;

                            // Obliczamy współrzędne punktu odbitego względem środka
                            // Wzór: x' = 2*cX - x, y' = 2*cY - y
                            int oppX = (int)Math.Round(2 * cX - x);
                            int oppY = (int)Math.Round(2 * cY - y);

                            // Sprawdzamy czy punkt odbity mieści się w obrazku i czy jest "zamalowany"
                            if (oppX >= 0 && oppX < 224 && oppY >= 0 && oppY < 224) {
                                if (img[oppY, oppX]) {
                                    symmetricPoints++;
                                }
                            }
                        }
                    }
                }

                // Wynik to stosunek punktów posiadających parę do wszystkich punktów
                // 1.0 = idealna symetria, blisko 0 = brak symetrii
                results[i] = (float)symmetricPoints / totalPoints;
            });
        });

        return results.ToList();
    }
    
    public (float threshold, float impurity) GiniImpurityFast(List<float> values, List<Shape> shapes)
    {
        int n = values.Count;

        var sorted = values
            .Select((v, i) => (value: v, shape: shapes[i]))
            .OrderBy(x => x.value)
            .ToList();

        // 🔹 total counts
        Dictionary<Shape, int> rightCounts = new();
        foreach (var s in shapes)
        {
            if (rightCounts.ContainsKey(s)) rightCounts[s]++;
            else rightCounts[s] = 1;
        }

        Dictionary<Shape, int> leftCounts = new();

        float bestThreshold = 0;
        float bestImpurity = float.MaxValue;

        int leftSize = 0;
        int rightSize = n;

        for (int i = 0; i < n - 1; i++)
        {
            var shape = sorted[i].shape;

            // 🔹 move element from right → left
            if (!leftCounts.ContainsKey(shape)) leftCounts[shape] = 0;
            leftCounts[shape]++;

            rightCounts[shape]--;
            if (rightCounts[shape] == 0) rightCounts.Remove(shape);

            leftSize++;
            rightSize--;

            // 🔹 pomijamy identyczne wartości
            if (sorted[i].value == sorted[i + 1].value) continue;

            float threshold = (sorted[i].value + sorted[i + 1].value) / 2;

            float leftImp = ComputeGini(leftCounts, leftSize);
            float rightImp = ComputeGini(rightCounts, rightSize);

            float totalImp = (leftSize / (float)n) * leftImp +
                             (rightSize / (float)n) * rightImp;

            if (totalImp < bestImpurity)
            {
                bestImpurity = totalImp;
                bestThreshold = threshold;
            }
        }

        return (bestThreshold, bestImpurity);
    }
    
    private float ComputeGini(Dictionary<Shape, int> counts, int total)
    {
        if (total == 0) return 0;

        float impurity = 1f;

        foreach (var c in counts.Values)
        {
            float p = c / (float)total;
            impurity -= p * p;
        }

        return impurity;
    }
    
    public void StandardScale(List<float> values)
    {
        int n = values.Count;
        if (n == 0) return;

        // 🔹 mean
        double sum = 0;
        for (int i = 0; i < n; i++)
            sum += values[i];

        double mean = sum / n;

        // 🔹 std
        double variance = 0;
        for (int i = 0; i < n; i++)
        {
            double diff = values[i] - mean;
            variance += diff * diff;
        }

        variance /= n;
        double std = Math.Sqrt(variance);

        if (std == 0) return;

        // 🔹 skalowanie
        for (int i = 0; i < n; i++)
        {
            values[i] = (float)((values[i] - mean) / std);
        }
    }
}