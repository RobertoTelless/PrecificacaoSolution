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
    
    public partial class FORMA_PAGTO_RECTO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public FORMA_PAGTO_RECTO()
        {
            this.CONTA_PAGAR = new HashSet<CONTA_PAGAR>();
            this.CONTA_PAGAR_PARCELA = new HashSet<CONTA_PAGAR_PARCELA>();
            this.CONTA_RECEBER = new HashSet<CONTA_RECEBER>();
        }
    
        public int FOPR_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public Nullable<int> EMPR_CD_ID { get; set; }
        public int COBA_CD_ID { get; set; }
        public int FOPA_IN_TIPO_FORMA { get; set; }
        public string FOPR_NM_NOME_FORMA { get; set; }
        public System.DateTime FOPR_DT_CADASTRO { get; set; }
        public int FOPR_IN_ATIVO { get; set; }
    
        public virtual ASSINANTE ASSINANTE { get; set; }
        public virtual CONTA_BANCO CONTA_BANCO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONTA_PAGAR> CONTA_PAGAR { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONTA_PAGAR_PARCELA> CONTA_PAGAR_PARCELA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONTA_RECEBER> CONTA_RECEBER { get; set; }
        public virtual EMPRESA EMPRESA { get; set; }
    }
}