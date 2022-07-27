namespace Base
{
    /// <summary>
    /// Abstract Uptatable which executes a Conditioned Update on a given Condition. Implement ConditionedUpdate() and Condition()
    /// in derived classes. Implements IUptatable.
    /// </summary>
    public abstract class ConditionalUpdate : IUpdatable
    {
        /// <summary>
        /// Basic Update method.
        /// </summary>
        public void Update()
        {
            if(Condition())
                ConditionedUpdate();
        }
        
        /// <summary>
        /// Condition to met, before ConditionedUpdate() is executed.
        /// </summary>
        /// <returns>True, if the condition is met.</returns>
        public abstract bool Condition();
        
        /// <summary>
        /// Code which is executed, if the condition is met.
        /// </summary>
        protected abstract void ConditionedUpdate();
    }
}