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
    
    public partial class CRM_PEDIDO_VENDA
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CRM_PEDIDO_VENDA()
        {
            this.CONTA_RECEBER = new HashSet<CONTA_RECEBER>();
            this.CRM_PEDIDO_VENDA_ACOMPANHAMENTO = new HashSet<CRM_PEDIDO_VENDA_ACOMPANHAMENTO>();
            this.CRM_PEDIDO_VENDA_ANEXO = new HashSet<CRM_PEDIDO_VENDA_ANEXO>();
            this.CRM_PEDIDO_VENDA_ITEM = new HashSet<CRM_PEDIDO_VENDA_ITEM>();
            this.DIARIO_PROCESSO = new HashSet<DIARIO_PROCESSO>();
        }
    
        public int CRPV_CD_ID { get; set; }
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
        public System.DateTime CRPV_DT_VALIDADE { get; set; }
        public System.DateTime CRPV_DT_PEDIDO { get; set; }
        public string CRPV_TX_INTRODUCAO { get; set; }
        public string CRPV_NR_NUMERO { get; set; }
        public int CRPV_IN_STATUS { get; set; }
        public string CRPV_TX_INFORMACOES_GERAIS { get; set; }
        public string CRPV_TX_OUTROS_ITENS { get; set; }
        public Nullable<decimal> CRPV_VL_TOTAL_SERVICOS { get; set; }
        public Nullable<decimal> CRPV_VL_DESCONTO { get; set; }
        public Nullable<decimal> CRPV_VL_FRETE { get; set; }
        public Nullable<decimal> CRPV_VL_PESO_BRUTO { get; set; }
        public Nullable<decimal> CRPV_VL_PESO_LIQUIDO { get; set; }
        public Nullable<decimal> CRPV_VL_TOTAL_ITENS { get; set; }
        public Nullable<decimal> CRPV_VL_IPI { get; set; }
        public Nullable<decimal> CRPV_VL_ICMS { get; set; }
        public Nullable<decimal> CRPV_TOTAL_PEDIDO { get; set; }
        public string CRPV_TX_CONDICOES_COMERCIAIS { get; set; }
        public Nullable<int> CRPV_IN_PRAZO_ENTREGA { get; set; }
        public string CRPV_TX_OBSERVACAO { get; set; }
        public Nullable<System.DateTime> CRPV_DT_ENVIO { get; set; }
        public string CRPV_DS_ENVIO { get; set; }
        public string CRPV_LK_LINK { get; set; }
        public Nullable<System.DateTime> CRPV_DT_CANCELAMENTO { get; set; }
        public string CRPV_DS_CANCELAMENTO { get; set; }
        public Nullable<System.DateTime> CRPV_DT_REPROVACAO { get; set; }
        public string CRPV_DS_REPROVACAO { get; set; }
        public Nullable<System.DateTime> CRPV_DT_APROVACAO { get; set; }
        public string CRPV_DS_APROVACAO { get; set; }
        public string CRPV_NM_NOME { get; set; }
        public Nullable<int> CRPV_IN_GERAR_CR { get; set; }
        public Nullable<int> CRPV_IN_GERAR_PEDIDO { get; set; }
        public Nullable<int> CRPC_IN_EMAIL { get; set; }
        public Nullable<int> CRPV_IN_GEROU_NF { get; set; }
        public string CRPV_NR_NOTA_FISCAL { get; set; }
        public Nullable<decimal> CRPV_VL_VALOR { get; set; }
        public Nullable<decimal> CRPV_VL_TOTAL { get; set; }
        public Nullable<int> CRPV_IN_NUMERO_GERADO { get; set; }
        public Nullable<System.DateTime> CRPV_DT_FATURAMENTO { get; set; }
        public Nullable<System.DateTime> CRPV_DT_EXPEDICAO { get; set; }
        public Nullable<System.DateTime> CRPV_DT_ENTREGA { get; set; }
        public int CRPV_IN_ATIVO { get; set; }
    
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
