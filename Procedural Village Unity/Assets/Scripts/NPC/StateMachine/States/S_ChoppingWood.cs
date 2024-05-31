public class S_ChoppingWood : State
{
    NPCInfo info = null;
    Sawmill sawmill;

    public override void Enter()
    {
        info = gameObject.GetComponent<NPCInfo>();
        sawmill = info.GetWorkPlace() as Sawmill;
    }

    public override void Exit()
    {

    }

    public override string ID()
    {
        return "Chopping Wood";
    }

    public override void Update(float dt)
    {
        sawmill.Work(info);
    }
}
