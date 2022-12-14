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
    
    public partial class PLATAFORMA_ENTREGA
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PLATAFORMA_ENTREGA()
        {
            this.CRM_PEDIDO_VENDA = new HashSet<CRM_PEDIDO_VENDA>();
            this.EMPRESA_PLATAFORMA = new HashSet<EMPRESA_PLATAFORMA>();
        }
    
        public int PLEN_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public string PLEN_NM_NOME { get; set; }
        public Nullable<decimal> PLEN_VL_FIXO { get; set; }
        public Nullable<decimal> PLEN_VL_LIMITE_FIXO { get; set; }
        public Nullable<decimal> PLEN_PC_VENDA { get; set; }
        public Nullable<decimal> PLEN_VL_TAXA_CARTAO { get; set; }
        public int PLEN_IN_ANTECIPACAO { get; set; }
        public Nullable<decimal> PLEN_PC_ANTECIPACAO { get; set; }
        public System.DateTime PLEN_DT_CADASTRO { get; set; }
        public int PLEN_IN_ATIVO { get; set; }
    
        public virtual ASSINANTE ASSINANTE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CRM_PEDIDO_VENDA> CRM_PEDIDO_VENDA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EMPRESA_PLATAFORMA> EMPRESA_PLATAFORMA { get; set; }
    }
}
