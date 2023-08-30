using Cysharp.Threading.Tasks;
using TOAFL.Core.Characters;
using TOAFL.Services.Camera;
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
        private readonly CameraOperatorService _cameraService;

        public WorldConstructor(DiContainer diContainer, PlayerStaticData playerData, CameraOperatorService cameraService)
        {
            _diContainer = diContainer;
            _playerData = playerData;
            _cameraService = cameraService;
        }

        public async void Construct()
        {
            var player = await SpawnPlayer();
            await _cameraService.LoadEquipment();
            _cameraService.CaptureObject(player.CameraFollowTransform, player.CameraLookAtTransform);
        }

        private async UniTask<Player> SpawnPlayer()
        {
            var reference = _playerData.PlayerReference;

            await Addressables.LoadAssetAsync<GameObject>(reference);

            var position = Vector3.zero + Vector3.up * 20f;
            var rotation = Quaternion.identity;
            var playerObject = await AddressablesUtils.InstantiateDisabledAsync(reference, position, rotation);
            
            var playerComponent = playerObject.GetComponent<Player>();
            
            _diContainer.Inject(playerComponent);
            
            playerObject.SetActive(true);
            
            return playerComponent;
        }
    }
}