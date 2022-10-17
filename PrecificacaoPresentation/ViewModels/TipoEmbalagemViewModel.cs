using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class TipoEmbalagemViewModel
    {
        [Key]
        public int TIEM_CD_ID { get; set; }
        public Nullable<int> ASSI_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo NOME obrigatorio")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "O NOME deve conter no minimo 1 e no máximo 200 caracteres.")]
        public string TIEM_NM_NOME { get; set; }
        [StringLength(500, ErrorMessage = "A DESCRIÇÃO deve conter no máximo 500 caracteres.")]
        public string TIEM_DS_DESCRICAO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> TIEM_NR_ALTURA { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> TIEM_NR_LARGURA { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> TIEM_NR_COMPRIMENTO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> TIEM_NR_DIAMETRO { get; set; }
        [Required(ErrorMessage = "Campo CUSTO UNITÁRIO obrigatorio")]
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> TIIEM_VL_CUSTO_UNITARIO { get; set; }
        [Required(ErrorMessage = "Campo ESTOQUE obrigatorio")]
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> TIEM_NR_ESTOQUE { get; set; }
        [Required(ErrorMessage = "Campo CUSTO REPASSADO obrigatorio")]
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> TIEM_VLCUSTO_REPASSADO { get; set; }
        public Nullable<int> TIEM_IN_ATIVO { get; set; }
        [Required(ErrorMessage = "Campo ESTOQUE INICIAL obrigatorio")]
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> TIEM_ESTOQUE_INICIAL { get; set; }

        public string CustoUnitario
        {
            get
            {
                return CrossCutting.Formatters.DecimalFormatter(TIIEM_VL_CUSTO_UNITARIO.Value);
            }
            set
            {
                TIIEM_VL_CUSTO_UNITARIO = Convert.ToDecimal(CrossCutting.CommonHelpers.GetOnlyDigits(value, true));
            }
        }
        public string CustoRepasse
        {
            get
            {
                return CrossCutting.Formatters.DecimalFormatter(TIEM_VLCUSTO_REPASSADO.Value);
            }
            set
            {
                TIEM_VLCUSTO_REPASSADO = Convert.ToDecimal(CrossCutting.CommonHelpers.GetOnlyDigits(value, true));
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PRODUTO> PRODUTO { get; set; }
    }
}