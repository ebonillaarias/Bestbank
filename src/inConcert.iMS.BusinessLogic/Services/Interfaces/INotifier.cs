using System.Threading.Tasks;
using inConcert.iMS.BusinessLogic.Services.Model;

namespace inConcert.iMS.BusinessLogic.Services.Interfaces
{
   public interface INotifier
   {
      bool Notify(NotifierData notifierConfiguration);

      Task<bool> NotifyAsync(NotifierData notifierConfiguration);
   }
}
