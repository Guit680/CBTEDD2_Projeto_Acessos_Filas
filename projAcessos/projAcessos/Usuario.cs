using System;
using System.Collections.Generic;

namespace projAcessos
{
    internal class Usuario
    {
        private int id;
        private string nome;
        private List<Ambiente> ambientes;

        public int Id { get => id; set => id = value; }
        public string Nome { get => nome; set => nome = value; }
        public List<Ambiente> Ambientes { get => ambientes; }

        public Usuario(int id, string nome)
        {
            this.id = id;
            this.nome = nome;
            this.ambientes = new List<Ambiente>();
        }

        public Usuario(int id) : this(id, "")
        { }

        public Usuario() : this(0, "")
        { }

        // Cada usuário só pode ter uma permissão por ambiente.
        public bool concederPermissao(Ambiente ambiente)
        {
            if (ambiente == null || ambientes.Contains(ambiente))
                return false;
            ambientes.Add(ambiente);
            return true;
        }

        public bool revogarPermissao(Ambiente ambiente)
        {
            return ambientes.Remove(ambiente);
        }

        public bool temPermissao(Ambiente ambiente)
        {
            return ambientes.Contains(ambiente);
        }

        public override string ToString()
        {
            return id + " - " + nome;
        }

        public override bool Equals(object obj)
        {
            Usuario outro = obj as Usuario;
            return outro != null && this.id == outro.id;
        }

        public override int GetHashCode()
        {
            return id.GetHashCode();
        }
    }
}
