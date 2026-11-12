using System;

namespace projAcessos
{
    internal class Log
    {
        private DateTime dtAcesso;
        private Usuario usuario;
        private bool tipoAcesso;   // true = Autorizado, false = Negado

        public DateTime DtAcesso { get => dtAcesso; set => dtAcesso = value; }
        public Usuario Usuario { get => usuario; set => usuario = value; }
        public bool TipoAcesso { get => tipoAcesso; set => tipoAcesso = value; }

        public Log(DateTime dtAcesso, Usuario usuario, bool tipoAcesso)
        {
            this.dtAcesso = dtAcesso;
            this.usuario = usuario;
            this.tipoAcesso = tipoAcesso;
        }

        public Log() : this(DateTime.Now, new Usuario(), false)
        { }

        public override string ToString()
        {
            return string.Format("{0:dd/MM/yyyy HH:mm:ss} - {1} - {2}",
                dtAcesso, usuario.Nome + " (id " + usuario.Id + ")",
                tipoAcesso ? "AUTORIZADO" : "NEGADO");
        }
    }
}
