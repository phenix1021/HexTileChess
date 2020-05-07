using Phenix.Unity.TurnBased;

public class CommandBase : ITurnBasedCommand
{
    public bool Finished { get; set; }

    public virtual void OnStart() { }

    public virtual void OnUpdate() { }
    public virtual void OnEnd() { }
} 
