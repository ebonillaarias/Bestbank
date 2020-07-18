using inConcert.iMS.Domain;

namespace inConcert.iMS.BusinessLogic.Services.Interfaces
{
   public interface IBOSupervisorsService
   {
      BOGetSupervisorsResponseModel GetSupervisors(string name, bool withSuperuser);

      /// <summary> 
      /// Crea un supervisor en el sistema.
      /// </summary> 
      /// <param name="supervisorData">Datos del supervisor a crear.</param> 
      /// <returns> 
      /// Modelo <see cref="PostBOSupervisorResponseModel"/> con los datos de la respuesta. 
      /// </returns> 
      PostBOSupervisorResponseModel CreateSupervisor(PostBOSupervisorRequestModel supervisorData);

      /// <summary> 
      /// Aprobar o rechazar el alta de un supervisor.
      /// En caso de rechazar, el mismo es eliminado
      /// </summary> 
      /// <param name="data">Id del supervisor y booleano para indicar aprobar/rechazar.</param> 
      /// <returns> 
      /// Modelo <see cref="PostBOSupervisorResponseModel"/> con los datos de la respuesta. 
      /// </returns> 
      StatusResponseModel ApproveSupervisor(PatchBOSupervisorsRequestModel data);

      /// <summary> 
      /// Elimina supervisor del sistema (tenga o no aprobacion pendiente).
      /// </summary> 
      /// <param name="id">Id del supervisor a eliminar.</param> 
      /// <returns> 
      /// Modelo <see cref="StatusResponseModel"/> con los datos de la respuesta. 
      /// </returns> 
      StatusResponseModel DeleteSupervisor(int id);
   }
}
