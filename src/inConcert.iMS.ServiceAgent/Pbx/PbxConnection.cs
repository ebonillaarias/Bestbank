using System;
using inConcert.iMS.AMI;
using inConcert.iMS.ServiceAgent.Exceptions;

namespace inConcert.iMS.ServiceAgent.PBX
{
   public class PbxConnection
   {
      private readonly Ami _amiManager;

      public PbxConnection(string server, string username, string password, out string message)
      {
         _amiManager = new Ami(server, username, password, out message);
      }

      public bool IsInitialized() => _amiManager.IsInitialized;

      public void New(int peer, string name, string password, Action action)
      {
         if (!_amiManager.InsertPeer(peer.ToString(), name, password, out string message))
         {
            throw new AmiException($"Unable to insert peer: {message}");
         }

         action.Invoke();
      }

      public void Update(int peer, string name, string password, Action action)
      {
         if (!_amiManager.UpdatePeer(peer.ToString(), name, password, out string message))
         {
            throw new AmiException($"Unable to update peer: {message}");
         }

         action.Invoke();
      }

      public void Delete(int peer, Action action)
      {
         if (!_amiManager.DeletePeer(peer.ToString(), out string message))
         {
            throw new AmiException($"Unable to delete peer: {message}");
         }

         action.Invoke();
      }

      public void PauseCall(string channelId, Action action)
      {
         if (!_amiManager.PauseCallRecord(channelId, out string message))
         {
            throw new AmiException($"Unable to pause call: {message}");
         }

         action.Invoke();
      }

      public void ResumeCall(string channelId, Action action)
      {
         if (!_amiManager.ResumeCallRecord(channelId, out string message))
         {
            throw new AmiException($"Unable to resume call: {message}");
         }

         action.Invoke();
      }

      public void RedirectCall(string origChannel, string redirectChannel, int destinationPeer, Action action)
      {
         if (!_amiManager.RedirectCall(origChannel, redirectChannel, destinationPeer.ToString(), out string message))
         {
            throw new AmiException($"Unable to redirect call: {message}");
         }

         action.Invoke();
      }
   }
}
