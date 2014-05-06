﻿using System;
using System.Threading.Tasks;

namespace MapsControl.Engine
{
    public interface ITileSourceProvider
    {
        TileSource GetTileSource(int levelOfDetail, int x, int y);

        Task<TileSource> GetTileSourceAsync(int levelOfDetail, int x, int y);
    }
}
