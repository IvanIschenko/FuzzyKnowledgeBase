using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
namespace WebApplication1.FuzzyLogicBase
{
    public class Term
    {
        public Guid ID { get; set; }
        public String Name { get; set; }
        public String NameLP { get; set; }
        public String ShortNameLP{ get; set; }
        public String ShortNameTerm { get; set; }
        public double a { get; set; }                //где вектор х – базовое множество, на котором определяется ФП.
        public double c { get; set; }                //Величины а и с задают основание треугольника, b – его вершину.
        public double b { get; set; }
        public double sigmGaus { get; set; } //параметры Гаусовской ФП
        public double aGaus { get; set; } // параметры Гаусовской ФП

        public double d { get; set; }                //Величины а и d задают нижнее основание трапеции, b и с – верхнее
                                                     //основание.

        public double ZnachFp { get; set; }

        public bool ProverkTruk { get; set; }

        public bool ProverkTrap { get; set; }

        public bool ProverGaus { get; set; }

        public bool ProverLast { get; set; }

        public bool ProverFirst { get; set; }
        public bool ProverOut { get; set; }

        public int WeightOfTerm { get; set; }
        public double ag { get; set; }
        public double sigm { get; set; }
        public static void Sort(FuzzyKnowledgeBase FKB)
        {
            for (int l = 0; l < FKB.ListVar.Count; l++)
            {
                for (int i = 1; i < FKB.ListVar[l].terms.Count; i++)
                {
                    Term cur = FKB.ListVar[l].terms[i];
                    int j = i;
                    while (j > 0 && cur.WeightOfTerm < FKB.ListVar[l].terms[j - 1].WeightOfTerm)
                    {
                        FKB.ListVar[l].terms[j] = FKB.ListVar[l].terms[j - 1];
                        j--;
                    }
                    FKB.ListVar[l].terms[j] = cur;
                }
            }
            for (int l = 0; l < FKB.ListVar.Count; l++)
            {
                FKB.ListVar[l].terms[0].ProverFirst = true;
                FKB.ListVar[l].terms[FKB.ListVar[l].terms.Count - 1].ProverLast = true;
                for (int i = 1; i < FKB.ListVar[l].terms.Count - 1; i++)
                {
                    FKB.ListVar[l].terms[i].ProverFirst = false;
                    FKB.ListVar[l].terms[i].ProverLast = false;
                }
            }
        }
        public Term(Guid ID, String Name, String NameLinLinguisticVariable)
        {
            this.NameLP = NameLinLinguisticVariable;
            this.ID = ID;
            this.Name = Name;
            this.a = 0;
            this.ProverLast = false;
            this.ProverFirst = false;
            this.ShortNameTerm = "";
        }
        public Term()
        {
            this.ProverOut = false;
        }
        public void zafatFP(double x1, double x2, double x3)
        {
            this.ProverkTruk = true;
            this.ProverGaus = false;
            this.ProverkTrap = false;
            this.a = x1;
            this.b = x2;
            this.c = x3;
        }
        public void zafatFP(double x1, double x2, double x3, double x4)
        {
            this.ProverkTrap = true;
            this.ProverGaus = false;
            this.ProverkTruk = false;
            this.a = x1;
            this.b = x2;
            this.c = x3;
            this.d = x4;
        }
        public void zafatFP(double ag, double sigm)
        {
            this.ProverkTruk = false;
            this.ProverkTrap = false;
            this.ProverGaus = true;
            this.ag = ag;
            this.sigm = sigm;
        }
        public double Fp(double vhid)
        {
            if (ProverLast == false && ProverFirst == false)
            {
                if (vhid < a)
                {
                    return 0;
                }
                if (vhid >= a && vhid <= b)
                {
                    return (vhid - a) / (b - a);
                }

                if (vhid >= b && vhid <= c)
                {
                    return (c - vhid) / (c - b);
                }
                if (vhid > c)
                {
                    return 0;
                }
            }else if (ProverLast == true && ProverFirst == false)
            {
                if (vhid < a)
                {
                    return 0;
                }
                if (vhid >= a && vhid <= b)
                {
                    return (vhid - a) / (b - a);
                }

                if (vhid >= b && vhid <= c)
                {
                    return 1;
                }
                if (vhid > c)
                {
                    return 1;
                }
            }else if (ProverLast == false && ProverFirst == true)
            {
                if (vhid < a)
                {
                    return 1;
                }
                if (vhid >= a && vhid <= b)
                {
                    return 1;
                }

                if (vhid >= b && vhid <= c)
                {
                    return (c - vhid) / (c - b);
                }
                if (vhid > c)
                {
                    return 0;
                }
            }
            //else if (ProverkTrap)
            //{
            //    if (vhid < a)
            //    {
            //        return 0;
            //    }
            //    if (vhid >= a && vhid <= b)
            //    {
            //        return (vhid - a) / (vhid - a);
            //    }
            //    if (vhid > b && vhid <= c)
            //    {
            //        return 1;
            //    }
            //    if (vhid >= c && vhid <= d)
            //    {
            //        return (d - vhid) / (d - c);
            //    }
            //    if (vhid > d)
            //    {
            //        return 0;
            //    }
            //}
            else if (ProverGaus)
            {
                return 0;
            }
            return 0;
        }
    }
}
