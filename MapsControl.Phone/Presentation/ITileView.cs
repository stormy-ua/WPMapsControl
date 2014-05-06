using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapsControl.Engine;

namespace MapsControl.Presentation
{
    public interface ITileView : IMapEntityView
    {
        TileSource TileSource { get; set; }
    }
}
