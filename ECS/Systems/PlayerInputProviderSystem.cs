using TOAFL.UserInput;
using Unity.Burst;
using Unity.Entities;

namespace TOAFL.ECS.Systems
{
    public partial class PlayerInputProviderSystem : ComponentSystemBase
    {
        public Controls Controls;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CharacterUnderPlayerControl>();
        }

        public override void Update()
        {
            // foreach (var (playerMoveInput, entity) in SystemAPI.Query<RefRW<CharacterUnderPlayerControl>>().WithEntityAccess())
            // {
            //     // var input = Controls.PlayerControls.Movement.ReadValue<Vector2>();
            //     // state.EntityManager.AddComponentData(entity, 
            //     //     new PlayerMoveInput()
            //     // {
            //     //     Horizontal = input.x,
            //     //     Vertical   = input.y
            //     // });
            // }
        }
    }
}