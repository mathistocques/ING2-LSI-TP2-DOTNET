using Labyrinth.Build;
using Labyrinth.Crawl;
using Labyrinth.Items;
using Labyrinth.Tiles;

namespace LabyrinthTest.Crawl;

[TestFixture(Description = "Integration test for the crawler implementation in the labyrinth")]
public class LabyrinthCrawlerTest
{
    #region Initialization
    [Test]
    public void InitWithCenteredX()
    {
        var laby = new Labyrinth.Labyrinth(new AsciiParser("""
                +--+
                | x|
                +--+
                """));
        var test = laby.NewCrawler();

        using var all = Assert.EnterMultipleScope();

        Assert.That(test.X, Is.EqualTo(2));
        Assert.That(test.Y, Is.EqualTo(1));
        Assert.That(test.Direction, Is.EqualTo(Direction.North));
        Assert.That(test.FacingTile, Is.TypeOf<Wall>());
    }

    [Test]
    public void InitWithMultipleXUsesLastOne()
    {
        var laby = new Labyrinth.Labyrinth(new AsciiParser("""
                +--+
                | x|
                | x|
                +--+
                """));
        var test = laby.NewCrawler();

        using var all = Assert.EnterMultipleScope();

        Assert.That(test.X, Is.EqualTo(2));
        Assert.That(test.Y, Is.EqualTo(2));
        Assert.That(test.Direction, Is.EqualTo(Direction.North));
        Assert.That(test.FacingTile, Is.TypeOf<Room>());
    }

    [Test]
    public void InitWithNoXThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Labyrinth.Labyrinth(new AsciiParser("""
                +--+
                |  |
                +--+
                """)));

    }
    #endregion

    #region Labyrinth borders
    [Test]
    public void FacingNorthOnUpperTileReturnsOutside()
    { 
        var laby = new Labyrinth.Labyrinth(new AsciiParser("""
                +-x+
                |  |
                +--+
                """));
        var test = laby.NewCrawler();

        Assert.That(test.Direction, Is.EqualTo(Direction.North));
        Assert.That(test.FacingTile, Is.TypeOf<Outside>());
    }

    [Test]
    public void FacingWestOnFarLeftTileReturnsOutside()
    {
        var laby = new Labyrinth.Labyrinth(new AsciiParser("""
                +--+
                x  |
                +--+
                """));
        var test = laby.NewCrawler();
        test.Direction.TurnLeft();

        Assert.That(test.Direction, Is.EqualTo(Direction.West));
        Assert.That(test.FacingTile, Is.TypeOf<Outside>());
    }

    [Test]
    public void FacingEastOnFarRightTileReturnsOutside()
    {
        var laby = new Labyrinth.Labyrinth(new AsciiParser("""
                +--+
                |  x
                +--+
                """));
        var test = laby.NewCrawler();
        test.Direction.TurnRight();

        Assert.That(test.Direction, Is.EqualTo(Direction.East));
        Assert.That(test.FacingTile, Is.TypeOf<Outside>());
    }

    [Test]
    public void FacingSouthOnBottomTileReturnsOutside()
    {
        var laby = new Labyrinth.Labyrinth(new AsciiParser("""
                +--+
                |  |
                |  |
                +-x+
                """));
        var test = laby.NewCrawler();
        test.Direction.TurnRight();
        test.Direction.TurnRight();

        Assert.That(test.Direction, Is.EqualTo(Direction.South));
        Assert.That(test.FacingTile, Is.TypeOf<Outside>());
    }
    #endregion

    #region Moves
    [Test]
    public void TurnLeftFacesWestTile()
    {
        var laby = new Labyrinth.Labyrinth(new AsciiParser("""
                +--+
                |x |
                +--+
                """));
        var test = laby.NewCrawler();
        test.Direction.TurnLeft();

        Assert.That(test.FacingTile, Is.TypeOf<Wall>());
        Assert.That(test.Direction, Is.EqualTo(Direction.West));
    }

    [Test]
    public void WalkReturnsInventoryAndChangesPositionAndFacingTile()
    {
        var laby = new Labyrinth.Labyrinth(new AsciiParser("""
                +--+
                |  |
                |x |
                +--+
                """));
        var test = laby.NewCrawler();

        var oldFacingTile = test.FacingTile;
        var oldY = test.Y;
        var inventory = test.Walk();

        Assert.That(test.Y, Is.Not.EqualTo(oldY));
        Assert.That(test.FacingTile, Is.Not.EqualTo(oldFacingTile));
        Assert.That(inventory, Is.Not.Null);
    }

    [Test]
    public void TurnAndWalkReturnsInventoryChangesPositionAndFacingTile()
    {
        var laby = new Labyrinth.Labyrinth(new AsciiParser("""
                +--+
                |  |
                |x |
                +--+
                """));
        var test = laby.NewCrawler();

        var oldFacingTile = test.FacingTile;
        var oldX = test.X;
        test.Direction.TurnRight();
        var inventory = test.Walk();

        Assert.That(test.X, Is.Not.EqualTo(oldX));
        Assert.That(test.FacingTile, Is.Not.EqualTo(oldFacingTile));
        Assert.That(inventory, Is.Not.Null);
    }

    [Test]
    public void WalkOnNonTraversableTileThrowsInvalidOperationException()
    {
        var laby = new Labyrinth.Labyrinth(new AsciiParser("""
                +--+
                |x |
                |  |
                +--+
                """));
        var test = laby.NewCrawler();
        Assert.Throws<InvalidOperationException>(() => test.Walk());
    }
    #endregion

    #region Items and doors
    [Test]
    public void WalkInARoomWithAnItem()
    {
        var laby = new Labyrinth.Labyrinth(new AsciiParser("""
                +--+
                |k/|
                |x |
                +--+
                """));
        var test = laby.NewCrawler();
        var inventory = test.Walk();

        Assert.That(inventory.HasItem, Is.True);
        
    }

    [Test]
    public void WalkUseAWrongKeyToOpenADoor()
    {
        var laby = new Labyrinth.Labyrinth(new AsciiParser("""
                +--+
                |k/|
                |x |
                | /|
                | k|
                +--+
                """));
        var test = laby.NewCrawler();
        var inventory = test.Walk();
        test.Direction.TurnLeft();
        test.Direction.TurnLeft();
        test.Walk();
        test.Walk();
        test.Direction.TurnLeft();

        var door = (Door)test.FacingTile;
        using var all = Assert.EnterMultipleScope();
        Assert.That(door.Open(inventory), Is.False);
        Assert.That(door.IsOpened, Is.False);
        Assert.Throws<InvalidOperationException>(() => test.Walk());
    }

    [Test]
    public void WalkUseKeyToOpenADoorAndPass()
    {
        var laby = new Labyrinth.Labyrinth(new AsciiParser("""
                +--+
                |xk|
                +-/|
                """));
        var test = laby.NewCrawler();

        test.Direction.TurnRight();

        var inventory = test.Walk();

        test.Direction.TurnRight();
        ((Door)test.FacingTile).Open(inventory);

        test.Walk();

        using var all = Assert.EnterMultipleScope();

        Assert.That(test.X, Is.EqualTo(2));
        Assert.That(test.Y, Is.EqualTo(2));
        Assert.That(test.Direction, Is.EqualTo(Direction.South));
        Assert.That(test.FacingTile, Is.TypeOf<Outside>());
    }
    #endregion
}