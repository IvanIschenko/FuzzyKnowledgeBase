using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication1.FuzzyLogicBase
{
    public  class Rule
    {
        public Guid ID { get; set; }
        public Term Cоnsequens { get; set; }
        public List<Term> Antecedents { get; set; }
        public String Description { get; set; }
        public float Weight { get; set; }
        public int Level { get; set; }

        public const float constMaxWeight = 1;

        public double MinZnach { get; set; }

        public Rule(Guid ID, Term Cоnsequens, List<Term> Antecedents, float Weight = 0, String description = "")
        {
            this.ID = ID;
            this.Cоnsequens = Cоnsequens;
            this.Antecedents = Antecedents;
            this.Description = Description;
            this.Weight = Weight;
            this.Level = -1;
        }
        public Rule()
        {
            this.Level = -1;
        }
    }
}
