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
    
    public partial class TIPO_TRANSPORTE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TIPO_TRANSPORTE()
        {
            this.TRANSPORTADORA = new HashSet<TRANSPORTADORA>();
        }
    
        public int TITR_CD_ID { get; set; }
        public string TITR_NM_NOME { get; set; }
        public Nullable<int> TITR_IN_ATIVO { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TRANSPORTADORA> TRANSPORTADORA { get; set; }
    }
}
