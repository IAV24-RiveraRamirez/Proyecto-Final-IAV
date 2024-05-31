public class S_Crafting : State
{
    NPCInfo info;
    WoodShop woodShop;
    public override void Enter()
    {
        object obj = fsm.blackboard.Get("Craft_ItemsCrafted", typeof(int));
        if(obj == null)
        {
            fsm.blackboard.Set("Craft_ItemsCrafted", typeof(int), 0);
        }
        info = gameObject.GetComponent<NPCInfo>();
        woodShop = info.GetWorkPlace() as WoodShop;
    }

    public override void Exit()
    {

    }

    public override string ID()
    {
        return "Crafting";
    }
    
    public override void Update(float dt)
    {
        woodShop.Work(info);
    }
}
