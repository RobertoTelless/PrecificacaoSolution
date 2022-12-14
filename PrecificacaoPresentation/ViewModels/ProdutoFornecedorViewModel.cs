using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;
using EntitiesServices.Attributes;

namespace ERP_Condominios_Solution.ViewModels
{
    public class ProdutoFornecedorViewModel
    {
        [Key]
        public int PRFO_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo PRODUTO obrigatorio")]
        public int PROD_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo FORNECEDOR obrigatorio")]
        public int FORN_CD_ID { get; set; }
        public int PRFO_IN_ATIVO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor num?rico positivo")]
        public Nullable<int> PRFO_NR_DIAS_ENTREGA { get; set; }

        public virtual FORNECEDOR FORNECEDOR { get; set; }
        public virtual PRODUTO PRODUTO { get; set; }

    }
}