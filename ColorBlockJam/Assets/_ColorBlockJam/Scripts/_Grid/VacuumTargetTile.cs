using _ColorBlockJam.Scripts._Blocks;
using _ColorBlockJam.Scripts._Enums;
using UnityEngine;

namespace _ColorBlockJam.Scripts._Grid
{
    public class VacuumTargetTile : MonoBehaviour
    {
        public VacuumBlock parentVacuum;
        public BlockColor TileColor => parentVacuum != null ? parentVacuum.vacuumColor : BlockColor.Red; 
    }

}