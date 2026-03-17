using System.Drawing;

namespace ShapeClassifier.Services;

public class ImageGenerator
{
    const int ImageSize = 224;
    
    public void GenerateImage(bool[] image)
    {
            var bitmap = new Bitmap(ImageSize, ImageSize);
            
            for (var i = 0; i < ImageSize; i++)
            {
                for (var j = 0; j < ImageSize; j++)
                {
                    var pixelValue = image[i * ImageSize + j] ? Color.Black : Color.White;
                    bitmap.SetPixel(j, i, pixelValue);
                }
            }
            
            bitmap.Save("generated_image.png");
    }
}