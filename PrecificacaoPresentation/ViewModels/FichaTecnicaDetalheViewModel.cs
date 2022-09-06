using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;
using EntitiesServices.Attributes;

namespace ERP_CRM_Solution.ViewModels
{
    public class FichaTecnicaDetalheViewModel
    {
        [Key]
        public int FITD_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo FICHA TÉCNICA obrigatorio")]
        public int FITE_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo COMPONENTE obrigatorio")]
        public int PROD_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo QUANTIDADE obrigatorio")]
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public decimal FITD_QN_QUANTIDADE { get; set; }
        [StringLength(50, MinimumLength = 1, ErrorMessage = "O NOME deve conter no minimo 1 e no máximo 50 caracteres.")]
        public string FITD_NM_NOME { get; set; }
        [StringLength(100, ErrorMessage = "A DESCRIÇÃO deve conter no máximo 100 caracteres.")]
        public string FITD_DS_DESCRICAO { get; set; }
        public System.DateTime FITD_DT_CADASTRO { get; set; }
        public int FITD_IN_ATIVO { get; set; }

        public virtual FICHA_TECNICA FICHA_TECNICA { get; set; }
        public virtual PRODUTO PRODUTO { get; set; }
    }
}