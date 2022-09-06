//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EntitiesServices.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class PRODUTO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PRODUTO()
        {
            this.CRM_PEDIDO_VENDA_ITEM = new HashSet<CRM_PEDIDO_VENDA_ITEM>();
            this.MOVIMENTO_ESTOQUE_PRODUTO = new HashSet<MOVIMENTO_ESTOQUE_PRODUTO>();
            this.PRODUTO_ANEXO = new HashSet<PRODUTO_ANEXO>();
            this.PRODUTO_ESTOQUE_EMPRESA = new HashSet<PRODUTO_ESTOQUE_EMPRESA>();
            this.PRODUTO_FORNECEDOR = new HashSet<PRODUTO_FORNECEDOR>();
            this.PRODUTO_KIT = new HashSet<PRODUTO_KIT>();
            this.PRODUTO_TABELA_PRECO = new HashSet<PRODUTO_TABELA_PRECO>();
            this.PRODUTO_ULTIMOS_CUSTOS = new HashSet<PRODUTO_ULTIMOS_CUSTOS>();
        }
    
        public int PROD_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public int CAPR_CD_ID { get; set; }
        public int SCPR_CD_ID { get; set; }
        public int UNID_CD_ID { get; set; }
        public Nullable<int> PROR_CD_ID { get; set; }
        public Nullable<int> TAMA_CD_ID { get; set; }
        public string PROD_AQ_FOTO { get; set; }
        public Nullable<int> PROD_IN_TIPO_PRODUTO { get; set; }
        public Nullable<int> PROD_IN_COMPOSTO { get; set; }
        public int PROD_IN_KIT { get; set; }
        public string PROD_CD_CODIGO { get; set; }
        public string PROD_BC_BARCODE { get; set; }
        public string PROD_NM_NOME { get; set; }
        public string PROD_DS_DESCRICAO { get; set; }
        public string PROD_DS_INFORMACOES { get; set; }
        public string PROD_DS_INFORMACAO_NUTRICIONAL { get; set; }
        public string PROD_NM_FABRICANTE { get; set; }
        public string PROD_NM_MARCA { get; set; }
        public string PROD_NM_MODELO { get; set; }
        public string PROD_NR_REFERENCIA { get; set; }
        public Nullable<int> PROD_QN_QUANTIDADE_INICIAL { get; set; }
        public int PROD_QN_QUANTIDADE_MINIMA { get; set; }
        public int PROD_QN_QUANTIDADE_MAXIMA { get; set; }
        public int PROD_QN_ESTOQUE { get; set; }
        public Nullable<System.DateTime> PROD_DT_ULTIMA_MOVIMENTACAO { get; set; }
        public Nullable<decimal> PROD_VL_ULTIMO_CUSTO { get; set; }
        public Nullable<decimal> PROD_VL_PRECO_VENDA { get; set; }
        public Nullable<decimal> PROD_VL_PRECO_PROMOCAO { get; set; }
        public Nullable<decimal> PROD_VL_PRECO_MINIMO { get; set; }
        public Nullable<decimal> PROD_VL_MARKUP_MINIMO { get; set; }
        public Nullable<decimal> PROD_VL_MARKUP_PADRAO { get; set; }
        public int PROD_IN_AVISA_MINIMO { get; set; }
        public string PROD_NR_NCM { get; set; }
        public string PROD_CD_GTIN_EAN { get; set; }
        public string PROD_NR_CEST { get; set; }
        public string PROD_NR_GTIN_EAN_TRIB { get; set; }
        public string PROD_NM_UNIDADE_TRIBUTARIA { get; set; }
        public string PROD_NR_ENQUADRE_IPI { get; set; }
        public Nullable<decimal> PROD_VL_IPI_FIXO { get; set; }
        public Nullable<System.DateTime> PROD_DT_CADASTRO { get; set; }
        public Nullable<int> PROD_IN_ATIVO { get; set; }
    
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
    }
}
