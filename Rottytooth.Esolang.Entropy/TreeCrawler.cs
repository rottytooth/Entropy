using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Parsing;

namespace Rottytooth.Esolang.Entropy
{
    public class TreeCrawler
    {
        private ParseTree _programTree;

        public ParseTree ProgramTree
        {
            get { return _programTree; }
        }

        public TreeCrawler(ParseTree programTree)
        {
            _programTree = programTree;
        }

        public void DisplayTree()
        {
            DisplayTree(_programTree.Root, 1);
        }

        private void DisplayTree(ParseTreeNode currentNode, int level)
        {

            if (currentNode == null) return;
            DisplaySpaced(currentNode.Term.ToString(), level);

            if (currentNode.Token != null)
            {
                //DisplaySpaced(currentNode.Token.Symbol.Text, level);
                Console.Write("\t" + currentNode.Token.Value.ToString());
            }
            Console.WriteLine();

            foreach (ParseTreeNode node in currentNode.ChildNodes)
            {
                DisplayTree(node, level+1);
            }
        }

        private void DisplaySpaced(string message, int tabs)
        {
            for (int i = 0; i < tabs; i++) Console.Write("\t");
            Console.Write(message);
        }
    }
}
