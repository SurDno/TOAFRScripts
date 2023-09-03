using Cysharp.Threading.Tasks;
using TOAFL.Core.Characters;
using TOAFL.Modules.StateMachine;
using TOAFL.Services.Camera;
using TOAFL.Utils.Addressables;
using UnityEngine;
using Zenject;

namespace TOAFL.Core.World
{
    public class WorldSetupState : BaseZenState
    {
        private readonly DiContainer _diContainer;
        private readonly PlayerStaticData _playerData;
        private readonly CameraOperatorService _cameraService;
        private readonly StateMachine _stateMachine;

        public WorldSetupState(StateMachine stateMachine, DiContainer diContainer, PlayerStaticData playerData, CameraOperatorService cameraService) : base(stateMachine)
        {
            _stateMachine = stateMachine;
            _diContainer = diContainer;
            _playerData = playerData;
            _cameraService = cameraService;
        }
        
        public override async void Enter()
        { 
            await Setup();
            _stateMachine.Enter<WorldGameState>();
        }

        public override void Exit()
        {
            
        }
        
        private async UniTask Setup()
        {
            var player = await SpawnPlayer();
            CapturePlayerOnCamera(player);
        }

        private async UniTask<Player> SpawnPlayer()
        {
            var reference = _playerData.PlayerReference;
            var position = Vector3.zero + Vector3.up * 20f;
            var rotation = Quaternion.identity;
            
            var playerObject = await AddressablesUtils.InstantiateDisabledAsync(reference, position, rotation);
            
            var playerComponent = playerObject.GetComponent<Player>();
            
            _diContainer.Inject(playerComponent);
            
            playerObject.SetActive(true);
            
            return playerComponent;
        }

        private void CapturePlayerOnCamera(Player player)
        {
            _cameraService.CaptureObject(player.CameraFollowTransform, player.CameraLookAtTransform);
        }
    }
}