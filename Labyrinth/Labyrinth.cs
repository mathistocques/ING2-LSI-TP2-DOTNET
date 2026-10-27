using Labyrinth.Build;
using Labyrinth.Crawl;
using Labyrinth.Tiles;
using System.Text;

namespace Labyrinth
{
    public partial class Labyrinth
    {
        /// <summary>
        /// Labyrinth with walls, doors and collectable items.
        /// </summary>
        /// <param name="builder">Builder used to create the labyrinth.</param>
        /// <exception cref="ArgumentException">Thrown when string argument reveals inconsistent map sizes or characters with no starting position.</exception>
        /// <exception cref="NotSupportedException">Thrown for multiple doors (resp. key locations) before key locations (resp. doors).</exception>
        public Labyrinth(IBuilder builder)
        {
            _tiles = builder.Build(out _startPosition);
            if (_tiles.GetLength(0) < 3 || _tiles.GetLength(1) < 3)
            {
                throw new ArgumentException("Labyrinth must be at least 3x3");
            }
            if (_startPosition == IBuilder.InvalidPosition)
            {
                throw new ArgumentException("Labyrinth must have at least one starting position marked with x");
            }
        }

        /// <summary>
        /// Labyrinth width (number of columns).
        /// </summary>
        public int Width { get; private init; }

        /// <summary>
        /// Labyrinth height (number of rows).
        /// </summary>
        public int Height { get; private init; }

        /// <summary>
        /// An ascii representation of the labyrinth.
        /// </summary>
        /// <returns>Formatted string</returns>
        public override string ToString() => ToString(_tiles);

        public static string ToString(Tile[,] tiles)
        {
            var res = new StringBuilder();

            for (int y = 0; y < tiles.GetLength(1); y++)
            {
                for (int x = 0; x < tiles.GetLength(0); x++)
                {
                    res.Append(tiles[x, y] switch
                    {
                        Room => ' ',
                        Wall => '#',
                        Door => '/',
                        _ => throw new NotSupportedException("Unknown tile type")
                    });
                }
                res.AppendLine();
            }
            return res.ToString();
        }

        public ICrawler NewCrawler() => 
            new Crawler(
            _startPosition.Item1, 
            _startPosition.Item2, 
            Direction.North, 
            _startPosition.Item1 < 0 || _startPosition.Item2 - 1 < 0 ? Outside.Singleton : _tiles[_startPosition.Item1, _startPosition.Item2 +1]
            );

        private readonly Tile[,] _tiles;
        private (int, int) _startPosition;
    }
}