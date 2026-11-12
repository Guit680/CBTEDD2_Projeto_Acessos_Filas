using System;
using System.Collections.Generic;

namespace projAcessos
{
    internal class Ambiente
    {
        public const int MAX_LOGS = 100;

        private int id;
        private string nome;
        private Queue<Log> logs;

        public int Id { get => id; set => id = value; }
        public string Nome { get => nome; set => nome = value; }
        public Queue<Log> Logs { get => logs; }

        public Ambiente(int id, string nome)
        {
            this.id = id;
            this.nome = nome;
            this.logs = new Queue<Log>();
        }

        public Ambiente(int id) : this(id, "")
        { }

        public Ambiente() : this(0, "")
        { }

        // Mantém no máximo 100 logs: ao exceder, descarta o mais antigo.
        public void registrarLog(Log log)
        {
            logs.Enqueue(log);
            while (logs.Count > MAX_LOGS)
                logs.Dequeue();
        }

        public override string ToString()
        {
            return id + " - " + nome;
        }

        public override bool Equals(object obj)
        {
            Ambiente outro = obj as Ambiente;
            return outro != null && this.id == outro.id;
        }

        public override int GetHashCode()
        {
            return id.GetHashCode();
        }
    }
}
