using System.Threading.Tasks;

namespace MapsControl.Engine
{
    public interface ITileLoader
    {
        Task LoadAsync(Tile tile);
    }
}