using Labyrinth.Items;
using Labyrinth.Tiles;


namespace Labyrinth.Crawl
{
    public class Crawler(int X, int Y, Direction direction, Tile[,] tiles) : ICrawler
    {
        int ICrawler.X => X;

        int ICrawler.Y => Y;

        Direction ICrawler.Direction => direction;

        Tile ICrawler.FacingTile
        {
            get
            {
                int facingX = X + direction.DeltaX;
                int facingY = Y + direction.DeltaY;

                return IsOut(facingX, 0) || IsOut(facingY, 1)
                    ? Outside.Singleton
                    : tiles[facingX, facingY];
            }
        }

        public Inventory Walk()
        {
            var inventory = tiles[X + direction.DeltaX, Y + direction.DeltaY].Pass();
            X += direction.DeltaX;
            Y += direction.DeltaY;
            return inventory;
        }


        private bool IsOut(int pos, int dimension) => pos < 0 || pos >= tiles.GetLength(dimension);

        private int X = X;
        private int Y = Y;

        private Direction direction = direction;
    }
}