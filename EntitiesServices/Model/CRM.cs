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
    
    public partial class CRM
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CRM()
        {
            this.AGENDA = new HashSet<AGENDA>();
            this.CONTA_RECEBER = new HashSet<CONTA_RECEBER>();
            this.CRM_ACAO = new HashSet<CRM_ACAO>();
            this.CRM_ANEXO = new HashSet<CRM_ANEXO>();
            this.CRM_COMENTARIO = new HashSet<CRM_COMENTARIO>();
            this.CRM_CONTATO = new HashSet<CRM_CONTATO>();
            this.CRM_PEDIDO_VENDA = new HashSet<CRM_PEDIDO_VENDA>();
            this.DIARIO_PROCESSO = new HashSet<DIARIO_PROCESSO>();
        }
    
        public int CRM1_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public int CLIE_CD_ID { get; set; }
        public Nullable<int> TICR_CD_ID { get; set; }
        public Nullable<int> USUA_CD_ID { get; set; }
        public Nullable<int> ORIG_CD_ID { get; set; }
        public Nullable<int> MOCA_CD_ID { get; set; }
        public Nullable<int> MOEN_CD_ID { get; set; }
        public Nullable<int> CRM1_IN_ATIVO { get; set; }
        public Nullable<System.DateTime> CRM1_DT_CRIACAO { get; set; }
        public string CRM1_NM_NOME { get; set; }
        public string CRM1_DS_DESCRICAO { get; set; }
        public string CRM1_TX_INFORMACOES_GERAIS { get; set; }
        public int CRM1_IN_STATUS { get; set; }
        public Nullable<System.DateTime> CRM1_DT_CANCELAMENTO { get; set; }
        public string CRM1_DS_MOTIVO_CANCELAMENTO { get; set; }
        public Nullable<System.DateTime> CRM1_DT_ENCERRAMENTO { get; set; }
        public string CRM1_DS_INFORMACOES_ENCERRAMENTO { get; set; }
        public Nullable<int> CRM1_IN_ESTRELA { get; set; }
        public Nullable<int> CRM1_IN_DUMMY { get; set; }
        public string CRM1_AQ_IMAGEM { get; set; }
        public Nullable<int> CRM1_NR_TEMPERATURA { get; set; }
        public Nullable<decimal> CRM1_VL_VALOR_INICIAL { get; set; }
        public Nullable<decimal> CRM1_VL_VALOR_FINAL { get; set; }
        public Nullable<int> CRM1_NR_ATRASO { get; set; }
        public Nullable<int> TRAN_CD_ID { get; set; }
        public Nullable<System.DateTime> CRM1_DT_PREVISAO_ENTREGA { get; set; }
        public Nullable<int> CRM1_IN_AVISO_ENTREGA { get; set; }
        public Nullable<System.DateTime> CRM1_DT_DATA_SAIDA { get; set; }
        public Nullable<int> CRM1_IN_ENTREGA_CONFIRMADA { get; set; }
        public string CRM_DS_INFORMACOES_SAIDA { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AGENDA> AGENDA { get; set; }
        public virtual ASSINANTE ASSINANTE { get; set; }
        public virtual CLIENTE CLIENTE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONTA_RECEBER> CONTA_RECEBER { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CRM_ACAO> CRM_ACAO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CRM_ANEXO> CRM_ANEXO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CRM_COMENTARIO> CRM_COMENTARIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CRM_CONTATO> CRM_CONTATO { get; set; }
        public virtual CRM_ORIGEM CRM_ORIGEM { get; set; }
        public virtual MOTIVO_CANCELAMENTO MOTIVO_CANCELAMENTO { get; set; }
        public virtual MOTIVO_ENCERRAMENTO MOTIVO_ENCERRAMENTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CRM_PEDIDO_VENDA> CRM_PEDIDO_VENDA { get; set; }
        public virtual TIPO_CRM TIPO_CRM { get; set; }
        public virtual USUARIO USUARIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DIARIO_PROCESSO> DIARIO_PROCESSO { get; set; }
    }
}
