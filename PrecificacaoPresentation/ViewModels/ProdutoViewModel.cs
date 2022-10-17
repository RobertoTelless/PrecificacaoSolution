using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;
using EntitiesServices.Attributes;

namespace ERP_Condominios_Solution.ViewModels
{
    public class ProdutoViewModel
    {
        [Key]
        public int PROD_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo CATEGORIA obrigatorio")]
        public int CAPR_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo SUBCATEGORIA obrigatorio")]
        public int SCPR_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo UNIDADE obrigatorio")]
        public int UNID_CD_ID { get; set; }
        public Nullable<int> PROR_CD_ID { get; set; }
        public Nullable<int> TAMA_CD_ID { get; set; }
        public string PROD_AQ_FOTO { get; set; }
        [Required(ErrorMessage = "Campo TIPO obrigatorio")]
        public Nullable<int> PROD_IN_TIPO_PRODUTO { get; set; }
        public Nullable<int> PROD_IN_COMPOSTO { get; set; }
        public int PROD_IN_KIT { get; set; }
        [Required(ErrorMessage = "Campo CÓDIGO obrigatorio")]
        [StringLength(30, ErrorMessage = "O CÓDIGO deve conter no máximo 30 caracteres.")]
        public string PROD_CD_CODIGO { get; set; }
        [StringLength(30, ErrorMessage = "O CÓDIGO DE BARRAS deve conter no máximo 30 caracteres.")]
        public string PROD_BC_BARCODE { get; set; }
        [Required(ErrorMessage = "Campo NOME obrigatorio")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "O NOME deve conter no minimo 1 e no máximo 200 caracteres.")]
        public string PROD_NM_NOME { get; set; }
        [StringLength(1000, ErrorMessage = "A DESCRIÇÃO deve conter no máximo 1000 caracteres.")]
        public string PROD_DS_DESCRICAO { get; set; }
        [StringLength(1000, ErrorMessage = "AS INFORMAÇÕES devem conter no máximo 1000 caracteres.")]
        public string PROD_DS_INFORMACOES { get; set; }
        [StringLength(5000, ErrorMessage = "A INFORMAÇÃO NUTRICIONAL deve conter no máximo 5000 caracteres.")]
        public string PROD_DS_INFORMACAO_NUTRICIONAL { get; set; }
        [StringLength(50, ErrorMessage = "O FABRICANTE deve conter no máximo 50 caracteres.")]
        public string PROD_NM_FABRICANTE { get; set; }
        [StringLength(50, ErrorMessage = "A MARCA deve conter no máximo 50 caracteres.")]
        public string PROD_NM_MARCA { get; set; }
        [StringLength(50, ErrorMessage = "O MODELO deve conter no máximo 50 caracteres.")]
        public string PROD_NM_MODELO { get; set; }
        [StringLength(50, ErrorMessage = "A REFERENCIA deve conter no máximo 50 caracteres.")]
        public string PROD_NR_REFERENCIA { get; set; }
        [Required(ErrorMessage = "Campo QUANTIDADE INICIAL obrigatorio")]
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> PROD_QN_QUANTIDADE_INICIAL { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public int PROD_QN_QUANTIDADE_MINIMA { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public int PROD_QN_QUANTIDADE_MAXIMA { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public int PROD_QN_ESTOQUE { get; set; }
        public Nullable<System.DateTime> PROD_DT_ULTIMA_MOVIMENTACAO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> PROD_VL_ULTIMO_CUSTO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> PROD_VL_PRECO_VENDA { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> PROD_VL_PRECO_PROMOCAO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> PROD_VL_PRECO_MINIMO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> PROD_VL_MARKUP_MINIMO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> PROD_VL_MARKUP_PADRAO { get; set; }
        public int PROD_IN_AVISA_MINIMO { get; set; }
        [StringLength(50, ErrorMessage = "O NCM deve conter no máximo 50 caracteres.")]
        public string PROD_NR_NCM { get; set; }
        [StringLength(50, ErrorMessage = "O GTIN_EAN deve conter no máximo 50 caracteres.")]
        public string PROD_CD_GTIN_EAN { get; set; }
        [StringLength(50, ErrorMessage = "O CEST deve conter no máximo 50 caracteres.")]
        public string PROD_NR_CEST { get; set; }
        [StringLength(50, ErrorMessage = "O GTIN_EAN deve conter no máximo 50 caracteres.")]
        public string PROD_NR_GTIN_EAN_TRIB { get; set; }
        [StringLength(50, ErrorMessage = "A UNIDADE TRIBUTÁRIA deve conter no máximo 50 caracteres.")]
        public string PROD_NM_UNIDADE_TRIBUTARIA { get; set; }
        [StringLength(50, ErrorMessage = "O ENQUADRE IPI deve conter no máximo 50 caracteres.")]
        public string PROD_NR_ENQUADRE_IPI { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> PROD_VL_IPI_FIXO { get; set; }
        public Nullable<System.DateTime> PROD_DT_CADASTRO { get; set; }
        public Nullable<int> PROD_IN_ATIVO { get; set; }
        [StringLength(50, ErrorMessage = "A LOCALIZAÇÃO deve conter no máximo 50 caracteres.")]
        public string PROD_NM_LOCALIZACAO_ESTOQUE { get; set; }
        [StringLength(5000, ErrorMessage = "A OBSERVAÇÃO deve conter no máximo 5000 caracteres.")]
        public string PROD_TX_OBSERVACOES { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> PROD_QN_RESERVA_ESTOQUE { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> PROD_QN_PESO_LIQUIDO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> PROD_QN_PESO_BRUTO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> PROD_NR_DIAS_PREPARACAO { get; set; }
        public Nullable<int> PROD_IN_TIPO_EMBALAGEM { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> PROD_NR_ALTURA { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> PROD_NR_LARGURA { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> PROD_NR_COMPRIMENTO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> PROD_NR_DIAMETRO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public string PROD_NR_FATOR_CONVERSAO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> PROD_QN_QUANTIDADE_ALTERADA { get; set; }
        [StringLength(500, ErrorMessage = "A JUSTIFICATIVA deve conter no máximo 500 caracteres.")]
        public string PROD_DS_JUSTIFICATIVA { get; set; }

        public Nullable<int> EntradaSaida { get; set; }

        public string UltimoCusto
        {
            get
            {
                return PROD_VL_ULTIMO_CUSTO.HasValue ? CrossCutting.Formatters.DecimalFormatter(PROD_VL_ULTIMO_CUSTO.Value) : string.Empty;
            }
            set
            {
                PROD_VL_ULTIMO_CUSTO = value != null ? Convert.ToDecimal(CrossCutting.CommonHelpers.GetOnlyDigits(value, true)) : 0;
            }
        }
        public string PrecoVenda
        {
            get
            {
                return PROD_VL_PRECO_VENDA.HasValue ? CrossCutting.Formatters.DecimalFormatter(PROD_VL_PRECO_VENDA.Value) : string.Empty;
            }
            set
            {
                PROD_VL_PRECO_VENDA = value != null ? Convert.ToDecimal(CrossCutting.CommonHelpers.GetOnlyDigits(value, true)) : 0;
            }
        }
        public string PrecoPromocao
        {
            get
            {
                return PROD_VL_PRECO_PROMOCAO.HasValue ? CrossCutting.Formatters.DecimalFormatter(PROD_VL_PRECO_PROMOCAO.Value) : string.Empty;
            }
            set
            {
                PROD_VL_PRECO_PROMOCAO = value != null ? Convert.ToDecimal(CrossCutting.CommonHelpers.GetOnlyDigits(value, true)) : 0;
            }
        }
        public string PrecoMinimo
        {
            get
            {
                return PROD_VL_PRECO_MINIMO.HasValue ? CrossCutting.Formatters.DecimalFormatter(PROD_VL_PRECO_MINIMO.Value) : string.Empty;
            }
            set
            {
                PROD_VL_PRECO_MINIMO = value != null ? Convert.ToDecimal(CrossCutting.CommonHelpers.GetOnlyDigits(value, true)) : 0;
            }
        }
        public string MarkupMinimo
        {
            get
            {
                return PROD_VL_MARKUP_MINIMO.HasValue ? CrossCutting.Formatters.DecimalFormatter(PROD_VL_MARKUP_MINIMO.Value) : string.Empty;
            }
            set
            {
                PROD_VL_MARKUP_MINIMO = value != null ? Convert.ToDecimal(CrossCutting.CommonHelpers.GetOnlyDigits(value, true)) : 0;
            }
        }
        public string MarkupPadrao
        {
            get
            {
                return PROD_VL_MARKUP_PADRAO.HasValue ? CrossCutting.Formatters.DecimalFormatter(PROD_VL_MARKUP_PADRAO.Value) : string.Empty;
            }
            set
            {
                PROD_VL_MARKUP_PADRAO = value != null ? Convert.ToDecimal(CrossCutting.CommonHelpers.GetOnlyDigits(value, true)) : 0;
            }
        }

        public bool AvisaMinima 
        {
            get
            {
                if (PROD_IN_AVISA_MINIMO == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PROD_IN_AVISA_MINIMO = (value == true) ? 1 : 0;
            }
        }

        public string ValorIPIFixo
        {
            get
            {
                return PROD_VL_IPI_FIXO.HasValue ? CrossCutting.Formatters.DecimalFormatter(PROD_VL_IPI_FIXO.Value) : string.Empty;
            }
            set
            {
                PROD_VL_IPI_FIXO = value != null ? Convert.ToDecimal(CrossCutting.CommonHelpers.GetOnlyDigits(value, true)) : 0;
            }
        }

        [RegularExpression(@"^[0-9]+([,][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> QuantidadeMaxima
        {
            get
            {
                return PROD_QN_QUANTIDADE_MAXIMA;
            }
            set
            {
                PROD_QN_QUANTIDADE_MAXIMA = value.Value;
            }
        }
        public bool Composto
        {
            get
            {
                if (PROD_IN_COMPOSTO == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PROD_IN_COMPOSTO = (value == true) ? 1 : 0;
            }
        }

        public bool Kit
        {
            get
            {
                if (PROD_IN_KIT == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PROD_IN_KIT = (value == true) ? 1 : 0;
            }
        }

        public String Tipo
        {
            get
            {
                if (PROD_IN_TIPO_PRODUTO == 1)
                {
                    return "Produto";
                }
                return "Insumo";
            }
        }

        public virtual ASSINANTE ASSINANTE { get; set; }
        public virtual CATEGORIA_PRODUTO CATEGORIA_PRODUTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CRM_PEDIDO_VENDA_ITEM> CRM_PEDIDO_VENDA_ITEM { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MOVIMENTO_ESTOQUE_PRODUTO> MOVIMENTO_ESTOQUE_PRODUTO { get; set; }
        public virtual PRODUTO_ORIGEM PRODUTO_ORIGEM { get; set; }
        public virtual TAMANHO TAMANHO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PRODUTO_ANEXO> PRODUTO_ANEXO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PRODUTO_ESTOQUE_EMPRESA> PRODUTO_ESTOQUE_EMPRESA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PRODUTO_FORNECEDOR> PRODUTO_FORNECEDOR { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PRODUTO_KIT> PRODUTO_KIT { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PRODUTO_TABELA_PRECO> PRODUTO_TABELA_PRECO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PRODUTO_ULTIMOS_CUSTOS> PRODUTO_ULTIMOS_CUSTOS { get; set; }
        public virtual UNIDADE UNIDADE { get; set; }
        public virtual SUBCATEGORIA_PRODUTO SUBCATEGORIA_PRODUTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FICHA_TECNICA> FICHA_TECNICA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FICHA_TECNICA_DETALHE> FICHA_TECNICA_DETALHE { get; set; }

    }
}