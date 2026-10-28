namespace Labyrinth.Build
{
    public class StartEventArgs(int x, int y) : EventArgs
    {
        public int X { get; } = x;

        public int Y { get; } = y;
    }
}
