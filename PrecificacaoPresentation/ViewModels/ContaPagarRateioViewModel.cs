using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class ContaPagarRateioViewModel
    {
        [Key]
        public int CPRA_CD_ID { get; set; }
        public int CAPA_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo PLANO DE CONTA obrigatorio")]
        public int CECU_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo PERCENTUAL obrigatorio")]
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public int CRPA_NR_PERCENTUAL { get; set; }
        public int CRPA_IN_ATIVO { get; set; }

        public virtual PLANO_CONTA PLANO_CONTA { get; set; }
        public virtual CONTA_PAGAR CONTA_PAGAR { get; set; }
    }
}