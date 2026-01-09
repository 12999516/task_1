using System.Diagnostics;
using System.Threading.Tasks;
using static System.Console;

namespace es
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var stampe = new List<(string nome, int pag)>
            {
                ("file_italiano.pdf", 5),
                ("file_storia.pdf", 2),
                ("file_st.pdf", 6),
            };

            WriteLine("stampa sequenziale");
            long tempoSequenziale = await stampasequenziale(stampe);

            WriteLine("stampa parallela");
            long tempoParallelo = await stampaparallela(stampe);

            WriteLine($"tempo sequenziale {tempoSequenziale} \n\rtempo parallelo {tempoParallelo}");
        }

        static async Task<string> stampa(string nome, int pagine)
        {
            WriteLine($"Inizio stampa file {nome} con {pagine} pagine...");

            await Task.Delay(pagine * 300);

            return $"Fine stampa file {nome} con {pagine} pagine.";
        }

        static async Task<long> stampasequenziale(List<(string nome, int pagine)> documenti)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            for(int i = 0; i < documenti.Count; i ++)
            {
                string risultato = await stampa(documenti[i].nome, documenti[i].pagine);
                WriteLine(risultato);
            }

            stopwatch.Stop();

            return stopwatch.ElapsedMilliseconds;
        }

        static async Task<long> stampaparallela(List<(string nome, int pagine)> documenti)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            List<Task<string>> tasks = new List<Task<string>>();

            for(int i = 0; i < documenti.Count; i ++)
            {
                tasks.Add(stampa(documenti[i].nome, documenti[i].pagine));
            }

            List<Task<string>> taskdafare = new List<Task<string>>(tasks);

            while (taskdafare.Count > 0)
            {
                Task<string> finito = await Task.WhenAny(taskdafare);
                WriteLine("--" + finito.Result);
                taskdafare.Remove(finito);
            }

            await Task.WhenAll(tasks);

            stopwatch.Stop();

            return stopwatch.ElapsedMilliseconds;
        }
    }
}
