using ExpenseManageBack.Model;
using Newtonsoft.Json.Linq;
using yuyu.Infrastructure;
using yuyu.Service;

namespace ExpenseManageBack.Service
{
    public class WxHelperService : BaseService
    {
        private IUnitWork unitWork { get; set; }

        public WxHelperService(IUnitWork _unitWork) : base(_unitWork)
        {
            unitWork = _unitWork;
        }
    }
}
