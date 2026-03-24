

namespace ShapeClassifier.Services;

public class FigureFunctions
{
        public (double cX, double cY, double area) CalculateCentersAndAreasAsync(bool[,] images)
        {
            var results = (0.0, 0.0, 0.0);
            {
                var img = images;
                int rows = img.GetLength(0);
                int cols = img.GetLength(1);

                double sumX = 0, sumY = 0, area = 0;
                for (int y = 0; y < rows; y++)
                {
                    for (int x = 0; x < cols; x++)
                    {
                        if (img[y, x])
                        {
                            sumX += x;
                            sumY += y;
                            area++;
                        }
                    }
                }

                if (area == 0)
                    results = (0, 0, 0);
                else
                    results = (sumX / area, sumY / area, area);
            }
            return results;
        }

        public float CalculateRadialRatiosAsync(bool[,] img, (double cX, double cY, double area) metric)
        {
            float results;
            int rows = img.GetLength(0);
            int cols = img.GetLength(1);

            if (metric.area == 0) { results = 0; return 0; }

            double cX = metric.cX;
            double cY = metric.cY;

            double maxD = 0;
            double minD = double.MaxValue;

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    if (img[y, x])
                    {
                        if (y == 0 || !img[y - 1, x] || y == rows - 1 || !img[y + 1, x] ||
                            x == 0 || !img[y, x - 1] || x == cols - 1 || !img[y, x + 1])
                        {
                            double dist = Math.Sqrt(Math.Pow(x - cX, 2) + Math.Pow(y - cY, 2));
                            if (dist > maxD) maxD = dist;
                            if (dist < minD) minD = dist;
                        }
                    }
                }
            }
            results = (minD > 0) ? (float)(maxD / minD) : 0;
            return results;
        }

        public float CalculateExtentsAsync(bool[,] img, double areas)
        {
            float results;
            int rows = img.GetLength(0);
            int cols = img.GetLength(1);

            int minX = cols, maxX = 0, minY = rows, maxY = 0;

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

            results = (boundingBoxArea > 0) ? (float)areas / boundingBoxArea : 0;
            return results;
        }

        public float CalculateInertiaRatiosAsync(bool[,] img, (double cX, double cY, double area) metric)
        {
            float results;
            int rows = img.GetLength(0);
            int cols = img.GetLength(1);

            if (metric.area == 0) return 0;

            double cX = metric.cX, cY = metric.cY;
            double m00 = metric.area;
            double mu20 = 0, mu02 = 0, mu11 = 0;

            for (int y = 0; y < rows; y++)
                for (int x = 0; x < cols; x++)
                    if (img[y, x])
                    {
                        mu20 += Math.Pow(x - cX, 2);
                        mu02 += Math.Pow(y - cY, 2);
                        mu11 += (x - cX) * (y - cY);
                    }

            double common = Math.Sqrt(Math.Pow(mu20 - mu02, 2) + 4 * Math.Pow(mu11, 2));
            double axisMajor = Math.Sqrt(2 * (mu20 + mu02 + common) / m00);
            double axisMinor = Math.Sqrt(2 * (mu20 + mu02 - common) / m00);
            results = (axisMinor > 0) ? (float)(axisMajor / axisMinor) : 1f;
            return results;
        }



        public float CalculateCentralSymmetryAsync(bool[,] img, (double cX, double cY, double area) metric)
        {
            float results;
            int rows = img.GetLength(0);
            int cols = img.GetLength(1);

            if (metric.area == 0) { results = 0; return 0; }

            double cX = metric.cX;
            double cY = metric.cY;

            int symmetricPoints = 0;
            int totalPoints = 0;

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    if (img[y, x])
                    {
                        totalPoints++;
                        int oppX = (int)Math.Round(2 * cX - x);
                        int oppY = (int)Math.Round(2 * cY - y);

                        if (oppX >= 0 && oppX < cols && oppY >= 0 && oppY < rows)
                            if (img[oppY, oppX]) symmetricPoints++;
                    }
                }
            }
            results = (float)symmetricPoints / totalPoints;
            return results;
        }
}