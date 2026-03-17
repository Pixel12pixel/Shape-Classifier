

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
        
        if(dx == 0 || dy == 0) return img;
        
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
}