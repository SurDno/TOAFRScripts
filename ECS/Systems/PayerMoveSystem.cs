using Unity.Burst;
using Unity.Entities;

namespace TOAFL.ECS.Systems
{
    public partial struct PayerMoveSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CharacterUnderPlayerControl>();
            state.RequireForUpdate<PlayerMoveInput>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // foreach (var (transform, moveData) in SystemAPI.Query<RefRW<rigidb>, RefRO<PlayerMoveInput>>())
            // {
            //     // transform.r
            // }
        }
    }
}