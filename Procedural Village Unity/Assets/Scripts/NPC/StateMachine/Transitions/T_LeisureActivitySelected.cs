
/// <summary>
/// Indica cuándo se ha seleccionado una actividad de ocio
/// </summary>
public class T_LeisureActivitySelected : Transition
{
    NPCInfo info;
    public T_LeisureActivitySelected(State nextState) : base(nextState)
    {
    }

    public override bool Check()
    {
        return info.GetLeisurePlace() != null && info.GetLeisurePlace() is LeisurePlace;
    }

    public override void Enter()
    {
        info = gameObject.GetComponent<NPCInfo>();
    }

    public override void Exit()
    {

    }

    public override string ID()
    {
        return "Activity Found!";
    }
}
