using Cysharp.Threading.Tasks;
using TOAFL.Core.Characters;
using TOAFL.Utils.Addressables;
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

            await Addressables.LoadAssetAsync<GameObject>(reference);

            var position = Vector3.zero + Vector3.up * 20f;
            var rotation = Quaternion.identity;
            var playerObject = await AddressablesUtils.InstantiateDisabledAsync(reference, position, rotation);
            
            _diContainer.Inject(playerObject.GetComponent<Player>());
            
            playerObject.SetActive(true);
        }
    }
}