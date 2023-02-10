namespace AgileBlazor.Shared.Models.Authentication
{
    public class UserData
    {
        /// <summary>
        /// Login - ret.Usuario.NomeUsuario
        /// </summary>
        public string UserDatabase { get; set; }

        /// <summary>
        /// Login do usuário
        /// Parametro utilizado nos filtros do WebService
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// NomeBaseDados - ret.Usuario.NomeBaseDados
        /// </summary>
        public string AccessKey { get; set; }

        /// <summary>
        /// Id da sessão - ret.Usuario.IdSessao
        /// </summary>
        public string SessionUserId { get; set; }

        /// <summary>
        /// Id da empresa - está sempre fixo em 01
        /// </summary>
        public string Customer { get; set; }

        /// <summary>
        /// Id da divisão - está sempre fixo em 01
        /// </summary>
        public string Division { get; set; }

        /// <summary>
        /// Senha do usuário
        /// </summary>
        public string Password { get; set; }
    }
}
