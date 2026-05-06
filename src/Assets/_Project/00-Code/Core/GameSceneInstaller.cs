using Mergeburgers.Data;
using Mergeburgers.Gameplay;
using UnityEngine;
using Zenject;

namespace Mergeburgers.Core
{
    public sealed class GameSceneInstaller : MonoInstaller
    {
        [SerializeField] private Board _board;
        [SerializeField] private IngredientDatabase _ingredientDatabase;

        public override void InstallBindings()
        {
            Container.Bind<IngredientDatabase>().FromInstance(_ingredientDatabase).AsSingle();
            Container.Bind<Board>().FromInstance(_board).AsSingle();
        }
    }
}