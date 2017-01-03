using System.Threading.Tasks;
using TreeSharp;

namespace EatShit.Logic
{
    /// <summary>
    /// Base Implementation of the ILogic Interface
    /// </summary>
    public abstract class BaseLogic : ILogic
    {
        private Composite _execute;
        /// <summary>
        /// Composite that gets hooked onto the main TreeRoot
        /// </summary>
        public Composite Execute
        {
            get
            {
                return _execute ?? (_execute = new Decorator(ctx => NeedToEatShit, new ActionRunCoroutine(x => SoEatShit())));
            }
        }
        /// <summary>
        /// Boolean that tells the main TreeRoot if it needs to run down this branch of Logic
        /// </summary>
        protected abstract bool NeedToEatShit { get; }
        /// <summary>
        /// The logic that gets run when NeedToEatShit is true
        /// </summary>
        protected abstract Task<bool> SoEatShit();
    }
}