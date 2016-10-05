using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication1.FuzzyLogicBase
{
    public class LinguisticVariable
    {
        public Guid ID { get; set; }
        public String Name { get; set; }
        public String Description { get; set; }
        public List<Term> terms;
        public double min = 0;
        public double max = 0;
        public string ShortName = "";
        public LinguisticVariable()
        {
        }
        public LinguisticVariable(Guid IDLing, String NameLing, List<Term> termLing, double min, double max, String DesreptionLing = " ", String Short = "")
        {
            this.ID = IDLing;
            this.Name = NameLing;
            this.Description = DesreptionLing;
            this.terms = termLing;
            this.ShortName = Short;
        }
    }
}
