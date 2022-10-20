using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class ContaReceberParcelaViewModel
    {
        [Key]
        public int CRPA_CD_ID { get; set; }
        public int CARE_CD_ID { get; set; }
        public Nullable<int> CRPA_IN_PARCELA { get; set; }
        public Nullable<decimal> CRPA_VL_VALOR { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> CRPA_DT_VENCIMENTO { get; set; }
        public string CRPA_DS_DESCRICAO { get; set; }
        public Nullable<int> CRPA_IN_ATIVO { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> CRPA_DT_QUITACAO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> CRPA_VL_RECEBIDO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> CRPA_VL_DESCONTO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> CRPA_VL_JUROS { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> CRPA_VL_TAXAS { get; set; }
        public Nullable<int> CRPA_IN_QUITADA { get; set; }
        public string CRPA_NR_PARCELA { get; set; }

        public string ValorDesconto
        {
            get
            {
                return CRPA_VL_DESCONTO.HasValue ? CrossCutting.Formatters.DecimalFormatter(CRPA_VL_DESCONTO.Value) : string.Empty;
            }
            set
            {
                CRPA_VL_DESCONTO = Convert.ToDecimal(CrossCutting.CommonHelpers.GetOnlyDigits(value, true));
            }
        }
        public string Juros
        {
            get
            {
                return CRPA_VL_JUROS.HasValue ? CrossCutting.Formatters.DecimalFormatter(CRPA_VL_JUROS.Value) : string.Empty;
            }
            set
            {
                CRPA_VL_JUROS = Convert.ToDecimal(CrossCutting.CommonHelpers.GetOnlyDigits(value, true));
            }
        }
        public string Taxas
        {
            get
            {
                return CRPA_VL_TAXAS.HasValue ? CrossCutting.Formatters.DecimalFormatter(CRPA_VL_TAXAS.Value) : string.Empty;
            }
            set
            {
                CRPA_VL_TAXAS = Convert.ToDecimal(CrossCutting.CommonHelpers.GetOnlyDigits(value, true));
            }
        }
        public string Valor
        {
            get
            {
                return CRPA_VL_VALOR.HasValue ? CrossCutting.Formatters.DecimalFormatter(CRPA_VL_VALOR.Value) : string.Empty;
            }
            set
            {
                CRPA_VL_VALOR = Convert.ToDecimal(CrossCutting.CommonHelpers.GetOnlyDigits(value, true));
            }
        }

        public virtual CONTA_RECEBER CONTA_RECEBER { get; set; }

    }
}