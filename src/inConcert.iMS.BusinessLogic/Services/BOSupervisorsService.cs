using inConcert.iMS.BusinessLogic.Services.Interfaces;
using inConcert.iMS.DataAccess.Repositories.Interfaces;
using inConcert.iMS.Domain;
using inConcert.iMS.Domain.Entities;
using inConcert.iMS.Enums;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace inConcert.iMS.BusinessLogic.Services
{
   public class BOSupervisorsService : IBOSupervisorsService
   {
      private readonly IGenericRepository<Supervisors> _supervisorRepository;

      public BOSupervisorsService(IGenericRepository<Supervisors> supervisorRepository)
      {
         _supervisorRepository = supervisorRepository;
      }

      /// <summary>
      /// <para>
      /// Busca todos los supervisores en los cuales el campo 'Name' o 'Email'  coincide y/o contiene la palabra
      /// clave indicada por el parametro <paramref name="name"/>.
      /// </para>
      /// <para>
      /// Si <paramref name="name"/> es NULL, string vacio o string blanco (todos espacios)
      /// se recuperan todos los supervisores.
      /// </para>
      /// </summary>
      /// <param name="name">Palabra clave a buscar en los nombres de los comerciales.</param>
      /// <param name="withSuperuser">Indica si se deben incluir los superusuarios en el listado.</param>
      /// <returns>
      /// Modelo <see cref="BOGetSupervisorsResponseModel"/> con los datos de la respuesta.
      /// </returns>
      public BOGetSupervisorsResponseModel GetSupervisors(string name, bool withSuperuser)
      {
         BOGetSupervisorsResponseModel supervisorsModel = new BOGetSupervisorsResponseModel
         {
            Supervisors = new List<BOSupervisorModel>(),
            Status = ResultStatus.SUCCESS
         };

         try
         {
            List<Supervisors> supervisorsDB = new List<Supervisors>();

            if (string.IsNullOrWhiteSpace(name))
            {
               supervisorsDB.AddRange(_supervisorRepository.FindAll(s => withSuperuser ? true : s.State != (int)SupervisorState.Superuser).ToList());
            }
            else
            {
               string filtro = NormalizeWhiteSpace(name);
               var compareInfo = CultureInfo.InvariantCulture.CompareInfo;
               Func<Supervisors, bool> selector = c => ((compareInfo.IndexOf(c.Name, filtro, CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace) > -1) ||
                                                        (compareInfo.IndexOf(c.Email, filtro, CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace) > -1)) &&
                                                       (withSuperuser ? true : c.State != (int)SupervisorState.Superuser);
               supervisorsDB.AddRange(_supervisorRepository.FindAll(selector).ToList());
            }

            foreach (Supervisors c in supervisorsDB)
            {
               supervisorsModel.Supervisors.Add(new BOSupervisorModel
               {
                  Id = c.Id,
                  Name = String.Format("{0}", c.Name),
                  Email = c.Email,
                  State = c.State
               });
            }
            return supervisorsModel;
         }
         catch (Exception e)
         {
            return new BOGetSupervisorsResponseModel
            {
               Status = ResultStatus.ERROR
            };
         }
      }

      /// <summary> 
      /// Crea un supervisor en el sistema.
      /// </summary> 
      /// <param name="supervisorData">Datos del supervisor a crear.</param> 
      /// <returns> 
      /// Modelo <see cref="PostBOSupervisorResponseModel"/> con los datos de la respuesta. 
      /// </returns> 
      public PostBOSupervisorResponseModel CreateSupervisor(PostBOSupervisorRequestModel supervisorData)
      {

         PostBOSupervisorResponseModel result = new PostBOSupervisorResponseModel
         {
            Status = ResultStatus.ERROR
         };

         var context = _supervisorRepository.GetContext();
         using (var dbTransaction = context.Database.BeginTransaction())
         {
            try
            {
               var toInsert = new Supervisors()
               {
                  Name = supervisorData.Name,
                  Email = supervisorData.Email,
                  Password = CreateHash(supervisorData.Password),
                  State = 0
               };

               // Se da de alta en BD 
               _supervisorRepository.Insert(toInsert);
               _supervisorRepository.Save();

               dbTransaction.Commit();
               result.Status = ResultStatus.SUCCESS;
               return result;
            }
            catch (DbUpdateException updEx)
            {
               dbTransaction.Rollback();

               SqlException sqlEx = updEx.GetBaseException() as SqlException;
               if (sqlEx != null)
               {
                  switch (sqlEx.Number)
                  {
                     case 515: // NOT NULL values
                        // Determino nombre de la columna que no acepta valor NULL
                        // EJ: "Cannot insert the value NULL into column 'SiebelId', table 'InConcert.dbo.Commercials'."
                        int indexStart = sqlEx.Message.IndexOf("'", 0) + 1;
                        int indexEnd = sqlEx.Message.IndexOf("'", indexStart);
                        string columName = sqlEx.Message[indexStart..indexEnd];

                        // Determino nombre de la tabla
                        indexStart = sqlEx.Message.IndexOf(".dbo.", 0) + 1;
                        indexEnd = sqlEx.Message.IndexOf("'", indexStart);
                        string tableName = sqlEx.Message[indexStart..indexEnd];

                        result.Status = ResultStatus.NOT_NULL;
                        result.Message = string.Format("Cannot insert the value NULL into column '{0}', table '{1}'.", columName, tableName);
                        break;

                     case 2601: // Duplicated key row error
                        string errorEmail = "IX_Supervisors_Email";

                        if (sqlEx.Message.Contains(errorEmail))
                        {
                           result.Status = ResultStatus.SUPERVISOR_ROW_DUPLICATE_EMAIL;
                           result.Message = "Email is unique in table Supervisors.";
                        }
                        else
                        {
                           result.Status = ResultStatus.SUPERVISOR_ROW_DUPLICATE;
                           result.Message = sqlEx.Message;
                        }
                        break;

                     default: // Otros errores no contemplados
                        result.Status = ResultStatus.ERROR;
                        result.Message = sqlEx.Message;
                        break;
                  }
               }
               else
               {
                  result.Status = ResultStatus.ERROR;
                  result.Message = updEx.Message;
               }
               return result;
            }
            catch (Exception e)
            {
               dbTransaction.Rollback();
               return result;
            }
         }
      }

      /// <summary> 
      /// Aprobar o rechazar el alta de un supervisor.
      /// En caso de rechazar, el mismo es eliminado
      /// </summary> 
      /// <param name="data">Id del supervisor y booleano para indicar aprobar/rechazar.</param> 
      /// <returns> 
      /// Modelo <see cref="PostBOSupervisorResponseModel"/> con los datos de la respuesta. 
      /// </returns> 
      public StatusResponseModel ApproveSupervisor(PatchBOSupervisorsRequestModel data)
      {
         StatusResponseModel result = new StatusResponseModel
         {
            Status = ResultStatus.ERROR
         };

         if (data == null)
         {
            result.Status = ResultStatus.BAD_REQUEST;
            result.Message = "The parameter 'data' cannot be null";
            return result;
         }

         var context = _supervisorRepository.GetContext();
         using (var dbTransaction = context.Database.BeginTransaction())
         {
            try
            {
               Supervisors toUpdate = _supervisorRepository.GetById(data.Id, data.Email);
               if (toUpdate == null)
               {
                  result.Status = ResultStatus.NOT_FOUND;
                  return result;
               }

               if (data.Approve)
               {
                  toUpdate.State = 1;
                  _supervisorRepository.Update(toUpdate);
               }
               else
               {
                  // Rechazar el alta: se elimina de la BD
                  _supervisorRepository.Delete(toUpdate);
               }

               _supervisorRepository.Save();
               dbTransaction.Commit();

               result.Status = ResultStatus.SUCCESS;
               return result;
            }
            catch (Exception e)
            {
               result.Message = e.Message;
               return result;
            }
         }
      }

      /// <summary> 
      /// Elimina supervisor del sistema (tenga o no aprobacion pendiente).
      /// </summary> 
      /// <param name="id">Id del supervisor a eliminar.</param> 
      /// <returns> 
      /// Modelo <see cref="StatusResponseModel"/> con los datos de la respuesta. 
      /// </returns> 
      public StatusResponseModel DeleteSupervisor(int id)
      {
         StatusResponseModel result = new StatusResponseModel
         {
            Status = ResultStatus.ERROR
         };

         var context = _supervisorRepository.GetContext();
         using (var dbTransaction = context.Database.BeginTransaction())
         {
            try
            {
               // Se recupera supervisor de la BD
               Supervisors toDelete = _supervisorRepository.SingleOrDefault(s => s.Id == id);
               if (toDelete == null)
               {
                  result.Status = ResultStatus.NOT_FOUND;
                  return result;
               }

               switch (toDelete.State)
               {
                  case (int)SupervisorState.Superuser:
                     // No se permite eliminar superusuarios
                     result.Status = ResultStatus.CONFLICT;
                     result.Message = "Deleting superusers is not allowed.";
                     break;

                  case (int)SupervisorState.Approved:
                  case (int)SupervisorState.Pending:
                     _supervisorRepository.Delete(toDelete);
                     _supervisorRepository.Save();
                     dbTransaction.Commit();
                     result.Status = ResultStatus.SUCCESS;
                     break;

                  default:
                     result.Message = "Invalid supervisor state.";
                     result.Status = ResultStatus.ERROR;
                     break;
               }
               return result;
            }
            catch (Exception e)
            {
               dbTransaction.Rollback();
               result.Message = "Exception: " + e.Message;
               if (e.InnerException != null)
                  result.Message += " InnerException: " + e.InnerException.Message;
               return result;
            }
         }
      }

      /// <summary>
      /// Dato un string elimina los espacios extras.
      /// </summary>
      /// <param name="input">String de donde se eliminarán los espacios extras</param>
      /// <returns>String sin espacios extras.</returns>
      private static string NormalizeWhiteSpace(string input)
      {
         var srcAux = input.Trim();
         int len = srcAux.Length;
         int index = 0;

         var src = srcAux.ToCharArray();
         bool skip = false;
         char ch;

         for (int i = 0; i < len; i++)
         {
            ch = src[i];
            switch (ch)
            {
               case '\u0020':
               case '\u00A0':
               case '\u1680':
               case '\u2000':
               case '\u2001':
               case '\u2002':
               case '\u2003':
               case '\u2004':
               case '\u2005':
               case '\u2006':
               case '\u2007':
               case '\u2008':
               case '\u2009':
               case '\u200A':
               case '\u202F':
               case '\u205F':
               case '\u3000':
               case '\u2028':
               case '\u2029':
               case '\u0009':
               case '\u000A':
               case '\u000B':
               case '\u000C':
               case '\u000D':
               case '\u0085':
                  if (skip) continue;
                  src[index++] = ch;
                  skip = true;
                  continue;
               default:
                  skip = false;
                  src[index++] = ch;
                  continue;
            }
         }

         return new string(src, 0, index);
      }

      /// <summary>
      /// Genera un hash a partir de un string indicado por el parámetro <paramref name="str"/>
      /// </summary>
      /// <param name="str">String a codificar</param>
      /// <returns>
      /// String con el hash calculado
      /// </returns>
      private static string CreateHash(string str)
      {
         using (SHA256 sha256Hash = SHA256.Create())
         {
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(str));

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
               builder.Append(bytes[i].ToString("x2"));
            }

            return builder.ToString();
         }
      }
   }
}