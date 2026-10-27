using Labyrinth.Items;
using Labyrinth.Tiles;


namespace Labyrinth.Crawl
{
    public class Crawler(int X, int Y, Direction direction, Tile facingTile) : ICrawler
    {
        int ICrawler.X => X;

        int ICrawler.Y => Y;

        Direction ICrawler.Direction => direction;

        Tile ICrawler.FacingTile => facingTile;

        public Inventory Walk()
        {
            throw new NotImplementedException();
        }
    }
}