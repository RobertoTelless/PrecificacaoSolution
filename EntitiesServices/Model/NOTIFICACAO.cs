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
    
    public partial class NOTIFICACAO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public NOTIFICACAO()
        {
            this.NOTIFICACAO_ANEXO = new HashSet<NOTIFICACAO_ANEXO>();
        }
    
        public int NOTC_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public int USUA_CD_ID { get; set; }
        public int CANO_CD_ID { get; set; }
        public Nullable<System.DateTime> NOTC_DT_EMISSAO { get; set; }
        public System.DateTime NOTC_DT_VALIDADE { get; set; }
        public string NOTC_NM_TITULO { get; set; }
        public byte[] NOTC_TX_TEXTO { get; set; }
        public string NOTC_TX_NOTIFICACAO { get; set; }
        public int NOTC_IN_VISTA { get; set; }
        public Nullable<System.DateTime> NOTC_DT_VISTA { get; set; }
        public int NOTC_IN_ATIVO { get; set; }
        public Nullable<int> USUARIOUSUA_CD_ID { get; set; }
        public int NOTC_IN_NIVEL { get; set; }
    
        public virtual ASSINANTE ASSINANTE { get; set; }
        public virtual CATEGORIA_NOTIFICACAO CATEGORIA_NOTIFICACAO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NOTIFICACAO_ANEXO> NOTIFICACAO_ANEXO { get; set; }
        public virtual USUARIO USUARIO { get; set; }
    }
}
