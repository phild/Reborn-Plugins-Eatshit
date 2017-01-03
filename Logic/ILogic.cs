using TreeSharp;

namespace EatShit.Logic
{
    /// <summary>
    /// Interface that exposes a base Composite for running logic
    /// </summary>
    public interface ILogic
    {
        Composite Execute { get; }
    }
}