public class MainRoleUpdater : EntityUpdateLogic
{
    private InputToCommandProcress inputToCommandProcress;
    public MainRoleUpdater(Contexts contexts, GameEntity entity) : base(entity)
    {
        inputToCommandProcress = new InputToCommandProcress(contexts, entity);
    }

    public override void Update(long frame)
    {
        //更新输入指令
        inputToCommandProcress.Execute(frame);
    }
}