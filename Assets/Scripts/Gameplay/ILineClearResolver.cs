using NumbersBlast.Board;

namespace NumbersBlast.Gameplay
{
    /// <summary>
    /// Detects and clears fully completed rows and columns on the board.
    /// </summary>
    public interface ILineClearResolver
    {
        /// <summary>
        /// Scans the board for full rows and columns, clears them, and returns the result.
        /// </summary>
        LineClearResult Resolve(BoardModel model);
    }
}
