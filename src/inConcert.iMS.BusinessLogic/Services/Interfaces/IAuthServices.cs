using inConcert.iMS.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace inConcert.iMS.BusinessLogic.Services.Interfaces
{
   public interface IAuthServices
   {
      PostSigninResponseModel SignIn(PostSigninRequestModel data);

      StatusResponseModel SignOut(int id);

      StatusResponseModel KeepAlive(int id);

      PostUpdatePasswordResponseModel ChangePassword(PostUpdatePasswordRequestModel data);

      ForgotPasswordResponseModel ForgotPassword(PostForgotPasswordRequestModel data);
   }
}
