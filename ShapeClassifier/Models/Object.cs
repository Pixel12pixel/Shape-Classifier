namespace ShapeClassifier.Models;

public class Figure
{
    public bool[,] img { get; set; }
    public float area { get; set; }
    public float height { get; set; }
    public float width { get; set; }
    public float aspect_ratio { get; set; }
    public float perimeter { get; set; }
    public float symmetry { get; set; }
    public float row_variance  { get; set; }
    public float col_variance { get; set; }
    public Shape shape { get; set; }
}