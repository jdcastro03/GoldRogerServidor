using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldRoger.Entity.Entities
{
    public class PlayerStats
    {
        public int PlayerId { get; set; }
        public int Goals { get; set; }
        public int YellowCards { get; set; }
        public int RedCards { get; set; }

        // Relaciones
        public Player Player { get; set; }
    }
}
