using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DFAOperator
{
    public class Automata
    {
        public HashSet<string> Alphabet { get; set; }
        public HashSet<string> Vertices { get; set; }
        public HashSet<string> Terminals { get; set; }
        public string Start { get; set; }
        public bool Deterministic { get; set; }
        public Dictionary<string,string> Transitions { get; set; }

        public Automata() 
        {
            Deterministic = true;
            Alphabet = new HashSet<string>();
            Vertices = new HashSet<string>();
            Terminals = new HashSet<string>();
            Start = "";
            Transitions = new Dictionary<string,string>();
        }

        public Automata(Automata auto) 
        {
            Deterministic = auto.Deterministic;
            Alphabet = new HashSet<string>(auto.Alphabet);
            Vertices = new HashSet<string>(auto.Vertices);
            Terminals = new HashSet<string>(auto.Terminals);
            Start = auto.Start;
            Transitions = new Dictionary<string, string>(auto.Transitions);
        }

        public Automata(string filePath) 
        {
            Deterministic = true;
            List<string> rawData = File.ReadAllLines(filePath).ToList();

            Alphabet = Collect(rawData[0].Split()).ToHashSet();
            Vertices = Collect(rawData[1].Split()).ToHashSet();
            Terminals = Collect(rawData[2].Split()).ToHashSet();
            Start = rawData[3];

            Transitions = new Dictionary<string, string>();
            foreach (string s in rawData.Skip(4))
                if (s != "")
                {
                    string[] raw = s.Split();
                    if (!Transitions.Keys.Contains($"{raw[0]},{raw[1]}"))
                        Transitions.Add($"{raw[0]},{raw[1]}", raw[2]);
                    else
                        Transitions[$"{raw[0]},{raw[1]}"] += $",{raw[2]}";

                    if ((Transitions[$"{raw[0]},{raw[1]}"]).Contains(','))
                        Deterministic = false;
                }
            
        }

        public override string ToString() => $"\u03a3 = {{ {string.Join(" ", Alphabet)} }}  \n" +
                $"Q = {{ {string.Join(" ", Vertices)} }}  \n" +
                $"T = {{ {string.Join(" ", Terminals)} }}  \n" +
                $"s = {Start}  \n" +
                $"Transition table:  \n" +
                $"{string.Join("  \n", Deterministic ? Transitions.Select(d => $"\u03b4({d.Key}) = {d.Value}") : Transitions.Select(d => string.Join("  \n", d.Value.Split(',').Select(o => $"\u03b4({d.Key}) = {o}"))))}";

        public string ToDot(int size = 5) => "digraph {" +
                $"size = {size};" +
                "rankdir = LR;" +
                "margin = 0;" +
                "node[shape=circle];" +
                $"{string.Join(",", Terminals.Select(l => $"\"{l}\""))}[shape=doublecircle];" +
                "\" \"[color=white width=0];" +
                string.Join(" ", Deterministic ? Transitions.Select(d => $"\"{d.Key.Substring(0,d.Key.LastIndexOf(','))}\" -> \"{d.Value}\"[label=\"{d.Key.Substring(d.Key.LastIndexOf(',')+1)}\"];") : Transitions.Select(d => string.Join(" ", d.Value.Split(',').Select(o => $"\"{d.Key.Split(',')[0]}\" -> \"{o}\"[label=\"{(d.Key.Split(',')[1] != "~" ? d.Key.Split(',')[1] : "\u03bb")}\"];")))) +
                "}";

        public Automata Complement()
        {
            Automata result = new Automata(this);
            result.Terminals = Vertices.Except(Terminals).ToHashSet();
            return result;
        }

        public Automata Product(Automata auto) => OperationBase(auto, a => CartesianProduct(Terminals, a.Terminals).ToHashSet());

        public Automata Difference(Automata auto) => OperationBase(auto, a => CartesianProduct(Terminals, a.Vertices.Except(a.Terminals).ToHashSet()).ToHashSet());

        public Automata Union(Automata auto) => OperationBase(auto, a => CartesianProduct(Terminals, a.Vertices).Union(CartesianProduct(Vertices, a.Terminals)).ToHashSet());

        private Automata OperationBase(Automata auto, Func<Automata, HashSet<string>> terminalsFunc) 
        {
            Automata result = new Automata();

            result.Alphabet = Alphabet.Union(auto.Alphabet).ToHashSet();
            result.Vertices = CartesianProduct(Vertices, auto.Vertices).ToHashSet();
            result.Start = $"{Start}-{auto.Start}";
            result.Terminals = terminalsFunc(auto);

            foreach (string s in result.Vertices)
                foreach (string sym in result.Alphabet)
                {
                    string[] verts = s.Split('-');
                    if (Transitions.Keys.Contains($"{verts[0]},{sym}") && auto.Transitions.Keys.Contains($"{verts[1]},{sym}"))
                        result.Transitions.Add($"{s},{sym}", $"{Transitions[$"{verts[0]},{sym}"]}-{auto.Transitions[$"{verts[1]},{sym}"]}");
                }

            return result;
        }

        public Automata TompsonAlgorithm(out string NFAtoDFA) 
        {
            


            Automata det = new Automata();
            Automata old = RemoveLambdas();

            det.Deterministic = true;
            det.Alphabet = old.Alphabet;

            Queue<string> P = new Queue<string>();

            P.Enqueue($"{{{Start}}}");
            det.Vertices.Add($"{{{Start}}}");
            while (P.Count != 0)
            {
                string pd = P.Dequeue();

                foreach (string sym in old.Alphabet)
                {
                    string qd = "";

                    foreach (string p in pd.Substring(1, pd.Length-2).Split(','))
                        if(old.Transitions.Keys.Contains($"{p},{sym}"))
                            qd += $",{old.Transitions[$"{p},{sym}"]}";

                    
                    if (qd.Length != 0 && qd[0] == ',')
                        qd = qd.Substring(1);

                    qd = $"{{{string.Join(",", qd.Split(',').OrderBy(d => d).ToHashSet())}}}";


                    if (qd != "{}")
                    {
                        det.Transitions.Add($"{pd},{sym}", $"{qd}");

                        if (!det.Vertices.Contains(qd))
                        {
                            P.Enqueue(qd);
                            det.Vertices.Add(qd);
                        }
                    }
                        
                }
            }

            
            int max = det.Vertices.Select(d => d.Length).Max();
            NFAtoDFA = $"|{new string(' ', max + 2)}|{string.Join($"|", Alphabet.Select(d => $" {d}{new string(' ', max - d.Length + 1)}"))}|\n" +
                $"|{string.Concat(Enumerable.Repeat($"{new string('-', max + 2)}|", Alphabet.Count + 1))}\n";
                   
            

            foreach (string qd in det.Vertices)
            {
                Console.WriteLine(qd);
                
                NFAtoDFA += $"| {qd}{new string(' ', max - qd.Length + 1)}";
                foreach (string sym in det.Alphabet)
                {
                    
                    if (det.Transitions.Keys.Contains($"{qd},{sym}"))
                        NFAtoDFA += $"| {det.Transitions[$"{qd},{sym}"]}{new string(' ', max - det.Transitions[$"{qd},{sym}"].Length + 1)}";
                    else
                        NFAtoDFA += $"|{new string(' ', max + 2)}";
                }
                NFAtoDFA += "|  \n";
                foreach (string p in qd.Substring(1, qd.Length - 2).Split(','))
                    if (old.Terminals.Contains(p))
                    {
                        det.Terminals.Add(qd);
                        break;
                    }
            }

            det.Start = $"{{{Start}}}";

            return det;
        
        }

        private Automata RemoveLambdas() 
        {   
            Automata result = new Automata(this);
            foreach(string q in result.Vertices)
                result.Transitions = GetAll(result.Transitions, q, new List<string>() { q }, q);
            return result;
        }

        private Dictionary<string, string> GetAll(Dictionary<string,string> transitions, string start, List<string> passed, string current) 
        {
            if (transitions.Keys.Contains($"{current},~"))
            {
                foreach (string q in transitions[$"{current},~"].Split(','))
                    if (!passed.Contains(q))
                    {
                        passed.Add(q);
                        foreach (string sym in Alphabet)
                        {
                            if (transitions.Keys.Contains($"{q},{sym}"))
                            {
                                if (transitions.Keys.Contains($"{start},{sym}"))
                                    transitions[$"{start},{sym}"] += $",{transitions[$"{q},{sym}"]}";
                                else
                                    transitions.Add($"{start},{sym}", transitions[$"{q},{sym}"]);
                            }


                            transitions = GetAll(transitions, start, passed, q);

                        }
                    }
            }

            return transitions;
        }

        public Automata NormalizeVertices() 
        {
            Automata result = new Automata(this);
            List<string> vertices = result.Vertices.ToList();
            List<string> terminals = result.Terminals.ToList();

            for (int i = 0; i < vertices.Count; i++) 
            {
                string temp = vertices[i];
                vertices[i] = i.ToString();

                if(terminals.Contains(temp))
                    terminals[terminals.IndexOf(temp)] = i.ToString();

                if (result.Start == temp)
                    result.Start = i.ToString();

                foreach (string sym in result.Alphabet)
                    if (result.Transitions.Keys.Contains($"{temp},{sym}"))
                    {
                        result.Transitions.Add($"{i},{sym}", result.Vertices.ToList().IndexOf(result.Transitions[$"{temp},{sym}"]).ToString());
                        result.Transitions.Remove($"{temp},{sym}");
                    }
            }

            result.Vertices = vertices.ToHashSet();
            result.Terminals = terminals.ToHashSet();

            return result;
        }
        private IEnumerable<string> CartesianProduct(HashSet<string> hs1, HashSet<string> hs2)
        {
            foreach (string s1 in hs1)
                foreach (string s2 in hs2)
                    yield return $"{s1}-{s2}";
        }

        private IEnumerable<string> Collect(IEnumerable<string> collection) 
        {
            foreach (string s in collection)
                yield return s;
        }

    }
}
