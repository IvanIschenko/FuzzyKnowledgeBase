﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebApplication1.FuzzyLogicBase;
using C__04._07._2015.Algorithms.Clustering;// progr_V.cs
using L.DataStructures.Matrix;// multidim..cs
using L.Algorithms.Clustering;// k_means.cs
using L.DataStructures.Geometry; // cluster.cs
using WebApplication1;

namespace WebApplication1
{
    public interface IMetagraphTextParser<T, K>
    {
        MetaGraph<T, K> GenerateMetagraph(string text);
    }
}
