using GoldRoger.Entity.Entities.GoldRoger.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldRoger.Entity.Entities.Security
{
    public class UserPermission
    {

        public int Id { get; set; }

        public int UserId { get; set; }

        public int PermissionId { get; set; }

        public virtual User? User { get; set; }

        public virtual Permission? Permission { get; set; }


    }
}
