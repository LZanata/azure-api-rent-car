using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace fnPaymanet.Model
{
    internal class PaymentModel
    {
        public string nome { get; set; }
        public string email { get; set; }
        public string modelo { get; set; }
        public int ano { get; set; }
        public string tempoAluguel { get; set; }
        public DateTime data { get; set; }
        public string id { get { return Guid.NewGuid().ToString(); } }
        public string idPayment { get { return Guid.NewGuide().ToString(); } }
        public string Status { get; set; }
        public DateTime DataAprovacao { get; set; }
    }
}
