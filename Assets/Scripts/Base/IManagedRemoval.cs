namespace Base
{
    /// <summary>
    /// Delegate, executed when a IManagedRemoval is triggered.
    /// </summary>
    public delegate void OnRemoval(IManagedRemoval triggeredBy);
    
    /// <summary>
    /// Base Interface for objects which require a active removal on certain conditions.
    /// </summary>
    public interface IManagedRemoval
    {
        public void ManageRemoval();
    }
}