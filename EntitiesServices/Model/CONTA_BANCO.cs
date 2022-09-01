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
    
    public partial class CONTA_BANCO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CONTA_BANCO()
        {
            this.CONTA_BANCO_ANEXO = new HashSet<CONTA_BANCO_ANEXO>();
            this.CONTA_BANCO_LANCAMENTO = new HashSet<CONTA_BANCO_LANCAMENTO>();
            this.CONTA_BANCO_CONTATO = new HashSet<CONTA_BANCO_CONTATO>();
            this.FORMA_PAGTO_RECTO = new HashSet<FORMA_PAGTO_RECTO>();
        }
    
        public int COBA_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public int TICO_CD_ID { get; set; }
        public int BANC_CD_ID { get; set; }
        public int COBA_IN_PRINCIPAL { get; set; }
        public string COBA_NM_NOME { get; set; }
        public string COBA_NR_AGENCIA { get; set; }
        public string COBA_NM_AGENCIA { get; set; }
        public string COBA_NR_CONTA { get; set; }
        public string COBA_NR_DIGITO_CONTA { get; set; }
        public string COBA_NM_GERENTE { get; set; }
        public string COBA_NR_TELEFONE { get; set; }
        public System.DateTime COBA_DT_ABERTURA { get; set; }
        public decimal COBA_VL_SALDO_INICIAL { get; set; }
        public decimal COBA_VL_SALDO_ATUAL { get; set; }
        public int COBA_IN_CONTA_PADRAO { get; set; }
        public System.DateTime COBA_DT_CADASTRO { get; set; }
        public int COBA_IN_ATIVO { get; set; }
        public string COBA_NM_NOME_EXIBE { get; set; }
    
        public virtual ASSINANTE ASSINANTE { get; set; }
        public virtual BANCO BANCO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONTA_BANCO_ANEXO> CONTA_BANCO_ANEXO { get; set; }
        public virtual TIPO_CONTA TIPO_CONTA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONTA_BANCO_LANCAMENTO> CONTA_BANCO_LANCAMENTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONTA_BANCO_CONTATO> CONTA_BANCO_CONTATO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FORMA_PAGTO_RECTO> FORMA_PAGTO_RECTO { get; set; }
    }
}
