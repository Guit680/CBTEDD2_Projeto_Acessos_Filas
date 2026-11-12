using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace projAcessos
{
    internal class Cadastro
    {
        // Modelo relacional: uma "tabela" (arquivo) por entidade/relacionamento
        //   ambientes.csv   : id;nome
        //   usuarios.csv    : id;nome
        //   permissoes.csv  : usuario_id;ambiente_id        (N:N usuário-ambiente)
        //   logs.csv        : ambiente_id;usuario_id;usuario_nome;dt_acesso;autorizado
        private const string FORMATO_DATA = "yyyy-MM-dd HH:mm:ss";

        private readonly string pasta;
        private List<Usuario> usuarios = new List<Usuario>();
        private List<Ambiente> ambientes = new List<Ambiente>();

        public List<Usuario> Usuarios { get => usuarios; }
        public List<Ambiente> Ambientes { get => ambientes; }

        public Cadastro()
        {
            pasta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dados");
        }

        public Cadastro(string pasta)
        {
            this.pasta = pasta;
        }

        // ---------- usuários ----------
        public void adicionarUsuario(Usuario usuario)
        {
            usuarios.Add(usuario);
        }

        // Só remove se o usuário não tem nenhuma permissão de acesso.
        public bool removerUsuario(Usuario usuario)
        {
            Usuario achado = pesquisarUsuario(usuario);
            if (achado == null || achado.Ambientes.Count > 0)
                return false;
            return usuarios.Remove(achado);
        }

        public Usuario pesquisarUsuario(Usuario usuario)
        {
            int i = usuarios.IndexOf(usuario);
            return i >= 0 ? usuarios[i] : null;
        }

        // ---------- ambientes ----------
        public void adicionarAmbiente(Ambiente ambiente)
        {
            ambientes.Add(ambiente);
        }

        // Não remove ambiente que ainda tenha usuários com permissão.
        public bool removerAmbiente(Ambiente ambiente)
        {
            Ambiente achado = pesquisarAmbiente(ambiente);
            if (achado == null || usuariosComPermissao(achado).Count > 0)
                return false;
            return ambientes.Remove(achado);
        }

        public Ambiente pesquisarAmbiente(Ambiente ambiente)
        {
            int i = ambientes.IndexOf(ambiente);
            return i >= 0 ? ambientes[i] : null;
        }

        public List<Usuario> usuariosComPermissao(Ambiente ambiente)
        {
            return usuarios.FindAll(u => u.temPermissao(ambiente));
        }

        // ---------- persistência ----------
        // Grava todas as tabelas (chamado ao encerrar a aplicação).
        public void upload()
        {
            Directory.CreateDirectory(pasta);

            List<string> tab = new List<string>();
            tab.Add("id;nome");
            foreach (Ambiente a in ambientes)
                tab.Add(a.Id + ";" + limpa(a.Nome));
            File.WriteAllLines(Path.Combine(pasta, "ambientes.csv"), tab.ToArray());

            tab = new List<string>();
            tab.Add("id;nome");
            foreach (Usuario u in usuarios)
                tab.Add(u.Id + ";" + limpa(u.Nome));
            File.WriteAllLines(Path.Combine(pasta, "usuarios.csv"), tab.ToArray());

            tab = new List<string>();
            tab.Add("usuario_id;ambiente_id");
            foreach (Usuario u in usuarios)
                foreach (Ambiente a in u.Ambientes)
                    tab.Add(u.Id + ";" + a.Id);
            File.WriteAllLines(Path.Combine(pasta, "permissoes.csv"), tab.ToArray());

            tab = new List<string>();
            tab.Add("ambiente_id;usuario_id;usuario_nome;dt_acesso;autorizado");
            foreach (Ambiente a in ambientes)
                foreach (Log l in a.Logs)
                    tab.Add(a.Id + ";" + l.Usuario.Id + ";" + limpa(l.Usuario.Nome) + ";" +
                            l.DtAcesso.ToString(FORMATO_DATA, CultureInfo.InvariantCulture) + ";" +
                            (l.TipoAcesso ? "1" : "0"));
            File.WriteAllLines(Path.Combine(pasta, "logs.csv"), tab.ToArray());
        }

        // Carrega as tabelas (chamado ao iniciar). Arquivos ausentes = cadastro vazio.
        public void download()
        {
            usuarios = new List<Usuario>();
            ambientes = new List<Ambiente>();

            foreach (string[] c in ler("ambientes.csv", 2))
                ambientes.Add(new Ambiente(int.Parse(c[0]), c[1]));

            foreach (string[] c in ler("usuarios.csv", 2))
                usuarios.Add(new Usuario(int.Parse(c[0]), c[1]));

            foreach (string[] c in ler("permissoes.csv", 2))
            {
                Usuario u = pesquisarUsuario(new Usuario(int.Parse(c[0])));
                Ambiente a = pesquisarAmbiente(new Ambiente(int.Parse(c[1])));
                if (u != null && a != null)
                    u.concederPermissao(a);
            }

            foreach (string[] c in ler("logs.csv", 5))
            {
                Ambiente a = pesquisarAmbiente(new Ambiente(int.Parse(c[0])));
                if (a == null) continue;
                int idUsu = int.Parse(c[1]);
                // usuário já excluído: mantém o histórico com um usuário "avulso"
                Usuario u = pesquisarUsuario(new Usuario(idUsu)) ?? new Usuario(idUsu, c[2]);
                DateTime dt = DateTime.ParseExact(c[3], FORMATO_DATA, CultureInfo.InvariantCulture);
                a.registrarLog(new Log(dt, u, c[4] == "1"));
            }
        }

        // Lê as linhas de dados de um arquivo (ignora cabeçalho e linhas inválidas).
        private List<string[]> ler(string arquivo, int colunas)
        {
            List<string[]> linhas = new List<string[]>();
            string caminho = Path.Combine(pasta, arquivo);
            if (!File.Exists(caminho))
                return linhas;

            string[] todas = File.ReadAllLines(caminho);
            for (int i = 1; i < todas.Length; i++)     // linha 0 = cabeçalho
            {
                string[] c = todas[i].Split(';');
                if (c.Length == colunas)
                    linhas.Add(c);
            }
            return linhas;
        }

        // Evita que ';' ou quebra de linha no texto quebrem o formato do arquivo.
        private static string limpa(string s)
        {
            return (s ?? "").Replace(';', ',').Replace('\r', ' ').Replace('\n', ' ');
        }
    }
}
