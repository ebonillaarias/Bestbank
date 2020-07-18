using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace inConcert.iMS.AMI
{
    public class Ami
    {
        private const string NewLine = "\r\n";

        private readonly object socketLock = new object();
        private Socket clientSocket;

        /// <summary>
        /// Inicializa una instancia de la clase, conectándose al interfaz AMI de un servidor Asterisk
        /// </summary>
        /// <param name="server">Dirección IP del servidor Asterisk</param>
        /// <param name="user">Usuario de AMI manager. Debe existir en manager.conf</param>
        /// <param name="password">Contraseña asociada a user</param>
        /// <param name="message">Parámetro de salida en el que se indica el resultado de la conexión</param>
        public Ami(string server, string user, string password, out string message)
        {
            this.IsInitialized = false;
            message = string.Empty;
            try
            {
                this.clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                this.clientSocket.Connect((EndPoint)new IPEndPoint(IPAddress.Parse(server), 5038));
                byte[] numArray = new byte[1024];
                int count = this.clientSocket.Receive(numArray);
                message = Encoding.ASCII.GetString(numArray, 0, count);
                string command =
                    $"Action: Login{NewLine}" +
                    $"Username: {user}{NewLine}" +
                    $"Secret: {password}{NewLine}" +
                    $"Events: off{NewLine}" +
                    $"{NewLine}";
                string input;
                try
                {
                    input = this.SendAmiCommand(command);
                }
                catch (Exception ex)
                {
                    input = ex.Message;
                }
                if (Regex.Match(input, "Response: Success", RegexOptions.IgnoreCase).Success)
                {
                    message += $"Connected{NewLine}";
                    this.IsInitialized = true;
                }
                else
                {
                    if (Regex.Match(input, "Authentication failed", RegexOptions.IgnoreCase).Success)
                        message += $"Authentication failed{NewLine}";
                    this.IsInitialized = false;
                }
            }
            catch (Exception ex)
            {
                message += ex.Message;
                this.IsInitialized = false;
            }
        }

        public bool IsInitialized { get; private set; }

        /// <summary>Inserta un nuevo peer en la configuración de Asterisk</summary>
        /// <param name="peer">Numérico con valores válidos entre 1100 y 1999</param>
        /// <param name="name">Nombre asociado al peer</param>
        /// <param name="password">Contraseña asociada al peer</param>
        /// <param name="message">Parámetro de salida en el que se indica el detalle del resultado de la operación</param>
        /// <returns>
        ///     true: Si el nuevo peer se insertó correctamente
        ///     false: Si se produjo algún error al insertar el peer.
        /// </returns>
        public bool InsertPeer(string peer, string name, string password, out string message)
        {
            if (!this.IsInitialized)
            {
                message = "Not initialized";
                return false;
            }
            int result;
            if (!int.TryParse(peer, out result))
            {
                message = "Peer must be number between 1100 and 1999";
                return false;
            }
            if (result < 1100 || result > 1999)
            {
                message = "Peer must be number between 1100 and 1999";
                return false;
            }
            if (string.IsNullOrWhiteSpace(name))
            {
                message = "name can't be empty";
                return false;
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                message = "password can't be empty";
                return false;
            }
            string command =
                $"Action: UpdateConfig{NewLine}" +
                $"SrcFilename: sip_additional.conf{NewLine}" +
                $"DstFilename: sip_additional.conf{NewLine}" +
                $"Reload: chan_sip{NewLine}" +
                $"Action-000000: NewCat{NewLine}" +
                $"Cat-000000: {peer}{NewLine}" +
                $"Action-000001: append{NewLine}" +
                $"Cat-000001: {peer}{NewLine}" +
                $"Var-000001: username{NewLine}" +
                $"Value-000001: {peer}{NewLine}" +
                $"Action-000002: append{NewLine}" +
                $"Cat-000002: {peer}{NewLine}" +
                $"Var-000002: type{NewLine}" +
                $"Value-000002: friend{NewLine}" +
                $"Action-000003: append{NewLine}" +
                $"Cat-000003: {peer}{NewLine}" +
                $"Var-000003: secret{NewLine}" +
                $"Value-000003: {password}{NewLine}" +
                $"Action-000004: append{NewLine}" +
                $"Cat-000004: {peer}{NewLine}" +
                $"Var-000004: qualify{NewLine}" +
                $"Value-000004: no{NewLine}" +
                $"Action-000005: append{NewLine}" +
                $"Cat-000005: {peer}{NewLine}" +
                $"Var-000005: port{NewLine}" +
                $"Value-000005: 5060{NewLine}" +
                $"Action-000006: append{NewLine}" +
                $"Cat-000006: {peer}{NewLine}" +
                $"Var-000006: pickupgroup{NewLine}" +
                $"Value-000006: {NewLine}" +
                $"Action-000007: append{NewLine}" +
                $"Cat-000007: {peer}{NewLine}" +
                $"Var-000007: nat{NewLine}" +
                $"Value-000007: force_rport,comedia{NewLine}" +
                $"Action-000008: append{NewLine}" +
                $"Cat-000008: {peer}{NewLine}" +
                $"Var-000008: mailbox{NewLine}" +
                $"Value-000008: {NewLine}" +
                $"Action-000009: append{NewLine}" +
                $"Cat-000009: {peer}{NewLine}" +
                $"Var-000009: host{NewLine}" +
                $"Value-000009: dynamic{NewLine}" +
                $"Action-000010: append{NewLine}" +
                $"Cat-000010: {peer}{NewLine}" +
                $"Var-000010: dtmfmode{NewLine}" +
                $"Value-000010: rfc2833{NewLine}" +
                $"Action-000011: append{NewLine}" +
                $"Cat-000011: {peer}{NewLine}" +
                $"Var-000011: context{NewLine}" +
                $"Value-000011: context-inconcert{NewLine}" +
                $"Action-000012: append{NewLine}" +
                $"Cat-000012: {peer}{NewLine}" +
                $"Var-000012: canreinvite{NewLine}" +
                $"Value-000012: no{NewLine}" +
                $"Action-000013: append{NewLine}" +
                $"Cat-000013: {peer}{NewLine}" +
                $"Var-000013: callgroup{NewLine}" +
                $"Value-000013: {NewLine}" +
                $"Action-000014: append{NewLine}" +
                $"Cat-000014: {peer}{NewLine}" +
                $"Var-000014: callerid{NewLine}" +
                $"Value-000014: {name}{NewLine}" +
                $"Action-000015: append{NewLine}" +
                $"Cat-000015: {peer}{NewLine}" +
                $"Var-000015: record{NewLine}" +
                $"Value-000015: no{NewLine}" +
                $"{NewLine}";
            try
            {
                message = this.SendAmiCommand(command);
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            if (Regex.Match(message, "Response: Success", RegexOptions.IgnoreCase).Success)
            {
                message = $"Peer inserted{NewLine}";
                return true;
            }
            if (Regex.Match(message, "Create category did not complete successfully", RegexOptions.IgnoreCase).Success)
                message = $"Peer already exists{NewLine}";
            return false;
        }

        /// <summary>Actualiza el name y password asociados a un peer</summary>
        /// <param name="peer">Numérico con valores válidos entre 1100 y 1999</param>
        /// <param name="name">Nombre asociado al peer</param>
        /// <param name="password">Contraseña asociada al peer</param>
        /// <param name="message">Parámetro de salida en el que se indica el detalle del resultado de la operación</param>
        /// <returns>
        ///     true: Si el nuevo peer se actualizó correctamente
        ///     false: Si se produjo algún error al actualizar el peer.
        /// </returns>
        public bool UpdatePeer(string peer, string name, string password, out string message)
        {
            if (!this.IsInitialized)
            {
                message = "Not initialized";
                return false;
            }
            int result;
            if (!int.TryParse(peer, out result))
            {
                message = "Peer must be number between 1100 and 1999";
                return false;
            }
            if (result < 1100 || result > 1999)
            {
                message = "Peer must be number between 1100 and 1999";
                return false;
            }
            if (string.IsNullOrWhiteSpace(name))
            {
                message = "name can't be empty";
                return false;
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                message = "password can't be empty";
                return false;
            }
            string command =
                $"Action: UpdateConfig{NewLine}" +
                $"SrcFilename: sip_additional.conf{NewLine}" +
                $"DstFilename: sip_additional.conf{NewLine}" +
                $"Reload: chan_sip{NewLine}" +
                $"Action-000000: Update{NewLine}" +
                $"Cat-000000: {peer}{NewLine}" +
                $"Var-000000: secret{NewLine}" +
                $"Value-000000: {password}{NewLine}" +
                $"Action-000001: Update{NewLine}" +
                $"Cat-000001: {peer}{NewLine}" +
                $"Var-000001: callerid{NewLine}" +
                $"Value-000001: {name}{NewLine}" +
                $"{NewLine}";
            try
            {
                message = this.SendAmiCommand(command);
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            if (Regex.Match(message, "Response: Success", RegexOptions.IgnoreCase).Success)
            {
                message = $"Peer updated{NewLine}";
                return true;
            }
            if (Regex.Match(message, "Given category does not exist", RegexOptions.IgnoreCase).Success)
                message = $"Peer does not exist{NewLine}";
            return false;
        }

        /// <summary>Elimina un peer de la configuración de Asterisk</summary>
        /// <param name="peer">Numérico con valores válidos entre 1100 y 1999</param>
        /// <param name="message">Parámetro de salida en el que se indica el detalle del resultado de la operación</param>
        /// <returns>
        ///     true: Si el nuevo peer se eliminó correctamente
        ///     false: Si se produjo algún error al eliminar el peer.
        /// </returns>
        public bool DeletePeer(string peer, out string message)
        {
            if (!this.IsInitialized)
            {
                message = "Not initialized";
                return false;
            }
            int result;
            if (!int.TryParse(peer, out result))
            {
                message = "Peer must be number between 1100 and 1999";
                return false;
            }
            if (result < 1100 || result > 1999)
            {
                message = "Peer must be number between 1100 and 1999";
                return false;
            }
            string command =
                $"Action: UpdateConfig{NewLine}" +
                $"SrcFilename: sip_additional.conf{NewLine}" +
                $"DstFilename: sip_additional.conf{NewLine}" +
                $"Reload: chan_sip{NewLine}" +
                $"Action-000000: DelCat{NewLine}" +
                $"Cat-000000: {peer}{NewLine}" +
                $"{NewLine}";
            try
            {
                message = this.SendAmiCommand(command);
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            if (Regex.Match(message, "Response: Success", RegexOptions.IgnoreCase).Success)
            {
                message = $"Peer deleted{NewLine}";
                return true;
            }
            if (Regex.Match(message, "Given category does not exist", RegexOptions.IgnoreCase).Success)
                message = $"Peer does not exist{NewLine}";
            return false;
        }

        /// <summary>Pausa la grabación de una llamada</summary>
        /// <param name="origChannel">Nombre del canal que originó la llamada</param>
        /// <param name="message">Parámetro de salida en el que se indica el detalle del resultado de la operación</param>
        /// <returns></returns>
        public bool PauseCallRecord(string origChannel, out string message)
        {
            if (!this.IsInitialized)
            {
                message = "Not initialized";
                return false;
            }
            string command =
                $"Action: MixMonitorMute{NewLine}" +
                $"Channel: {origChannel}{NewLine}" +
                $"Direction: both{NewLine}" +
                $"State: 1{NewLine}" +
                $"{NewLine}";
            try
            {
                message = this.SendAmiCommand(command);
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            if (!Regex.Match(message, "Response: Success", RegexOptions.IgnoreCase).Success)
                return false;
            message = $"Call record paused{NewLine}";
            return true;
        }

        /// <summary>Reanuda la grabación de una llamada</summary>
        /// <param name="origChannel">Nombre del canal que originó la llamada</param>
        /// <param name="message">Parámetro de salida en el que se indica el detalle del resultado de la operación</param>
        /// <returns></returns>
        public bool ResumeCallRecord(string origChannel, out string message)
        {
            if (!this.IsInitialized)
            {
                message = "Not initialized";
                return false;
            }
            string command =
                $"Action: MixMonitorMute{NewLine}" +
                $"Channel: {origChannel}{NewLine}" +
                $"Direction: both{NewLine}" +
                $"State: 0{NewLine}" +
                $"{NewLine}";
            try
            {
                message = this.SendAmiCommand(command);
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            if (!Regex.Match(message, "Response: Success", RegexOptions.IgnoreCase).Success)
                return false;
            message = $"Call record resumed{NewLine}";
            return true;
        }

        /// <summary>Redirige una llamada de un peer hacia otro peer</summary>
        /// <param name="origChannel">Nombre del canal SIP que originó la llamada</param>
        /// <param name="redirectChannel">Nombre del canal SIP que debe redirigirse a otro peer</param>
        /// <param name="destinationPeer">Peer destino</param>
        /// <param name="message">Parámetro de salida en el que se indica el detalle del resultado de la operación</param>
        /// <returns></returns>
        public bool RedirectCall(string origChannel, string redirectChannel, string destinationPeer, out string message)
        {
            if (!this.IsInitialized)
            {
                message = "Not initialized";
                return false;
            }
            string command1 =
                $"Action: Setvar{NewLine}" +
                $"Channel: {origChannel}{NewLine}" +
                $"Variable: IsCallTransferred{NewLine}" +
                $"Value: 1{NewLine}" +
                $"{NewLine}";
            try
            {
                message = this.SendAmiCommand(command1);
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            if (!Regex.Match(message, "Response: Success", RegexOptions.IgnoreCase).Success)
                return false;
            string command2 =
                $"Action: Redirect{NewLine}" +
                $"Channel: {redirectChannel}{NewLine}" +
                $"Exten: {destinationPeer}{NewLine}" +
                $"Context: context-redirect{NewLine}" +
                $"Priority: 1{NewLine}" +
                $"{NewLine}";
            try
            {
                message = this.SendAmiCommand(command2);
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            if (!Regex.Match(message, "Response: Success", RegexOptions.IgnoreCase).Success)
                return false;
            message = $"Call redirected{NewLine}";
            return true;
        }

        /// <summary>
        /// Envia un comando AMI y devuelve la respuesta del servidor
        /// </summary>
        /// <param name="command">Comando AMI a enviar</param>
        /// <returns></returns>
        private string SendAmiCommand(string command)
        {
            byte[] numArray = new byte[1024];
            string str = string.Empty;
            lock (this.socketLock)
            {
                this.clientSocket.Send(Encoding.UTF8.GetBytes(command));
                int count;
                for (; !str.Contains($"{NewLine}{NewLine}"); str += Encoding.UTF8.GetString(numArray, 0, count))
                    count = this.clientSocket.Receive(numArray);
            }
            return str;
        }
    }
}
