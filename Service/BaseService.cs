using yuyu.Infrastructure;

namespace yuyu.Service
{
    public class BaseService
    {
        public IUnitWork UnitWork;

        public BaseService(IUnitWork unitWork)
        {
            UnitWork = unitWork;
        }
    }
}