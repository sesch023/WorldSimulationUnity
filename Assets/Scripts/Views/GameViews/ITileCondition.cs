using Model.Map;

namespace Views.GameViews
{
    public interface ITileCondition
    {
        public bool CheckCondition(MapUnit unit);
    }
}