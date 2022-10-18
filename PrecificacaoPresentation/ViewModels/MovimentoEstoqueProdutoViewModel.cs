using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;
using EntitiesServices.Attributes;

namespace ERP_Condominios_Solution.ViewModels
{
    public class MovimentoEstoqueProdutoViewModel
    {
        public int MOEP_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public Nullable<int> EMPR_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo PRODUTO obrigatorio")]
        public int PROD_CD_ID { get; set; }
        public int USUA_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo DATA obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public System.DateTime MOEP_DT_MOVIMENTO { get; set; }
        [Required(ErrorMessage = "Campo TIPO DE MOVIMENTAÇÃO obrigatorio")]
        public int MOEP_IN_TIPO_MOVIMENTO { get; set; }
        [Required(ErrorMessage = "Campo QUANTIDADE obrigatorio")]
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public int MOEP_QN_QUANTIDADE { get; set; }
        public string MOEP_IN_ORIGEM { get; set; }
        public int MOEP_IN_CHAVE_ORIGEM { get; set; }
        public int MOEP_IN_ATIVO { get; set; }
        [Required(ErrorMessage = "Campo JUSTIFICATIVA obrigatorio")]
        [StringLength(500, ErrorMessage = "A JUSTIFICATIVA deve conter no máximo 500 caracteres.")]
        public string MOEP_DS_JUSTIFICATIVA { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> MOEP_QN_ANTES { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> MOEP_QN_DEPOIS { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> MOEP_QN_ALTERADA { get; set; }
        public Nullable<int> MOEP_IN_OPERACAO { get; set; }
        public Nullable<int> MOEP_IN_OPERACO_SAIDA { get; set; }

        public virtual ASSINANTE ASSINANTE { get; set; }
        public virtual EMPRESA EMPRESA { get; set; }
        public virtual PRODUTO PRODUTO { get; set; }
        public virtual USUARIO USUARIO { get; set; }
    }
}