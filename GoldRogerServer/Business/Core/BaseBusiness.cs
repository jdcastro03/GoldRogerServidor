using GoldRoger.Data;
namespace GoldRogerServer.Business.Core
{
    public class BaseBusiness
    {

        protected readonly UnitOfWork uow;

        public BaseBusiness(UnitOfWork _uow)
        {
            uow = _uow;
        }
    }
}
