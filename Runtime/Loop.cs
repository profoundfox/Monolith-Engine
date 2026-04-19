namespace Monolith.Runtime
{
  public abstract class Loop : Instance
  {
    public virtual void OnEnter() {}
    public virtual void PhysicsUpdate(float delta) {}
    public virtual void ProcessUpdate(float delta) {}
    public virtual void OnExit(float delta) {}
  }
}
