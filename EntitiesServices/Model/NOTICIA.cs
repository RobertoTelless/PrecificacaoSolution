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
    
    public partial class NOTICIA
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public NOTICIA()
        {
            this.NOTICIA_COMENTARIO = new HashSet<NOTICIA_COMENTARIO>();
        }
    
        public int NOTC_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public System.DateTime NOTC_DT_EMISSAO { get; set; }
        public System.DateTime NOTC_DT_VALIDADE { get; set; }
        public string NOTC_NM_TITULO { get; set; }
        public string NOTC_NM_AUTOR { get; set; }
        public Nullable<System.DateTime> NOTC_DT_DATA_AUTOR { get; set; }
        public byte[] NOTC_TX_TEXTO { get; set; }
        public string NOTC_LK_LINK { get; set; }
        public string NOTC_NM_ORIGEM { get; set; }
        public string NOTC_AQ_FOTO { get; set; }
        public int NOTC_NR_ACESSOS { get; set; }
        public int NOTC_IN_ATIVO { get; set; }
        public string NOTC_TX_NOTICIA { get; set; }
    
        public virtual ASSINANTE ASSINANTE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NOTICIA_COMENTARIO> NOTICIA_COMENTARIO { get; set; }
    }
}
