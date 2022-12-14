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
    
    public partial class TAREFA
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TAREFA()
        {
            this.AGENDA = new HashSet<AGENDA>();
            this.TAREFA_ACOMPANHAMENTO = new HashSet<TAREFA_ACOMPANHAMENTO>();
            this.TAREFA_ANEXO = new HashSet<TAREFA_ANEXO>();
        }
    
        public int TARE_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public int USUA_CD_ID { get; set; }
        public int TITR_CD_ID { get; set; }
        public Nullable<int> PETA_CD_ID { get; set; }
        public Nullable<int> TARE_IN_TAREFA_PAI { get; set; }
        public System.DateTime TARE_DT_CADASTRO { get; set; }
        public System.DateTime TARE_DT_ESTIMADA { get; set; }
        public Nullable<System.DateTime> TARE_DT_REALIZADA { get; set; }
        public string TARE_NM_TITULO { get; set; }
        public string TARE_DS_DESCRICAO { get; set; }
        public string TARE_NM_LOCAL { get; set; }
        public int TARE_IN_STATUS { get; set; }
        public int TARE_IN_PRIORIDADE { get; set; }
        public int TARE_IN_AVISA { get; set; }
        public Nullable<int> TARE_NR_PERIODICIDADE_QUANTIDADE { get; set; }
        public byte[] TARE_TX_OBSERVACOES { get; set; }
        public int TARE_IN_ATIVO { get; set; }
        public string TARE_TEX_OBSERVACAO { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AGENDA> AGENDA { get; set; }
        public virtual ASSINANTE ASSINANTE { get; set; }
        public virtual PERIODICIDADE_TAREFA PERIODICIDADE_TAREFA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TAREFA_ACOMPANHAMENTO> TAREFA_ACOMPANHAMENTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TAREFA_ANEXO> TAREFA_ANEXO { get; set; }
        public virtual TIPO_TAREFA TIPO_TAREFA { get; set; }
        public virtual USUARIO USUARIO { get; set; }
    }
}
