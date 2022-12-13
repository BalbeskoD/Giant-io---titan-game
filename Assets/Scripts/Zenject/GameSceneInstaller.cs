using Managers;
using Zenject;
using Zenject.Signals;

public class GameSceneInstaller : MonoInstaller<GameSceneInstaller>
{
    public override void InstallBindings()
    {
        SignalBusInstaller.Install(Container);
        Container.BindInterfacesAndSelfTo<GameManager>().AsSingle().NonLazy();
        BinsSignals();
    }

    private void BinsSignals()
    {
        Container.DeclareSignal<PlayerLevelValueSignal>();
        Container.DeclareSignal<PlayerProgressBarSignal>();
        Container.DeclareSignal<PlayerMaxLevelSignal>();
        Container.DeclareSignal<GameStartSignal>();
        Container.DeclareSignal<TimeOverSignal>();
        Container.DeclareSignal<PlayerFailSignal>();
        Container.DeclareSignal<PlayerBuildDestroySignal>();
        Container.DeclareSignal<LevelWinSignal>();
        Container.DeclareSignal<GetRewardSignal>();
        Container.DeclareSignal<PlayerUpgradeSignal>();
        Container.DeclareSignal<CoinsUpdateSignal>();
        Container.DeclareSignal<GameEndSignal>();
        Container.DeclareSignal<StickmanKillSignal>();
        Container.DeclareSignal<PlayerPointsSignal>();
        Container.DeclareSignal<TitanKillSignal>();
        Container.DeclareSignal<GameRestartSignal>();
        Container.DeclareSignal<PointsChangeSignal>();
        Container.DeclareSignal<Timer10>();
        Container.DeclareSignal<HideTimer>();
        Container.DeclareSignal<StartBossActionSignal>();
        Container.DeclareSignal<StartBossFightSignal>();
        Container.DeclareSignal<BossWinSignal>();
        Container.DeclareSignal<BossFailSignal>();
        Container.DeclareSignal<BuildHitSignal>();
    }
}