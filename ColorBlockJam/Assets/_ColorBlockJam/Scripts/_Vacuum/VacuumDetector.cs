using _ColorBlockJam.Scripts._Blocks;
using UnityEngine;

namespace _ColorBlockJam.Scripts._Vacuum
{
    public class VacuumDetector : MonoBehaviour
    {
        public VacuumBlock parentVacuum;

        private void OnTriggerEnter(Collider other)
        {
            DraggableBlock block = other.GetComponent<DraggableBlock>();
            if (block == null) return;

            if (block.blockColor == parentVacuum.vacuumColor)
            {
                block.ShrinkAndDestroyZ();
                parentVacuum.VacuumPushAnimation();
            }
        }
}
}