namespace Assets.Scripts.GameMechanics
{
    interface IApplicationHandler
    {
        bool OnApplicationClient(IPlayerApplicable target, Intent intent);
        bool OnApplicationServer(IPlayerApplicable target, Intent intent);
    }
}
