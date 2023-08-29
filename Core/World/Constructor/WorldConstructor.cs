using Cysharp.Threading.Tasks;
using TOAFL.Core.Characters;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace TOAFL.Core.World
{
    public class WorldConstructor
    {
        private readonly DiContainer _diContainer;
        private readonly PlayerStaticData _playerData;

        public WorldConstructor(DiContainer diContainer, PlayerStaticData playerData)
        {
            _diContainer = diContainer;
            _playerData = playerData;
        }

        public async void Construct()
        {
            await SpawnPlayer();
        }

        private async UniTask SpawnPlayer()
        {
            var reference = _playerData.PlayerReference;

            var playerObject = await Addressables.LoadAssetAsync<GameObject>(reference);
            
            playerObject.SetActive(false);
            
            var position = Vector3.zero + Vector3.up * 20f;
            var rotation = Quaternion.identity;
            var playerObjectInstance = await Addressables.InstantiateAsync(reference, position, rotation).Task;
            
            _diContainer.Inject(playerObjectInstance.GetComponent<Player>());
            
            playerObjectInstance.SetActive(true);
        }
    }
}