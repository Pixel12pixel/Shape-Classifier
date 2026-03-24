using System;
using System.Collections.Generic;
using ShapeClassifier.Models;

namespace ShapeClassifier.Services;

public class LabelRecogniser
{
    /// <summary>
    /// Recognises shape labels from a list of strings. The method iterates through each label in the input list, extracts the shape name by removing any suffixes (e.g., "_1", "-2"), and attempts to parse it into a Shape enum value. If the parsing is successful, the corresponding Shape is added to the recognisedLabels list; otherwise, Shape.Unknown is added. The method returns a list of recognised shapes based on the input labels.
    /// </summary>
    public List<Shape> RecogniseLabels(List<string> labels)
    {
        var recognisedLabels = new List<Shape>();
        
        foreach (var label in labels)
        {
            var endIndex = label.IndexOfAny(new[] { '_', '-' });
            
            var cleanLabel = "";
            
            if (endIndex > 0)
            {
                cleanLabel = label.Substring(0, endIndex);
            }
            
            
            if (Enum.TryParse(cleanLabel, true, out Shape shape))
            {
                recognisedLabels.Add(shape);
            }
            else
            {
                recognisedLabels.Add(Shape.Unknown);
            }
        }

        return recognisedLabels;
    }
}