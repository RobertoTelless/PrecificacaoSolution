using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;
using EntitiesServices.Attributes;

namespace ERP_Condominios_Solution.ViewModels
{
    public class CustoFixoViewModel
    {
        [Key]
        public int CUFX_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo EMPRESA obrigatorio")]
        public int EMPR_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo CATEGORIA obrigatorio")]
        public int CACF_CD_ID { get; set; }
        public Nullable<int> CECU_CD_ID { get; set; }
        public Nullable<int> FORN_CD_ID { get; set; }
        [StringLength(250, MinimumLength = 1, ErrorMessage = "O NOME deve conter no minimo 1 e no máximo 250 caracteres.")]
        public string CUFX_NM_NOME { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> CUFX_DT_INICIO { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> CUFX_DT_TERMINO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "O NÚMERO DE OCORRÊNCIAS deve ser um valor numérico positivo")]
        public Nullable<int> CUFX_IN_NUMERO_OCORRENCIAS { get; set; }
        [Required(ErrorMessage = "Campo PERIODICIDADE obrigatorio")]
        public int PETA_CD_ID { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "O DIA DO VENCIMENTO deve ser um valor numérico positivo")]
        public Nullable<int> CUFX_NR_DIA_VENCIMENTO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "O VALOR DO CUSTO deve ser um valor numérico positivo")]
        public Nullable<decimal> CUFX_VL_VALOR { get; set; }
        public Nullable<System.DateTime> CUFX_DT_CADASTRO { get; set; }
        public Nullable<int> CUFX_IN_ATIVO { get; set; }
        public Nullable<int> ASSI_CD_ID { get; set; }

        public virtual CATEGORIA_CUSTO_FIXO CATEGORIA_CUSTO_FIXO { get; set; }
        public virtual EMPRESA EMPRESA { get; set; }
        public virtual PERIODICIDADE_TAREFA PERIODICIDADE_TAREFA { get; set; }
        public virtual ASSINANTE ASSINANTE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONTA_PAGAR> CONTA_PAGAR { get; set; }
        public virtual FORNECEDOR FORNECEDOR { get; set; }
        public virtual PLANO_CONTA PLANO_CONTA { get; set; }
    }
}