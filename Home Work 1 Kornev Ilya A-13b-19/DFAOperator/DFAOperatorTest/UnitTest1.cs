using Microsoft.VisualStudio.TestTools.UnitTesting;
using DFAOperator;
using System;

namespace DFAOperatorTest
{
    [TestClass]
    public class UnitTest1
    {
        public Automata auto1 = new Automata("input1.txt");
        public Automata auto2 = new Automata("input2.txt");
        public Automata auto3 = new Automata("input3.txt");

        [TestMethod]
        public void ProductTest()
        {
            string actual = auto1.Product(auto2).ToString();
            string expected = "Σ = { a b }  \n" +
                "Q = { A-D A-E A-F B-D B-E B-F C-D C-E C-F }  \n" +
                "T = { C-F }  \n" +
                "s = A-D  \n" +
                "Transition table:  \n" +
                "δ(A-D,a) = B-D  \nδ(A-D,b) = A-E  \nδ(A-E,a) = B-E  \nδ(A-E,b) = A-F  \n" +
                "δ(A-F,a) = B-F  \nδ(A-F,b) = A-F  \nδ(B-D,a) = C-D  \nδ(B-D,b) = B-E  \n" +
                "δ(B-E,a) = C-E  \nδ(B-E,b) = B-F  \nδ(B-F,a) = C-F  \nδ(B-F,b) = B-F  \n" +
                "δ(C-D,a) = C-D  \nδ(C-D,b) = C-E  \nδ(C-E,a) = C-E  \nδ(C-E,b) = C-F  \n" +
                "δ(C-F,a) = C-F  \nδ(C-F,b) = C-F";

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UnionTest()
        {
            string actual = auto1.Union(auto2).ToString();
            string expected = "Σ = { a b }  \n" +
                "Q = { A-D A-E A-F B-D B-E B-F C-D C-E C-F }  \n" +
                "T = { C-D C-E C-F A-F B-F }  \n" +
                "s = A-D  \n" +
                "Transition table:  \n" +
                "δ(A-D,a) = B-D  \nδ(A-D,b) = A-E  \nδ(A-E,a) = B-E  \nδ(A-E,b) = A-F  \n" +
                "δ(A-F,a) = B-F  \nδ(A-F,b) = A-F  \nδ(B-D,a) = C-D  \nδ(B-D,b) = B-E  \n" +
                "δ(B-E,a) = C-E  \nδ(B-E,b) = B-F  \nδ(B-F,a) = C-F  \nδ(B-F,b) = B-F  \n" +
                "δ(C-D,a) = C-D  \nδ(C-D,b) = C-E  \nδ(C-E,a) = C-E  \nδ(C-E,b) = C-F  \n" +
                "δ(C-F,a) = C-F  \nδ(C-F,b) = C-F";

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void DifferenceTest()
        {
            string actual = auto1.Difference(auto2).ToString();
            string expected = "Σ = { a b }  \n" +
                "Q = { A-D A-E A-F B-D B-E B-F C-D C-E C-F }  \n" +
                "T = { C-D C-E }  \n" +
                "s = A-D  \n" +
                "Transition table:  \n" +
                "δ(A-D,a) = B-D  \nδ(A-D,b) = A-E  \nδ(A-E,a) = B-E  \nδ(A-E,b) = A-F  \n" +
                "δ(A-F,a) = B-F  \nδ(A-F,b) = A-F  \nδ(B-D,a) = C-D  \nδ(B-D,b) = B-E  \n" +
                "δ(B-E,a) = C-E  \nδ(B-E,b) = B-F  \nδ(B-F,a) = C-F  \nδ(B-F,b) = B-F  \n" +
                "δ(C-D,a) = C-D  \nδ(C-D,b) = C-E  \nδ(C-E,a) = C-E  \nδ(C-E,b) = C-F  \n" +
                "δ(C-F,a) = C-F  \nδ(C-F,b) = C-F";

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ComplimentTest()
        {
            string actual1 = auto1.Complement().ToString();
            string actual2 = auto2.Complement().ToString();
            string expected1 = "Σ = { a b }  \n" +
                "Q = { A B C }  \n" +
                "T = { A B }  \n" +
                "s = A  \n" +
                "Transition table:  \n" +
                "δ(A,a) = B  \nδ(A,b) = A  \nδ(B,a) = C  \n" +
                "δ(B,b) = B  \nδ(C,a) = C  \nδ(C,b) = C";
            string expected2 = "Σ = { a b }  \n" +
                "Q = { D E F }  \n" +
                "T = { D E }  \n" +
                "s = D  \n" +
                "Transition table:  \n" +
                "δ(D,a) = D  \nδ(D,b) = E  \nδ(E,a) = E  \n" +
                "δ(E,b) = F  \nδ(F,a) = F  \nδ(F,b) = F";

            Assert.AreEqual(expected1, actual1);
            Assert.AreEqual(expected2, actual2);
        }

        [TestMethod]
        public void NFAToDFATest()
        {
            
            string actual = auto3.TompsonAlgorithm(out string log).ToString();
            string expected = "Σ = { a b }  \n" +
                "Q = { {1} {10,3,6} {4,7} {10,3,6,8} }  \n" +
                "T = { {10,3,6} {10,3,6,8} }  \n" +
                "s = {1}  \n" +
                "Transition table:  \n" +
                "δ({1},a) = {10,3,6}  \nδ({10,3,6},b) = {4,7}  \nδ({4,7},a) = {10,3,6,8}  \n" +
                "δ({10,3,6,8},a) = {10,3,6}  \nδ({10,3,6,8},b) = {4,7}";

            Assert.AreEqual(expected, actual);
        }
    }
}
