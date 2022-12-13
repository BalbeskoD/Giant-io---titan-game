using System.Threading;
using Cysharp.Threading.Tasks;
using Managers;
using UnityEngine;
using Zenject;

public class ProjectInstaller : MonoInstaller<ProjectInstaller>
{
    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<GameSetup>().AsSingle().NonLazy();
    }
    public class SyncContextInjecter
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void Inject()
        {
            SynchronizationContext.SetSynchronizationContext(new UniTaskSynchronizationContext());
        }
    }
}