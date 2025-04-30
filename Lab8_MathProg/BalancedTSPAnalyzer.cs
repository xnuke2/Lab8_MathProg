using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab8_MathProg
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.IO;
    using System.Windows.Forms;

    public class BalancedTSPAnalyzer
    {
        public int[,] distances;
        private int cityCount;
        private Dictionary<string, PathRecord> pathRecords;
        private List<PathLog> pathLogs;
        public List<int> optimalPath;
        private int optimalCost = int.MaxValue;

        public BalancedTSPAnalyzer(int[,] distanceMatrix)
        {
            this.distances = distanceMatrix;
            this.cityCount = distanceMatrix.GetLength(0);
            this.pathRecords = new Dictionary<string, PathRecord>();
            this.pathLogs = new List<PathLog>();
        }

        public (int cost, List<int> path) FindOptimalPath()
        {
            if (cityCount == 1) return (0, new List<int> { 0 });

            // Используем очередь для пошагового исследования всех путей
            var queue = new List<PathState>();
            var initialVisited = new bool[cityCount];
            initialVisited[0] = true;
            queue.Add(new PathState(0, initialVisited, new List<int> { 0 }, 0));

            while (queue.Count > 0)
            {
                var currentState = queue[0];
                queue.RemoveAt(0);
                ProcessState(currentState, queue);
            }

            return (optimalCost, optimalPath);
        }

        private void ProcessState(PathState state, List<PathState> queue)
        {
            string stateKey = $"{state.CurrentCity};{VisitedKey(state.Visited)}";


            // Проверка на отбрасывание пути
            if (pathRecords.TryGetValue(stateKey, out var existing))
            {

                if (state.Path.Count >= 3)
                {
                    LogPath(state.Path, state.CurrentCost, false,
                        $"есть более короткий путь к городу {state.CurrentCity + 1}");
                    var tmp = queue.Find(f => f.CurrentCity == state.CurrentCity && VisitedKey(f.Visited) == VisitedKey(state.Visited));


                }
                return;

            }

            // Сохраняем текущий путь
            pathRecords[stateKey] = new PathRecord(
                state.CurrentCity,
                state.CurrentCost,
                new List<int>(state.Path),
                GetVisitedSet(state.Visited)
            );
            bool isComplete = state.Path.Count == cityCount;
            // Обработка полного пути
            if (isComplete)
            {
                int totalCost = state.CurrentCost + distances[state.CurrentCity, 0];
                bool isOptimal = totalCost < optimalCost;
                string reason = isOptimal ? null : "не минимальная стоимость";
                state.Path.Add(0);
                LogPath(state.Path, totalCost, isOptimal, reason);

                if (isOptimal)
                {
                    optimalCost = totalCost;
                    optimalPath = new List<int>(state.Path);
                }
                return;
            }

            // Добавляем в очередь все возможные продолжения
            var nextCities = Enumerable.Range(0, cityCount)
                                     .Where(c => !state.Visited[c])
                                     .OrderBy(c => distances[state.CurrentCity, c])
                                     .ToList();

            foreach (int next in nextCities)
            {
                var newVisited = (bool[])state.Visited.Clone();
                newVisited[next] = true;
                var newPath = new List<int>(state.Path) { next };
                int newCost = state.CurrentCost + distances[state.CurrentCity, next];

                queue.Add(new PathState(next, newVisited, newPath, newCost));
            }
        }

        // Остальные вспомогательные методы остаются без изменений
        private HashSet<int> GetVisitedSet(bool[] visited)
        {
            var set = new HashSet<int>();
            for (int i = 0; i < cityCount; i++)
                if (visited[i]) set.Add(i);
            return set;
        }

        private string VisitedKey(bool[] visited)
        {
            var key = new System.Text.StringBuilder();
            for (int i = 0; i < cityCount; i++)
                key.Append(visited[i] ? '1' : '0');
            return key.ToString();
        }

        private void LogPath(List<int> path, int cost, bool isOptimal, string rejectReason)
        {
            if (path.Count >= 3)
            {
                pathLogs.Add(new PathLog
                {
                    Path = new List<int>(path),
                    Cost = cost,
                    IsOptimal = isOptimal,
                    IsComplete = path.Count == cityCount,
                    RejectReason = rejectReason
                });
            }
        }
        public void PrintAnalysis(TextBox outputTextBox)
        {
            if (outputTextBox == null)
                throw new ArgumentNullException(nameof(outputTextBox));

            outputTextBox.Clear();
            outputTextBox.AppendText("Анализ путей (длина 3 и более):\r\n");
            outputTextBox.AppendText("-------------------------------\r\n");

            int stage = 4;

            while (stage <= cityCount)
            {
                var rejectedLength3Paths = pathLogs
                    .Where(p => p.Path.Count == stage && p.RejectReason != null)
                    .OrderBy(p => p.Cost)
                    .ToList();

                if (rejectedLength3Paths.Any())
                {
                    outputTextBox.AppendText($"\r\nОтброшенные пути длины {(stage - 1)}:\r\n");
                    foreach (var path in rejectedLength3Paths)
                    {
                        var tmpPath = path.Path.Select(x => x + 1).ToList();
                        outputTextBox.AppendText($"{string.Join(" → ", tmpPath)} (стоимость: {path.Cost}) - {path.RejectReason}\r\n");
                    }
                }
                stage++;
            }

            // Полные пути
            var completePaths = pathLogs
                .Where(p => p.Path.Count == cityCount + 1)
                .OrderBy(p => p.Cost)
                .ToList();

            outputTextBox.AppendText("\r\nПолные пути:\r\n");
            foreach (var path in completePaths)
            {
                string status = path.Cost == optimalCost
                    ? "- ОПТИМАЛЬНЫЙ"
                    : $"- ОТБРОШЕН: {path.RejectReason}";
                var tmpPath = path.Path.Select(x => x + 1).ToList();
                outputTextBox.AppendText($"{string.Join(" → ", tmpPath)} (стоимость: {path.Cost}) {status}\r\n");
            }

            var tmpPathFin = optimalPath.Select(x => x + 1).ToList();
            outputTextBox.AppendText("\r\nИтоговый оптимальный путь:\r\n");
            outputTextBox.AppendText($"{string.Join(" → ", tmpPathFin)} (стоимость: {optimalCost})\r\n");
        }
        public void PrintAnalysis()
        {
            Console.WriteLine("Анализ путей (длина 3 и более):");
            Console.WriteLine("-------------------------------");

            int stage = 4;

            while (stage <= cityCount)
            {

                var rejectedLength3Paths = pathLogs
                    .Where(p => p.Path.Count == stage && p.RejectReason != null)
                    .OrderBy(p => p.Cost)
                    .ToList();
                if (rejectedLength3Paths.Any())
                {
                    Console.WriteLine("\nОтброшенные пути длины " + (stage - 1) + ":");
                    foreach (var path in rejectedLength3Paths)
                    {
                        var tmpPath = new List<int>(path.Path);
                        for (int i = 0; i < tmpPath.Count; i++)
                            tmpPath[i]++;

                        Console.WriteLine($"{string.Join(" → ", tmpPath)} (стоимость: {path.Cost}) - {path.RejectReason}");
                    }
                }
                stage++;
            }
            // Полные пути
            var completePaths = pathLogs
                .Where(p => p.Path.Count == cityCount + 1)
                .OrderBy(p => p.Cost)
                .ToList();

            Console.WriteLine("\nПолные пути:");
            foreach (var path in completePaths)
            {
                string status = path.Cost == optimalCost
                    ? "- ОПТИМАЛЬНЫЙ"
                    : $"- ОТБРОШЕН: {path.RejectReason}";
                var tmpPath = new List<int>(path.Path);
                for (int i = 0; i < tmpPath.Count; i++)
                    tmpPath[i]++;
                Console.WriteLine($"{string.Join(" → ", (tmpPath))}  (стоимость: {path.Cost}) {status}");
            }
            var tmpPathFin = new List<int>(optimalPath);
            for (int i = 0; i < tmpPathFin.Count; i++)
                tmpPathFin[i]++;
            Console.WriteLine("\nИтоговый оптимальный путь:");
            Console.WriteLine($"{string.Join(" → ", tmpPathFin)} (стоимость: {optimalCost})");
        }
    }

    // Новый класс для хранения состояния пути в очереди
    public class PathState
    {
        public int CurrentCity { get; }
        public bool[] Visited { get; }
        public List<int> Path { get; }
        public int CurrentCost { get; }

        public PathState(int currentCity, bool[] visited, List<int> path, int currentCost)
        {
            CurrentCity = currentCity;
            Visited = visited;
            Path = path;
            CurrentCost = currentCost;
        }
    }

    // Остальные классы (PathRecord, PathLog) остаются без изменений


    public class PathRecord
    {
        public int LastCity { get; }
        public int Cost { get; }
        public List<int> Path { get; }
        public HashSet<int> Visited { get; }

        public PathRecord(int lastCity, int cost, List<int> path, HashSet<int> visited)
        {
            LastCity = lastCity;
            Cost = cost;
            Path = path;
            Visited = visited;
        }
    }

    public class PathLog
    {
        public List<int> Path { get; set; }
        public int Cost { get; set; }
        public bool IsOptimal { get; set; }
        public bool IsComplete { get; set; }
        public string RejectReason { get; set; }
    }
}
