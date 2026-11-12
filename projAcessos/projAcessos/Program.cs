using System;

namespace projAcessos
{
    internal class Program
    {
        static Cadastro cadastro = new Cadastro();

        static void Main(string[] args)
        {
            cadastro.download();       // carga dos dados ao iniciar

            int op;
            do
            {
                Console.WriteLine();
                Console.WriteLine("===== CONTROLE DE ACESSOS =====");
                Console.WriteLine(" 0. Sair");
                Console.WriteLine(" 1. Cadastrar ambiente");
                Console.WriteLine(" 2. Consultar ambiente");
                Console.WriteLine(" 3. Excluir ambiente");
                Console.WriteLine(" 4. Cadastrar usuário");
                Console.WriteLine(" 5. Consultar usuário");
                Console.WriteLine(" 6. Excluir usuário");
                Console.WriteLine(" 7. Conceder permissão de acesso");
                Console.WriteLine(" 8. Revogar permissão de acesso");
                Console.WriteLine(" 9. Registrar acesso");
                Console.WriteLine("10. Consultar logs de acesso");
                op = lerInt("Opção: ");

                switch (op)
                {
                    case 0: break;
                    case 1: cadastrarAmbiente(); break;
                    case 2: consultarAmbiente(); break;
                    case 3: excluirAmbiente(); break;
                    case 4: cadastrarUsuario(); break;
                    case 5: consultarUsuario(); break;
                    case 6: excluirUsuario(); break;
                    case 7: conceder(); break;
                    case 8: revogar(); break;
                    case 9: registrarAcesso(); break;
                    case 10: consultarLogs(); break;
                    default: Console.WriteLine("Opção inválida!"); break;
                }
            } while (op != 0);

            cadastro.upload();         // persistência ao encerrar
            Console.WriteLine("Dados salvos. Encerrando...");
        }

        // ---------- ambientes ----------
        static void cadastrarAmbiente()
        {
            int id = lerInt("Id do ambiente: ");
            if (id <= 0) { Console.WriteLine("O id deve ser maior que zero!"); return; }
            if (cadastro.pesquisarAmbiente(new Ambiente(id)) != null)
            {
                Console.WriteLine("Já existe ambiente com esse id!");
                return;
            }
            Console.Write("Nome: ");
            cadastro.adicionarAmbiente(new Ambiente(id, Console.ReadLine()));
            Console.WriteLine("Ambiente cadastrado.");
        }

        static void consultarAmbiente()
        {
            Ambiente a = pedirAmbiente();
            if (a == null) return;
            Console.WriteLine(a);
            Console.WriteLine("Logs registrados: {0} (máx. {1})", a.Logs.Count, Ambiente.MAX_LOGS);
            Console.WriteLine("Usuários com permissão:");
            var lista = cadastro.usuariosComPermissao(a);
            if (lista.Count == 0)
                Console.WriteLine("  (nenhum)");
            foreach (Usuario u in lista)
                Console.WriteLine("  " + u);
        }

        static void excluirAmbiente()
        {
            Ambiente a = pedirAmbiente();
            if (a == null) return;
            if (cadastro.removerAmbiente(a))
                Console.WriteLine("Ambiente excluído.");
            else
                Console.WriteLine("Não foi possível excluir: ainda há usuários com permissão neste ambiente.");
        }

        // ---------- usuários ----------
        static void cadastrarUsuario()
        {
            int id = lerInt("Id do usuário: ");
            if (id <= 0) { Console.WriteLine("O id deve ser maior que zero!"); return; }
            if (cadastro.pesquisarUsuario(new Usuario(id)) != null)
            {
                Console.WriteLine("Já existe usuário com esse id!");
                return;
            }
            Console.Write("Nome: ");
            cadastro.adicionarUsuario(new Usuario(id, Console.ReadLine()));
            Console.WriteLine("Usuário cadastrado.");
        }

        static void consultarUsuario()
        {
            Usuario u = pedirUsuario();
            if (u == null) return;
            Console.WriteLine(u);
            Console.WriteLine("Ambientes com permissão:");
            if (u.Ambientes.Count == 0)
                Console.WriteLine("  (nenhum)");
            foreach (Ambiente a in u.Ambientes)
                Console.WriteLine("  " + a);
        }

        static void excluirUsuario()
        {
            Usuario u = pedirUsuario();
            if (u == null) return;
            if (cadastro.removerUsuario(u))
                Console.WriteLine("Usuário excluído.");
            else
                Console.WriteLine("Não foi possível excluir: o usuário ainda possui permissão de acesso.");
        }

        // ---------- permissões e acessos ----------
        static void conceder()
        {
            Ambiente a = pedirAmbiente();
            if (a == null) return;
            Usuario u = pedirUsuario();
            if (u == null) return;
            Console.WriteLine(u.concederPermissao(a)
                ? "Permissão concedida."
                : "O usuário já tem permissão neste ambiente.");
        }

        static void revogar()
        {
            Ambiente a = pedirAmbiente();
            if (a == null) return;
            Usuario u = pedirUsuario();
            if (u == null) return;
            Console.WriteLine(u.revogarPermissao(a)
                ? "Permissão revogada."
                : "O usuário não tinha permissão neste ambiente.");
        }

        // Autoriza se o usuário tem permissão; registra o log nos dois casos.
        static void registrarAcesso()
        {
            Ambiente a = pedirAmbiente();
            if (a == null) return;
            Usuario u = pedirUsuario();
            if (u == null) return;

            bool autorizado = u.temPermissao(a);
            a.registrarLog(new Log(DateTime.Now, u, autorizado));
            Console.WriteLine(autorizado ? "ACESSO AUTORIZADO." : "ACESSO NEGADO.");
        }

        static void consultarLogs()
        {
            Ambiente a = pedirAmbiente();
            if (a == null) return;

            Console.WriteLine("Filtrar: 1-Autorizados  2-Negados  3-Todos");
            int f = lerInt("Filtro: ");
            if (f < 1 || f > 3) { Console.WriteLine("Filtro inválido!"); return; }

            int n = 0;
            Console.WriteLine("=== Logs de {0} ===", a.Nome);
            foreach (Log l in a.Logs)           // do mais antigo ao mais recente
            {
                if (f == 3 || (f == 1 && l.TipoAcesso) || (f == 2 && !l.TipoAcesso))
                {
                    Console.WriteLine(l);
                    n++;
                }
            }
            Console.WriteLine("Total listado: {0}", n);
        }

        // ---------- auxiliares ----------
        static Ambiente pedirAmbiente()
        {
            Ambiente a = cadastro.pesquisarAmbiente(new Ambiente(lerInt("Id do ambiente: ")));
            if (a == null) Console.WriteLine("Ambiente não encontrado!");
            return a;
        }

        static Usuario pedirUsuario()
        {
            Usuario u = cadastro.pesquisarUsuario(new Usuario(lerInt("Id do usuário: ")));
            if (u == null) Console.WriteLine("Usuário não encontrado!");
            return u;
        }

        static int lerInt(string msg)
        {
            int v;
            Console.Write(msg);
            while (!int.TryParse(Console.ReadLine(), out v))
            {
                Console.Write("Valor inválido. " + msg);
            }
            return v;
        }
    }
}
