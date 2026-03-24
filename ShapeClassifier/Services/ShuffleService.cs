// // -----------------------------------------------------------------------------
// // File        : SuffleService.cs
// // Project     : ShapeClassifier
// //
// // Author      : Dorian Koehler
// // Created     : 2026.03.16
// // Last Edited : 2026.03.16
// //
// // Copyright (c) 2026 Dorian Koehler
// // Licensed under the MIT License.
// // -----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

namespace ShapeClassifier.Services;

public class ShuffleService
{
    /// <summary>
    /// Shuffles a list of strings using a random order.
    /// </summary>
    /// <param name="list">List of strings to shuffle</param>
    /// <returns>Shuffled list</returns>
    public List<string> Shuffle(List<string> list)
    {
        var random = new Random();
        return list.OrderBy(x => random.Next()).ToList();
    }
}