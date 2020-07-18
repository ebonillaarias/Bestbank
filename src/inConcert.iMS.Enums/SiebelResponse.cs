using System.ComponentModel;

namespace inConcert.iMS.Enums
{
   public enum SiebelResponse
   {
      [Description("Ok")]
      OK = 0,
      [Description("Inputs Obrigatórios não preenchidos")]
      ECTI001,
      [Description("User não encontrado")]
      ECTI002,
      [Description("Interaction já existe com o Id recebido")]
      ECTI003,
      [Description("Customer não encontrado")]
      ECTI004,
      [Description("Direction inválida")]
      ECTI005,
      [Description("StartDate mal formatada. Experimente o formato MM/DD/YYYY HH24:MI:SS")]
      ECTI006,
      [Description("EndDate mal formatada. Experimente o formato MM/DD/YYYY HH24:MI:SS")]
      ECTI007,
      [Description("Data com caracter inválido")]
      ECTI008,
      [Description("Erro ao criar nova data")]
      ECTI009,
      [Description("Interaction já tem resultado preenchido")]
      ECTI010,
      [Description("Interaction não encontrada")]
      ECTI011,
      [Description("Call Result inválido")]
      ECTI012,

      #region Errores Genericos
      [Description("Ocorreu um erro técnico")]
      EGEN001 = 101,
      [Description("Access Token vazio")]
      EGEN002,
      [Description("Access Token inativo")]
      EGEN003,
      [Description("Access Token inválido")]
      EGEN004,
      [Description("Utilizador deverá alterar a password de acesso")]
      EGEN005,
      [Description("O id do objeto deverá estar preenchdo")]
      EGEN006,
      [Description("Lamentamos mas de momento não podemos satisfazer o seu pedido")]
      EGEN007,
      [Description("Chave de autenticação Inválida")]
      EGEN008,
      [Description("Chave de autenticação expirada")]
      EGEN009,
      [Description("Número máximo de tentativas de autenticação excedido")]
      EGEN010,
      [Description("Serviço só poderá ser invocado com um Access Token GUEST")]
      EGEN011,
      [Description("Serviço só poderá ser invocado com um Access Token diferente de GUEST")]
      EGEN012,
      #endregion
   }
}