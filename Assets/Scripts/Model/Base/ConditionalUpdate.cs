namespace Model
{
    public abstract class ConditionalUpdate : IUpdatable
    {
        public void Update()
        {
            if(Condition())
                ConditionedUpdate();
        }

        public abstract bool Condition();
        protected abstract void ConditionedUpdate();
    }
}