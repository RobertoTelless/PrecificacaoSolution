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
    
    public partial class UNIDADE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public UNIDADE()
        {
            this.CRM_PEDIDO_VENDA_ITEM = new HashSet<CRM_PEDIDO_VENDA_ITEM>();
            this.PRODUTO = new HashSet<PRODUTO>();
            this.SERVICO = new HashSet<SERVICO>();
        }
    
        public int UNID_CD_ID { get; set; }
        public Nullable<int> ASSI_CD_ID { get; set; }
        public string UNID_NM_NOME { get; set; }
        public string UNID_SG_SIGLA { get; set; }
        public Nullable<int> UNID_IN_TIPO_UNIDADE { get; set; }
        public int UNID_IN_ATIVO { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CRM_PEDIDO_VENDA_ITEM> CRM_PEDIDO_VENDA_ITEM { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PRODUTO> PRODUTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SERVICO> SERVICO { get; set; }
    }
}
