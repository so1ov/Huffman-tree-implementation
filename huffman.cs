using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
 
namespace HuffmanTree
{
    /** Класс ноды содержит в качестве полей указатели на дочерние ветви бинарного дерева Хаффмана, звеном которого является нода.
     * Оставшиеся поля представлены символом char и ключом-частотой повторений символа.
     */
    public class Node
    {
        public char Symbol;
        public int freq;
 
        public Node right;
        public Node left;
 
        /** Метод makeEncode выполняет поиск символа по значению в дереве Хаффмана
         * а также построение бинарного списка-закодированного представления этого символа,
         * цепочка рекурсивных вызовов метода начинается с вызова для корня дерева в методе
         * HuffmanTree::makeEncode.
         */
        public List<bool> makeEncode(char symbol, List<bool> data)
        {
            if (right == null && left == null)
            {
                if (symbol.Equals(this.Symbol))
                {
                    return data;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                List<bool> lstr = null;
                List<bool> rstr = null;
 
                if (left != null)
                {
                    List<bool> leftPath = new List<bool>();
                    leftPath.AddRange(data);
                    leftPath.Add(false);
 
                    lstr = left.makeEncode(symbol, leftPath);
                }
 
                if (right != null)
                {
                    List<bool> rightPath = new List<bool>();
                    rightPath.AddRange(data);
                    rightPath.Add(true);
                    rstr = right.makeEncode(symbol, rightPath);
                }
 
                if (lstr != null)
                {
                    return lstr;
                }
                else
                {
                    return rstr;
                }
            }
        }
    }
 
    /* Класс бинарного дерева Хаффмана содержит список нод, корневую ноду, а также словарь,
     * хранящий результат частотного анализа входной строки.
     */
    public class HuffmanTree
    {
        private List<Node> nodeList = new List<Node>();
        public Node root;
        public Dictionary<char, int> freqDict = new Dictionary<char, int>();
 
        /* Метод buildTree выполняет частотный анализ входной строки и построение для нее
         * бинарного дерева Хаффмана. Сортировка списка нод выполняется по признаку
         * node.freq с использованием соответствующего предиката. В теле нод, не содержащих
         * непосредственно символа, в качестве символа используется '~'.
         */
        public void buildTree(string src)
        {
            for (int i = 0; i < src.Length; i++)
            {
                if (!freqDict.ContainsKey(src[i]))
                {
                    freqDict.Add(src[i], 0);
                }
 
                freqDict[src[i]]++;
            }
 
            foreach (KeyValuePair<char, int> symbol in freqDict)
            {
                nodeList.Add(new Node() { Symbol = symbol.Key, freq = symbol.Value });
            }
 
            while (nodeList.Count > 1)
            {
                List<Node> sortedList = nodeList.OrderBy(node => node.freq).ToList<Node>();
 
                if (sortedList.Count >= 2)
                {
                    List<Node> firstTwo = sortedList.Take(2).ToList<Node>();
 
                    Node parent = new Node()
                    {
                        Symbol = '~',
                        freq = firstTwo[0].freq + firstTwo[1].freq,
                        left = firstTwo[0],
                        right = firstTwo[1]
                    };
 
                    nodeList.Remove(firstTwo[0]);
                    nodeList.Remove(firstTwo[1]);
                    nodeList.Add(parent);
                }
 
                this.root = nodeList.FirstOrDefault();
 
            }
 
        }
 
        /* Метод encodeStr выполняет кодирование входной строки с помощью имеющегося дерева Хаффмана
         * и преобразование ее в последовательность бит.
         */
        public BitArray encodeStr(string src)
        {
            List<bool> buffList = new List<bool>();
 
            for (int i = 0; i < src.Length; i++)
            {
                List<bool> chList = this.root.makeEncode(src[i], new List<bool>());
                buffList.AddRange(chList);
            }
 
            BitArray ret = new BitArray(buffList.ToArray());
 
            return ret;
        }
 
        /* Метод decodeBits выполняет декодирование входного массива бит на основе имеющегося
         * дерева Хаффмана, двигаясь по дереву последовательно в соответствии с данными, полученными из массива бит.
         * Метод начинает следующую итерацию как только достигнут лист дерева Хаффмана, т.е. найден очередной символ.
         */
        public string decodeBits(BitArray src)
        {
            Node curr = this.root;
            string ret = "";
 
            foreach (bool it in src)
            {
                if (it)
                {
                    if (curr.right != null)
                    {
                        curr = curr.right;
                    }
                }
                else
                {
                    if (curr.left != null)
                    {
                        curr = curr.left;
                    }
                }
 
                if (isLeaf(curr))
                {
                    ret += curr.Symbol;
                    curr = this.root;
                }
            }
 
            return ret;
        }
 
        /* Метод isLeaf проверки ноды дерева Хаффмана на наличие ветвей
         * проверяет наличие ссылок на ветви у обьекта ноды, и возвращает true
         * если нода(звено) является листом, т.е. не имеет ветвей.
         */
        public bool isLeaf(Node node)
        {
            return (node.left == null && node.right == null);
        }
 
    }
 
    class Program
    {
        static void Main(string[] args)
        {
            /* Ввод строки для кодирования */
            Console.WriteLine("Enter the string to encode:");
            string inputStr = Console.ReadLine();
 
            /* Инициализация, построение дерева Хаффмана.
             * Кодирование входной последовательности.
             */
            HuffmanTree hfTree = new HuffmanTree();
            hfTree.buildTree(inputStr);
            BitArray encodedBitSeq = hfTree.encodeStr(inputStr);
 
            /* Вывод закодированной последовательности с использованием конструкции foreach
             * для массива бит.
             */
            Console.Write("Encoded string: ");
            foreach (bool it in encodedBitSeq)
            {
                Console.Write((it ? 1 : 0) + "");
            }
            Console.WriteLine();
 
            /* Декодирование массива бит и вывод результата на экран. */
            string decodedStr = hfTree.decodeBits(encodedBitSeq);
            Console.WriteLine("Decoded string: " + decodedStr);
 
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
