using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;
using EntitiesServices.Attributes;

namespace ERP_Condominios_Solution.ViewModels
{
    public class FichaTecnicaDetalheViewModel
    {
        [Key]
        public int FITD_CD_ID { get; set; }
        public int FITE_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo COMPONENTE obrigatorio")]
        public int PROD_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo QUANTIDADE obrigatorio")]
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> FITD_QN_QUANTIDADE { get; set; }
        public Nullable<int> FITD_IN_ATIVO { get; set; }

        public virtual FICHA_TECNICA FICHA_TECNICA { get; set; }
        public virtual PRODUTO_TABELA_PRECO PRODUTO_TABELA_PRECO { get; set; }
        public virtual PRODUTO PRODUTO { get; set; }
    }
}