using Configs;
using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "ProjectConfigsInstaller", menuName = "Installers/ProjectConfigsInstaller")]
public class ProjectConfigsInstaller : ScriptableObjectInstaller<ProjectConfigsInstaller>
{
    [SerializeField] private GameSettings gameSettings;
    [SerializeField] private PlayerSettings playerSettings;
    [SerializeField] private PrefabsContainer prefabsContainer;
    [SerializeField] private PlayerUpgrade playerUpgrade;
    [SerializeField] private EnemySettings enemySettings;
    [SerializeField] private BossFightSettings bossFightSettings;

    public override void InstallBindings()
    {
        Container.BindInstances(
            gameSettings,
            playerSettings,
            prefabsContainer,
            playerUpgrade,
            enemySettings,
            bossFightSettings
        );
    }
}