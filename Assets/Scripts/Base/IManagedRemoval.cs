namespace Base
{
    public delegate void OnRemoval(IManagedRemoval triggeredBy);
    public interface IManagedRemoval
    {
        public void ManageRemoval();
    }
}