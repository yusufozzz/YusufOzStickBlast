using GameManagement;
using UnityEngine;

namespace PoolSystem
{
    public class PoolManager: ManagerBase
    {
        [field: SerializeField]
        public HighlightPool HighlightPool { get; private set; }

        [field: SerializeField]
        public StickPool StickPool { get; private set; }
    }
}