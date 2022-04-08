using System;
using System.Diagnostics;
using System.IO;


namespace DFAOperator
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Automata auto1 = new Automata("input1.txt");
            Automata auto2 = new Automata("input2.txt");
            Automata auto3 = new Automata("input3.txt");

            Automata product = auto1.Product(auto2);
            Automata union = auto1.Union(auto2);
            Automata difference = auto1.Difference(auto2);
            Automata compl1 = auto1.Complement();
            Automata compl2 = auto2.Complement();
            string NFAtoDFALog = "";
            Automata NFAtoDFA = auto3.TompsonAlgorithm(out NFAtoDFALog);

            Console.WriteLine(NFAtoDFALog);

            DotToSvg(auto1.ToDot(), "graph1.svg");
            DotToSvg(auto2.ToDot(), "graph2.svg");
            DotToSvg(auto3.ToDot(), "graph3.svg");
            DotToSvg(product.ToDot(), "product.svg");
            DotToSvg(union.ToDot(), "union.svg");
            DotToSvg(difference.ToDot(), "difference.svg");
            DotToSvg(compl1.ToDot(), "compl1.svg");
            DotToSvg(compl2.ToDot(), "compl2.svg");
            DotToSvg(NFAtoDFA.ToDot(), "dfa.svg");

            string markdown = "# DFAOperator\n" +
                "* Имеем два входных автомата:\n" +
                "## Автомат 1\n" +
                $"```c\n{auto1}\n```\n" +
                "![](graph1.svg)\n" +
                "## Автомат 2\n" +
                $"```c\n{auto2}\n```\n" +
                "![](graph2.svg)\n" +
                "## Произведение:\n" +
                $"```c\n{product}\n```\n" +
                "![](product.svg)\n" +
                "## Объединение:\n" +
                $"```c\n{union}\n```\n" +
                "![](union.svg)\n" +
                "## Разность:\n" +
                $"```c\n{difference}\n```\n" +
                "![](difference.svg)\n" +
                "## Дополнение первого автомата:\n" +
                $"```c\n{compl1}\n```\n" +
                "![](compl1.svg)\n" +
                "## Дополнение второго автомата:\n" +
                $"```c\n{compl2}\n```\n" +
                "![](compl2.svg)\n" +
                "# Перевод НКА в ДКА:\n" +
                "* Исходный автомат:\n\n" +
                "![](graph3.svg)\n" +
                "* Переводим в эквивалентный ДКА:\n" +
                $"{NFAtoDFALog}\n" +
                "![](dfa.svg)\n";

            File.WriteAllText("result.md", markdown);

            Console.WriteLine("Done!");
        }

        static void DotToSvg(string dot, string output) 
        {
            File.WriteAllText("graph.dot", dot);

            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.Arguments = $"/C dot -Tsvg <graph.dot >{output}";
            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            p.Start();
            p.WaitForExit();
            Console.WriteLine($"{output} - Done!");
        }

    }
}
