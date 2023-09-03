using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using TOAFL.Core.Characters;
using TOAFL.Modules.StateMachine;
using TOAFL.Services.Camera;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace TOAFL.Core.World
{
    public class WorldLoadState : BaseZenState
    {
        private readonly PlayerStaticData _playerData;
        private readonly CameraOperatorService _cameraService;
        private readonly StateMachine _stateMachine;

        public WorldLoadState(StateMachine stateMachine, PlayerStaticData playerData, CameraOperatorService cameraService) : base(stateMachine)
        {
            _stateMachine = stateMachine;
            _playerData = playerData;
            _cameraService = cameraService;
        }
        
        public override async void Enter()
        { 
            await LoadResources();
            _stateMachine.Enter<WorldSetupState>();
        }
        
        public override void Exit()
        {
        }

        private async UniTask LoadResources()
        {
            await LoadPlayer();
            await LoadCameraOperator();
        }

        private async UniTask LoadPlayer()
        {
            var reference = _playerData.PlayerReference;
            await Addressables.LoadAssetAsync<GameObject>(reference);
        }
        
        private async Task LoadCameraOperator()
        {
            await _cameraService.LoadEquipment();
        }
    }
}