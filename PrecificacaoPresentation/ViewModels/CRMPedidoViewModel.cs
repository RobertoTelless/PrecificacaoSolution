using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class CRMPedidoViewModel
    {
        [Key]
        public Nullable<int> CRPV_CD_ID { get; set; }
        public Nullable<int> ASSI_CD_ID { get; set; }
        public Nullable<int> CRM1_CD_ID { get; set; }
        public Nullable<int> CLIE_CD_ID { get; set; }
        public Nullable<int> FOEN_CD_ID { get; set; }
        public Nullable<int> FOFR_CD_ID { get; set; }
        public Nullable<int> USUA_CD_ID { get; set; }
        public Nullable<int> PLEN_CD_ID { get; set; }
        public Nullable<int> TEPR_CD_ID { get; set; }
        public Nullable<int> MOCA_CD_ID { get; set; }
        public Nullable<int> MOEN_CD_ID { get; set; }
        public Nullable<int> TRAN_CD_ID { get; set; }
        public Nullable<int> FILI_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo DATA DE VALIDADE obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "A DATA DE VALIDADE deve ser uma data válida")]
        public System.DateTime CRPV_DT_VALIDADE { get; set; }
        [Required(ErrorMessage = "Campo DATA DE CRIAÇÃO obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "A DATA DE CRIAÇÂO deve ser uma data válida")]
        public System.DateTime CRPV_DT_PEDIDO { get; set; }
        public string CRPV_TX_INTRODUCAO { get; set; }
        [StringLength(20, ErrorMessage = "O NÚMERO deve conter no máximo 20 caracteres.")]
        public string CRPV_NR_NUMERO { get; set; }
        [Required(ErrorMessage = "Campo STATUS obrigatorio")]
        public int CRPV_IN_STATUS { get; set; }
        public string CRPV_TX_INFORMACOES_GERAIS { get; set; }
        public string CRPV_TX_OUTROS_ITENS { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> CRPV_VL_TOTAL_SERVICOS { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> CRPV_VL_DESCONTO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> CRPV_VL_FRETE { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> CRPV_VL_PESO_BRUTO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> CRPV_VL_PESO_LIQUIDO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> CRPV_VL_TOTAL_ITENS { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> CRPV_VL_IPI { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> CRPV_VL_ICMS { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> CRPV_TOTAL_PEDIDO { get; set; }
        [StringLength(5000, ErrorMessage = "AS CONDIÇÕES COMERCIAIS deve conter no máximo 5000 caracteres.")]
        public string CRPV_TX_CONDICOES_COMERCIAIS { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> CRPV_IN_PRAZO_ENTREGA { get; set; }
        [StringLength(5000, ErrorMessage = "AS OBSERVAÇÕES deve conter no máximo 5000 caracteres.")]
        public string CRPV_TX_OBSERVACAO { get; set; }
        public int CRPV_IN_ATIVO { get; set; }
        [DataType(DataType.Date, ErrorMessage = "A DATA DE ENVIO deve ser uma data válida")]
        public Nullable<System.DateTime> CRPV_DT_ENVIO { get; set; }
        [StringLength(1000, ErrorMessage = "AS INFORMAÇÕES DE ENVIO deve conter no máximo 1000 caracteres.")]
        public string CRPV_DS_ENVIO { get; set; }
        public string CRPV_LK_LINK { get; set; }
        [DataType(DataType.Date, ErrorMessage = "A DATA DE CANCELAMENTO deve ser uma data válida")]
        public Nullable<System.DateTime> CRPV_DT_CANCELAMENTO { get; set; }
        [StringLength(1000, ErrorMessage = "AS INFORMAÇÕES DE CANCELAMENTO deve conter no máximo 1000 caracteres.")]
        public string CRPV_DS_CANCELAMENTO { get; set; }
        [DataType(DataType.Date, ErrorMessage = "A DATA DE REPROVAÇÃO deve ser uma data válida")]
        public Nullable<System.DateTime> CRPV_DT_REPROVACAO { get; set; }
        [StringLength(1000, ErrorMessage = "AS INFORMAÇÕES DE REPROVAÇÃO deve conter no máximo 1000 caracteres.")]
        public string CRPV_DS_REPROVACAO { get; set; }
        [DataType(DataType.Date, ErrorMessage = "A DATA DE APROVAÇÃO deve ser uma data válida")]
        public Nullable<System.DateTime> CRPV_DT_APROVACAO { get; set; }
        [StringLength(1000, ErrorMessage = "AS INFORMAÇÕES DE APROVAÇÃO deve conter no máximo 1000 caracteres.")]
        public string CRPV_DS_APROVACAO { get; set; }
        public CLIENTE CLIENTE_NOME { get; set; }
        public string CRPV_NM_NOME { get; set; }
        public Nullable<int> CRPV_IN_GERAR_CR { get; set; }
        public Nullable<int> CRPV_IN_GEROU_NF { get; set; }
        public string CRPV_NR_NOTA_FISCAL { get; set; }
        public Nullable<decimal> CRPV_VL_VALOR { get; set; }
        public Nullable<int> CRPV_IN_NUMERO_GERADO { get; set; }
        [DataType(DataType.Date, ErrorMessage = "A DATA DE VALIDADE deve ser uma data válida")]
        public Nullable<System.DateTime> CRPV_DT_FATURAMENTO { get; set; }
        [DataType(DataType.Date, ErrorMessage = "A DATA DE VALIDADE deve ser uma data válida")]
        public Nullable<System.DateTime> CRPV_DT_EXPEDICAO { get; set; }
        [DataType(DataType.Date, ErrorMessage = "A DATA DE VALIDADE deve ser uma data válida")]
        public Nullable<System.DateTime> CRPV_DT_ENTREGA { get; set; }
        [StringLength(500, ErrorMessage = "AS INFORMAÇÕES DE FATURAMENTO deve conter no máximo 500 caracteres.")]
        public string CRPV_DS_FATURAMENTO { get; set; }
        [StringLength(500, ErrorMessage = "AS INFORMAÇÕES DE EXPEDIÇÃO deve conter no máximo 500 caracteres.")]
        public string CRPV_DS_EXPEDICAO { get; set; }
        [StringLength(500, ErrorMessage = "AS INFORMAÇÕES DE ENTREGA deve conter no máximo 500 caracteres.")]
        public string CRPV_DS_ENTREGA { get; set; }
        public Nullable<int> CRPC_IN_EMAIL { get; set; }

        public string NumeroProposta { get; set; }
        public string NomeProposta { get; set; }
        public Nullable<System.DateTime> DataProposta { get; set; }
        public Nullable<System.DateTime> DataAprovacao { get; set; }
        public Nullable<int> CRPV_IN_GERAR_PEDIDO { get; set; }

        public bool GeraVenda
        {
            get
            {
                if (CRPV_IN_GERAR_PEDIDO == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                CRPV_IN_GERAR_PEDIDO = (value == true) ? 1 : 0;
            }
        }
        public bool GeraCR
        {
            get
            {
                if (CRPV_IN_GERAR_CR == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                CRPV_IN_GERAR_CR = (value == true) ? 1 : 0;
            }
        }
        public string Valor
        {
            get
            {
                return CRPV_VL_VALOR.HasValue ? CrossCutting.Formatters.DecimalFormatter(CRPV_VL_VALOR.Value) : string.Empty;
            }
            set
            {
                CRPV_VL_VALOR = Convert.ToDecimal(CrossCutting.CommonHelpers.GetOnlyDigits(value, true));
            }
        }
        public string Desconto
        {
            get
            {
                return CRPV_VL_DESCONTO.HasValue ? CrossCutting.Formatters.DecimalFormatter(CRPV_VL_DESCONTO.Value) : string.Empty;
            }
            set
            {
                CRPV_VL_DESCONTO = Convert.ToDecimal(CrossCutting.CommonHelpers.GetOnlyDigits(value, true));
            }
        }
        public string Frete
        {
            get
            {
                return CRPV_VL_FRETE.HasValue ? CrossCutting.Formatters.DecimalFormatter(CRPV_VL_FRETE.Value) : string.Empty;
            }
            set
            {
                CRPV_VL_FRETE = Convert.ToDecimal(CrossCutting.CommonHelpers.GetOnlyDigits(value, true));
            }
        }
        public string TotalItens
        {
            get
            {
                return CRPV_VL_TOTAL_ITENS.HasValue ? CrossCutting.Formatters.DecimalFormatter(CRPV_VL_TOTAL_ITENS.Value) : string.Empty;
            }
            set
            {
                CRPV_VL_TOTAL_ITENS = Convert.ToDecimal(CrossCutting.CommonHelpers.GetOnlyDigits(value, true));
            }
        }
        public string IPI
        {
            get
            {
                return CRPV_VL_IPI.HasValue ? CrossCutting.Formatters.DecimalFormatter(CRPV_VL_IPI.Value) : string.Empty;
            }
            set
            {
                CRPV_VL_IPI = Convert.ToDecimal(CrossCutting.CommonHelpers.GetOnlyDigits(value, true));
            }
        }
        public string ICMS
        {
            get
            {
                return CRPV_VL_ICMS.HasValue ? CrossCutting.Formatters.DecimalFormatter(CRPV_VL_ICMS.Value) : string.Empty;
            }
            set
            {
                CRPV_VL_ICMS = Convert.ToDecimal(CrossCutting.CommonHelpers.GetOnlyDigits(value, true));
            }
        }
        public string TotalPedido
        {
            get
            {
                return CRPV_TOTAL_PEDIDO.HasValue ? CrossCutting.Formatters.DecimalFormatter(CRPV_TOTAL_PEDIDO.Value) : string.Empty;
            }
            set
            {
                CRPV_TOTAL_PEDIDO = Convert.ToDecimal(CrossCutting.CommonHelpers.GetOnlyDigits(value, true));
            }
        }

        public virtual ASSINANTE ASSINANTE { get; set; }
        public virtual CLIENTE CLIENTE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONTA_RECEBER> CONTA_RECEBER { get; set; }
        public virtual CRM CRM { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CRM_PEDIDO_VENDA_ACOMPANHAMENTO> CRM_PEDIDO_VENDA_ACOMPANHAMENTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CRM_PEDIDO_VENDA_ANEXO> CRM_PEDIDO_VENDA_ANEXO { get; set; }
        public virtual FORMA_ENVIO FORMA_ENVIO { get; set; }
        public virtual FORMA_FRETE FORMA_FRETE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CRM_PEDIDO_VENDA_ITEM> CRM_PEDIDO_VENDA_ITEM { get; set; }
        public virtual MOTIVO_CANCELAMENTO MOTIVO_CANCELAMENTO { get; set; }
        public virtual MOTIVO_ENCERRAMENTO MOTIVO_ENCERRAMENTO { get; set; }
        public virtual PLATAFORMA_ENTREGA PLATAFORMA_ENTREGA { get; set; }
        public virtual TEMPLATE_PROPOSTA TEMPLATE_PROPOSTA { get; set; }
        public virtual TRANSPORTADORA TRANSPORTADORA { get; set; }
        public virtual USUARIO USUARIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DIARIO_PROCESSO> DIARIO_PROCESSO { get; set; }


    }
}