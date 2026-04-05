using NumbersBlast.Board;

namespace NumbersBlast.Gameplay
{
    public interface ILineClearResolver
    {
        LineClearResult Resolve(BoardModel model);
    }
}
