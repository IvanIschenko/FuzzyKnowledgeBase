using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L.DataStructures.Matrix
{
    public class MultiDimensionalVector : List<double>
    {
        delegate double MathOperation(double number1, double number2);

        private static Dictionary<string, Func<double, double, double>> operations = new Dictionary<string, Func<double, double, double>>
    {
        { "+", (x, y) => x + y },
        { "-", (x, y) => x - y },
        { "*", (x, y) => x * y},
        { "/", (x, y) => x / y }
    };

        //выполнение операции над двумя векторми поэлементно, операция задается строкой и должна быть задана в operations для элементов типа double
        public static MultiDimensionalVector MathOperationMultiDimensionalVectorElementWise(MultiDimensionalVector mdVector1, MultiDimensionalVector mdVector2, String operation)
        {
            MultiDimensionalVector rezultMdVector = new MultiDimensionalVector();

            if (operations.ContainsKey(operation) && mdVector1.Count == mdVector2.Count && mdVector1.Count > 0)
            {
                int countElements = mdVector1.Count;
                // MathOperation mathOper = operations[operation];
                for (int i = 0; i < countElements; ++i)
                {
                    rezultMdVector.Add(operations[operation](mdVector1.ElementAt(i), mdVector2.ElementAt(i)));
                }
            }
            else { throw new ArgumentException("Невозможно выполнить операцию"); }
            return rezultMdVector;
        }

        //перегрузка оператора "+" - поэлементное сложение для векторов
        public static MultiDimensionalVector operator +(MultiDimensionalVector mdVector1, MultiDimensionalVector mdVector2)
        {
            return MathOperationMultiDimensionalVectorElementWise(mdVector1, mdVector2, "+");
        }

        //перегрузка оператора "-" - поэлементное вычитание для векторов
        public static MultiDimensionalVector operator -(MultiDimensionalVector mdVector1, MultiDimensionalVector mdVector2)
        {
            return MathOperationMultiDimensionalVectorElementWise(mdVector1, mdVector2, "-");
        }

        //умножение векторов
        public static double operator *(MultiDimensionalVector mdVector1, MultiDimensionalVector mdVector2)
        {
            double rezult = 0;

            if (mdVector1.Count == mdVector2.Count && mdVector1.Count > 0)
            {
                int countElements = mdVector1.Count;
                for (int i = 0; i < countElements; ++i)
                {
                    rezult += mdVector1.ElementAt(i) * mdVector2.ElementAt(i);
                }
            }
            else { throw new ArgumentException("Невозможно выполнить операцию"); }
            return rezult;
        }

        //деление векторов
        public static double operator /(MultiDimensionalVector mdVector1, MultiDimensionalVector mdVector2)
        {
            double rezult = 0;

            if (mdVector1.Count == mdVector2.Count && mdVector1.Count > 0)
            {
                int countElements = mdVector1.Count;
                for (int i = 0; i < countElements; ++i)
                {
                    rezult += mdVector1.ElementAt(i) / mdVector2.ElementAt(i);
                }
            }
            else { throw new ArgumentException("Невозможно выполнить операцию"); }
            return rezult;
        }

        //сравнение векторов поэлементно
        public static bool operator ==(MultiDimensionalVector mdVector1, MultiDimensionalVector mdVector2)
        {
            bool equivalent = true;

            if (mdVector1.Count == mdVector2.Count && mdVector1.Count > 0)
            {
                int countElements = mdVector1.Count;
                for (int i = 0; i < countElements; ++i)
                {
                    if (mdVector1.ElementAt(i) != mdVector2.ElementAt(i))
                    {
                        equivalent = false;
                        break;
                    }
                }
            }
            else { throw new ArgumentException("Невозможно выполнить операцию"); }
            return equivalent;
        }

        //сравнение векторов поэлементно
        public static bool operator !=(MultiDimensionalVector mdVector1, MultiDimensionalVector mdVector2)
        {
            if (mdVector1 == mdVector2) { return false; } else { return true; }
        }



        //выполнение операции над вектором и числом, операция задается строкой и должна быть задана в operations для элементов типа double
        public static MultiDimensionalVector MathOperationMultiDimensionalVectorWithNumber(MultiDimensionalVector mdVector, double number, String operation)
        {
            MultiDimensionalVector rezultMdVector = new MultiDimensionalVector();

            if (operations.ContainsKey(operation) && mdVector.Count >= 0)
            {
                int countElements = mdVector.Count;
                for (int i = 0; i < countElements; ++i)
                {
                    rezultMdVector.Add(operations[operation](mdVector.ElementAt(i), number));
                }
            }
            else { throw new ArgumentException("Невозможно выполнить операцию"); }
            return rezultMdVector;
        }

        //перегрузка оператора "+" - прибасвление к каждому элементу вектора числа number
        public static MultiDimensionalVector operator +(MultiDimensionalVector mdVector, double number)
        {
            return MathOperationMultiDimensionalVectorWithNumber(mdVector, number, "+");
        }

        //перегрузка оператора "-" - вычитание из каждого элемента вектора числа number
        public static MultiDimensionalVector operator -(MultiDimensionalVector mdVector, double number)
        {
            return MathOperationMultiDimensionalVectorWithNumber(mdVector, number, "-");
        }

        //перегрузка оператора "*" - умножение каждого элемента вектора на число number
        public static MultiDimensionalVector operator *(MultiDimensionalVector mdVector, double number)
        {
            return MathOperationMultiDimensionalVectorWithNumber(mdVector, number, "*");
        }

        //перегрузка оператора "/" - деление каждого элемента вектора на число number
        public static MultiDimensionalVector operator /(MultiDimensionalVector mdVector, double number)
        {
            return MathOperationMultiDimensionalVectorWithNumber(mdVector, number, "/");
        }


        public double EuclideanDistance(MultiDimensionalVector mdVector)
        {
            double distance = 0;
            if (this.Count == mdVector.Count && this.Count > 0)
            {
                int countElements = this.Count();

                for (int i = 0; i < countElements; ++i)
                {
                    distance += Math.Pow(this.ElementAt(i) - mdVector.ElementAt(i), 2);
                }

                distance = Math.Sqrt(distance);
            }
            else 
            { 
                throw new ArgumentException("Невозможно вычислить расстояние между векторами"); 
            }

            return distance;
        }

        public MultiDimensionalVector ToMultiDimensionalVector()
        {
            MultiDimensionalVector rezult =new MultiDimensionalVector();
            var newVector = this.ToList();
            foreach (var v in newVector)
            {
                rezult.Add(v);
            }

            return rezult;
        }
    }
}
