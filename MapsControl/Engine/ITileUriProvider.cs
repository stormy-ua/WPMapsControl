using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapsControl.Engine
{
    public interface ITileUriProvider
    {
        Uri GetTileUri(int levelOfDetail, int x, int y);
    }
}
