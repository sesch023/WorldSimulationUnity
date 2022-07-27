namespace Base
{
    /// <summary>
    /// Base Interface for Updateable Objects independent of the Game Engines MonoBehavior and managed by an UpdateManager.
    /// </summary>
    public interface IUpdatable
    {
        /// <summary>
        /// Method to be called to update the object.
        /// </summary>
        public void Update();
    }
}