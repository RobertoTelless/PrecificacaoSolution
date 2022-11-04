using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class ContaReceberViewModel
    {
        [Key]
        public int CARE_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public Nullable<int> FILI_CD_ID { get; set; }
        public Nullable<int> USUA_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo CLIENTE obrigatorio")]
        public Nullable<int> CLIE_CD_ID { get; set; }
        public Nullable<int> PEVE_CD_ID { get; set; }
        public Nullable<int> COBA_CD_ID { get; set; }

        [Required(ErrorMessage = "Campo DATA DE LANÇAMENTO obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public System.DateTime CARE_DT_LANCAMENTO { get; set; }
        [Required(ErrorMessage = "Campo VALOR obrigatorio")]
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> CARE_VL_VALOR { get; set; }
        [Required(ErrorMessage = "Campo DESCRIÇÃO obrigatorio")]
        [StringLength(250, MinimumLength = 1, ErrorMessage = "A DESCRIÇÃO deve conter no minimo 1 caracteres e no máximo 250.")]
        public string CARE_DS_DESCRICAO { get; set; }
        public int CARE_IN_TIPO_LANCAMENTO { get; set; }
        public string CARE_NM_FAVORECIDO { get; set; }
        [StringLength(50, ErrorMessage = "A FORMA DE PAGAMENTO deve conter no máximo 50.")]
        public string CARE_NM_FORMA_PAGAMENTO { get; set; }
        public int CARE_IN_LIQUIDADA { get; set; }
        public int CARE_IN_ATIVO { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> CARE_DT_DATA_LIQUIDACAO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> CARE_VL_VALOR_LIQUIDADO { get; set; }
        [Required(ErrorMessage = "Campo DATA DE VENCIMENTO obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> CARE_DT_VENCIMENTO { get; set; }
        public Nullable<int> CARE_NR_ATRASO { get; set; }
        [Required(ErrorMessage = "Campo FORMA DE PAGAMENTO obrigatorio")]
        public Nullable<int> FOPR_CD_ID { get; set; }
        public string CARE_TX_OBSERVACOES { get; set; }
        public Nullable<int> CARE_IN_PARCELADA { get; set; }
        public Nullable<int> CARE_IN_PARCELAS { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> CARE_DT_INICIO_PARCELA { get; set; }
        public Nullable<int> PETA_CD_ID { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> CARE_VL_PARCELADO { get; set; }
        [Required(ErrorMessage = "Campo NÚMERO DO DOCUMENTO obrigatorio")]
        [StringLength(50, ErrorMessage = "O NÙMERO DO DOCUMENTO deve conter no máximo 50.")]
        public string CARE_NR_DOCUMENTO { get; set; }
        [Required(ErrorMessage = "Campo DATA DE COMPETÊNCIA obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> CARE_DT_COMPETENCIA { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> CARE_VL_DESCONTO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> CARE_VL_JUROS { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> CARE_VL_TAXAS { get; set; }
        [Required(ErrorMessage = "Campo CENTRO DE CUSTO obrigatorio")]
        public Nullable<int> CECU_CD_ID { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> CARE_VL_SALDO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> CARE_IN_PAGA_PARCIAL { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> CARE_VL_PARCIAL { get; set; }
        public Nullable<int> TITA_CD_ID { get; set; }
        public int CARE_IN_RECORRENTE { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> CARE_DT_INICIO_RECORRENCIA { get; set; }
        public Nullable<int> CECU_CD_RATEIO { get; set; }
        public Nullable<int> CARE_VL_PERCENTUAL { get; set; }
        public Nullable<int> CARE_IN_PAGAMENTO { get; set; }
        public Nullable<decimal> CARE_VL_VALOR_RECEBIDO { get; set; }
        public Nullable<int> CRPV_CD_ID { get; set; }

        public bool Liquidada
        {
            get
            {
                if (CARE_IN_LIQUIDADA == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                CARE_IN_LIQUIDADA = (value == true) ? 1 : 0;
            }
        }
        public bool Parcial
        {
            get
            {
                if (CARE_IN_PAGA_PARCIAL == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                CARE_IN_PAGA_PARCIAL = (value == true) ? 1 : 0;
            }
        }
        public bool Parcelado
        {
            get
            {
                if (CARE_IN_PARCELADA == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                CARE_IN_PARCELADA = (value == true) ? 1 : 0;
            }
        }

        public string ValorRecebido
        {
            get
            {
                return CARE_VL_VALOR_RECEBIDO.HasValue ? CrossCutting.Formatters.DecimalFormatter(CARE_VL_VALOR_RECEBIDO.Value) : string.Empty;
            }
            set
            {
                CARE_VL_VALOR_RECEBIDO = Convert.ToDecimal(CrossCutting.CommonHelpers.GetOnlyDigits(value, true));
            }
        }
        public string ValorLiquidado
        {
            get
            {
                return CARE_VL_VALOR_LIQUIDADO.HasValue ? CrossCutting.Formatters.DecimalFormatter(CARE_VL_VALOR_LIQUIDADO.Value) : string.Empty;
            }
            set
            {
                CARE_VL_VALOR_LIQUIDADO = Convert.ToDecimal(CrossCutting.CommonHelpers.GetOnlyDigits(value, true));
            }
        }
        public string ValorDesconto
        {
            get
            {
                return CARE_VL_DESCONTO.HasValue ? CrossCutting.Formatters.DecimalFormatter(CARE_VL_DESCONTO.Value) : string.Empty;
            }
            set
            {
                CARE_VL_DESCONTO = Convert.ToDecimal(CrossCutting.CommonHelpers.GetOnlyDigits(value, true));
            }
        }
        public string Juros
        {
            get
            {
                return CARE_VL_JUROS.HasValue ? CrossCutting.Formatters.DecimalFormatter(CARE_VL_JUROS.Value) : string.Empty;
            }
            set
            {
                CARE_VL_JUROS = Convert.ToDecimal(CrossCutting.CommonHelpers.GetOnlyDigits(value, true));
            }
        }
        public string Taxas
        {
            get
            {
                return CARE_VL_TAXAS.HasValue ? CrossCutting.Formatters.DecimalFormatter(CARE_VL_TAXAS.Value) : string.Empty;
            }
            set
            {
                CARE_VL_TAXAS = Convert.ToDecimal(CrossCutting.CommonHelpers.GetOnlyDigits(value, true));
            }
        }
        public string Valor
        {
            get
            {
                return CARE_VL_VALOR.HasValue ? CrossCutting.Formatters.DecimalFormatter(CARE_VL_VALOR.Value) : string.Empty;
            }
            set
            {
                CARE_VL_VALOR = Convert.ToDecimal(CrossCutting.CommonHelpers.GetOnlyDigits(value, true));
            }
        }

        public virtual ASSINANTE ASSINANTE { get; set; }
        public virtual PLANO_CONTA PLANO_CONTA { get; set; }
        public virtual CLIENTE CLIENTE { get; set; }
        public virtual CONTA_BANCO CONTA_BANCO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONTA_RECEBER_ANEXO> CONTA_RECEBER_ANEXO { get; set; }
        public virtual CRM CRM { get; set; }
        //public virtual FILIAL FILIAL { get; set; }
        public virtual FORMA_PAGTO_RECTO FORMA_PAGTO_RECTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONTA_RECEBER_PARCELA> CONTA_RECEBER_PARCELA { get; set; }
        public virtual PERIODICIDADE_TAREFA PERIODICIDADE_TAREFA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONTA_RECEBER_RATEIO> CONTA_RECEBER_RATEIO { get; set; }
        public virtual USUARIO USUARIO { get; set; }
        public virtual CRM_PEDIDO_VENDA CRM_PEDIDO_VENDA { get; set; }
    }
}