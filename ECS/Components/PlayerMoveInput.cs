using Unity.Entities;

namespace TOAFL.ECS
{
    public struct PlayerMoveInput : IComponentData
    {
        public float Horizontal;
        public float Vertical;
    }
} 